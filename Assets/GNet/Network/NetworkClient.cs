using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Reflection;
using GNet.Protocols;
using GNet.Packets;
using static GNet.Player;

namespace GNet.Network
{
    public enum EConnectionStatus
    {
        DISCONNECTED,
        CONNECTING,
        CONNECTED
    }

    public class NetworkClient
    {
        private static NetworkClient instance = null;
        public static NetworkClient Instance
        {
            get
            {
                if (instance == null)
                    instance = new NetworkClient();
                return instance;
            }
        }

        public readonly int dataBufferSize = 4096;
        private string ip = "82.253.113.191";
        private int port = 26950;

        private int clientId = -1;
        public int ClientId
        {
            get { return clientId; }
            set { clientId = value; OnConnected(); }
        }

        public TCP tcp;
        
        public delegate void ConnectionUpdateHandler(bool connected);
        public ConnectionUpdateHandler OnTCPConnectionUpdate;
        
        public Dictionary<int, PacketReaderInfo> packetHandlers;
        private IDictionary<int, List<PacketBinder>> packetBinders;

        public delegate void OnClientConnectedHandler();
        public OnClientConnectedHandler OnClientConnected;

        public EConnectionStatus Status { get; private set; }

        private NetworkClient()
        {
            InitializeClientData();
            tcp = new TCP(this);

            Application.quitting += Disconnect;
        }

        public void ConnectToServer()
        {
            if (Status != EConnectionStatus.DISCONNECTED)
                return;

            Status = EConnectionStatus.CONNECTING;
            tcp.Connect(ip, port, 5);
        }

        public void OnFailedToConnect()
        {
            if (Status != EConnectionStatus.CONNECTING)
                return;

            Status = EConnectionStatus.DISCONNECTED;
            Debug.Log("Failed To connect");
        }

        private void OnConnected()
        {
            Status = EConnectionStatus.CONNECTED;

            Debug.Log("Connected to server.");
            Player player = NetworkManager.LocalPlayer;
            PacketWriter.CheckVersion(Application.version);
            PacketWriter.UpdatePlayer(player.PlayerName, player.IsReady);
            OnClientConnected?.Invoke();;
        }

        public void Disconnect()
        {
            if (Status == EConnectionStatus.DISCONNECTED)
                return;

            tcp.socket.Close();
            PlayerInternalModifier.Reset(NetworkManager.LocalPlayer);
            Status = EConnectionStatus.DISCONNECTED;

            Debug.Log("Disconnected from server.");
        }

        private void InitializeClientData()
        {
            packetHandlers = new Dictionary<int, PacketReaderInfo>();
            packetBinders = new Dictionary<int, List<PacketBinder>>();
            AddPacketReader(typeof(PacketReader));

            Debug.Log("Initialized packets.");
        }

        public void AddPacketReader(Type _readerType)
        {
            MethodInfo[] methods = _readerType.GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (MethodInfo mInfo in methods)
            {
                ReadPacketAttribute[] attributes = mInfo.GetCustomAttributes<ReadPacketAttribute>(true).ToArray();
                foreach (var attr in attributes)
                {
                    packetHandlers.Add(attr.Packet, new PacketReaderInfo(_readerType, mInfo));
                    packetBinders.Add(attr.Packet, new List<PacketBinder>());
                }
            }
        }

        public void RemovePacketReader(Type _readerType)
        {
            packetHandlers = packetHandlers.Where(pair =>
            {
                if (pair.Value.type == _readerType)
                    packetBinders.Remove(pair.Key);
                return pair.Value.type != _readerType;
            }).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public void AddPacketBinder(int _packet, PacketBinder _binder)
        {
            packetBinders[_packet].Add(_binder);
        }

        public void RemovePacketBinder(int _packet, object _instance)
        {
            packetBinders[_packet].RemoveAll((pb) => pb.instance == _instance);
        }

        public void InvokePacketBinders(int _packet, object[] parameters)
        {
            packetBinders[_packet].ForEach((pb) => pb.Invoke(parameters));
        }
    }
}