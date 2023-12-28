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
    internal struct RelayServerPacket : IMemoryPackable<RelayServerPacket>, INetworkMessage
    {
        /// <summary>
        ///     负载
        /// </summary>
        public byte[] Payload;

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="payload">负载</param>
        public RelayServerPacket(byte[] payload) => Payload = payload;

        /// <summary>
        ///     静态构造
        /// </summary>
        static RelayServerPacket() => RegisterFormatter();

        /// <summary>
        ///     注册序列化器
        /// </summary>
        [Preserve]
        public static void RegisterFormatter()
        {
            if (!MemoryPackFormatterProvider.IsRegistered<RelayServerPacket>())
                MemoryPackFormatterProvider.Register(new RelayServerPacketFormatter());
            if (!MemoryPackFormatterProvider.IsRegistered<RelayServerPacket[]>())
                MemoryPackFormatterProvider.Register(new ArrayFormatter<RelayServerPacket>());
            if (!MemoryPackFormatterProvider.IsRegistered<byte[]>())
                MemoryPackFormatterProvider.Register(new ArrayFormatter<byte>());
        }

        /// <summary>
        ///     序列化
        /// </summary>
        [Preserve]
        public static void Serialize(ref MemoryPackWriter writer, ref RelayServerPacket value)
        {
            writer.WriteObjectHeader(1);
            writer.WriteValue(value.Payload);
        }

        /// <summary>
        ///     反序列化
        /// </summary>
        [Preserve]
        public static void Deserialize(ref MemoryPackReader reader, ref RelayServerPacket value)
        {
            if (!reader.TryReadObjectHeader(out var num))
            {
                value = new RelayServerPacket();
            }
            else
            {
                byte[] payload;
                if (num == 1)
                {
                    payload = reader.ReadUnmanagedArray<byte>();
                }
                else
                {
                    if (num > 1)
                    {
                        MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(RelayServerPacket), 1, num);
                        return;
                    }

                    payload = null;
                    if (num != 0)
                        reader.ReadValue(ref payload);
                }

                value = new RelayServerPacket(payload);
            }
        }

        /// <summary>
        ///     序列化器
        /// </summary>
        [Preserve]
        private sealed class RelayServerPacketFormatter : MemoryPackFormatter<RelayServerPacket>
        {
            /// <summary>
            ///     序列化
            /// </summary>
            [Preserve]
            public override void Serialize(ref MemoryPackWriter writer, ref RelayServerPacket value) => RelayServerPacket.Serialize(ref writer, ref value);

            /// <summary>
            ///     反序列化
            /// </summary>
            [Preserve]
            public override void Deserialize(ref MemoryPackReader reader, ref RelayServerPacket value) => RelayServerPacket.Deserialize(ref reader, ref value);
        }
    }
}