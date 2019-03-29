//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace TankBattle {

    public class LoginForm : UGuiForm {
        public InputField UserName = null;

        public InputField PassWord = null;

        public Text Account = null;
        public Text Pass = null;

        public Text LoginBtnText = null;

        public InputField Host = null;
        public InputField Port = null;

        public static bool enterHome = false;
        public static string loginMsg = null;

        private ProcedureLogin m_ProcedureLogin = null;

        public void LoginBtnClick() {
            string userName = UserName.text;
            string pwd = PassWord.text;
            if (userName == null || ("").Equals(userName) || (" ").Equals(userName)) {
                GameEntry.UI.OpenDialog(new DialogParams() {
                    Mode = 1,
                    Title = "用户名",
                    Message = "请填写用户名",
                    ConfirmText = "确认",

                    OnClickConfirm = Return2Game
                });
            }
            else if (pwd == null || ("").Equals(pwd) || (" ").Equals(pwd)) {
                GameEntry.UI.OpenDialog(new DialogParams() {
                    Mode = 1,
                    Title = "密码",
                    Message = "请填写密码",
                    ConfirmText = "确认",
                    OnClickConfirm = Return2Game
                });
            }
            else {
                string host = Host.text;
                int port = Convert.ToInt32(Port.text);
                NetWorkChannel.InitNetWork(host, port);
                System.Threading.Thread.Sleep(3000);

                LoginReq loginReq = new LoginReq();
                loginReq.Username = userName;
                loginReq.Password = pwd;
                NetWorkChannel.send(loginReq);
            }
        }

        protected override void OnOpen(object userData) {
            base.OnOpen(userData);

            Account.text = "用户名：";
            Pass.text = "密  码：";
            LoginBtnText.text = "登陆";
            Host.text = "192.168.90.93";
            //Host.text = "127.0.0.1";
            Port.text = "50000";

            m_ProcedureLogin = (ProcedureLogin)userData;
            if (m_ProcedureLogin == null) {
                Log.Warning("ProcedureMenu is invalid when open MenuForm.");
                return;
            }
        }

        private void Return2Game(object obj) {
            GameEntry.Base.GameSpeed = 1.0f;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds) {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (enterHome) {
                m_ProcedureLogin.goHome();
            }
            if (loginMsg != null) {
                GameEntry.UI.OpenDialog(new DialogParams() {
                    Mode = 1,
                    Title = "登陆失败",
                    Message = loginMsg,
                    ConfirmText = "确认",

                    OnClickConfirm = Return2Game
                });

                loginMsg = null;
            }
        }
    }
}