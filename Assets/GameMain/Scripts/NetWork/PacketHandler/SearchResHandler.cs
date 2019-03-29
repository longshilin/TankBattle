using GameFramework;
using GameFramework.Network;
using GameFramework.ObjectPool;
using System.Collections.Generic;
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
           
            PlayerInfo[] playerArray = new PlayerInfo[packetImpl.PlayerInfoList.Count];
            packetImpl.PlayerInfoList.CopyTo(playerArray, 0);
            GameEntry.NetData.mFightData.PlayerInfoList = new List<PlayerInfo>();
            for (int i = 0; i < playerArray.Length; i++) {
                GameEntry.NetData.mFightData.PlayerInfoList.Add(playerArray[i]);
            }
            GameEntry.NetData.mFightData.RoomId = packetImpl.RoomId;
            Debug.Log(GameEntry.NetData.mFightData.PlayerInfoList);
            //GameEntry.PlayerB = packetImpl;
        }
    }
}