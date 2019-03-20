using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ProtoBuf;

namespace TankBattle
{
    public static class MessagePackets
    {
       

    }

    

    //[Serializable, ProtoContract(Name = @"LoginReq")]
    public partial class LoginReq : CSPacketBase
    {
        public override int Id
        {
            get
            {
                return 1001;
            }
        }

        public override void Clear()
        {

        }
    }

    public partial class LoginRes : SCPacketBase 
    {
        public override int Id
        {
            get
            {
                return 1002;
            }
        }

        public override void Clear()
        {

        }
    }

    public partial class SearchReq : CSPacketBase
    {
        public override int Id
        {
            get
            {
                return 1003;
            }
        }

        public override void Clear()
        {

        }
    }

    public partial class SearchRes : SCPacketBase
    {
        public override int Id
        {
            get
            {
                return 1004;
            }
        }

        public override void Clear()
        {

        }
    }


    public partial class CancelSearchReq : CSPacketBase
    {
        public override int Id
        {
            get
            {
                return 1005;
            }
        }

        public override void Clear()
        {

        }
    }


}
