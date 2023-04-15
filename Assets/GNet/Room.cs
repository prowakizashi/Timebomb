using GNet.Network;
using GNet.Packets;
using System.Collections.Generic;
using System.Linq;

namespace GNet
{
    public class Room
    {
        public string Name { get; private set; }

        private int size = 1;
        public int Size
        {
            get { return size; }
            set
            {
                if (NetworkManager.LocalPlayer == NetworkManager.LeaderPlayer)
                {
                    size = value;
                    SetDirty();
                }
            }
        }

        private bool visible = true;
        public bool Visible
        {
            get { return visible; }
            set
            {
                if (NetworkManager.LocalPlayer == NetworkManager.LeaderPlayer)
                {
                    visible = value;
                    SetDirty();
                }
            }
        }

        private bool opened = true;
        public bool Opened
        {
            get { return opened; }
            set
            {
                if (NetworkManager.LocalPlayer == NetworkManager.LeaderPlayer)
                {
                    opened = value;
                    SetDirty();
                }
            }
        }

        private Player leader = null;
        public Player Leader { get { return leader; } set { SetPlayerLeader(value); } }

        public IDictionary<int, Player> Players { get; private set; }
        public List<Player> PlayerList = new List<Player>();

        private bool isDirty = false;

        public Room(string _name, int _size, bool _visible, bool _opened, Player player)
        {
            Players = new Dictionary<int, Player>();

            Name = _name;
            size = _size;
            visible = _visible;
            opened = _opened;
            leader = player;
        }

        public Player GetPlayer(int _playerId)
        {
            Player player;
            Players.TryGetValue(_playerId, out player);
            return player;
        }

        public int GetPlayerPos(int _playerId)
        {
            return PlayerList.FindIndex(_player => _player.PlayerId == _playerId);
        }

        public void SetPlayerLeader(Player player)
        {
            if (NetworkManager.LocalPlayer.IsLeader && player != NetworkManager.LocalPlayer)
            {
                PacketWriter.ChangePlayerLeader(player.PlayerId);
            }
        }

        private void SetDirty()
        {
            if (isDirty || NetworkClient.Instance.Status != EConnectionStatus.CONNECTED)
                return;

            isDirty = true;
            ThreadManager.AddMainThreadTask(() =>
            {
                PacketWriter.UpdateRoom(size, visible, opened);
                isDirty = false;
            });
        }

        public static class RoomInternalModifier
        {
            public static void UpdateRoomInfo(Room _room, int _size, bool _visible, bool _opened)
            {
                _room.size = _size;
                _room.visible = _visible;
                _room.opened = _opened;
            }

            public static void SetPlayerLeader(Room _room, Player _player)
            {
                _room.leader = _player;
            }

            public static void AddPlayers(Room _room, Player[] _players)
            {
                foreach (var _player in _players)
                {
                    _room.Players.Add(_player.PlayerId, _player);
                    _room.PlayerList.Add(_player);
                }
            }

            public static void AddPlayer(Room _room, Player _player)
            {
                _room.Players.Add(_player.PlayerId, _player);
                _room.PlayerList.Add(_player);
            }

            public static void RemovePlayer(Room _room, int _playerId)
            {
                _room.Players.Remove(_playerId);
                _room.PlayerList.RemoveAll(_player => _player.PlayerId == _playerId);
            }
        }
    }
}