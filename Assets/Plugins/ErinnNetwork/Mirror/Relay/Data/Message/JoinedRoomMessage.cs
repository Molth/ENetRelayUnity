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
    ///     加入房间信息
    /// </summary>
    internal struct JoinedRoomMessage : IMemoryPackable<JoinedRoomMessage>, INetworkMessage
    {
        /// <summary>
        ///     房间Id
        /// </summary>
        public uint RoomId;

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="roomId">房间Id</param>
        public JoinedRoomMessage(uint roomId) => RoomId = roomId;

        /// <summary>
        ///     静态构造
        /// </summary>
        static JoinedRoomMessage() => RegisterFormatter();

        /// <summary>
        ///     注册序列化器
        /// </summary>
        [Preserve]
        public static void RegisterFormatter()
        {
            if (!MemoryPackFormatterProvider.IsRegistered<JoinedRoomMessage>())
                MemoryPackFormatterProvider.Register(new JoinedRoomMessageFormatter());
            if (!MemoryPackFormatterProvider.IsRegistered<JoinedRoomMessage[]>())
                MemoryPackFormatterProvider.Register(new ArrayFormatter<JoinedRoomMessage>());
        }

        /// <summary>
        ///     序列化
        /// </summary>
        [Preserve]
        public static void Serialize(ref MemoryPackWriter writer, ref JoinedRoomMessage value) => writer.WriteUnmanaged(value);

        /// <summary>
        ///     反序列化
        /// </summary>
        [Preserve]
        public static void Deserialize(ref MemoryPackReader reader, ref JoinedRoomMessage value) => reader.ReadUnmanaged(out value);

        /// <summary>
        ///     序列化器
        /// </summary>
        [Preserve]
        private sealed class JoinedRoomMessageFormatter : MemoryPackFormatter<JoinedRoomMessage>
        {
            /// <summary>
            ///     序列化
            /// </summary>
            [Preserve]
            public override void Serialize(ref MemoryPackWriter writer, ref JoinedRoomMessage value) => JoinedRoomMessage.Serialize(ref writer, ref value);

            /// <summary>
            ///     反序列化
            /// </summary>
            [Preserve]
            public override void Deserialize(ref MemoryPackReader reader, ref JoinedRoomMessage value) => JoinedRoomMessage.Deserialize(ref reader, ref value);
        }
    }
}