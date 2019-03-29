using GameFramework;
using GameFramework.Network;
using GameFramework.ObjectPool;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace TankBattle {

    public class GameInitResHandler : PacketHandlerBase {

        public override int Id {
            get {
                return 1015;
            }
        }

        public override void Handle(object sender, Packet packet) {
            GameInitRes packetImpl = (GameInitRes)packet;
            GameEntry.NetData.mFightData.GameInitSuccess = packetImpl.Success;

            Debug.Log("服务器发送，所有玩家准备：" + packetImpl.Success);
        }
    }
}