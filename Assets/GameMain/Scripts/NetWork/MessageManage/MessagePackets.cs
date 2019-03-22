using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ProtoBuf;

namespace TankBattle {

    public static class MessagePackets {
    }

    //[Serializable, ProtoContract(Name = @"LoginReq")]
    public partial class LoginReq : CSPacketBase {

        public override int Id {
            get {
                return 1001;
            }
        }

        public override void Clear() {
        }
    }

    public partial class LoginRes : SCPacketBase {

        public override int Id {
            get {
                return 1002;
            }
        }

        public override void Clear() {
        }
    }

    public partial class MatchReq : CSPacketBase {

        public override int Id {
            get {
                return 1003;
            }
        }

        public override void Clear() {
        }
    }

    public partial class MatchRes : SCPacketBase {

        public override int Id {
            get {
                return 1004;
            }
        }

        public override void Clear() {
        }
    }

    public partial class CancelSearchReq : CSPacketBase {

        public override int Id {
            get {
                return 1005;
            }
        }

        public override void Clear() {
        }
    }

    public partial class StartMoveReq : CSPacketBase {

        public override int Id {
            get {
                return 1006;
            }
        }

        public override void Clear() {
        }
    }

    public partial class StartMoveRes : CSPacketBase {

        public override int Id {
            get {
                return 1007;
            }
        }

        public override void Clear() {
        }
    }

    public partial class ChangeDirReq : CSPacketBase {

        public override int Id {
            get {
                return 1008;
            }
        }

        public override void Clear() {
        }
    }

    public partial class ChangeDirRes : CSPacketBase {

        public override int Id {
            get {
                return 1009;
            }
        }

        public override void Clear() {
        }
    }

    public partial class EndMoveReq : CSPacketBase {

        public override int Id {
            get {
                return 1010;
            }
        }

        public override void Clear() {
        }
    }

    public partial class EndMoveRes : CSPacketBase {

        public override int Id {
            get {
                return 1011;
            }
        }

        public override void Clear() {
        }
    }
}