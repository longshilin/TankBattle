using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using GameFramework;
using GameFramework.Event;
using GameFramework.Network;
using ProtoBuf;
using ProtoBuf.Meta;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace TankBattle {

    public class NetworkChannelHelper : INetworkChannelHelper {
        private readonly Dictionary<int, Type> m_ServerToClientPacketTypes = new Dictionary<int, Type>();
        private readonly Dictionary<int, Type> m_ClientToServerPacketTypes = new Dictionary<int, Type>();
        public static readonly Dictionary<int, Type> m_AllPacketTypes = new Dictionary<int, Type>();

        //private readonly MemoryStream m_CachedStream = new MemoryStream();
        private INetworkChannel m_NetworkChannel = null;

        /// <summary>
        /// 获取消息包头长度。
        /// </summary>
        public int PacketHeaderLength {
            get {
                return 8;
            }
        }

        /// <summary>
        /// 初始化网络频道辅助器。
        /// </summary>
        /// <param name="networkChannel">网络频道。</param>
        public void Initialize(INetworkChannel networkChannel) {
            m_NetworkChannel = networkChannel;

            // 反射注册包和包处理函数。
            Type packetBaseType = typeof(SCPacketBase);
            Type CSpacketBaseType = typeof(CSPacketBase);
            Type packetHandlerBaseType = typeof(PacketHandlerBase);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++) {
                if (!types[i].IsClass || types[i].IsAbstract) {
                    continue;
                }

                if (types[i].BaseType == packetBaseType) {
                    PacketBase packetBase = (PacketBase)Activator.CreateInstance(types[i]);
                    Type packetType = GetServerToClientPacketType(packetBase.Id);
                    if (packetType != null) {
                        Log.Warning("Already exist packet type '{0}', check '{1}' or '{2}'?.", packetBase.Id.ToString(), packetType.Name, packetBase.GetType().Name);
                        continue;
                    }

                    m_ServerToClientPacketTypes.Add(packetBase.Id, types[i]);
                    m_AllPacketTypes.Add(packetBase.Id, types[i]);
                }
                else if (types[i].BaseType == packetHandlerBaseType) {
                    IPacketHandler packetHandler = (IPacketHandler)Activator.CreateInstance(types[i]);
                    m_NetworkChannel.RegisterHandler(packetHandler);
                }
                else if (types[i].BaseType == CSpacketBaseType)
                {
                    PacketBase packetBase = (PacketBase)Activator.CreateInstance(types[i]);
                    Type packetType = GetClientToServerPacketType(packetBase.Id);
                    if (packetType != null)
                    {
                        Log.Warning("Already exist packet type '{0}', check '{1}' or '{2}'?.", packetBase.Id.ToString(), packetType.Name, packetBase.GetType().Name);
                        continue;
                    }

                    m_ClientToServerPacketTypes.Add(packetBase.Id, types[i]);
                    m_AllPacketTypes.Add(packetBase.Id, types[i]);
                }
            }

            // 获取框架事件组件
            EventComponent Event
                = UnityGameFramework.Runtime.GameEntry.GetComponent<EventComponent>();

            Event.Subscribe(UnityGameFramework.Runtime.NetworkConnectedEventArgs.EventId, OnNetworkConnected);
            Event.Subscribe(UnityGameFramework.Runtime.NetworkClosedEventArgs.EventId, OnNetworkClosed);
            Event.Subscribe(UnityGameFramework.Runtime.NetworkMissHeartBeatEventArgs.EventId, OnNetworkMissHeartBeat);
            Event.Subscribe(UnityGameFramework.Runtime.NetworkErrorEventArgs.EventId, OnNetworkError);
            Event.Subscribe(UnityGameFramework.Runtime.NetworkCustomErrorEventArgs.EventId, OnNetworkCustomError);
        }

        /// <summary>
        /// 关闭并清理网络频道辅助器。
        /// </summary>
        public void Shutdown() {
            // 获取框架事件组件
            EventComponent Event
                = UnityGameFramework.Runtime.GameEntry.GetComponent<EventComponent>();

            Event.Unsubscribe(UnityGameFramework.Runtime.NetworkConnectedEventArgs.EventId, OnNetworkConnected);
            Event.Unsubscribe(UnityGameFramework.Runtime.NetworkClosedEventArgs.EventId, OnNetworkClosed);
            Event.Unsubscribe(UnityGameFramework.Runtime.NetworkMissHeartBeatEventArgs.EventId, OnNetworkMissHeartBeat);
            Event.Unsubscribe(UnityGameFramework.Runtime.NetworkErrorEventArgs.EventId, OnNetworkError);
            Event.Unsubscribe(UnityGameFramework.Runtime.NetworkCustomErrorEventArgs.EventId, OnNetworkCustomError);

            m_NetworkChannel = null;
        }

        /// <summary>
        /// 发送心跳消息包。
        /// </summary>
        /// <returns>是否发送心跳消息包成功。</returns>
        public bool SendHeartBeat() {
            return true;
        }

        /// <summary>
        /// 序列化消息包。
        /// </summary>
        /// <typeparam name="T">消息包类型。</typeparam>
        /// <param name="packet">要序列化的消息包。</param>
        /// <returns>序列化后的消息包字节流。</returns>
        public byte[] Serialize<T>(T packet) where T : Packet {
            PacketBase packetImpl = packet as PacketBase;
            if (packetImpl == null) {
                Log.Warning("Packet is invalid.");
                return null;
            }

            if (packetImpl.PacketType != PacketType.ClientToServer) {
                Log.Warning("Send packet invalid.");
                return null;
            }

            // 恐怖的 GCAlloc，这里是例子，不做优化(这句注释是框架作者写的,我本人并不懂GC什么的)
            using (MemoryStream memoryStream = new MemoryStream()) {
                /* 以下内容为木头本人做的改动,不知道是否有错误的地方(虽然它运行起来是正确的),希望大家能帮忙指正 */
                // 因为头部消息有8字节长度，所以先跳过8字节

                memoryStream.Position = 8;
                Serializer.SerializeWithLengthPrefix(memoryStream, packet, PrefixStyle.Fixed32);

                // 头部消息
                CSPacketHeader packetHeader = ReferencePool.Acquire<CSPacketHeader>();
                packetHeader.Id = packet.Id;
                packetHeader.PacketLength = (int)memoryStream.Length - 8; // 消息内容长度需要减去头部消息长度

                memoryStream.Position = 0;
                Serializer.SerializeWithLengthPrefix(memoryStream, packetHeader, PrefixStyle.Fixed32);

                ReferencePool.Release(packetHeader);

                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// 反序列消息包头。
        /// </summary>
        /// <param name="source">要反序列化的来源流。</param>
        /// <param name="customErrorData">用户自定义错误数据。</param>
        /// <returns></returns>
        public IPacketHeader DeserializePacketHeader(Stream source, out object customErrorData) {
            // 注意：此函数并不在主线程调用！
            customErrorData = null;
            SCPacketHeader scHeader = ReferencePool.Acquire<SCPacketHeader>();
            MemoryStream memoryStream = source as MemoryStream;
            if (memoryStream != null) {
                // Byte[] memData = memoryStream.GetBuffer();
                Byte[] lengthData = new Byte[4];
                memoryStream.Read(lengthData, 0, 4);
                Byte[] idData = new Byte[4];
                memoryStream.Read(idData, 0, 4);
                Array.Reverse(lengthData);
                Array.Reverse(idData);
                int length = BitConverter.ToInt32(lengthData, 0);
                int id = BitConverter.ToInt32(idData, 0);
                scHeader.Id = id;
                scHeader.PacketLength = length;
                return scHeader;
            }
            return null;
            //return Serializer.DeserializeWithLengthPrefix<SCPacketHeader> (source, PrefixStyle.Fixed32);
            // return (IPacketHeader)RuntimeTypeModel.Default.Deserialize(source, ReferencePool.Acquire<SCPacketHeader>(), typeof(SCPacketHeader));
        }

        /// <summary>
        /// 反序列化消息包。
        /// </summary>
        /// <param name="packetHeader">消息包头。</param>
        /// <param name="source">要反序列化的来源流。</param>
        /// <param name="customErrorData">用户自定义错误数据。</param>
        /// <returns>反序列化后的消息包。</returns>
        public Packet DeserializePacket(IPacketHeader packetHeader, Stream source, out object customErrorData) {
            // 注意：此函数并不在主线程调用！
            customErrorData = null;

            SCPacketHeader scPacketHeader = packetHeader as SCPacketHeader;
            if (scPacketHeader == null) {
                Log.Warning("Packet header is invalid.");
                return null;
            }

            Packet packet = null;
            if (scPacketHeader.IsValid) {
                Type packetType = GetServerToClientPacketType(scPacketHeader.Id);
                if (packetType != null && source is MemoryStream) {
                    //packet = (Packet)RuntimeTypeModel.Default.DeserializeWithLengthPrefix(source, ReferencePool.Acquire(packetType), packetType, PrefixStyle.Fixed32, 0);
                    object instance = Activator.CreateInstance(packetType);
                    //packet = (Packet)ProtobufHelper.FromStream(packetType, (MemoryStream)source);
                    packet = (Packet)ProtobufHelper.FromStream(instance, (MemoryStream)source);
                }
                else {
                    Log.Warning("Can not deserialize packet for packet id '{0}'.", scPacketHeader.Id.ToString());
                }
            }
            else {
                Log.Warning("Packet header is invalid.");
            }

            ReferencePool.Release(scPacketHeader);

            return packet;
        }

        public static Type GetAllPacketType(int id)
        {
            Type type = null;
            if (m_AllPacketTypes.TryGetValue(id, out type))
            {
                return type;
            }

            return null;
        }

        private  Type GetServerToClientPacketType(int id) {
            Type type = null;
            if (m_ServerToClientPacketTypes.TryGetValue(id, out type)) {
                return type;
            }

            return null;
        }

        private Type GetClientToServerPacketType(int id)
        {
            Type type = null;
            if (m_ClientToServerPacketTypes.TryGetValue(id, out type))
            {
                return type;
            }

            return null;
        }

        private void OnNetworkConnected(object sender, GameEventArgs e) {
            UnityGameFramework.Runtime.NetworkConnectedEventArgs ne = (UnityGameFramework.Runtime.NetworkConnectedEventArgs)e;
            if (ne.NetworkChannel != m_NetworkChannel) {
                return;
            }

            Log.Info("Network channel '{0}' connected, local address '{1}:{2}', remote address '{3}:{4}'.", ne.NetworkChannel.Name, ne.NetworkChannel.LocalIPAddress, ne.NetworkChannel.LocalPort.ToString(), ne.NetworkChannel.RemoteIPAddress, ne.NetworkChannel.RemotePort.ToString());
        }

        private void OnNetworkClosed(object sender, GameEventArgs e) {
            UnityGameFramework.Runtime.NetworkClosedEventArgs ne = (UnityGameFramework.Runtime.NetworkClosedEventArgs)e;
            if (ne.NetworkChannel != m_NetworkChannel) {
                return;
            }

            Log.Info("Network channel '{0}' closed.", ne.NetworkChannel.Name);
        }

        private void OnNetworkMissHeartBeat(object sender, GameEventArgs e) {
            UnityGameFramework.Runtime.NetworkMissHeartBeatEventArgs ne = (UnityGameFramework.Runtime.NetworkMissHeartBeatEventArgs)e;
            if (ne.NetworkChannel != m_NetworkChannel) {
                return;
            }

            Log.Info("Network channel '{0}' miss heart beat '{1}' times.", ne.NetworkChannel.Name, ne.MissCount.ToString());

            if (ne.MissCount < 2) {
                return;
            }

            ne.NetworkChannel.Close();
        }

        private void OnNetworkError(object sender, GameEventArgs e) {
            UnityGameFramework.Runtime.NetworkErrorEventArgs ne = (UnityGameFramework.Runtime.NetworkErrorEventArgs)e;
            if (ne.NetworkChannel != m_NetworkChannel) {
                return;
            }

            Log.Info("Network channel '{0}' error, error code is '{1}', error message is '{2}'.", ne.NetworkChannel.Name, ne.ErrorCode.ToString(), ne.ErrorMessage);

            ne.NetworkChannel.Close();
        }

        private void OnNetworkCustomError(object sender, GameEventArgs e) {
            UnityGameFramework.Runtime.NetworkCustomErrorEventArgs ne = (UnityGameFramework.Runtime.NetworkCustomErrorEventArgs)e;
            if (ne.NetworkChannel != m_NetworkChannel) {
                return;
            }
        }

        //
        // 摘要:
        //     序列化消息包。
        //
        // 参数:
        //   packet:
        //     要序列化的消息包。
        //
        //   destination:
        //     要序列化的目标流。
        //
        // 类型参数:
        //   T:
        //     消息包类型。
        //
        // 返回结果:
        //     是否序列化成功。
        public bool Serialize<T>(T packet, Stream destination) where T : Packet {
            PacketBase packetImpl = packet as PacketBase;
            if (packetImpl == null) {
                Log.Warning("Packet is invalid.");
                return false;
            }

            if (packetImpl.PacketType != PacketType.ClientToServer) {
                Log.Warning("Send packet invalid.");
                return false;
            }

            // 恐怖的 GCAlloc，这里是例子，不做优化(这句注释是框架作者写的,我本人并不懂GC什么的)

            /* 以下内容为木头本人做的改动,不知道是否有错误的地方(虽然它运行起来是正确的),希望大家能帮忙指正 */
            // 因为头部消息有8字节长度，所以先跳过8字节
            using (MemoryStream memoryStream = new MemoryStream()) {
                memoryStream.SetLength(memoryStream.Capacity); // 此行防止 Array.Copy 的数据无法写入
                memoryStream.Position = 8;

                ProtobufHelper.ToStream(packet, memoryStream);

                //Serializer.SerializeWithLengthPrefix<T>(m_CachedStream, packet, PrefixStyle.Fixed32);
                //Serializer.Serialize(m_CachedStream, packet);
                //

                // 头部消息
                //CSPacketHeader packetHeader = ReferencePool.Acquire<CSPacketHeader>();

                memoryStream.Position = 0;
                int mid = packet.Id;
                int length = (int)memoryStream.Length - 4; // 消息内容长度需要减去头部消息长度
                byte[] midData = BitConverter.GetBytes(mid);
                byte[] lenData = BitConverter.GetBytes(length);
                //反转
                Array.Reverse(midData);
                Array.Reverse(lenData);

                memoryStream.Write(lenData, 0, 4);
                memoryStream.Position = 4;
                memoryStream.Write(midData, 0, 4);
                //Serializer.SerializeWithLengthPrefix(m_CachedStream, packetHeader, PrefixStyle.Fixed32BigEndian);

                //ReferencePool.Release(packetHeader);

                memoryStream.WriteTo(destination);
            }

            //Debug.Log("msgSend");
            return true;
        }
    }
}