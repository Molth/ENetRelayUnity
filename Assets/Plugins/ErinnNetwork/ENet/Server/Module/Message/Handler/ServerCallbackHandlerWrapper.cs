//------------------------------------------------------------
// Erinn Network
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using System;

namespace Erinn
{
    /// <summary>
    ///     句柄包装器
    /// </summary>
    public static class ServerCallbackHandlerWrapper
    {
        /// <summary>
        ///     获取句柄
        /// </summary>
        /// <param name="handler">句柄</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>获得的句柄</returns>
        public static ServerCallbackHandler WrapHandler<T>(Action<uint, T> handler) where T : struct
        {
            return Wrapped;

            void Wrapped(uint id, byte[] bytes)
            {
                if (NetworkSerializer.Deserialize<T>(bytes, out var obj))
                    handler.Invoke(id, obj);
            }
        }
    }
}