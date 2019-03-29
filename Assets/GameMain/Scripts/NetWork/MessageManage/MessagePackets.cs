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

    public partial class ChangeDirReq : CSPacketBase {

        public override int Id {
            get {
                return 1008;
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

    public partial class PlayerInfoReq : CSPacketBase {

        public override int Id {
            get {
                return 1012;
            }
        }

        public override void Clear() {
        }
    }

    public partial class GameInitReq : CSPacketBase {

        public override int Id {
            get {
                return 1014;
            }
        }

        public override void Clear() {
        }
    }

    public partial class GameInitRes : SCPacketBase {

        public override int Id {
            get {
                return 1015;
            }
        }

        public override void Clear() {
        }
    }

    public partial class TransformReq : CSPacketBase {

        public override int Id {
            get {
                return 1016;
            }
        }

        public override void Clear() {
        }
    }

    public partial class TransformRes : SCPacketBase {

        public override int Id {
            get {
                return 1017;
            }
        }

        public override void Clear() {
        }
    }

    public partial class GameMessage : SCPacketBase {

        public override int Id {
            get {
                return 1111;
            }
        }

        public override void Clear() {
        }
    }
}