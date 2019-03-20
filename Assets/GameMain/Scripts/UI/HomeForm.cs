//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework.ObjectPool;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace TankBattle {

    public class HomeForm : UGuiForm {
        private LoginRes loginRes = null;
        public Text UserName = null;
        public Text GoldNum = null;
        public Text CupNum = null;
        public Text StartBtn = null;
        public GameObject SearchCav = null;
        public Text SearchMsg = null;
        public Text CancelBtn = null;

        private ProcedureHome m_procedureHome = null;
        //private ProcedureLogin m_ProcedureLogin = null;

        public void StartBtnClick() {
        }

        protected override void OnOpen(object userData) {
            base.OnOpen(userData);
            m_procedureHome = (ProcedureHome)userData;
            if (GameEntry.User != null) {
                loginRes = GameEntry.User;
                UserName.text = loginRes.Account;
                GoldNum.text = loginRes.Gold.ToString();
                CupNum.text = loginRes.Cup.ToString();
            }
            else {
                UserName.text = "Error";
                GoldNum.text = "0";
                CupNum.text = "0";
            }

            SearchMsg.text = "匹配中";
            CancelBtn.text = "取消";
            StartBtn.text = "开始游戏";
        }

        private void Return2Game(object obj) {
            GameEntry.Base.GameSpeed = 1.0f;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds) {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            //检测匹配是否成功
            if (GameEntry.PlayerB != null) {
                if (GameEntry.PlayerB.UserId == 0) {
                    GameEntry.PlayerB = null;
                    SearchCav.SetActive(false);
                    //匹配失败
                    GameEntry.UI.OpenDialog(new DialogParams() {
                        Mode = 1,
                        Title = "匹配失败",
                        Message = "未匹配到玩家，请稍后重试",
                        ConfirmText = "确认",

                        OnClickConfirm = Return2Game
                    });
                }
                else {
                    m_procedureHome.StartGame();
                }
            }
        }

        public void SearchUser() {
            //SearchCav.SetActive(true);
            //SearchReq searchReq = new SearchReq();
            //searchReq.UserId = loginRes.UserId;
            //searchReq.Account = loginRes.Account;
            //searchReq.UserId = loginRes.UserId;
            //searchReq.Gold = loginRes.Gold;
            //searchReq.Cup = loginRes.Cup;
            //NetWorkChannel.send(searchReq);
            m_procedureHome.StartGame();
        }

        public void CancelSearch() {
            //发送取消请求

            SearchCav.SetActive(false);
            CancelSearchReq req = new CancelSearchReq();
            req.UserId = GameEntry.User.UserId;
            NetWorkChannel.send(req);
        }
    }
}