using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GNet.Network
{
    public interface INetworkCallback
    {
        void OnConnectToServer();
        void OnDisconnectFromServer();
        void OnFailedToCreateRoom();
        void OnJoinRoom(Room room);
        void OnLeaveRoom();
        void OnFailedToJoinRoom(int _errorCode);
        void OnPlayerJoinedRoom(Player player);
        void OnPlayerLeftRoom(Player player);
        void OnLeaderRoomChange(Player player);
    }
}