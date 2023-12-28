//------------------------------------------------------------
// Erinn Framework
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using ENet;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using EventType = ENet.EventType;

namespace Netcode.Transports.Enet
{
    /// <summary>
    ///     ENet传输
    /// </summary>
    [DefaultExecutionOrder(1000)]
    public sealed class ENetTransport : UnityTransport
    {
        /// <summary>
        ///     最大客户端数量
        /// </summary>
        [Header("最大客户端数量")] public int MaxClients = 100;

        /// <summary>
        ///     缓冲区容量
        /// </summary>
        [Header("缓冲区容量")] public int MessageBufferSize = 5120;

        /// <summary>
        ///     心跳间隔
        /// </summary>
        [Header("ENet")] [Header("心跳间隔")] public uint HeartbeatInterval = 500;

        /// <summary>
        ///     超时限制
        /// </summary>
        [Header("超时限制")] public uint TimeoutLimit = 32;

        /// <summary>
        ///     最小超时
        /// </summary>
        [Header("最小超时")] public uint TimeoutMinimum = 5000;

        /// <summary>
        ///     最大超时
        /// </summary>
        [Header("最大超时")] public uint TimeoutMaximum = 30000;

        /// <summary>
        ///     连接节点
        /// </summary>
        private readonly Dictionary<uint, Peer> _connectedEnetPeers = new();

        /// <summary>
        ///     缓冲区
        /// </summary>
        private byte[] _messageBuffer;

        /// <summary>
        ///     临时缓冲区
        /// </summary>
        private WeakReference _temporaryBufferReference;

        /// <summary>
        ///     主机
        /// </summary>
        private Host _host;

        /// <summary>
        ///     节点Id
        /// </summary>
        private uint _serverPeerId;

        /// <summary>
        ///     已经服务
        /// </summary>
        private bool _hasServiced;

        /// <summary>
        ///     是否支持
        /// </summary>
        public override bool IsSupported => Application.platform != RuntimePlatform.WebGLPlayer;

        /// <summary>
        ///     本地Id
        /// </summary>
        public override ulong ServerClientId => GetLocalClientId(0U, true);

        /// <summary>
        ///     Update时调用
        /// </summary>
        public void Update()
        {
            _host?.Flush();
            _hasServiced = false;
        }

        /// <summary>
        ///     发送数据包
        /// </summary>
        /// <param name="clientId">客户端Id</param>
        /// <param name="data">数据包</param>
        /// <param name="delivery">传输方式</param>
        public override void Send(ulong clientId, ArraySegment<byte> data, NetworkDelivery delivery)
        {
            var packet = new Packet();
            packet.Create(data.Array, data.Offset, data.Count, NetworkDeliveryToPacketFlag(delivery));
            GetEnetClientId(clientId, out var peerId);
            _connectedEnetPeers[peerId].Send(0, ref packet);
        }

        /// <summary>
        ///     轮询事件
        /// </summary>
        /// <param name="clientId">客户端Id</param>
        /// <param name="payload">负载</param>
        /// <param name="receiveTime">接收时间戳</param>
        /// <returns>轮询的事件</returns>
        public override NetworkEvent PollEvent(out ulong clientId, out ArraySegment<byte> payload, out float receiveTime)
        {
            if (_host.CheckEvents(out var @event) <= 0)
            {
                if (_hasServiced || _host.Service(0, out @event) <= 0)
                {
                    clientId = 0UL;
                    payload = new ArraySegment<byte>();
                    receiveTime = Time.realtimeSinceStartup;
                    return NetworkEvent.Nothing;
                }

                _hasServiced = true;
            }

            var peer1 = @event.Peer;
            var localClientId = (long)GetLocalClientId(peer1.ID, false);
            clientId = (ulong)localClientId;
            switch (@event.Type)
            {
                case EventType.Connect:
                    payload = new ArraySegment<byte>();
                    receiveTime = Time.realtimeSinceStartup;
                    peer1 = @event.Peer;
                    var id1 = (int)peer1.ID;
                    var peer2 = @event.Peer;
                    _connectedEnetPeers.Add((uint)id1, peer2);
                    peer1 = @event.Peer;
                    peer1.PingInterval(HeartbeatInterval);
                    peer1 = @event.Peer;
                    peer1.Timeout(TimeoutLimit, TimeoutMinimum, TimeoutMaximum);
                    return NetworkEvent.Connect;
                case EventType.Disconnect:
                    payload = new ArraySegment<byte>();
                    receiveTime = Time.realtimeSinceStartup;
                    peer1 = @event.Peer;
                    var id2 = (int)peer1.ID;
                    _connectedEnetPeers.Remove((uint)id2);
                    return NetworkEvent.Disconnect;
                case EventType.Receive:
                    receiveTime = Time.realtimeSinceStartup;
                    var length = @event.Packet.Length;
                    if (length > _messageBuffer.Length)
                    {
                        byte[] numArray;
                        if (_temporaryBufferReference != null && _temporaryBufferReference.IsAlive && ((byte[])_temporaryBufferReference.Target).Length >= length)
                        {
                            numArray = (byte[])_temporaryBufferReference.Target;
                        }
                        else
                        {
                            numArray = new byte[length];
                            _temporaryBufferReference = new WeakReference(numArray);
                        }

                        @event.Packet.CopyTo(numArray);
                        payload = new ArraySegment<byte>(numArray, 0, length);
                    }
                    else
                    {
                        @event.Packet.CopyTo(_messageBuffer);
                        payload = new ArraySegment<byte>(_messageBuffer, 0, length);
                    }

                    @event.Packet.Dispose();
                    return NetworkEvent.Data;
                case EventType.Timeout:
                    payload = new ArraySegment<byte>();
                    receiveTime = Time.realtimeSinceStartup;
                    peer1 = @event.Peer;
                    var id3 = (int)peer1.ID;
                    _connectedEnetPeers.Remove((uint)id3);
                    return NetworkEvent.Disconnect;
                default:
                    payload = new ArraySegment<byte>();
                    receiveTime = Time.realtimeSinceStartup;
                    return NetworkEvent.Nothing;
            }
        }

        /// <summary>
        ///     开始客户端
        /// </summary>
        /// <returns>成功启动客户端</returns>
        public override bool StartClient()
        {
            _host = new Host();
            _host.Create(1, 16);
            var address = new Address { Port = ConnectionData.Port };
            address.SetHost(ConnectionData.Address);
            var peer = _host.Connect(address, 1);
            peer.PingInterval(HeartbeatInterval);
            peer.Timeout(TimeoutLimit, TimeoutMinimum, TimeoutMaximum);
            _serverPeerId = peer.ID;
            return true;
        }

        /// <summary>
        ///     启动服务器
        /// </summary>
        /// <returns>成功启动服务器</returns>
        public override bool StartServer()
        {
            _host = new Host();
            _host.Create(new Address { Port = ConnectionData.Port }, MaxClients, 1);
            return true;
        }

        /// <summary>
        ///     断开连接
        /// </summary>
        /// <param name="clientId">客户端Id</param>
        public override void DisconnectRemoteClient(ulong clientId)
        {
            GetEnetClientId(_serverPeerId, out var peerId);
            _connectedEnetPeers[peerId].DisconnectNow(0U);
        }

        /// <summary>
        ///     断开连接
        /// </summary>
        public override void DisconnectLocalClient()
        {
            _host.Flush();
            GetEnetClientId(_serverPeerId, out var peerId);
            if (!_connectedEnetPeers.TryGetValue(peerId, out var peer))
                return;
            peer.DisconnectNow(0U);
        }

        /// <summary>
        ///     获取延迟
        /// </summary>
        /// <param name="clientId">客户端Id</param>
        /// <returns>获得的延迟</returns>
        public override ulong GetCurrentRtt(ulong clientId)
        {
            GetEnetClientId(clientId, out var peerId);
            if (_connectedEnetPeers.TryGetValue(peerId, out var peer))
                return peer.RoundTripTime;
            return TimeoutMaximum;
        }

        /// <summary>
        ///     关闭
        /// </summary>
        public override void Shutdown()
        {
            if (_host != null)
            {
                _host.Flush();
                _host.Dispose();
                _host = null;
            }

            _connectedEnetPeers.Clear();
            Library.Deinitialize();
        }

        /// <summary>
        ///     初始化
        /// </summary>
        /// <param name="networkManager">网络管理器</param>
        public override void Initialize(NetworkManager networkManager = null)
        {
            Library.Initialize();
            _connectedEnetPeers.Clear();
            _messageBuffer = new byte[MessageBufferSize];
        }

        /// <summary>
        ///     获取PacketFlags
        /// </summary>
        /// <param name="delivery">传输方式</param>
        /// <returns>获得的PacketFlags</returns>
        /// <exception cref="ArgumentOutOfRangeException">报错</exception>
        public PacketFlags NetworkDeliveryToPacketFlag(NetworkDelivery delivery) => delivery switch
        {
            NetworkDelivery.Unreliable => PacketFlags.Unsequenced,
            NetworkDelivery.UnreliableSequenced => PacketFlags.None,
            NetworkDelivery.Reliable => PacketFlags.Reliable,
            NetworkDelivery.ReliableSequenced => PacketFlags.Reliable,
            NetworkDelivery.ReliableFragmentedSequenced => PacketFlags.Reliable,
            _ => throw new ArgumentOutOfRangeException(nameof(delivery), delivery, null)
        };

        /// <summary>
        ///     获取本地客户端Id
        /// </summary>
        /// <param name="peerId">节点Id</param>
        /// <param name="isServer">是否服务器</param>
        /// <returns>获得的本地客户端Id</returns>
        public ulong GetLocalClientId(uint peerId, bool isServer) => !isServer ? peerId + 1U : 0UL;

        /// <summary>
        ///     获取ENet连接信息
        /// </summary>
        /// <param name="clientId">客户端Id</param>
        /// <param name="peerId">节点Id</param>
        public void GetEnetClientId(ulong clientId, out uint peerId) => peerId = clientId == 0UL ? _serverPeerId : (uint)clientId - 1U;
    }
}