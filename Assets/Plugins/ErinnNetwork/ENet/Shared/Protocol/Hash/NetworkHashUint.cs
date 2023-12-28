//------------------------------------------------------------
// Erinn Network
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using MemoryPack;

#pragma warning disable CS8604

namespace Erinn
{
    /// <summary>
    ///     网络哈希
    /// </summary>
    public static partial class NetworkHash
    {
        /// <summary>
        ///     获取Id32
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>获得的Id32</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetId32<T>() where T : struct, INetworkMessage => HashCache<T>.Id32;

        /// <summary>
        ///     获取命令
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>获得的命令</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetCommand32<T>() where T : struct, INetworkMessage => MemoryPackSerializer.Serialize(GetId32<T>());

        /// <summary>
        ///     获取Hash32
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        /// <returns>获得的Hash32</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Hash32(byte[] buffer)
        {
            var length = buffer.Length;
            unsafe
            {
                fixed (byte* pointer = buffer)
                {
                    return Hash32(pointer, length);
                }
            }
        }

        /// <summary>
        ///     获取Hash32
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>获得的Hash32</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Hash32(string text) => Hash32(MemoryPackSerializer.Serialize(text));

        /// <summary>
        ///     获取Hash32
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>获得的Hash32</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Hash32(Type type) => Hash32(type.FullName);

        /// <summary>
        ///     获取Hash32
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>获得的Hash32</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Hash32<T>() => Hash32(typeof(T).FullName);

        /// <summary>
        ///     获取Hash32
        /// </summary>
        /// <param name="input">输入</param>
        /// <param name="length">长度</param>
        /// <param name="seed">种子</param>
        /// <returns>获得的Hash32</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe uint Hash32(byte* input, int length, uint seed = 0)
        {
            unchecked
            {
                const uint prime1 = 2654435761u;
                const uint prime2 = 2246822519u;
                const uint prime3 = 3266489917u;
                const uint prime4 = 0668265263u;
                const uint prime5 = 0374761393u;
                var hash = seed + prime5;
                if (length >= 16)
                {
                    var val0 = seed + prime1 + prime2;
                    var val1 = seed + prime2;
                    var val2 = seed + 0;
                    var val3 = seed - prime1;
                    var count = length >> 4;
                    for (var i = 0; i < count; ++i)
                    {
                        var pos0 = *(uint*)(input + 0);
                        var pos1 = *(uint*)(input + 4);
                        var pos2 = *(uint*)(input + 8);
                        var pos3 = *(uint*)(input + 12);
                        val0 += pos0 * prime2;
                        val0 = (val0 << 13) | (val0 >> (32 - 13));
                        val0 *= prime1;
                        val1 += pos1 * prime2;
                        val1 = (val1 << 13) | (val1 >> (32 - 13));
                        val1 *= prime1;
                        val2 += pos2 * prime2;
                        val2 = (val2 << 13) | (val2 >> (32 - 13));
                        val2 *= prime1;
                        val3 += pos3 * prime2;
                        val3 = (val3 << 13) | (val3 >> (32 - 13));
                        val3 *= prime1;
                        input += 16;
                    }

                    hash = ((val0 << 01) | (val0 >> (32 - 01))) + ((val1 << 07) | (val1 >> (32 - 07))) + ((val2 << 12) | (val2 >> (32 - 12))) + ((val3 << 18) | (val3 >> (32 - 18)));
                }

                hash += (uint)length;
                length &= 15;
                while (length >= 4)
                {
                    hash += *(uint*)input * prime3;
                    hash = ((hash << 17) | (hash >> (32 - 17))) * prime4;
                    input += 4;
                    length -= 4;
                }

                while (length > 0)
                {
                    hash += *input * prime5;
                    hash = ((hash << 11) | (hash >> (32 - 11))) * prime1;
                    ++input;
                    --length;
                }

                hash ^= hash >> 15;
                hash *= prime2;
                hash ^= hash >> 13;
                hash *= prime3;
                hash ^= hash >> 16;
                return hash;
            }
        }

        /// <summary>
        ///     网络Id
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        private static partial class HashCache<T> where T : struct, INetworkMessage
        {
            /// <summary>
            ///     Id32
            /// </summary>
            public static readonly uint Id32 = Hash32(typeof(T).FullName);
        }
    }
}