
using GNet.Packets;

namespace Assets.Scripts.GameNetwork
{
    public enum EGameClientPackets
    {
        isReady = ClientPackets.lastClientPacket,
        hoverCard,
        watchCards,
        pickCard,
        returnToLobby
    }

    class GamePacketWriter
    {
        public static void IsGameReady()
        {
            using (Packet _packet = new Packet((int)EGameClientPackets.isReady))
            {
                PacketWriter.SendTCPData(_packet);
            }
        }

        public static void HoverACard(int _cardId, bool _state)
        {
            using (Packet _packet = new Packet((int)EGameClientPackets.hoverCard))
            {
                _packet.Write(_cardId);
                _packet.Write(_state);
                PacketWriter.SendTCPData(_packet);
            }
        }

        public static void WatchCards()
        {
            using (Packet _packet = new Packet((int)EGameClientPackets.watchCards))
            {
                PacketWriter.SendTCPData(_packet);
            }
        }

        public static void PickACard(int _cardId)
        {
            using (Packet _packet = new Packet((int)EGameClientPackets.pickCard))
            {
                _packet.Write(_cardId);
                PacketWriter.SendTCPData(_packet);
            }
        }

        public static void ReturnToLobby()
        {
            using (Packet _packet = new Packet((int)EGameClientPackets.returnToLobby))
            {
                PacketWriter.SendTCPData(_packet);
            }
        }
    }
}
