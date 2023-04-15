using GNet.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GNet
{
    public static class GameNetwork
    {
        public static EConnectionStatus ConnectionStatus { get { return NetworkClient.Instance == null ? EConnectionStatus.DISCONNECTED : NetworkClient.Instance.Status; } }
        public static Player LocalPlayer { get { return NetworkManager.LocalPlayer; } }
        public static Player LeaderPlayer { get { return NetworkManager.LeaderPlayer; } }
        public static Room CurrentRoom { get { return NetworkManager.CurrentRoom; } }
        public static string PlayerName { get { return NetworkManager.PlayerName; } set { NetworkManager.PlayerName = value; } }
        
        public static bool Paused { get { return NetworkManager.Paused; } set { NetworkManager.Paused = value; } }

        public static bool IsLeaderPlayer
        {
            get { return LocalPlayer != null ? LocalPlayer.IsLeader : false; }
        }

        public static List<Player> PlayerList { get { return CurrentRoom != null ? CurrentRoom.PlayerList : new List<Player>(); } }

        public static void ConnectToServer()
        {
            NetworkManager.ConnectToServer();
        }

        public static void CreateRoom(string _name, int _size, bool _visible = true, bool _opened = true)
        {
            NetworkManager.CreateRoom(_name, _size, _visible, _opened);
        }

        public static void JoinRoom(string _name)
        {
            NetworkManager.JoinRoom(_name);
        }

        public static void LeaveRoom()
        {
            NetworkManager.LeaveRoom();
        }

        public static void SetLeaderPlayer(Player player)
        {
            NetworkManager.SetPlayerLeader(player);
        }

        public static void AddPacketReader(Type _type)
        {
            NetworkClient.Instance.AddPacketReader(_type);
        }

        public static void RemovePacketReader(Type _type)
        {
            NetworkClient.Instance.RemovePacketReader(_type);
        }
    }
}

