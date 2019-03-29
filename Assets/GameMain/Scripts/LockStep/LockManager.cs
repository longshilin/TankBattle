using UnityEngine;
using System.Collections;
using GameFramework.Network;
using System.Collections.Generic;

namespace TankBattle {

    public class LockManager : MonoBehaviour {
        public Dictionary<string, Tank> mActorDic = new Dictionary<string, Tank>();

        private static FrameData mFrameData;

        private static LockStep mLockStep;

        private void Start() {
            mFrameData = new FrameData();

            mLockStep = gameObject.AddComponent<LockStep>();
            Debug.Log("！！！！！！！！！！" + GameObject.FindObjectOfType<LockStep>());
        }

        public void AddActor(string tUserid, Tank tTank) {
            mActorDic[tUserid] = tTank;
        }

        public void RemoveActor(string tUserid) {
            Destroy(mActorDic[tUserid].gameObject);
            mActorDic.Remove(tUserid);
        }

        public Tank GetActor(string tUserid) {
            Tank tank = null;
            mActorDic.TryGetValue(tUserid, out tank);
            return tank;
        }

        public void AddOneFrame(int frameindex, List<Packet> list) {
            mFrameData.AddOneFrame(frameindex, list);
        }

        public bool LockFrameTurn(ref List<Packet> list) {
            return mFrameData.LockFrameTurn(ref list);
        }

        public void SetFaseForward(int tValue) {
            mLockStep.SetFaseForward(tValue);
        }

        // 游戏推出时销毁
        private void OnDestroy() {
            mFrameData = null;
            mLockStep = null;
        }
    }
}