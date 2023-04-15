using GNet.Network;
using GNet.Packets;
using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace GNet.Protocols
{
    public class TCP
    {
        public TcpClient socket;
        private NetworkClient client;

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public TCP(NetworkClient _client)
        {
            client = _client;
        }

        public void Connect(string _ip, int _port, int _timeOut)
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = client.dataBufferSize,
                SendBufferSize = client.dataBufferSize
            };

            receiveBuffer = new byte[client.dataBufferSize];
            var result = socket.BeginConnect(_ip, _port, ConnectCallback, socket);

            ThreadManager.DelayTask(_timeOut, () =>
            {
                if (client.Status == EConnectionStatus.CONNECTING)
                {
                    socket.Close();
                    client.OnFailedToConnect();
                }
            });
        }

        private void ConnectCallback(IAsyncResult _result)
        {
            try
            {
                socket.EndConnect(_result);
            }
            catch (Exception)
            {
                client.OnFailedToConnect();
                return;
            }

            stream = socket.GetStream();
            receivedData = new Packet();
            stream.BeginRead(receiveBuffer, 0, client.dataBufferSize, ReceiveCallback, null);
        }

        public void Disconnect()
        {
            client.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (_packet != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via TCP: {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    client.Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, client.dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error receiving TCP data: {_ex}");
                Disconnect();
            }
        }

        private bool HandleData(byte[] _data)
        {
            receivedData.SetBytes(_data);

            int _packetLength;
            if (!ReadPacketLength(out _packetLength))
                return true;

            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.AddMainThreadTask(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        object[] _parameters;
                        NetworkClient.Instance.packetHandlers[_packetId].Invoke(_packet, out _parameters);
                        NetworkClient.Instance.InvokePacketBinders(_packetId, _parameters);
                    }
                });

                if (!ReadPacketLength(out _packetLength))
                    return true;
            }

            return (_packetLength <= 1);
        }

        private bool ReadPacketLength(out int _packetLength)
        {
            _packetLength = 0;
            if (receivedData.UnreadLength() >= 4)
                _packetLength = receivedData.ReadInt();

            return (_packetLength > 0);
        }
    }
}