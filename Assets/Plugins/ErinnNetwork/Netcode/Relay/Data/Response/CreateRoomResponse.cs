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
    ///     创建房间响应
    /// </summary>
    internal struct CreateRoomResponse : IMemoryPackable<CreateRoomResponse>, INetworkMessage
    {
        /// <summary>
        ///     是否成功
        /// </summary>
        public bool Success;

        /// <summary>
        ///     主机Id
        /// </summary>
        public uint HostId;

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="success">是否成功</param>
        /// <param name="hostId">主机Id</param>
        public CreateRoomResponse(bool success, uint hostId)
        {
            Success = success;
            HostId = hostId;
        }

        /// <summary>
        ///     静态构造
        /// </summary>
        static CreateRoomResponse() => RegisterFormatter();

        /// <summary>
        ///     注册序列化器
        /// </summary>
        [Preserve]
        public static void RegisterFormatter()
        {
            if (!MemoryPackFormatterProvider.IsRegistered<CreateRoomResponse>())
                MemoryPackFormatterProvider.Register(new CreateRoomResponseFormatter());
            if (!MemoryPackFormatterProvider.IsRegistered<CreateRoomResponse[]>())
                MemoryPackFormatterProvider.Register(new ArrayFormatter<CreateRoomResponse>());
        }

        /// <summary>
        ///     序列化
        /// </summary>
        [Preserve]
        public static void Serialize(ref MemoryPackWriter writer, ref CreateRoomResponse value) => writer.WriteUnmanaged(value);

        /// <summary>
        ///     反序列化
        /// </summary>
        [Preserve]
        public static void Deserialize(ref MemoryPackReader reader, ref CreateRoomResponse value) => reader.ReadUnmanaged(out value);

        /// <summary>
        ///     序列化器
        /// </summary>
        [Preserve]
        private sealed class CreateRoomResponseFormatter : MemoryPackFormatter<CreateRoomResponse>
        {
            /// <summary>
            ///     序列化
            /// </summary>
            [Preserve]
            public override void Serialize(ref MemoryPackWriter writer, ref CreateRoomResponse value) => CreateRoomResponse.Serialize(ref writer, ref value);

            /// <summary>
            ///     反序列化
            /// </summary>
            [Preserve]
            public override void Deserialize(ref MemoryPackReader reader, ref CreateRoomResponse value) => CreateRoomResponse.Deserialize(ref reader, ref value);
        }
    }
}