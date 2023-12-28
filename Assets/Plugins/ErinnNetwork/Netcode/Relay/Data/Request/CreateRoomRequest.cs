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
    ///     创建房间请求
    /// </summary>
    internal struct CreateRoomRequest : IMemoryPackable<CreateRoomRequest>, INetworkMessage
    {
        /// <summary>
        ///     静态构造
        /// </summary>
        static CreateRoomRequest() => RegisterFormatter();

        /// <summary>
        ///     注册序列化器
        /// </summary>
        [Preserve]
        public static void RegisterFormatter()
        {
            if (!MemoryPackFormatterProvider.IsRegistered<CreateRoomRequest>())
                MemoryPackFormatterProvider.Register(new CreateRoomRequestFormatter());
            if (!MemoryPackFormatterProvider.IsRegistered<CreateRoomRequest[]>())
                MemoryPackFormatterProvider.Register(new ArrayFormatter<CreateRoomRequest>());
        }

        /// <summary>
        ///     序列化
        /// </summary>
        [Preserve]
        public static void Serialize(ref MemoryPackWriter writer, ref CreateRoomRequest value) => writer.WriteUnmanaged(value);

        /// <summary>
        ///     反序列化
        /// </summary>
        [Preserve]
        public static void Deserialize(ref MemoryPackReader reader, ref CreateRoomRequest value) => reader.ReadUnmanaged(out value);

        /// <summary>
        ///     序列化器
        /// </summary>
        [Preserve]
        private sealed class CreateRoomRequestFormatter : MemoryPackFormatter<CreateRoomRequest>
        {
            /// <summary>
            ///     序列化
            /// </summary>
            [Preserve]
            public override void Serialize(ref MemoryPackWriter writer, ref CreateRoomRequest value) => CreateRoomRequest.Serialize(ref writer, ref value);

            /// <summary>
            ///     反序列化
            /// </summary>
            [Preserve]
            public override void Deserialize(ref MemoryPackReader reader, ref CreateRoomRequest value) => CreateRoomRequest.Deserialize(ref reader, ref value);
        }
    }
}