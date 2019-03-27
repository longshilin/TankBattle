using UnityEngine;
using System.Collections;
using GameFramework.Network;
using System.Collections.Generic;

namespace TankBattle {

    public class LockManager : MonoBehaviour {
        public static LockManager Instance;

        private static FrameData mFrameData;

        private static LockStep mLockStep;

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            mFrameData = new FrameData();

            mLockStep = gameObject.AddComponent<LockStep>();
            Debug.Log("！！！！！！！！！！" + GameObject.FindObjectOfType<LockStep>());
        }

        public void AddOneFrame(uint frameindex, List<PacketBase> list) {
            mFrameData.AddOneFrame(frameindex, list);
        }

        public bool LockFrameTurn(ref List<PacketBase> list) {
            return mFrameData.LockFrameTurn(ref list);
        }

        public void SetFaseForward(int tValue) {
            mLockStep.SetFaseForward(tValue);
        }
    }
}