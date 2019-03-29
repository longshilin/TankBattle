using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;
using GameFramework.Network;

namespace TankBattle {

    public class LockStep : MonoBehaviour {
        private float mLogicTempTime = 0;

        //private GameMessageHandler gameMessageHandler = new GameMessageHandler();

        private void Update() {
            mLogicTempTime += Time.deltaTime;
            if (mLogicTempTime > LockStepConfig.mRenderFrameUpdateTime) {
                for (int i = 0; i < mFastForwardSpeed; i++) {
                    GameTurn();
                    mLogicTempTime = 0;
                }
            }
        }

        private int mFastForwardSpeed = 1;

        public void SetFaseForward(int tValue) {
            mFastForwardSpeed = tValue;
        }

        private int GameFrameInTurn = 0;

        private void GameTurn() {
            if (GameFrameInTurn == 0) {
                List<Packet> list = null;
                if (GameEntry.LockManager.LockFrameTurn(ref list)) {
                    if (list != null)
                        GameEntry.GameMessageHandler.MsgHandle(list);
                    GameFrameInTurn++;
                }
            }
            else {
                //GameBase.Instance.UpdateEvent();

                if (GameFrameInTurn == LockStepConfig.mRenderFrameCount)
                    GameFrameInTurn = 0;
                else
                    GameFrameInTurn++;
            }
        }
    }
}