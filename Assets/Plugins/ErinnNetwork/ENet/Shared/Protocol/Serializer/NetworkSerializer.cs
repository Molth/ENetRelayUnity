//------------------------------------------------------------
// Erinn Network
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using System;
using ENet;
using MemoryPack;

#pragma warning disable CS8601
#pragma warning disable CS8603
#pragma warning disable CS8625

namespace Erinn
{
    /// <summary>
    ///     网络序列化
    /// </summary>
    public static class NetworkSerializer
    {
        /// <summary>
        ///     创建数据包
        /// </summary>
        /// <param name="payload">缓冲区</param>
        /// <returns>创建的数据包</returns>
        public static Packet CreatePacket(byte[] payload)
        {
            var packet = new Packet();
            packet.Create(payload, PacketFlags.Reliable);
            return packet;
        }

        /// <summary>
        ///     创建数据包
        /// </summary>
        /// <param name="payload">缓冲区</param>
        /// <returns>创建的数据包</returns>
        public static Packet CreatePacket(ArraySegment<byte> payload)
        {
            var packet = new Packet();
            packet.Create(payload.Array, payload.Offset, payload.Count, PacketFlags.Reliable);
            return packet;
        }

        /// <summary>
        ///     创建数据包
        /// </summary>
        /// <param name="payload">缓冲区</param>
        /// <param name="flags">类型</param>
        /// <returns>创建的数据包</returns>
        public static Packet CreatePacket(byte[] payload, PacketFlags flags)
        {
            var packet = new Packet();
            packet.Create(payload, flags);
            return packet;
        }

        /// <summary>
        ///     创建数据包
        /// </summary>
        /// <param name="payload">缓冲区</param>
        /// <param name="flags">类型</param>
        /// <returns>创建的数据包</returns>
        public static Packet CreatePacket(ArraySegment<byte> payload, PacketFlags flags)
        {
            var packet = new Packet();
            packet.Create(payload.Array, payload.Offset, payload.Count, flags);
            return packet;
        }

        /// <summary>
        ///     创建数据包
        /// </summary>
        /// <param name="hashSize">哈希容量</param>
        /// <param name="obj">object</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>创建的数据包</returns>
        public static Packet CreatePacket<T>(RpcHashSize hashSize, T obj) where T : struct, INetworkMessage, IMemoryPackable<T>
        {
            var payload = SerializeNetworkPacket(hashSize, obj);
            var packet = new Packet();
            packet.Create(payload, PacketFlags.Reliable);
            return packet;
        }

        /// <summary>
        ///     创建数据包
        /// </summary>
        /// <param name="hashSize">哈希容量</param>
        /// <param name="obj">object</param>
        /// <param name="flags">类型</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>创建的数据包</returns>
        public static Packet CreatePacket<T>(RpcHashSize hashSize, T obj, PacketFlags flags) where T : struct, INetworkMessage, IMemoryPackable<T>
        {
            var payload = SerializeNetworkPacket(hashSize, obj);
            var packet = new Packet();
            packet.Create(payload, flags);
            return packet;
        }

        /// <summary>
        ///     序列化
        /// </summary>
        /// <param name="hashSize">哈希容量</param>
        /// <param name="obj">object</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>bytes</returns>
        public static byte[] SerializeNetworkPacket<T>(RpcHashSize hashSize, T obj) where T : struct, INetworkMessage, IMemoryPackable<T>
        {
            var command = NetworkHash.GetCommand<T>(hashSize);
            var value = MemoryPackSerializer.Serialize(typeof(T), obj);
            var packet = new NetworkPacket(command, value);
            var bytes = MemoryPackSerializer.Serialize(packet);
            return bytes;
        }

        /// <summary>
        ///     反序列化
        /// </summary>
        /// <param name="payload">缓冲区</param>
        /// <param name="packet">数据包</param>
        /// <returns>是否成功序列化</returns>
        public static bool DeserializeNetworkPacket(byte[] payload, out NetworkPacket packet)
        {
            try
            {
                packet = MemoryPackSerializer.Deserialize<NetworkPacket>(payload);
                return true;
            }
            catch
            {
                packet = default;
                return false;
            }
        }

        /// <summary>
        ///     反序列化
        /// </summary>
        /// <param name="payload">缓冲区</param>
        /// <param name="packet">数据包</param>
        /// <returns>是否成功序列化</returns>
        public static bool DeserializeNetworkPacket(Span<byte> payload, out NetworkPacket packet)
        {
            try
            {
                packet = MemoryPackSerializer.Deserialize<NetworkPacket>(payload);
                return true;
            }
            catch
            {
                packet = default;
                return false;
            }
        }

        /// <summary>
        ///     反序列化
        /// </summary>
        /// <param name="payload">缓冲区</param>
        /// <param name="packet">数据包</param>
        /// <returns>是否成功序列化</returns>
        public static bool DeserializeNetworkPacket(ArraySegment<byte> payload, out NetworkPacket packet)
        {
            try
            {
                packet = MemoryPackSerializer.Deserialize<NetworkPacket>(payload);
                return true;
            }
            catch
            {
                packet = default;
                return false;
            }
        }

        /// <summary>
        ///     序列化
        /// </summary>
        /// <param name="obj">object</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>bytes</returns>
        public static byte[] Serialize<T>(T obj) where T : struct => MemoryPackSerializer.Serialize(typeof(T), obj);

        /// <summary>
        ///     序列化
        /// </summary>
        /// <param name="obj">object</param>
        /// <param name="bytes">Bytes</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>是否成功序列化</returns>
        public static bool Serialize<T>(T obj, out byte[] bytes) where T : struct
        {
            try
            {
                bytes = MemoryPackSerializer.Serialize(typeof(T), obj);
                return true;
            }
            catch
            {
                bytes = null;
                return false;
            }
        }

        /// <summary>
        ///     序列化
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>bytes</returns>
        public static byte[] SerializeObject(object obj) => MemoryPackSerializer.Serialize(obj.GetType(), obj);

        /// <summary>
        ///     序列化
        /// </summary>
        /// <param name="obj">object</param>
        /// <param name="bytes">Bytes</param>
        /// <returns>是否成功序列化</returns>
        public static bool SerializeObject(object obj, out byte[] bytes)
        {
            try
            {
                bytes = MemoryPackSerializer.Serialize(obj.GetType(), obj);
                return true;
            }
            catch
            {
                bytes = null;
                return false;
            }
        }

        /// <summary>
        ///     反序列化
        /// </summary>
        /// <param name="bytes">bytes</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>结果</returns>
        public static T Deserialize<T>(byte[] bytes) where T : struct => MemoryPackSerializer.Deserialize<T>(bytes);

        /// <summary>
        ///     反序列化
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="bytes">bytes</param>
        /// <param name="result">结果</param>
        /// <returns>是否反序列化成功</returns>
        public static bool Deserialize<T>(byte[] bytes, out T result) where T : struct
        {
            try
            {
                result = MemoryPackSerializer.Deserialize<T>(bytes);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        ///     反序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="bytes">bytes</param>
        /// <returns>结果</returns>
        public static object Deserialize(Type type, byte[] bytes) => MemoryPackSerializer.Deserialize(type, bytes);

        /// <summary>
        ///     反序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="bytes">bytes</param>
        /// <param name="result">结果</param>
        /// <returns>是否反序列化成功</returns>
        public static bool Deserialize(Type type, byte[] bytes, out object result)
        {
            try
            {
                result = MemoryPackSerializer.Deserialize(type, bytes);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }
    }
}