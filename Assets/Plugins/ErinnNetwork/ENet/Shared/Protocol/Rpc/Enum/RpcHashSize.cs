//------------------------------------------------------------
// Erinn Network
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

namespace Erinn
{
    /// <summary>
    ///     网络Rpc哈希容量
    /// </summary>
    public enum RpcHashSize : byte
    {
        /// <summary>
        ///     4位
        /// </summary>
        VarIntFourBytes,

        /// <summary>
        ///     8位
        /// </summary>
        VarIntEightBytes
    }
}