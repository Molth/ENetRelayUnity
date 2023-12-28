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
    ///     加入房间响应
    /// </summary>
    internal struct JoinRoomResponse : IMemoryPackable<JoinRoomResponse>, INetworkMessage
    {
        /// <summary>
        ///     是否成功
        /// </summary>
        public bool Success;

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="success">是否成功</param>
        public JoinRoomResponse(bool success) => Success = success;

        /// <summary>
        ///     静态构造
        /// </summary>
        static JoinRoomResponse() => RegisterFormatter();

        /// <summary>
        ///     注册序列化器
        /// </summary>
        [Preserve]
        public static void RegisterFormatter()
        {
            if (!MemoryPackFormatterProvider.IsRegistered<JoinRoomResponse>())
                MemoryPackFormatterProvider.Register(new JoinRoomResponseFormatter());
            if (!MemoryPackFormatterProvider.IsRegistered<JoinRoomResponse[]>())
                MemoryPackFormatterProvider.Register(new ArrayFormatter<JoinRoomResponse>());
        }

        /// <summary>
        ///     序列化
        /// </summary>
        [Preserve]
        public static void Serialize(ref MemoryPackWriter writer, ref JoinRoomResponse value) => writer.WriteUnmanaged(value);

        /// <summary>
        ///     反序列化
        /// </summary>
        [Preserve]
        public static void Deserialize(ref MemoryPackReader reader, ref JoinRoomResponse value) => reader.ReadUnmanaged(out value);

        /// <summary>
        ///     序列化器
        /// </summary>
        [Preserve]
        private sealed class JoinRoomResponseFormatter : MemoryPackFormatter<JoinRoomResponse>
        {
            /// <summary>
            ///     序列化
            /// </summary>
            [Preserve]
            public override void Serialize(ref MemoryPackWriter writer, ref JoinRoomResponse value) => JoinRoomResponse.Serialize(ref writer, ref value);

            /// <summary>
            ///     反序列化
            /// </summary>
            [Preserve]
            public override void Deserialize(ref MemoryPackReader reader, ref JoinRoomResponse value) => JoinRoomResponse.Deserialize(ref reader, ref value);
        }
    }
}