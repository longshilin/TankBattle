//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using System.Collections.Generic;
using GameFramework.Network;
using UnityEngine;

namespace TankBattle {

    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry : MonoBehaviour {

        public static BuiltinDataComponent BuiltinData {
            get;
            private set;
        }

        public static HPBarComponent HPBar {
            get;
            private set;
        }

        #region 用户数据

        public static NetData NetData {
            get;
            private set;
        }

        #endregion 用户数据

        private static void InitCustomComponents() {
            BuiltinData = UnityGameFramework.Runtime.GameEntry.GetComponent<BuiltinDataComponent>();
            HPBar = UnityGameFramework.Runtime.GameEntry.GetComponent<HPBarComponent>();
        }

        // 打印用户数据
        private void print() {
            Debug.Log("UserData : ");
            Debug.Log("UserId: " + NetData.mUserData.UserId + "UserName: " + NetData.mUserData.UserName);

            Debug.Log("FightData : ");
            Debug.Log("RoomId: " + NetData.mFightData.RoomId);
            List<PlayerInfo> list = NetData.mFightData.PlayerInfoList;
            for (int i = 0; i < list.Count; i++) {
                Debug.Log("UserId: " + list[i].UserId + "UserName: " + list[i].UserName + "Cold: " + list[i].Gold + "Cup: " + list[i].Cup);
            }
        }
    }
}