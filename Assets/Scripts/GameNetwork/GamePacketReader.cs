using GNet.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameNetwork
{
    public enum EGameServerPackets
    {
        sendCards = ServerPackets.lastServerPacket,
        sendRoles,
        hoverCard,
        watchCards,
        startRound,
        endRound,
        startTurn,
        endTurn,
        gameOver,
        returnToLobby
    }

    static class GamePacketReader
    {
        [ReadPacket(Packet = (int)EGameServerPackets.sendCards)]
        public static void SendCards(Packet _packet, out object[] _data)
        {
            Debug.Log("Packets: SendCards");
            int _length = _packet.ReadInt();
            int[] _cards = new int[_length];
            for (int i = 0; i < _length; ++i)
                _cards[i] = _packet.ReadInt();

            _data = new object[] { _cards };
        }

        [ReadPacket(Packet = (int)EGameServerPackets.sendRoles)]
        public static void SendRoles(Packet _packet, out object[] _data)
        {
            Debug.Log("Packets: SendRoles");
            int _length = _packet.ReadInt();
            int[] _roles = new int[_length];
            for (int i = 0; i < _length; ++i)
                _roles[i] = _packet.ReadInt();

            _data = new object[] { _roles };
        }

        [ReadPacket(Packet = (int)EGameServerPackets.hoverCard)]
        public static void HoverACard(Packet _packet, out object[] _data)
        {
            Debug.Log("Packets: HoverACard");
            int _cardId = _packet.ReadInt();
            bool _state = _packet.ReadBool();
            _data = new object[] { _cardId, _state };
        }

        [ReadPacket(Packet = (int)EGameServerPackets.watchCards)]
        public static void WatchCards(Packet _packet, out object[] _data)
        {
            Debug.Log("Packets: HoverACard");
            int _playerId = _packet.ReadInt();
            _data = new object[] { _playerId };
        }

        [ReadPacket(Packet = (int)EGameServerPackets.startRound)]
        public static void StartRound(Packet _packet, out object[] _data)
        {
            Debug.Log("Packets: StartRound");
            int _round = _packet.ReadInt();
            int _length = _packet.ReadInt();

            int[] _cards = new int[_length];
            for (int i = 0; i < _length; ++i)
                _cards[i] = _packet.ReadInt();

            _data = new object[] { _round, _cards };
        }

        [ReadPacket(Packet = (int)EGameServerPackets.endRound)]
        public static void EndRound(Packet _packet, out object[] _data)
        {
            Debug.Log("Packets: EndRound");
            _data = new object[] { };
        }

        [ReadPacket(Packet = (int)EGameServerPackets.startTurn)]
        public static void StartTurn(Packet _packet, out object[] _data)
        {
            Debug.Log("Packets: StartTurn");
            int _turn = _packet.ReadInt();
            int _playerId = _packet.ReadInt();

            _data = new object[] { _turn, _playerId };
        }

        [ReadPacket(Packet = (int)EGameServerPackets.endTurn)]
        public static void EndTurn(Packet _packet, out object[] _data)
        {
            Debug.Log("Packets: EndTurn");
            int _cardId = _packet.ReadInt();

            _data = new object[] { _cardId };
        }

        [ReadPacket(Packet = (int)EGameServerPackets.gameOver)]
        public static void GameOver(Packet _packet, out object[] _data)
        {
            Debug.Log("Packets: GameOver");
            int _type = _packet.ReadInt();
            int _count = _packet.ReadInt();

            _data = new object[] { _type, _count };
        }

        [ReadPacket(Packet = (int)EGameServerPackets.returnToLobby)]
        public static void ReturnToLobby(Packet _packet, out object[] _data)
        {
            Debug.Log("Packets: ReturnToLobby");
            _data = new object[] { };
        }
    }
}
