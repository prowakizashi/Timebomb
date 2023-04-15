

using GNet.Network;
using GNet.Packets;

namespace GNet
{
    public class Player
    {
        public int PlayerId { get; private set; }

        private string playerName;
        public string PlayerName
        {
            get { return playerName; }
            set { if (IsLocal) { playerName = value; SetDirty(); } }
        }

        public bool IsLeader { get; private set; }

        private bool isReady;
        public bool IsReady
        {
            get { return isReady; }
            set { if (IsLocal) { isReady = value; SetDirty(); } }
        }

        public bool IsLocal { get; private set; }

        private bool isDirty = false;

        public Player(int _playerId, string _playerName, bool _isLocal, bool _isLeader, bool _isReady)
        {
            PlayerId = _playerId;
            playerName = _playerName;
            IsLocal = _isLocal;
            IsLeader = _isLeader;
            isReady = _isReady;

            if (IsLocal)
                NetworkManager.SetLocalPlayer(this);
        }

        private void SetDirty()
        {
            if (isDirty || NetworkClient.Instance.Status != EConnectionStatus.CONNECTED)
                return;

            isDirty = true;
            ThreadManager.AddMainThreadTask(() =>
            {
                PacketWriter.UpdatePlayer(playerName, isReady);
                isDirty = false;
            });
        }

        public static class PlayerInternalModifier
        {
            public static void UpdatePlayer(Player _player, string _name, bool _ready)
            {
                _player.playerName = _name;
                _player.isReady = _ready;
            }

            public static void SetLeader(Player _player, bool _value)
            {
                _player.IsLeader = _value;
            }

            public static void Reset(Player _player)
            {
                _player.IsLeader = false;
                _player.isReady = false;
            }
        }
    }
}