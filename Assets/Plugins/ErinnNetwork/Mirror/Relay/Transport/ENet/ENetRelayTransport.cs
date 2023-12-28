//------------------------------------------------------------
// Erinn Network
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using System;
using Erinn;
using UnityEngine;

namespace Mirror
{
    /// <summary>
    ///     ENet Netcode 传输
    /// </summary>
    public sealed class ENetRelayTransport : Transport
    {
        /// <summary>
        ///     主机Id
        /// </summary>
        [Header("主机Id")] public uint HostId;

        /// <summary>
        ///     地址
        /// </summary>
        public string Address = "127.0.0.1";

        /// <summary>
        ///     端口
        /// </summary>
        public ushort Port = 7777;

        /// <summary>
        ///     缓冲区
        /// </summary>
        private byte[] _messageBuffer;

        /// <summary>
        ///     客户端连接
        /// </summary>
        private bool _clientConnected;

        /// <summary>
        ///     服务器有效
        /// </summary>
        private bool _serverActive;

        /// <summary>
        ///     ENet客户端
        /// </summary>
        private static UnityClient ENetClient => UnityClient.Singleton;

        /// <summary>
        ///     Start时调用
        /// </summary>
        private void Start()
        {
            _messageBuffer = new byte[Erinn.NetworkClient.BufferSize];
            ENetClient.SetTimeout(0U);
            ENetClient.OnDisconnectedCallback += OnDisconnectedCallback;
            ENetClient.RegisterHandler<JoinedRoomMessage>(OnJoinedRoomMessage);
            ENetClient.RegisterHandler<LeftRoomMessage>(OnLeftRoomMessage);
            ENetClient.RegisterHandler<RelayServerPacket>(OnRelayServerPacket);
            ENetClient.RegisterHandler<RelayPacket>(OnRelayPacket);
            ENetClient.RegisterHandler<CreateRoomResponse>(OnCreateRoomResponse);
            ENetClient.RegisterHandler<JoinRoomResponse>(OnJoinRoomResponse);
        }

        /// <summary>
        ///     是否支持
        /// </summary>
        public override bool Available() => Application.platform != RuntimePlatform.WebGLPlayer;

        /// <summary>
        ///     断开回调
        /// </summary>
        private void OnDisconnectedCallback()
        {
            OnClientDisconnected();
            ENetClient.OnConnectedCallback -= OnClientConnectedToMaster;
            ENetClient.OnConnectedCallback -= OnServerConnectedToMaster;
            NetworkManager.singleton.StopHost();
        }

        /// <summary>
        ///     启动客户端
        /// </summary>
        public override void ClientConnect(string address)
        {
            Address = address;
            ENetClient.OnConnectedCallback += OnClientConnectedToMaster;
            ENetClient.StartClient(Address, Port);
        }

        /// <summary>
        ///     启动客户端
        /// </summary>
        private void OnClientConnectedToMaster()
        {
            ENetClient.OnConnectedCallback -= OnClientConnectedToMaster;
            ENetClient.Send(new JoinRoomRequest { HostId = HostId });
        }

        /// <summary>
        ///     启动服务端
        /// </summary>
        public override void ServerStart()
        {
            ENetClient.OnConnectedCallback += OnServerConnectedToMaster;
            ENetClient.StartClient(Address, Port);
        }

        /// <summary>
        ///     启动服务端
        /// </summary>
        private void OnServerConnectedToMaster()
        {
            ENetClient.OnConnectedCallback -= OnServerConnectedToMaster;
            ENetClient.Send(new CreateRoomRequest());
        }

        /// <summary>
        ///     客户端连接
        /// </summary>
        /// <returns>客户端连接</returns>
        public override bool ClientConnected() => _clientConnected;

        /// <summary>
        ///     发送Relay数据
        /// </summary>
        /// <param name="payload">负载</param>
        /// <param name="channelId">通道</param>
        public override void ClientSend(ArraySegment<byte> payload, int channelId = Channels.Reliable)
        {
            var size = payload.Count;
            Buffer.BlockCopy(payload.Array, payload.Offset, _messageBuffer, 0, size);
            var buffer = _messageBuffer.AsSpan(0, size).ToArray();
            var relayServerPacket = new RelayServerPacket(buffer);
            ENetClient.Send(relayServerPacket);
        }

        /// <summary>
        ///     断开本地客户端
        /// </summary>
        public override void ClientDisconnect()
        {
            _clientConnected = false;
            if (!_serverActive)
                ENetClient.Disconnect();
        }

        /// <summary>
        ///     服务器Uri
        /// </summary>
        /// <returns>服务器Uri</returns>
        public override Uri ServerUri() => null;

        /// <summary>
        ///     服务器有效
        /// </summary>
        /// <returns>服务器有效</returns>
        public override bool ServerActive() => _serverActive;

        /// <summary>
        ///     发送Relay数据
        /// </summary>
        /// <param name="connectionId">客户端Id</param>
        /// <param name="payload">负载</param>
        /// <param name="channelId">通道</param>
        public override void ServerSend(int connectionId, ArraySegment<byte> payload, int channelId = Channels.Reliable)
        {
            var size = payload.Count;
            Buffer.BlockCopy(payload.Array, payload.Offset, _messageBuffer, 0, size);
            var buffer = _messageBuffer.AsSpan(0, size).ToArray();
            var relayPacket = new RelayPacket((uint)connectionId, buffer);
            ENetClient.Send(relayPacket);
        }

        /// <summary>
        ///     断开远程客户端
        /// </summary>
        /// <param name="connectionId">客户端Id</param>
        public override void ServerDisconnect(int connectionId) => ENetClient.Send(new DisconnectRemoteClientMessage((uint)connectionId));

        /// <summary>
        ///     服务器获取客户端地址
        /// </summary>
        /// <param name="connectionId">客户端Id</param>
        /// <returns>服务器获得的客户端地址</returns>
        public override string ServerGetClientAddress(int connectionId) => null;

        /// <summary>
        ///     停止服务器
        /// </summary>
        public override void ServerStop()
        {
            _serverActive = false;
            ENetClient.Disconnect();
        }

        /// <summary>
        ///     获取最大包容量
        /// </summary>
        /// <param name="channelId">通道</param>
        /// <returns>获得的最大包容量</returns>
        public override int GetMaxPacketSize(int channelId = Channels.Reliable) => Erinn.NetworkClient.BufferSize;

        /// <summary>
        ///     停止
        /// </summary>
        public override void Shutdown() => ENetClient.Disconnect();

        /// <summary>
        ///     加入房间
        ///     Only Server/Host
        /// </summary>
        private void OnJoinedRoomMessage(JoinedRoomMessage data)
        {
            var clientId = (int)data.RoomId;
            OnServerConnected(clientId);
        }

        /// <summary>
        ///     离开房间
        ///     Only Server/Host
        /// </summary>
        private void OnLeftRoomMessage(LeftRoomMessage data)
        {
            var clientId = (int)data.RoomId;
            OnServerDisconnected(clientId);
        }

        /// <summary>
        ///     Relay数据包
        ///     Only Client
        /// </summary>
        private void OnRelayServerPacket(RelayServerPacket data) => OnClientDataReceived(data.Payload, 0);

        /// <summary>
        ///     Relay数据包
        ///     Only Server/Host
        /// </summary>
        private void OnRelayPacket(RelayPacket data)
        {
            var clientId = (int)data.RoomId;
            OnServerDataReceived(clientId, data.Payload, 0);
        }

        /// <summary>
        ///     创建房间
        ///     Only Server/Host
        /// </summary>
        private void OnCreateRoomResponse(CreateRoomResponse data)
        {
            if (!data.Success)
            {
                _serverActive = false;
                NetworkManager.singleton.StopHost();
            }
            else
            {
                HostId = data.HostId;
                _serverActive = true;
            }
        }

        /// <summary>
        ///     加入房间
        ///     Only Client
        /// </summary>
        private void OnJoinRoomResponse(JoinRoomResponse data)
        {
            if (!data.Success)
            {
                _clientConnected = false;
                NetworkManager.singleton.StopHost();
            }
            else
            {
                _clientConnected = true;
                OnClientConnected();
            }
        }
    }
}