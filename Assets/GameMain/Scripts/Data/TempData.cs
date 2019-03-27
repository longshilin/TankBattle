//using System.Collections.Generic;

//namespace TankBattle {
//    public class TempData {
//        private static TempData mInstance;

//        public static TempData Instance {
//            get {
//                if (mInstance == null) {
//                    mInstance = new TempData();
//                    mInstance.init();
//                }
//                return mInstance;
//            }
//        }

//        public UserData mUserData = new UserData();
//        public FightData mFightData = new FightData();

//        // 初始化测试数据
//        public void init() {
//            UserData mUserData = mInstance.mUserData;
//            FightData mFightData = mInstance.mFightData;

//            // 玩家数据
//            mUserData.UserId = "10000";
//            mUserData.UserName = "TANK001";

//            // 匹配房间数据
//            mFightData.RoomId = 1;

//            #region 初始化玩家信息

//            PlayerInfo player1 = new PlayerInfo();
//            player1.UserId = "10000";
//            player1.UserName = "TANK001";
//            PlayerInfo player2 = new PlayerInfo();
//            player2.UserId = "10001";
//            player2.UserName = "TANK007";

//            #endregion 初始化玩家信息

//            mFightData.PlayerInfoList = new List<PlayerInfo>();
//            mFightData.PlayerInfoList.Add(player1);
//            mFightData.PlayerInfoList.Add(player2);
//        }
//    }

//    public class UserData {
//        public string UserId { get; set; }
//        public string UserName { get; set; }
//    }

//    public class FightData {
//        public int RoomId { get; set; }
//        public List<TankBattle.PlayerInfo> PlayerInfoList { get; set; }
//    }
//}