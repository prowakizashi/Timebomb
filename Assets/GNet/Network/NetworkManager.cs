using GNet.Packets;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static GNet.Network.NetworkClient;
using static GNet.Player;

namespace GNet.Network
{
    public static class NetworkManager
    {
        public static Room CurrentRoom { get; private set; }
        
        private static Player localPlayer = new Player(-1, "NoName", true, false, false);
        public static Player LocalPlayer { get { return localPlayer; } private set { localPlayer = value; } }
        
        public static Player LeaderPlayer { get { return CurrentRoom != null ? CurrentRoom.Leader : null; } set { SetPlayerLeader(value); } }

        public static string PlayerName { get { return localPlayer.PlayerName; } set { localPlayer.PlayerName = value; } }

        public static List<INetworkCallback> MonoBehaviors = new List<INetworkCallback>();

        private static bool paused = false;
        public static bool Paused { get { return paused; } set { paused = value; } }

        public static void RegisterCallbacks(MonoBehaviourNetCallBacks monoBehavior)
        {
            MonoBehaviors.Add(monoBehavior);
            
            List<MethodInfo> methods = monoBehavior.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance).ToList();
            methods.AddRange(monoBehavior.GetType().GetMethods());
            foreach (MethodInfo mInfo in methods)
            {
                BindPacketAttribute[] attributes = mInfo.GetCustomAttributes<BindPacketAttribute>(true).ToArray();
                foreach (var attr in attributes)
                {
                    NetworkClient.Instance.AddPacketBinder(attr.Packet, new PacketBinder(monoBehavior, mInfo));
                }
            }
        }

        public static void UnregisterCallbacks(MonoBehaviourNetCallBacks monoBehavior)
        {
            MonoBehaviors.Remove(monoBehavior);

            List<MethodInfo> methods = monoBehavior.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance).ToList();
            methods.AddRange(monoBehavior.GetType().GetMethods());
            foreach (MethodInfo mInfo in methods)
            {
                BindPacketAttribute[] attributes = mInfo.GetCustomAttributes<BindPacketAttribute>(true).ToArray();
                foreach (var attr in attributes)
                {
                    NetworkClient.Instance.RemovePacketBinder(attr.Packet, monoBehavior);
                }
            }
        }

        public static void ConnectToServer()
        {
            if (NetworkClient.Instance.Status == EConnectionStatus.DISCONNECTED)
                NetworkClient.Instance.ConnectToServer();
        }

        public static void CreateRoom(string _name, int _size, bool _visible, bool _opened)
        {
            if (CurrentRoom != null)
                return;

            if (NetworkClient.Instance.Status != EConnectionStatus.CONNECTED)
            {
                OnClientConnectedHandler OnConnect = null;

                OnConnect = () => {
                    CreateRoom(_name, _size, _visible, _opened);
                    NetworkClient.Instance.OnClientConnected -= OnConnect;
                };

                NetworkClient.Instance.OnClientConnected += OnConnect;
                NetworkClient.Instance.ConnectToServer();
                return;
            }

            PacketWriter.CreateRoom(_name, _size, _visible, _opened);
        }

        public static void JoinRoom(string _name)
        {
            if (CurrentRoom != null)
                return;

            if (NetworkClient.Instance.Status != EConnectionStatus.CONNECTED)
            {
                OnClientConnectedHandler OnConnect = null;
                OnConnect = () => {
                    PacketWriter.JoinRoom(_name);
                    NetworkClient.Instance.OnClientConnected -= OnConnect;
                };

                NetworkClient.Instance.OnClientConnected += OnConnect;
                NetworkClient.Instance.ConnectToServer();
                return;
            }

            PacketWriter.JoinRoom(_name);
        }

        public static void LeaveRoom()
        {
            if (NetworkManager.CurrentRoom != null)
            {
                PacketWriter.LeaveRoom();
                NetworkManager.CurrentRoom = null;
                PlayerInternalModifier.Reset(NetworkManager.LocalPlayer);
            }
        }

        public static void SetCurrentRoom(Room _room)
        {
            CurrentRoom = _room;
            if (CurrentRoom != null)
            {
                for (int i = 0; i < MonoBehaviors.Count; ++i)
                {
                    MonoBehaviors[i].OnJoinRoom(_room);
                }

            }
            else
            {
                for (int i = 0; i < MonoBehaviors.Count; ++i)
                {
                    MonoBehaviors[i].OnLeaveRoom();
                }
            }
        }

        public static void SetPlayerLeader(Player _player)
        {
            if (CurrentRoom != null)
            {
                CurrentRoom.SetPlayerLeader(_player);
            }
        }

        public static void SetLocalPlayer(Player _player)
        {
            localPlayer = _player;
        }
    }
}
