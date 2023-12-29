//------------------------------------------------------------
// Erinn Network
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ENet;
using MemoryPack;

#pragma warning disable CS8618

namespace Erinn
{
    /// <summary>
    ///     网络服务器
    /// </summary>
    public sealed partial class NetworkServer
    {
        /// <summary>
        ///     服务器节点
        /// </summary>
        private readonly Host _serverPeer = new();

        /// <summary>
        ///     客户端连接
        /// </summary>
        private readonly Dictionary<uint, Peer> _connections = new();

        /// <summary>
        ///     正在监听
        /// </summary>
        public bool IsListening { get; private set; }

        /// <summary>
        ///     连接回调
        /// </summary>
        public event Action<uint> OnConnectedCallback;

        /// <summary>
        ///     断开回调
        /// </summary>
        public event Action<uint> OnDisconnectedCallback;

        /// <summary>
        ///     启动服务器
        /// </summary>
        /// <param name="port">端口</param>
        /// <param name="maxClients">最大连接数</param>
        public void Start(ushort port, int maxClients)
        {
            if (IsListening)
                return;
            Library.Initialize();
            var address = new Address { Port = port };
            _serverPeer.Create(address, maxClients);
            Log.Info($"服务器启动: 端口[{port}] 最大连接数量[{maxClients}]]");
            IsListening = true;
            StartPolling();
        }

        /// <summary>
        ///     销毁
        /// </summary>
        public void Dispose()
        {
            IsListening = false;
            if (_serverPeer.IsSet)
            {
                var peers = new List<Peer>(_connections.Values);
                foreach (var peer in peers)
                    peer.DisconnectNow(0);
                _connections.Clear();
                _serverPeer.Flush();
                _serverPeer.Dispose();
            }

            Library.Deinitialize();
        }

        /// <summary>
        ///     断开连接
        /// </summary>
        /// <param name="id">客户端Id</param>
        public void Disconnect(uint id)
        {
            if (!_connections.TryGetValue(id, out var peer))
                return;
            peer.DisconnectNow(0);
            _connections.Remove(id);
            OnDisconnectedCallback?.Invoke(id);
            Log.Info($"客户端断开[{id}] [{peer.EndPointString()}]");
        }

        /// <summary>
        ///     获取往返延迟时间
        /// </summary>
        /// <param name="clientId">客户端Id</param>
        /// <returns>获得的往返延迟时间</returns>
        public ulong GetCurrentRtt(uint clientId)
        {
            if (_connections.TryGetValue(clientId, out var peer))
                return peer.RoundTripTime;
            return 0UL;
        }

        /// <summary>
        ///     发送数据包
        /// </summary>
        /// <param name="id">客户端Id</param>
        /// <param name="obj">object</param>
        /// <typeparam name="T">类型</typeparam>
        public void Send<T>(uint id, T obj) where T : struct, INetworkMessage, IMemoryPackable<T>
        {
            if (!_connections.TryGetValue(id, out var peer))
                return;
            var packet = NetworkSerializer.CreatePacket(_messageChannel.RpcHashSize, obj);
            peer.Send(0, ref packet);
        }

        /// <summary>
        ///     发送数据包
        /// </summary>
        /// <param name="ids">客户端Ids</param>
        /// <param name="obj">object</param>
        /// <typeparam name="T">类型</typeparam>
        public void Broadcast<T>(uint[] ids, T obj) where T : struct, INetworkMessage, IMemoryPackable<T>
        {
            var hashSet = new HashSet<uint>();
            var peers = new List<Peer>();
            foreach (var id in ids)
            {
                if (hashSet.Add(id) && _connections.TryGetValue(id, out var peer))
                    peers.Add(peer);
            }

            if (peers.Count == 0)
                return;
            var packet = NetworkSerializer.CreatePacket(_messageChannel.RpcHashSize, obj);
            _serverPeer.Broadcast(0, ref packet, peers.ToArray());
        }

        /// <summary>
        ///     轮询节点
        /// </summary>
        private async void PollPeers()
        {
            while (IsListening)
            {
                var polled = false;
                while (!polled)
                {
                    if (_serverPeer.CheckEvents(out var netEvent) <= 0)
                    {
                        if (_serverPeer.Service(Timeout, out netEvent) <= 0)
                            break;
                        polled = true;
                    }

                    var id = netEvent.Peer.ID;
                    switch (netEvent.Type)
                    {
                        case EventType.None:
                            break;
                        case EventType.Connect:
                            _connections[id] = netEvent.Peer;
                            OnConnectedCallback?.Invoke(id);
                            Log.Info($"客户端连接[{id}] {netEvent.EndPointString()}");
                            break;
                        case EventType.Disconnect:
                            if (!_connections.Remove(id))
                                return;
                            OnDisconnectedCallback?.Invoke(id);
                            Log.Info($"客户端断开[{id}] {netEvent.EndPointString()}");
                            break;
                        case EventType.Receive:
                            var length = netEvent.Packet.Length;
                            netEvent.Packet.CopyTo(_buffer);
                            InvokeHandler(id, _buffer, length);
                            netEvent.Packet.Dispose();
                            break;
                        case EventType.Timeout:
                            netEvent.Peer.DisconnectNow(0);
                            Log.Info($"客户端超时[{id}] {netEvent.EndPointString()}");
                            break;
                    }
                }

                await UniTask.Delay(Tick);
                _serverPeer.Flush();
            }
        }
    }
}