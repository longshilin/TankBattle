using GameFramework;
using GameFramework.Network;
using GameFramework.ObjectPool;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace TankBattle {

    public class LoginResHandler : PacketHandlerBase {

        public override int Id {
            get {
                return 1002;
            }
        }

        public override void Handle(object sender, Packet packet) {
            LoginRes packetImpl = (LoginRes)packet;
            //SCHello packetImpl = (SCHello) packet;
            Debug.Log("Demo8_HelloPacketHandler 收到消息： '{0}'." + packetImpl.UserName);

            if (packetImpl.UserId.Equals("0")) {
                GameEntry.User = packetImpl;
                LoginForm.enterHome = true;
            }
            else {
                LoginForm.loginMsg = "登陆失败:请检查密码";
            }
        }
    }
}