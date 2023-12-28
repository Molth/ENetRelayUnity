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
    public static class ClientCallbackHandlerWrapper
    {
        /// <summary>
        ///     获取句柄
        /// </summary>
        /// <param name="handler">句柄</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>获得的句柄</returns>
        public static ClientCallbackHandler WrapHandler<T>(Action<T> handler) where T : struct
        {
            return Wrapped;

            void Wrapped(byte[] bytes)
            {
                if (NetworkSerializer.Deserialize<T>(bytes, out var obj))
                    handler.Invoke(obj);
            }
        }
    }
}