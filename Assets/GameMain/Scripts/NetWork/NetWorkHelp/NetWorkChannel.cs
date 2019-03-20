using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityGameFramework.Runtime;

namespace TankBattle
{
    class NetWorkChannel
    {
        private static GameFramework.Network.INetworkChannel m_Channel;
        private static NetworkChannelHelper m_NetworkChannelHelper;

        public static void InitNetWork(string host,int port)
        {
            // 启动服务器(服务端的代码随便找随便改的，大家可以忽略，假设有个服务端就行了)
            //Demo8_SocketServer.Start();
            // 获取框架网络组件
            NetworkComponent Network = UnityGameFramework.Runtime.GameEntry.GetComponent<NetworkComponent>();


            // 获取框架事件组件
            //EventComponent Event = UnityGameFramework.Runtime.GameEntry.GetComponent<EventComponent>();

            // Event.Subscribe(NetworkConnectedEventArgs.EventId, OnConnected);

            // 创建频道

            m_NetworkChannelHelper = new NetworkChannelHelper();

            m_Channel = Network.CreateNetworkChannel("testName", m_NetworkChannelHelper);

            // 连接服务器
            m_Channel.Connect(IPAddress.Parse(host), port);
        }

        public static void send(PacketBase packet)
        {
            m_Channel.Send(packet);
        }
    }
}
