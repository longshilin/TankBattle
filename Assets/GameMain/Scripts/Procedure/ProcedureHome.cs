using GameFramework.Event;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace TankBattle {

    public class ProcedureHome : ProcedureBase {
        private HomeForm m_HomeForm = null;

        //private bool enterHome = false;
        private bool m_StartGame = false;

        public void StartGame() {
            m_StartGame = true;
        }

        public override bool UseNativeDialog {
            get {
                return false;
            }
        }

        public void goHome() {
            //enterHome = true;
        }

        protected override void OnEnter(ProcedureOwner procedureOwner) {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            // 进入时先初始化StartGame为false
            m_StartGame = false;

            GameEntry.UI.OpenUIForm(UIFormId.HomeForm, this);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown) {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            if (m_HomeForm != null) {
                m_HomeForm.Close(isShutdown);
                m_HomeForm = null;
            }
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds) {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (m_StartGame) {
                //Debug.Log("跳转到游戏主界面");
                procedureOwner.SetData<VarInt>(Constant.ProcedureData.NextSceneId, GameEntry.Config.GetInt("Scene.Main"));
                procedureOwner.SetData<VarInt>(Constant.ProcedureData.GameMode, (int)GameMode.Survival);
                ChangeState<ProcedureChangeScene>(procedureOwner);
            }
        }

        private void OnOpenUIFormSuccess(object sender, GameEventArgs e) {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if (ne.UserData != this) {
                return;
            }

            m_HomeForm = (HomeForm)ne.UIForm.Logic;
        }
    }
}