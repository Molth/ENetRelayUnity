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
    ///     中转数据包
    /// </summary>
    internal struct RelayPacket : IMemoryPackable<RelayPacket>, INetworkMessage
    {
        /// <summary>
        ///     房间Id
        /// </summary>
        public uint RoomId;

        /// <summary>
        ///     负载
        /// </summary>
        public byte[] Payload;

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="roomId">房间Id</param>
        /// <param name="payload">负载</param>
        public RelayPacket(uint roomId, byte[] payload)
        {
            RoomId = roomId;
            Payload = payload;
        }

        /// <summary>
        ///     静态构造
        /// </summary>
        static RelayPacket() => RegisterFormatter();

        /// <summary>
        ///     注册序列化器
        /// </summary>
        [Preserve]
        public static void RegisterFormatter()
        {
            if (!MemoryPackFormatterProvider.IsRegistered<RelayPacket>())
                MemoryPackFormatterProvider.Register(new RelayPacketFormatter());
            if (!MemoryPackFormatterProvider.IsRegistered<RelayPacket[]>())
                MemoryPackFormatterProvider.Register(new ArrayFormatter<RelayPacket>());
            if (!MemoryPackFormatterProvider.IsRegistered<byte[]>())
                MemoryPackFormatterProvider.Register(new ArrayFormatter<byte>());
        }

        /// <summary>
        ///     序列化
        /// </summary>
        [Preserve]
        public static void Serialize(ref MemoryPackWriter writer, ref RelayPacket value)
        {
            writer.WriteUnmanagedWithObjectHeader(2, value.RoomId);
            writer.WriteValue(value.Payload);
        }

        /// <summary>
        ///     反序列化
        /// </summary>
        [Preserve]
        public static void Deserialize(ref MemoryPackReader reader, ref RelayPacket value)
        {
            if (!reader.TryReadObjectHeader(out var num))
            {
                value = new RelayPacket();
            }
            else
            {
                uint roomId;
                byte[] payload;
                if (num == 2)
                {
                    reader.ReadUnmanaged(out roomId);
                    payload = reader.ReadUnmanagedArray<byte>();
                }
                else
                {
                    if (num > 2)
                    {
                        MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(RelayPacket), 2, num);
                        return;
                    }

                    roomId = 0U;
                    payload = null;
                    if (num != 0)
                    {
                        reader.ReadUnmanaged(out roomId);
                        if (num != 1)
                            reader.ReadValue(ref payload);
                    }
                }

                value = new RelayPacket(roomId, payload);
            }
        }

        /// <summary>
        ///     序列化器
        /// </summary>
        [Preserve]
        private sealed class RelayPacketFormatter : MemoryPackFormatter<RelayPacket>
        {
            /// <summary>
            ///     序列化
            /// </summary>
            [Preserve]
            public override void Serialize(ref MemoryPackWriter writer, ref RelayPacket value) => RelayPacket.Serialize(ref writer, ref value);

            /// <summary>
            ///     反序列化
            /// </summary>
            [Preserve]
            public override void Deserialize(ref MemoryPackReader reader, ref RelayPacket value) => RelayPacket.Deserialize(ref reader, ref value);
        }
    }
}