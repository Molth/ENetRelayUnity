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
    ///     加入房间请求
    /// </summary>
    internal struct JoinRoomRequest : IMemoryPackable<JoinRoomRequest>, INetworkMessage
    {
        /// <summary>
        ///     主机Id
        /// </summary>
        public uint HostId;

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="hostId">主机Id</param>
        public JoinRoomRequest(uint hostId) => HostId = hostId;

        /// <summary>
        ///     静态构造
        /// </summary>
        static JoinRoomRequest() => RegisterFormatter();

        /// <summary>
        ///     注册序列化器
        /// </summary>
        [Preserve]
        public static void RegisterFormatter()
        {
            if (!MemoryPackFormatterProvider.IsRegistered<JoinRoomRequest>())
                MemoryPackFormatterProvider.Register(new JoinRoomRequestFormatter());
            if (!MemoryPackFormatterProvider.IsRegistered<JoinRoomRequest[]>())
                MemoryPackFormatterProvider.Register(new ArrayFormatter<JoinRoomRequest>());
        }

        /// <summary>
        ///     序列化
        /// </summary>
        [Preserve]
        public static void Serialize(ref MemoryPackWriter writer, ref JoinRoomRequest value) => writer.WriteUnmanaged(value);

        /// <summary>
        ///     反序列化
        /// </summary>
        [Preserve]
        public static void Deserialize(ref MemoryPackReader reader, ref JoinRoomRequest value) => reader.ReadUnmanaged(out value);

        /// <summary>
        ///     序列化器
        /// </summary>
        [Preserve]
        private sealed class JoinRoomRequestFormatter : MemoryPackFormatter<JoinRoomRequest>
        {
            /// <summary>
            ///     序列化
            /// </summary>
            [Preserve]
            public override void Serialize(ref MemoryPackWriter writer, ref JoinRoomRequest value) => JoinRoomRequest.Serialize(ref writer, ref value);

            /// <summary>
            ///     反序列化
            /// </summary>
            [Preserve]
            public override void Deserialize(ref MemoryPackReader reader, ref JoinRoomRequest value) => JoinRoomRequest.Deserialize(ref reader, ref value);
        }
    }
}