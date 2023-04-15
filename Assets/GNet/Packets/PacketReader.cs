using GNet.Network;
using static GNet.Player;
using static GNet.Room;

namespace GNet.Packets
{
    public static class PacketReader
    {
        [ReadPacket(Packet = (int)ServerPackets.welcome)]
        public static void Welcome(Packet _packet, out object[] _data)
        {
            int _clientId = _packet.ReadInt();
            NetworkClient.Instance.ClientId = _clientId;

            foreach (var mb in NetworkManager.MonoBehaviors)
                mb.OnConnectToServer();

            _data = new object[] { _clientId };
        }

        [ReadPacket(Packet = (int)ServerPackets.createRoom)]
        public static void CreateRoom(Packet _packet, out object[] _data)
        {
            bool _success = _packet.ReadBool();

            if (!_success)
            {
                foreach (var mb in NetworkManager.MonoBehaviors)
                    mb.OnFailedToCreateRoom();
            }

            _data = new object[] { };
        }

        [ReadPacket(Packet = (int)ServerPackets.joinRoomFailed)]
        public static void JoinRoomFailed(Packet _packet, out object[] _data)
        {
            int _errorCode = _packet.ReadInt();

            foreach (var mb in NetworkManager.MonoBehaviors)
                mb.OnFailedToJoinRoom(_errorCode);

            _data = new object[] { _errorCode };
        }

        [ReadPacket(Packet = (int)ServerPackets.joinRoom)]
        public static void JoinRoom(Packet _packet, out object[] _data)
        {
            string  _name           = _packet.ReadString();
            int     _size           = _packet.ReadInt();
            bool    _visible        = _packet.ReadBool();
            bool    _opened         = _packet.ReadBool();
            int     _leaderId       = _packet.ReadInt();
            int     _playerCount    = _packet.ReadInt();

            Player[] players = new Player[_playerCount];
            Player _leader = null;
            for (int i = 0; i < _playerCount; ++i)
            {
                int     _pId        = _packet.ReadInt();
                string  _playerName = _packet.ReadString();
                int     _clientId   = _packet.ReadInt();
                bool    _isReady    = _packet.ReadBool();
                players[i] = new Player(_pId, _playerName, _clientId == NetworkClient.Instance.ClientId, _pId == _leaderId, _isReady);

                if (_pId == _leaderId)
                    _leader = players[i];
            }

            Room _room = new Room(_name, _size, _visible, _opened, _leader);
            RoomInternalModifier.AddPlayers(_room, players);
            NetworkManager.SetCurrentRoom(_room);

            _data = new object[] { _room };
        }

        [ReadPacket(Packet = (int)ServerPackets.updateRoom)]
        public static void UpdateRoom(Packet _packet, out object[] _data)
        {
            int     _size       = _packet.ReadInt();
            bool    _visible    = _packet.ReadBool();
            bool    _opened     = _packet.ReadBool();

            if (NetworkManager.CurrentRoom != null)
            {
                RoomInternalModifier.UpdateRoomInfo(NetworkManager.CurrentRoom, _size, _visible, _opened);
                _data = new object[] { NetworkManager.CurrentRoom };
                return;
            }

            _data = new object[] { null };
        }

        [ReadPacket(Packet = (int)ServerPackets.playerJoin)]
        public static void PlayerJoin(Packet _packet, out object[] _data)
        {
            int     _playerId   = _packet.ReadInt();
            string  _playerName = _packet.ReadString();
            int     _clientId   = _packet.ReadInt();
            bool    _IsLeader   = _packet.ReadBool();

            if (NetworkManager.CurrentRoom != null)
            {
                Player _player;
                if (NetworkManager.LocalPlayer.PlayerId == _playerId)
                {
                    _player = NetworkManager.LocalPlayer;
                }
                else
                {
                    _player = new Player(_playerId, _playerName, _clientId == NetworkClient.Instance.ClientId, _IsLeader, false);
                    RoomInternalModifier.AddPlayer(NetworkManager.CurrentRoom, _player);
                }

                foreach (var mb in NetworkManager.MonoBehaviors)
                {
                    mb.OnPlayerJoinedRoom(_player);
                }

                _data = new object[] { _player };
                return;
            }

            _data = new object[] { null };
        }

        [ReadPacket(Packet = (int)ServerPackets.playerLeave)]
        public static void PlayerLeave(Packet _packet, out object[] _data)
        {
            int _playerId = _packet.ReadInt();

            if (NetworkManager.CurrentRoom != null)
            {
                Player _player = NetworkManager.CurrentRoom.GetPlayer(_playerId);
                RoomInternalModifier.RemovePlayer(NetworkManager.CurrentRoom, _playerId);
                foreach (var mb in NetworkManager.MonoBehaviors)
                {
                    mb.OnPlayerLeftRoom(_player);
                }

                _data = new object[] { _player };
                return;
            }

            _data = new object[] { null };
        }

        [ReadPacket(Packet = (int)ServerPackets.updatePlayer)]
        public static void UpdatePlayer(Packet _packet, out object[] _data)
        {
            int     _playerId   = _packet.ReadInt();
            string  _playerName = _packet.ReadString();
            bool    _isReady    = _packet.ReadBool();
            
            if (NetworkManager.CurrentRoom != null)
            {
                Player _player = NetworkManager.CurrentRoom.GetPlayer(_playerId);
                PlayerInternalModifier.UpdatePlayer(_player, _playerName, _isReady);

                _data = new object[] { _player };
                return;
            }

            _data = new object[] { null };
        }

        [ReadPacket(Packet = (int)ServerPackets.changeLeader)]
        public static void ChangeLeader(Packet _packet, out object[] _data)
        {
            int _playerId = _packet.ReadInt();

            if (NetworkManager.CurrentRoom != null)
            {
                PlayerInternalModifier.SetLeader(NetworkManager.LeaderPlayer, false);
                Player _player = NetworkManager.CurrentRoom.GetPlayer(_playerId);
                RoomInternalModifier.SetPlayerLeader(NetworkManager.CurrentRoom, _player);
                PlayerInternalModifier.SetLeader(NetworkManager.LeaderPlayer, true);

                foreach (var mb in NetworkManager.MonoBehaviors)
                {
                    mb.OnLeaderRoomChange(_player);
                }

                _data = new object[] { _player };
                return;
            }

            _data = new object[] { null };
        }

        [ReadPacket(Packet = (int)ServerPackets.startGame)]
        public static void StartGame(Packet _packet, out object[] _data)
        {
            bool _start = _packet.ReadBool();
            _data = new object[] { _start };
        }

        [ReadPacket(Packet = (int)ServerPackets.playerMessage)]
        public static void ReceivePlayerMessage(Packet _packet, out object[] _data)
        {
            int _senderId = _packet.ReadInt();
            string _message = _packet.ReadString();
            
            Player _player = NetworkManager.CurrentRoom.GetPlayer(_senderId);
            _data = new object[] { _player, _message };
        }

        [ReadPacket(Packet = (int)ServerPackets.systemMessage)]
        public static void ReceiveSystemMessage(Packet _packet, out object[] _data)
        {
            int _messageId = _packet.ReadInt();
            _data = new object[] { _messageId };
        }
    }
}
