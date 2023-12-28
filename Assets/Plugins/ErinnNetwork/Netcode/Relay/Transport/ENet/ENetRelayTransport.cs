//------------------------------------------------------------
// Erinn Network
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Erinn
{
    /// <summary>
    ///     ENet Netcode 传输
    /// </summary>
    public sealed class ENetRelayTransport : UnityTransport
    {
        /// <summary>
        ///     主机Id
        /// </summary>
        [Header("主机Id")] public uint HostId;

        /// <summary>
        ///     缓冲区
        /// </summary>
        private byte[] _messageBuffer;

        /// <summary>
        ///     是否支持
        /// </summary>
        public override bool IsSupported => Application.platform != RuntimePlatform.WebGLPlayer;

        /// <summary>
        ///     ENet客户端
        /// </summary>
        private static UnityClient ENetClient => UnityClient.Singleton;

        /// <summary>
        ///     服务器的客户端Id
        /// </summary>
        public override ulong ServerClientId => 0UL;

        /// <summary>
        ///     Start时调用
        /// </summary>
        private void Start()
        {
            _messageBuffer = new byte[NetworkClient.BufferSize];
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
        ///     断开回调
        /// </summary>
        private void OnDisconnectedCallback()
        {
            ENetClient.OnConnectedCallback -= OnClientConnectedToMaster;
            ENetClient.OnConnectedCallback -= OnServerConnectedToMaster;
            NetworkManager.Singleton.Shutdown();
        }

        /// <summary>
        ///     执行传输事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="clientId">客户端Id</param>
        /// <param name="payload">负载</param>
        private void InvokeTransportEvent(NetworkEvent eventType, ulong clientId = 0UL, ArraySegment<byte> payload = default) => InvokeOnTransportEvent(eventType, clientId, payload, Time.realtimeSinceStartup);

        /// <summary>
        ///     发送Relay数据
        /// </summary>
        /// <param name="clientId">客户端Id</param>
        /// <param name="payload">负载</param>
        /// <param name="networkDelivery">传输方式</param>
        public override void Send(ulong clientId, ArraySegment<byte> payload, NetworkDelivery networkDelivery)
        {
            var size = payload.Count;
            Buffer.BlockCopy(payload.Array, payload.Offset, _messageBuffer, 0, size);
            var buffer = _messageBuffer.AsSpan(0, size).ToArray();
            if (clientId == 0UL)
                ClientSend(buffer);
            else
                ServerSend((uint)clientId, buffer);
        }

        /// <summary>
        ///     发送Relay数据
        /// </summary>
        /// <param name="payload">负载</param>
        private void ClientSend(byte[] payload)
        {
            var relayServerPacket = new RelayServerPacket(payload);
            ENetClient.Send(relayServerPacket);
        }

        /// <summary>
        ///     发送Relay数据
        /// </summary>
        /// <param name="clientId">客户端Id</param>
        /// <param name="payload">负载</param>
        private void ServerSend(uint clientId, byte[] payload)
        {
            var relayPacket = new RelayPacket(clientId, payload);
            ENetClient.Send(relayPacket);
        }

        /// <summary>
        ///     轮询事件
        /// </summary>
        /// <param name="clientId">客户端Id</param>
        /// <param name="payload">负载</param>
        /// <param name="receiveTime">接收时间戳</param>
        /// <returns>获得的事件类型</returns>
        public override NetworkEvent PollEvent(out ulong clientId, out ArraySegment<byte> payload, out float receiveTime)
        {
            clientId = 0UL;
            receiveTime = Time.realtimeSinceStartup;
            payload = default;
            return NetworkEvent.Nothing;
        }

        /// <summary>
        ///     启动客户端
        /// </summary>
        public override bool StartClient()
        {
            ENetClient.OnConnectedCallback += OnClientConnectedToMaster;
            ENetClient.StartClient(ConnectionData.Address, ConnectionData.Port);
            return true;
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
        public override bool StartServer()
        {
            ENetClient.OnConnectedCallback += OnServerConnectedToMaster;
            ENetClient.StartClient(ConnectionData.Address, ConnectionData.Port);
            return true;
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
        ///     断开远程客户端
        /// </summary>
        /// <param name="clientId">客户端Id</param>
        public override void DisconnectRemoteClient(ulong clientId) => ENetClient.Send(new DisconnectRemoteClientMessage((uint)clientId));

        /// <summary>
        ///     断开本地客户端
        /// </summary>
        public override void DisconnectLocalClient() => ENetClient.Disconnect();

        /// <summary>
        ///     获取往返延迟时间
        /// </summary>
        /// <param name="clientId">客户端Id</param>
        /// <returns>获得的往返延迟时间</returns>
        public override ulong GetCurrentRtt(ulong clientId) => ENetClient.GetCurrentRtt();

        /// <summary>
        ///     停止
        /// </summary>
        public override void Shutdown() => ENetClient.Disconnect();

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize(NetworkManager networkManager = null)
        {
        }

        /// <summary>
        ///     加入房间
        ///     Only Server/Host
        /// </summary>
        private void OnJoinedRoomMessage(JoinedRoomMessage data)
        {
            var clientId = (ulong)data.RoomId;
            InvokeTransportEvent(NetworkEvent.Connect, clientId);
        }

        /// <summary>
        ///     离开房间
        ///     Only Server/Host
        /// </summary>
        private void OnLeftRoomMessage(LeftRoomMessage data)
        {
            var clientId = (ulong)data.RoomId;
            InvokeTransportEvent(NetworkEvent.Disconnect, clientId);
        }

        /// <summary>
        ///     Relay数据包
        ///     Only Client
        /// </summary>
        private void OnRelayServerPacket(RelayServerPacket data) => OnClientDataReceived(data.Payload);

        /// <summary>
        ///     客户端接收数据包
        /// </summary>
        /// <param name="payload">数据包</param>
        private void OnClientDataReceived(ArraySegment<byte> payload) => InvokeTransportEvent(NetworkEvent.Data, 0UL, payload);

        /// <summary>
        ///     Relay数据包
        ///     Only Server/Host
        /// </summary>
        private void OnRelayPacket(RelayPacket data)
        {
            var clientId = (ulong)data.RoomId;
            OnServerDataReceived(clientId, data.Payload);
        }

        /// <summary>
        ///     服务器接收数据包
        /// </summary>
        /// <param name="clientId">客户端Id</param>
        /// <param name="payload">数据包</param>
        private void OnServerDataReceived(ulong clientId, ArraySegment<byte> payload) => InvokeTransportEvent(NetworkEvent.Data, clientId, payload);

        /// <summary>
        ///     创建房间
        ///     Only Server/Host
        /// </summary>
        private void OnCreateRoomResponse(CreateRoomResponse data)
        {
            if (!data.Success)
                NetworkManager.Singleton.Shutdown();
            else
                HostId = data.HostId;
        }

        /// <summary>
        ///     加入房间
        ///     Only Client
        /// </summary>
        private void OnJoinRoomResponse(JoinRoomResponse data)
        {
            if (!data.Success)
                NetworkManager.Singleton.Shutdown();
            else
                InvokeTransportEvent(NetworkEvent.Connect);
        }
    }
}