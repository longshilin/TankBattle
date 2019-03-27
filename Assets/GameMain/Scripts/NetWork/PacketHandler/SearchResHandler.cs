using GameFramework;
using GameFramework.Network;
using GameFramework.ObjectPool;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace TankBattle {

    public class SearchResHandler : PacketHandlerBase {

        public override int Id {
            get {
                return 1004;
            }
        }

        public override void Handle(object sender, Packet packet) {
            MatchRes packetImpl = (MatchRes)packet;
            //SCHello packetImpl = (SCHello) packet;
            GameEntry.NetData.mFightData.RoomId = packetImpl.RoomId;
            PlayerInfo[] playerArray = new PlayerInfo[packetImpl.PlayerInfoList.Count];
            packetImpl.PlayerInfoList.CopyTo(playerArray, 0);

            for (int i = 0; i < playerArray.Length; i++) {
                GameEntry.NetData.mFightData.PlayerInfoList.Add(playerArray[i]);
            }

            //GameEntry.PlayerB = packetImpl;
        }
    }
}