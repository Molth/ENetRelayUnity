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
        ///     获取Id64
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>获得的Id64</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetId64<T>() where T : struct, INetworkMessage => HashCache<T>.Id64;

        /// <summary>
        ///     获取命令
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>获得的命令</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetCommand64<T>() where T : struct, INetworkMessage => MemoryPackSerializer.Serialize(GetId64<T>());

        /// <summary>
        ///     获取Hash64
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        /// <returns>获得的Hash64</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Hash64(byte[] buffer)
        {
            var length = buffer.Length;
            unsafe
            {
                fixed (byte* pointer = buffer)
                {
                    return Hash64(pointer, length);
                }
            }
        }

        /// <summary>
        ///     获取Hash64
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>获得的Hash64</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Hash64(string text) => Hash64(MemoryPackSerializer.Serialize(text));

        /// <summary>
        ///     获取Hash64
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>获得的Hash64</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Hash64(Type type) => Hash64(type.FullName);

        /// <summary>
        ///     获取Hash64
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>获得的Hash64</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Hash64<T>() => Hash64(typeof(T).FullName);

        /// <summary>
        ///     获取Hash64
        /// </summary>
        /// <param name="input">输入</param>
        /// <param name="length">长度</param>
        /// <param name="seed">种子</param>
        /// <returns>获得的Hash64</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong Hash64(byte* input, int length, uint seed = 0)
        {
            unchecked
            {
                const ulong prime1 = 11400714785074694791ul;
                const ulong prime2 = 14029467366897019727ul;
                const ulong prime3 = 01609587929392839161ul;
                const ulong prime4 = 09650029242287828579ul;
                const ulong prime5 = 02870177450012600261ul;
                var hash = seed + prime5;
                if (length >= 32)
                {
                    var val0 = seed + prime1 + prime2;
                    var val1 = seed + prime2;
                    ulong val2 = seed + 0;
                    var val3 = seed - prime1;
                    var count = length >> 5;
                    for (var i = 0; i < count; ++i)
                    {
                        var pos0 = *(ulong*)(input + 0);
                        var pos1 = *(ulong*)(input + 8);
                        var pos2 = *(ulong*)(input + 16);
                        var pos3 = *(ulong*)(input + 24);
                        val0 += pos0 * prime2;
                        val0 = (val0 << 31) | (val0 >> (64 - 31));
                        val0 *= prime1;
                        val1 += pos1 * prime2;
                        val1 = (val1 << 31) | (val1 >> (64 - 31));
                        val1 *= prime1;
                        val2 += pos2 * prime2;
                        val2 = (val2 << 31) | (val2 >> (64 - 31));
                        val2 *= prime1;
                        val3 += pos3 * prime2;
                        val3 = (val3 << 31) | (val3 >> (64 - 31));
                        val3 *= prime1;
                        input += 32;
                    }

                    hash = ((val0 << 01) | (val0 >> (64 - 01))) + ((val1 << 07) | (val1 >> (64 - 07))) + ((val2 << 12) | (val2 >> (64 - 12))) + ((val3 << 18) | (val3 >> (64 - 18)));
                    val0 *= prime2;
                    val0 = (val0 << 31) | (val0 >> (64 - 31));
                    val0 *= prime1;
                    hash ^= val0;
                    hash = hash * prime1 + prime4;
                    val1 *= prime2;
                    val1 = (val1 << 31) | (val1 >> (64 - 31));
                    val1 *= prime1;
                    hash ^= val1;
                    hash = hash * prime1 + prime4;
                    val2 *= prime2;
                    val2 = (val2 << 31) | (val2 >> (64 - 31));
                    val2 *= prime1;
                    hash ^= val2;
                    hash = hash * prime1 + prime4;
                    val3 *= prime2;
                    val3 = (val3 << 31) | (val3 >> (64 - 31));
                    val3 *= prime1;
                    hash ^= val3;
                    hash = hash * prime1 + prime4;
                }

                hash += (ulong)length;
                length &= 31;
                while (length >= 8)
                {
                    var lane = *(ulong*)input * prime2;
                    lane = ((lane << 31) | (lane >> (64 - 31))) * prime1;
                    hash ^= lane;
                    hash = ((hash << 27) | (hash >> (64 - 27))) * prime1 + prime4;
                    input += 8;
                    length -= 8;
                }

                if (length >= 4)
                {
                    hash ^= *(uint*)input * prime1;
                    hash = ((hash << 23) | (hash >> (64 - 23))) * prime2 + prime3;
                    input += 4;
                    length -= 4;
                }

                while (length > 0)
                {
                    hash ^= *input * prime5;
                    hash = ((hash << 11) | (hash >> (64 - 11))) * prime1;
                    ++input;
                    --length;
                }

                hash ^= hash >> 33;
                hash *= prime2;
                hash ^= hash >> 29;
                hash *= prime3;
                hash ^= hash >> 32;
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
            ///     Id64
            /// </summary>
            public static readonly ulong Id64 = Hash64(typeof(T).FullName);
        }
    }
}