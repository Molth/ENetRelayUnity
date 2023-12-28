//------------------------------------------------------------
// Erinn Network
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using System;
using System.Reflection;
using MemoryPack;

#pragma warning disable CS8601

namespace Erinn
{
    /// <summary>
    ///     网络服务器
    /// </summary>
    partial class NetworkServer
    {
        /// <summary>
        ///     信息通道
        /// </summary>
        private readonly NetworkServerMessageChannel _messageChannel = new();

        /// <summary>
        ///     切换哈希容量
        /// </summary>
        /// <param name="hashSize">哈希容量</param>
        public void ChangeRpcHashSize(RpcHashSize hashSize) => _messageChannel.ChangeRpcHashSize(hashSize);

        /// <summary>
        ///     注册命令句柄
        /// </summary>
        public void RegisterHandlers<T>(T listener) where T : IServerCallback => _messageChannel.RegisterHandlers(listener);

        /// <summary>
        ///     注册命令句柄
        /// </summary>
        public void RegisterHandlers<T>(T listener, Type type) where T : IServerCallback => _messageChannel.RegisterHandlers(listener, type);

        /// <summary>
        ///     注册命令句柄
        /// </summary>
        public void RegisterHandlers(Type type) => _messageChannel.RegisterHandlers(type);

        /// <summary>
        ///     注册命令句柄
        /// </summary>
        public void RegisterHandlers(Assembly assembly) => _messageChannel.RegisterHandlers(assembly);

        /// <summary>
        ///     注册命令句柄
        /// </summary>
        public void RegisterHandlers() => _messageChannel.RegisterHandlers();

        /// <summary>
        ///     注册命令句柄
        /// </summary>
        /// <param name="handler">句柄</param>
        /// <typeparam name="T">类型</typeparam>
        public void RegisterHandler<T>(Action<uint, T> handler) where T : struct, INetworkMessage, IMemoryPackable<T> => _messageChannel.RegisterHandler(handler);

        /// <summary>
        ///     移除命令句柄
        /// </summary>
        public void UnregisterHandlers<T>(T listener) where T : IServerCallback => _messageChannel.UnregisterHandlers(listener);

        /// <summary>
        ///     移除命令句柄
        /// </summary>
        public void UnregisterHandlers<T>(T listener, Type type) where T : IServerCallback => _messageChannel.UnregisterHandlers(listener, type);

        /// <summary>
        ///     移除命令句柄
        /// </summary>
        public void UnregisterHandlers(Type type) => _messageChannel.UnregisterHandlers(type);

        /// <summary>
        ///     移除命令句柄
        /// </summary>
        public void UnregisterHandlers(Assembly assembly) => _messageChannel.UnregisterHandlers(assembly);

        /// <summary>
        ///     移除命令句柄
        /// </summary>
        public void UnregisterHandlers() => _messageChannel.UnregisterHandlers();

        /// <summary>
        ///     移除命令句柄
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        public void UnregisterHandler<T>() where T : struct, INetworkMessage, IMemoryPackable<T> => _messageChannel.UnregisterHandler<T>();

        /// <summary>
        ///     清空命令句柄
        /// </summary>
        public void ClearHandlers() => _messageChannel.ClearHandlers();

        /// <summary>
        ///     调用句柄
        /// </summary>
        /// <param name="id">客户端Id</param>
        /// <param name="payload">网络数据包负载</param>
        public void InvokeHandler(uint id, byte[] payload)
        {
            if (NetworkSerializer.DeserializeNetworkPacket(payload, out var networkPacket))
                InvokeHandler(id, ref networkPacket);
        }

        /// <summary>
        ///     调用句柄
        /// </summary>
        /// <param name="id">客户端Id</param>
        /// <param name="payload">网络数据包负载</param>
        /// <param name="length">长度</param>
        public void InvokeHandler(uint id, byte[] payload, int length)
        {
            if (NetworkSerializer.DeserializeNetworkPacket(payload.AsSpan(0, length), out var networkPacket))
                InvokeHandler(id, ref networkPacket);
        }

        /// <summary>
        ///     调用句柄
        /// </summary>
        /// <param name="id">客户端Id</param>
        /// <param name="payload">网络数据包负载</param>
        public void InvokeHandler(uint id, ref Span<byte> payload)
        {
            if (NetworkSerializer.DeserializeNetworkPacket(payload, out var networkPacket))
                InvokeHandler(id, ref networkPacket);
        }

        /// <summary>
        ///     调用句柄
        /// </summary>
        /// <param name="id">客户端Id</param>
        /// <param name="payload">网络数据包负载</param>
        public void InvokeHandler(uint id, ref ArraySegment<byte> payload)
        {
            if (NetworkSerializer.DeserializeNetworkPacket(payload, out var networkPacket))
                InvokeHandler(id, ref networkPacket);
        }

        /// <summary>
        ///     调用句柄
        /// </summary>
        /// <param name="id">客户端Id</param>
        /// <param name="networkPacket">网络数据包</param>
        public void InvokeHandler(uint id, ref NetworkPacket networkPacket) => _messageChannel.InvokeHandler(id, ref networkPacket);
    }
}