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
            List<PacketBase> list = new List<PacketBase>();

            Command[] comm = new Command[gameMessage.Command.Count];
            gameMessage.Command.CopyTo(comm, 0);    // comm 是一帧中的所有事件的帧命令数据
            for (int i = 0; i < comm.Length; i++) {
                using (MemoryStream stream = new MemoryStream()) {
                    byte[] data = comm[i].Data.ToByteArray();
                    stream.Write(data, 0, data.Length);
                    stream.Position = 0;
                    Debug.Log(stream.Length);
                    Type packetType = NetworkChannelHelper.GetAllPacketType(comm[i].Type);
                    object instance = Activator.CreateInstance(packetType);
                    PacketBase framePacket = (PacketBase)ProtobufHelper.FromStream(instance, stream);

                    list.Add(framePacket);  // list是一帧的所有数据包命令
                }
            }

            // 将帧数据保存下来
        }

        #region 对包数据进行处理

        public void MsgHandle(List<PacketBase> list) {
            for (int i = 0; i < list.Count; i++) {
                int packId = list[i].Id;
                PacketBase framePacket = list[i];
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
                }
            }
        }

        private void Process(StartMoveReq startMoveReq) {
            // StartMoveReq handle
            int userId = int.Parse(startMoveReq.UserId);
            Debug.Log("StartMoveReq handle");
        }

        private void Process(ChangeDirReq changeDirReq) {
            // ChangeDirReq handle
            int userId = int.Parse(changeDirReq.UserId);
            Tank tank = GameBase.Instance.GetActor(userId);
            if (tank != null) {
                tank.Angle = changeDirReq.Angle;
            }
            Debug.Log("ChangeDirReq handle");
        }

        private void Process(EndMoveReq endMoveReq) {
            // EndMoveReq handle
            int userId = int.Parse(endMoveReq.UserId);
            Debug.Log("EndMoveReq handle");
        }

        #endregion 对包数据进行处理
    }
}