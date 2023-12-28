//------------------------------------------------------------
// Erinn Network
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using System;
using Cysharp.Threading.Tasks;
using ENet;
using MemoryPack;

#pragma warning disable CS8618

namespace Erinn
{
    /// <summary>
    ///     网络客户端
    /// </summary>
    public sealed partial class NetworkClient
    {
        /// <summary>
        ///     客户端节点
        /// </summary>
        private readonly Host _clientPeer = new();

        /// <summary>
        ///     服务器连接
        /// </summary>
        private Peer _connection;

        /// <summary>
        ///     正在监听
        /// </summary>
        public bool IsListening { get; private set; }

        /// <summary>
        ///     已经连接
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        ///     连接回调
        /// </summary>
        public event Action OnConnectedCallback;

        /// <summary>
        ///     断开回调
        /// </summary>
        public event Action OnDisconnectedCallback;

        /// <summary>
        ///     启动客户端
        /// </summary>
        /// <param name="ipAddress">服务器Ip地址</param>
        /// <param name="port">服务器端口</param>
        public void Start(string ipAddress, ushort port)
        {
            if (Connected)
                return;
            Library.Initialize();
            var address = new Address { Port = port };
            address.SetHost(ipAddress);
            _clientPeer.Create();
            _clientPeer.Connect(address);
            Log.Info($"客户端连接: 地址[{ipAddress}] 端口[{port}]");
            IsListening = true;
            StartPolling();
        }

        /// <summary>
        ///     断开连接
        /// </summary>
        public void Disconnect()
        {
            if (!Connected)
                return;
            Dispose();
        }

        /// <summary>
        ///     销毁
        /// </summary>
        private void Dispose()
        {
            IsListening = false;
            if (_connection.IsSet)
                _connection.DisconnectNow(0);
            if (_clientPeer.IsSet)
                _clientPeer.Flush();
            _clientPeer.Dispose();
            Library.Deinitialize();
            Connected = false;
        }

        /// <summary>
        ///     获取往返延迟时间
        /// </summary>
        /// <returns>获得的往返延迟时间</returns>
        public ulong GetCurrentRtt()
        {
            if (_connection.IsSet)
                return _connection.RoundTripTime;
            return 0UL;
        }

        /// <summary>
        ///     发送数据包
        /// </summary>
        /// <param name="obj">object</param>
        /// <typeparam name="T">类型</typeparam>
        public void Send<T>(T obj) where T : struct, INetworkMessage, IMemoryPackable<T>
        {
            if (!Connected)
                return;
            var packet = NetworkSerializer.CreatePacket(_messageChannel.RpcHashSize, obj);
            _connection.Send(0, ref packet);
        }

        /// <summary>
        ///     轮询节点
        /// </summary>
        private async void PollPeer()
        {
            while (IsListening)
            {
                var polled = false;
                while (!polled)
                {
                    if (_clientPeer.CheckEvents(out var netEvent) <= 0)
                    {
                        if (_clientPeer.Service(Timeout, out netEvent) <= 0)
                            break;
                        polled = true;
                    }

                    switch (netEvent.Type)
                    {
                        case EventType.None:
                            break;
                        case EventType.Connect:
                            _connection = netEvent.Peer;
                            Connected = true;
                            OnConnectedCallback?.Invoke();
                            Log.Info("连接成功");
                            break;
                        case EventType.Disconnect:
                            OnDisconnected();
                            Log.Info("连接断开");
                            break;
                        case EventType.Receive:
                            var length = netEvent.Packet.Length;
                            netEvent.Packet.CopyTo(_buffer);
                            InvokeHandler(_buffer, length);
                            netEvent.Packet.Dispose();
                            break;
                        case EventType.Timeout:
                            OnTimeout();
                            Log.Info("连接超时");
                            break;
                    }
                }

                if (!IsListening)
                    break;
                await UniTask.Delay(Tick);
                _clientPeer.Flush();
            }
        }

        /// <summary>
        ///     断开连接回调
        /// </summary>
        private void OnDisconnected()
        {
            Dispose();
            OnDisconnectedCallback?.Invoke();
        }

        /// <summary>
        ///     超时回调
        /// </summary>
        private void OnTimeout()
        {
            Dispose();
            OnDisconnectedCallback?.Invoke();
        }
    }
}