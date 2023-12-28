//------------------------------------------------------------
// Erinn Network
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using MemoryPack;

namespace Erinn
{
    /// <summary>
    ///     服务器信息管理器
    /// </summary>
    public abstract class ServerMessageManager : IServerCallback
    {
        /// <summary>
        ///     主管
        /// </summary>
        protected readonly NetworkServer Master;

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="master">主管</param>
        protected ServerMessageManager(NetworkServer master)
        {
            Master = master;
            Master.RegisterHandlers(this, GetType());
        }

        /// <summary>
        ///     发送数据包
        /// </summary>
        /// <param name="id">客户端Id</param>
        /// <param name="obj">值</param>
        /// <typeparam name="T">类型</typeparam>
        protected void Send<T>(uint id, T obj) where T : struct, INetworkMessage, IMemoryPackable<T> => Master.Send(id, obj);
    }
}