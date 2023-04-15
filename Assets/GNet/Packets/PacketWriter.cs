using GNet.Network;

namespace GNet.Packets
{
    public static class PacketWriter
    {
        public static void SendTCPData(Packet _packet)
        {
            _packet.WriteLength();
            NetworkClient.Instance.tcp.SendData(_packet);
        }

        public static void CheckVersion(string _version)
        {
            using (Packet _packet = new Packet((int)ClientPackets.version))
            {
                if (_version == "")
                    _packet.Write("none");
                else
                    _packet.Write(_version);

                SendTCPData(_packet);
            }
        }

        public static void Disconnect()
        {
            using (Packet _packet = new Packet((int)ClientPackets.goodBye))
            {
                SendTCPData(_packet);
            }
        }

        public static void UpdatePlayer(string _playerName, bool _isReady)
        {
            using (Packet _packet = new Packet((int)ClientPackets.updatePlayer))
            {
                _packet.Write(_playerName);
                _packet.Write(_isReady);

                SendTCPData(_packet);
            }
        }

        public static void ChangePlayerLeader(int _playerId)
        {
            using (Packet _packet = new Packet((int)ClientPackets.changePlayerLeader))
            {
                _packet.Write(_playerId);

                SendTCPData(_packet);
            }
        }

        public static void CreateRoom(string _name, int _size, bool _visible, bool _opened)
        {
            using (Packet _packet = new Packet((int)ClientPackets.createRoom))
            {
                _packet.Write(_name);
                _packet.Write(_size);
                _packet.Write(_visible);
                _packet.Write(_opened);

                SendTCPData(_packet);
            }
        }

        public static void UpdateRoom(int _size, bool _visible, bool _opened)
        {
            using (Packet _packet = new Packet((int)ClientPackets.updateRoom))
            {
                _packet.Write(_size);
                _packet.Write(_visible);
                _packet.Write(_opened);

                SendTCPData(_packet);
            }
        }

        public static void JoinRoom(string _name)
        {
            using (Packet _packet = new Packet((int)ClientPackets.joinRoom))
            {
                _packet.Write(_name);

                SendTCPData(_packet);
            }
        }

        public static void LeaveRoom()
        {
            using (Packet _packet = new Packet((int)ClientPackets.leaveRoom))
            {
                SendTCPData(_packet);
            }
        }

        public static void TryStartGame()
        {
            using (Packet _packet = new Packet((int)ClientPackets.tryStartGame))
            {
                SendTCPData(_packet);
            }
        }

        public static void SendPlayerMessage(string _message)
        {
            using (Packet _packet = new Packet((int)ClientPackets.playerMessage))
            {
                _packet.Write(_message);

                SendTCPData(_packet);
            }
        }
    }
}