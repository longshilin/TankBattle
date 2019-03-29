using UnityEngine;
using System.Collections;

namespace TankBattle {

    public class SendManager : MonoBehaviour {

        public static void SendStartMove() {
            StartMoveReq mStartMoveReq = new StartMoveReq();
            mStartMoveReq.UserId = "" + GameEntry.NetData.mUserData.UserId;
            mStartMoveReq.RoomId = GameEntry.NetData.mFightData.RoomId;
            Send(mStartMoveReq);
        }

        public static void SendChangeDir(int tAngle) {
            ChangeDirReq mChangeDirReq = new ChangeDirReq();

            mChangeDirReq.UserId = "" + GameEntry.NetData.mUserData.UserId;
            mChangeDirReq.RoomId = GameEntry.NetData.mFightData.RoomId;
            mChangeDirReq.Angle = tAngle;
            Send(mChangeDirReq);
        }

        public static void SendEndMove() {
            EndMoveReq mEndMoveReq = new EndMoveReq();
            mEndMoveReq.UserId = "" + GameEntry.NetData.mUserData.UserId;
            mEndMoveReq.RoomId = GameEntry.NetData.mFightData.RoomId;
            Send(mEndMoveReq);
        }

        private static void Send(PacketBase packet) {
            NetWorkChannel.send(packet);
        }
    }
}