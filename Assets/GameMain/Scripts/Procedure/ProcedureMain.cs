using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace TankBattle {

    public class ProcedureMain : ProcedureBase {
        private const float GameOverDelayedSeconds = 2f;

        private readonly Dictionary<GameMode, GameBase> m_Games = new Dictionary<GameMode, GameBase>();
        private GameBase m_CurrentGame = null;
        private bool m_GotoMenu = false;
        private bool m_GameInit = false;
        private float m_GotoMenuDelaySeconds = 0f;

        private CameraControlPro m_CameraControl;

        public override bool UseNativeDialog {
            get {
                return false;
            }
        }

        public void GotoMenu() {
            m_GotoMenu = true;
        }

        protected override void OnInit(ProcedureOwner procedureOwner) {
            base.OnInit(procedureOwner);

            m_Games.Add(GameMode.Survival, new SurvivalGame());
        }

        protected override void OnDestroy(ProcedureOwner procedureOwner) {
            base.OnDestroy(procedureOwner);

            m_Games.Clear();
        }

        protected override void OnEnter(ProcedureOwner procedureOwner) {
            base.OnEnter(procedureOwner);

            m_GotoMenu = false;
            GameMode gameMode = (GameMode)procedureOwner.GetData<VarInt>(Constant.ProcedureData.GameMode).Value;
            m_CurrentGame = m_Games[gameMode];
            m_CurrentGame.Initialize();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown) {
            if (m_CurrentGame != null) {
                m_CurrentGame.Shutdown();
                m_CurrentGame = null;
            }

            base.OnLeave(procedureOwner, isShutdown);
        }

        // Every frame will be executed
        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds) {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (!m_GameInit && GameEntry.LockManager.mActorDic.Count == 2) {
                GameInitReq gameInitReq = new GameInitReq();
                gameInitReq.Success = true;

                Debug.Log("客户端 发送消息给服务器，玩家都实例化成功");
                //客户端 发送消息给服务器，玩家都实例化成功
                NetWorkChannel.send(gameInitReq);
                m_GameInit = true;
            }
            else {
                return;
            }

            // Monitor if the game is over
            if (m_CurrentGame != null && !m_CurrentGame.GameOver) {
                m_CurrentGame.Update(elapseSeconds, realElapseSeconds);
                return;
            }

            // change the flag of m_GotoMenu, and init the delaySecond, start the timer
            if (!m_GotoMenu) {
                m_GotoMenu = true;
                m_GotoMenuDelaySeconds = 0;
            }
            // accumulate the elapsed time of each frame
            m_GotoMenuDelaySeconds += elapseSeconds;

            // when the cumulative time exceed the GameOverDelayedSeconds, then Switch scene
            if (m_GotoMenuDelaySeconds >= GameOverDelayedSeconds) {
                //Debug.Log("游戏结束，跳转到大厅界面");

                procedureOwner.SetData<VarInt>(Constant.ProcedureData.NextSceneId, GameEntry.Config.GetInt("Scene.Home"));
                ChangeState<ProcedureChangeScene>(procedureOwner);
            }
        }
    }
}