//------------------------------------------------------------
// Erinn Network
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;

namespace Erinn
{
    /// <summary>
    ///     网络哈希
    /// </summary>
    public static partial class NetworkHash
    {
        /// <summary>
        ///     获取Id32和Id64
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>获得的Id32和Id64</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (uint, ulong) GetId<T>() where T : struct, INetworkMessage => (GetId32<T>(), GetId64<T>());

        /// <summary>
        ///     获取命令
        /// </summary>
        /// <param name="hashSize">哈希容量</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>获得的命令</returns>
        /// <exception cref="ArgumentOutOfRangeException">报错</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetCommand<T>(RpcHashSize hashSize) where T : struct, INetworkMessage => hashSize switch
        {
            RpcHashSize.VarIntFourBytes => GetCommand32<T>(),
            RpcHashSize.VarIntEightBytes => GetCommand64<T>(),
            _ => throw new ArgumentOutOfRangeException(nameof(hashSize), hashSize, null)
        };
    }
}