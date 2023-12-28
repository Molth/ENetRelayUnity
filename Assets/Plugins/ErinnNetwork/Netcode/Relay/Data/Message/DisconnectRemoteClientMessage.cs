//------------------------------------------------------------
// Erinn Network
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

// ReSharper disable MemberHidesStaticFromOuterClass

namespace Erinn
{
    /// <summary>
    ///     断开客户端连接信息
    /// </summary>
    internal struct DisconnectRemoteClientMessage : IMemoryPackable<DisconnectRemoteClientMessage>, INetworkMessage
    {
        /// <summary>
        ///     房间Id
        /// </summary>
        public uint RoomId;

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="roomId">房间Id</param>
        public DisconnectRemoteClientMessage(uint roomId) => RoomId = roomId;

        /// <summary>
        ///     静态构造
        /// </summary>
        static DisconnectRemoteClientMessage() => RegisterFormatter();

        /// <summary>
        ///     注册序列化器
        /// </summary>
        [Preserve]
        public static void RegisterFormatter()
        {
            if (!MemoryPackFormatterProvider.IsRegistered<DisconnectRemoteClientMessage>())
                MemoryPackFormatterProvider.Register(new DisconnectRemoteClientMessageFormatter());
            if (!MemoryPackFormatterProvider.IsRegistered<DisconnectRemoteClientMessage[]>())
                MemoryPackFormatterProvider.Register(new ArrayFormatter<DisconnectRemoteClientMessage>());
        }

        /// <summary>
        ///     序列化
        /// </summary>
        [Preserve]
        public static void Serialize(ref MemoryPackWriter writer, ref DisconnectRemoteClientMessage value) => writer.WriteUnmanaged(value);

        /// <summary>
        ///     反序列化
        /// </summary>
        [Preserve]
        public static void Deserialize(ref MemoryPackReader reader, ref DisconnectRemoteClientMessage value) => reader.ReadUnmanaged(out value);

        /// <summary>
        ///     序列化器
        /// </summary>
        [Preserve]
        private sealed class DisconnectRemoteClientMessageFormatter : MemoryPackFormatter<DisconnectRemoteClientMessage>
        {
            /// <summary>
            ///     序列化
            /// </summary>
            [Preserve]
            public override void Serialize(ref MemoryPackWriter writer, ref DisconnectRemoteClientMessage value) => DisconnectRemoteClientMessage.Serialize(ref writer, ref value);

            /// <summary>
            ///     反序列化
            /// </summary>
            [Preserve]
            public override void Deserialize(ref MemoryPackReader reader, ref DisconnectRemoteClientMessage value) => DisconnectRemoteClientMessage.Deserialize(ref reader, ref value);
        }
    }
}