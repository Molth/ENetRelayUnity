//------------------------------------------------------------
// Erinn Network
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using MemoryPack;

namespace Erinn
{
    /// <summary>
    ///     客户端信息管理器
    /// </summary>
    public abstract class ClientMessageManager : IClientCallback
    {
        /// <summary>
        ///     主管
        /// </summary>
        protected readonly NetworkClient Master;

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="master">主管</param>
        protected ClientMessageManager(NetworkClient master)
        {
            Master = master;
            Master.RegisterHandlers(this, GetType());
        }

        /// <summary>
        ///     发送数据包
        /// </summary>
        /// <param name="obj">值</param>
        /// <typeparam name="T">类型</typeparam>
        protected void Send<T>(T obj) where T : struct, INetworkMessage, IMemoryPackable<T> => Master.Send(obj);
    }
}