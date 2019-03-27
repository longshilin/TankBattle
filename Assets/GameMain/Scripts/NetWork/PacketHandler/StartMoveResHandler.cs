using GameFramework;
using GameFramework.Network;
using GameFramework.ObjectPool;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace TankBattle {

    public class StartMoveResHandler : PacketHandlerBase {

        public override int Id {
            get {
                return 1007;
            }
        }

        public override void Handle(object sender, Packet packet) {
            StartMoveRes packetImpl = (StartMoveRes)packet;
            Debug.Log("收到消息： '{0}' - '{1}'" + packetImpl.UserId + packetImpl.RoomId);
            //GameEntry.PlayerB = packetImpl;
        }
    }
}