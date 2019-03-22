﻿using GameFramework;
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
            Debug.Log("收到消息： '{0}'." + packetImpl.UserName);
            GameEntry.PlayerB = packetImpl;
        }
    }
}