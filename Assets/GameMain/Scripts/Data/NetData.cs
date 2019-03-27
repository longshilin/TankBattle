using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankBattle {

    public class NetData {
        //private static NetData mInstance;

        //public static NetData Instance {
        //    get {
        //        if (mInstance == null)
        //            mInstance = new NetData();
        //        return mInstance;
        //    }
        //}

        public UserData mUserData = new UserData();
        public FightData mFightData = new FightData();
    }

    public class UserData {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int Gold { get; set; }
        public int Cup { get; set; }
    }

    public class FightData {
        public int RoomId { get; set; }
        public List<PlayerInfo> PlayerInfoList { get; set; }
    }
}