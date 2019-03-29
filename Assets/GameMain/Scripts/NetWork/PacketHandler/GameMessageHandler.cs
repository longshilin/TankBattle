using GameFramework;
using GameFramework.Network;
using GameFramework.ObjectPool;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace TankBattle {

    public class GameMessageHandler : PacketHandlerBase {

        public override int Id {
            get {
                return 1111;
            }
        }

        public override void Handle(object sender, Packet packet) {
            GameMessage gameMessage = (GameMessage)packet;

            int frameIndex = gameMessage.Frame;
            List<Packet> list = new List<Packet>();

            Command[] comm = new Command[gameMessage.Command.Count];
            gameMessage.Command.CopyTo(comm, 0);    // comm 是一帧中的所有事件的帧命令数据
            for (int i = 0; i < comm.Length; i++) {
                if (comm[i].Type != -1) {
                    using (MemoryStream stream = new MemoryStream()) {
                        byte[] data = comm[i].Data.ToByteArray();
                        stream.Write(data, 0, data.Length);
                        stream.Position = 0;
                        //Debug.Log(stream.Length);
                        Type packetType = NetworkChannelHelper.GetAllPacketType(comm[i].Type);
                        object instance = Activator.CreateInstance(packetType);
                        Packet framePacket = (Packet)ProtobufHelper.FromStream(instance, stream);

                        list.Add(framePacket);  // list是一帧的所有数据包命令
                    }
                }
            }
            // 将帧数据保存下来
            //Debug.Log("AddOneFrame ----  frameIndex: " + frameIndex);
            GameEntry.LockManager.AddOneFrame(frameIndex, list);
        }

        #region 对包数据进行处理

        public void MsgHandle(List<Packet> list) {
            for (int i = 0; i < list.Count; i++) {
                int packId = list[i].Id;
                Packet framePacket = list[i];
                switch (packId) {
                    case 1006:
                        StartMoveReq startMoveReq = (StartMoveReq)framePacket;
                        Process(startMoveReq);
                        break;

                    case 1008:
                        ChangeDirReq changeDirReq = (ChangeDirReq)framePacket;
                        Process(changeDirReq);
                        break;

                    case 1010:
                        EndMoveReq endMoveReq = (EndMoveReq)framePacket;
                        Process(endMoveReq);
                        break;

                        //case 1016:
                        //    TransformReq transformReq = (TransformReq)framePacket;
                        //    Process(transformReq);
                        //    break;
                }
            }
        }

        //private void Process(TransformReq transformReq) {
        //    // transformReq handle
        //    string userId = transformReq.UserId;
        //    Tank tank = GameEntry.LockManager.GetActor(userId);
        //    if (tank != null) {
        //        tank.updateTransform = true;
        //        // 每一帧刷新tank坐标和朝向
        //        tank.PositionX = transformReq.PositionX;
        //        tank.PositionZ = transformReq.PositionZ;
        //        tank.RotationY = transformReq.RotationY;
        //    }
        //}

        private void Process(StartMoveReq startMoveReq) {
            // StartMoveReq handle
            string userId = startMoveReq.UserId;
            Tank tank = GameEntry.LockManager.GetActor(userId);
            if (tank != null) {
                tank.GetComponentInChildren<Thruster>().X = 0;
                tank.GetComponentInChildren<Thruster>().Y = 0;
            }
        }

        private void Process(ChangeDirReq changeDirReq) {
            // ChangeDirReq handle
            string userId = changeDirReq.UserId;
            Tank tank = GameEntry.LockManager.GetActor(userId);
            if (tank != null) {
                tank.GetComponentInChildren<Thruster>().X = changeDirReq.DirX;
                tank.GetComponentInChildren<Thruster>().Y = changeDirReq.DirY;
            }
        }

        private void Process(EndMoveReq endMoveReq) {
            // EndMoveReq handle
            string userId = endMoveReq.UserId;
            Tank tank = GameEntry.LockManager.GetActor(userId);
            tank.GetComponentInChildren<Thruster>().X = 0;
            tank.GetComponentInChildren<Thruster>().Y = 0;
        }

        #endregion 对包数据进行处理
    }
}