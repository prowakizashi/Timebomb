using GNet.Network;
using GNet.Packets;
using System;
using UnityEngine;

namespace GNet
{
    public abstract class MonoBehaviourNetCallBacks : MonoBehaviour, INetworkCallback
    {
        public void OnEnable()
        {
            NetworkManager.RegisterCallbacks(this);
        }

        public void OnDisable()
        {
            NetworkManager.UnregisterCallbacks(this);
        }

        public virtual void OnConnectToServer() { }
        public virtual void OnDisconnectFromServer() { }
        public virtual void OnCreateRoom(Room room) { }
        public virtual void OnFailedToCreateRoom() { }
        public virtual void OnJoinRoom(Room room) { }
        public virtual void OnLeaveRoom() { }
        public virtual void OnFailedToJoinRoom(int _errorCode) { }
        public virtual void OnPlayerJoinedRoom(Player player) { }
        public virtual void OnPlayerLeftRoom(Player player) { }
        public virtual void OnLeaderRoomChange(Player player) { }
    }
}