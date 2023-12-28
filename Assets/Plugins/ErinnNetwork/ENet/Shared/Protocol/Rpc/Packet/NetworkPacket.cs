//------------------------------------------------------------
// Erinn Network
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable RedundantAssignment

namespace Erinn
{
    /// <summary>
    ///     网络数据包
    /// </summary>
    public struct NetworkPacket : IMemoryPackable<NetworkPacket>
    {
        /// <summary>
        ///     命令
        /// </summary>
        public byte[] Command;

        /// <summary>
        ///     值
        /// </summary>
        public byte[] Payload;

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="payload">值</param>
        public NetworkPacket(byte[] command, byte[] payload)
        {
            Command = command;
            Payload = payload;
        }

        /// <summary>
        ///     静态构造
        /// </summary>
        static NetworkPacket() => RegisterFormatter();

        /// <summary>
        ///     注册
        /// </summary>
        [Preserve]
        public static void RegisterFormatter()
        {
            if (!MemoryPackFormatterProvider.IsRegistered<NetworkPacket>())
                MemoryPackFormatterProvider.Register(new NetworkPacketFormatter());
            if (!MemoryPackFormatterProvider.IsRegistered<NetworkPacket[]>())
                MemoryPackFormatterProvider.Register(new ArrayFormatter<NetworkPacket>());
            if (!MemoryPackFormatterProvider.IsRegistered<byte[]>())
                MemoryPackFormatterProvider.Register(new ArrayFormatter<byte>());
        }

        /// <summary>
        ///     序列化
        /// </summary>
        [Preserve]
        public static void Serialize(ref MemoryPackWriter writer, ref NetworkPacket value)
        {
            writer.WriteObjectHeader(2);
            writer.WriteUnmanagedArray(value.Command);
            writer.WriteUnmanagedArray(value.Payload);
        }

        /// <summary>
        ///     反序列化
        /// </summary>
        [Preserve]
        public static void Deserialize(ref MemoryPackReader reader, ref NetworkPacket value)
        {
            if (!reader.TryReadObjectHeader(out var num))
            {
                value = new NetworkPacket();
            }
            else
            {
                byte[] command;
                byte[] payload;
                if (num <= 2)
                {
                    if (num == 2)
                    {
                        command = reader.ReadUnmanagedArray<byte>();
                        payload = reader.ReadUnmanagedArray<byte>();
                        goto label;
                    }
                }
                else
                {
                    MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(NetworkPacket), 2, num);
                }

                command = null;
                payload = null;
                if (num != 0)
                {
                    reader.ReadUnmanagedArray(ref command);
                    if (num != 1)
                        reader.ReadUnmanagedArray(ref payload);
                }

                label:
                value = new NetworkPacket(command, payload);
            }
        }

        /// <summary>
        ///     序列化器
        /// </summary>
        [Preserve]
        private sealed class NetworkPacketFormatter : MemoryPackFormatter<NetworkPacket>
        {
            /// <summary>
            ///     序列化
            /// </summary>
            [Preserve]
            public override void Serialize(ref MemoryPackWriter writer, ref NetworkPacket value) => NetworkPacket.Serialize(ref writer, ref value);

            /// <summary>
            ///     反序列化
            /// </summary>
            [Preserve]
            public override void Deserialize(ref MemoryPackReader reader, ref NetworkPacket value) => NetworkPacket.Deserialize(ref reader, ref value);
        }
    }
}