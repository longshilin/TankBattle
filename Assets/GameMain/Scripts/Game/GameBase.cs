//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using System.Collections.Generic;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace TankBattle {

    public abstract class GameBase {
        private const string SpawnPoint1 = "SpawnPoint1";
        private const string SpawnPoint2 = "SpawnPoint2";
        private Dictionary<int, Tank> mActorDic = new Dictionary<int, Tank>();

        public static GameBase Instance;

        private FrameData mFrameData;
        private LockStep mLockStep;

        public abstract GameMode GameMode {
            get;
        }

        public bool GameOver {
            get;
            protected set;
        }

        private void Awake() {
            Instance = this;
        }

        private Tank m_Tank = null;

        public virtual void Initialize() {
            //mFrameData = new FrameData();
            //mLockStep = gameObject.AddComponent<LockStep>();
            Debug.Log("!!!!!!!!!!!!" + GameObject.FindObjectOfType<LockStep>());

            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);

            // 实例化坦克
            for (int i = 0; i < GameEntry.NetData.mFightData.PlayerInfoList.Count; i++) {
                PlayerInfo info = GameEntry.NetData.mFightData.PlayerInfoList[i];
                string userid = info.UserId;
                string username = info.UserName;

                if (userid.Equals(GameEntry.NetData.mUserData.UserId)) {
                    GameEntry.Entity.ShowMyTank(new MyTankData(GameEntry.Entity.GenerateSerialId(), 10000 + i) {
                        Name = username,
                        Position = GameObject.Find("SpawnPoint" + i).transform.position,
                        Rotation = GameObject.Find("SpawnPoint" + i).transform.rotation,
                        TankId = userid,
                    });
                }
                else {
                    GameEntry.Entity.ShowEnemyTank(new MyTankData(GameEntry.Entity.GenerateSerialId(), 10000 + i) {
                        Name = username,
                        Position = GameObject.Find("SpawnPoint" + i).transform.position,
                        Rotation = GameObject.Find("SpawnPoint" + i).transform.rotation,
                        TankId = userid,
                    });
                }
            }

            m_Tank = null;
            GameOver = false;
        }

        // shutdown game
        public virtual void Shutdown() {
            GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Unsubscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
        }

        // Every frame will be executed when ProcedureMain calling Update Method;
        // please care,This method is not a method of Monobehavior, beacuse not override it.
        // function: change the end flag of the game when game over
        public virtual void Update(float elapseSeconds, float realElapseSeconds) {
            if (m_Tank != null && m_Tank.IsDead) {
                //Debug.Log("m_MyTank.IsDead : " + m_MyTank.IsDead);
                GameOver = true;
                return;
            }
        }

        // when show entity success, Instantiated tank entity and set tank camera target
        protected virtual void OnShowEntitySuccess(object sender, GameEventArgs e) {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne.EntityLogicType == typeof(MyTank)) {
                m_Tank = (MyTank)ne.Entity.Logic;
            }

            if (ne.EntityLogicType == typeof(EnemyTank)) {
                m_Tank = (EnemyTank)ne.Entity.Logic;
            }

            if (GetActor(m_Tank.Id) != m_Tank) {
                AddActor(m_Tank.Id, m_Tank);
                Debug.Log("AddActor - " + m_Tank.Id + " + " + m_Tank.Name);
            }
        }

        // when show entity failure, print the error message
        protected virtual void OnShowEntityFailure(object sender, GameEventArgs e) {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
            Log.Warning("Show entity failure with error message '{0}'.", ne.ErrorMessage);
        }

        public void AddActor(int tUserid, Tank tTank) {
            mActorDic[tUserid] = tTank;
        }

        public void RemoveActor(int tUserid) {
            //Destroy(mActorDic[tUserid].gameObject);
            mActorDic.Remove(tUserid);
        }

        public Tank GetActor(int tUserid) {
            Tank tank = null;
            mActorDic.TryGetValue(tUserid, out tank);
            return tank;
        }
    }
}