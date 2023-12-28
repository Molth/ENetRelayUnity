//------------------------------------------------------------
// Erinn Network
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using System;
using System.Reflection;
using MemoryPack;
using UnityEngine;

namespace Erinn
{
    /// <summary>
    ///     网络传输
    /// </summary>
    public sealed class UnityServer : MonoBehaviour
    {
        /// <summary>
        ///     单例
        /// </summary>
        public static UnityServer Singleton { get; private set; }

        /// <summary>
        ///     主管
        /// </summary>
        public NetworkServer Master { get; private set; }

        /// <summary>
        ///     加载时调用
        /// </summary>
        private void Awake()
        {
            Application.runInBackground = true;
            Master = new NetworkServer();
            Singleton = this;
        }

        /// <summary>
        ///     销毁时释放
        /// </summary>
        private void OnDestroy() => Dispose();

        /// <summary>
        ///     退出应用程序时释放
        /// </summary>
        private void OnApplicationQuit() => Dispose();

        /// <summary>
        ///     连接事件
        /// </summary>
        public event Action<uint> OnConnectedCallback
        {
            add => Master.OnConnectedCallback += value;
            remove => Master.OnConnectedCallback -= value;
        }

        /// <summary>
        ///     断开事件
        /// </summary>
        public event Action<uint> OnDisconnectedCallback
        {
            add => Master.OnDisconnectedCallback += value;
            remove => Master.OnDisconnectedCallback -= value;
        }

        /// <summary>
        ///     启动服务器
        /// </summary>
        /// <param name="port">端口</param>
        /// <param name="maxClients">最大连接数</param>
        public void StartServer(ushort port, int maxClients) => Master.Start(port, maxClients);

        /// <summary>
        ///     销毁
        /// </summary>
        public void Dispose() => Master.Dispose();

        /// <summary>
        ///     断开连接
        /// </summary>
        /// <param name="id">客户端Id</param>
        public void Disconnect(uint id) => Master.Disconnect(id);

        /// <summary>
        ///     切换哈希容量
        /// </summary>
        /// <param name="hashSize">哈希容量</param>
        public void ChangeRpcHashSize(RpcHashSize hashSize) => Master.ChangeRpcHashSize(hashSize);

        /// <summary>
        ///     设置超时
        /// </summary>
        /// <param name="timeout">超时</param>
        public void SetTimeout(uint timeout) => Master.SetTimeout(timeout);

        /// <summary>
        ///     设置间隔
        /// </summary>
        /// <param name="tick">间隔</param>
        public void SetTick(uint tick) => Master.SetTick(tick);

        /// <summary>
        ///     注册命令句柄
        /// </summary>
        public void RegisterHandlers<T>(T listener) where T : IServerCallback => Master.RegisterHandlers(listener);

        /// <summary>
        ///     注册命令句柄
        /// </summary>
        public void RegisterHandlers<T>(T listener, Type type) where T : IServerCallback => Master.RegisterHandlers(listener, type);

        /// <summary>
        ///     注册命令句柄
        /// </summary>
        public void RegisterHandlers(Type type) => Master.RegisterHandlers(type);

        /// <summary>
        ///     注册命令句柄
        /// </summary>
        public void RegisterHandlers(Assembly assembly) => Master.RegisterHandlers(assembly);

        /// <summary>
        ///     注册命令句柄
        /// </summary>
        public void RegisterHandlers() => Master.RegisterHandlers();

        /// <summary>
        ///     注册命令句柄
        /// </summary>
        /// <param name="handler">句柄</param>
        /// <typeparam name="T">类型</typeparam>
        public void RegisterHandler<T>(Action<uint, T> handler) where T : struct, INetworkMessage, IMemoryPackable<T> => Master.RegisterHandler(handler);

        /// <summary>
        ///     移除命令句柄
        /// </summary>
        public void UnregisterHandlers<T>(T listener) where T : IServerCallback => Master.UnregisterHandlers(listener);

        /// <summary>
        ///     移除命令句柄
        /// </summary>
        public void UnregisterHandlers<T>(T listener, Type type) where T : IServerCallback => Master.UnregisterHandlers(listener, type);

        /// <summary>
        ///     移除命令句柄
        /// </summary>
        public void UnregisterHandlers(Type type) => Master.UnregisterHandlers(type);

        /// <summary>
        ///     移除命令句柄
        /// </summary>
        public void UnregisterHandlers(Assembly assembly) => Master.UnregisterHandlers(assembly);

        /// <summary>
        ///     移除命令句柄
        /// </summary>
        public void UnregisterHandlers() => Master.UnregisterHandlers();

        /// <summary>
        ///     移除命令句柄
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        public void UnregisterHandler<T>() where T : struct, INetworkMessage, IMemoryPackable<T> => Master.UnregisterHandler<T>();

        /// <summary>
        ///     清空命令句柄
        /// </summary>
        public void ClearHandlers() => Master.ClearHandlers();

        /// <summary>
        ///     获取往返延迟时间
        /// </summary>
        /// <param name="clientId">客户端Id</param>
        /// <returns>获得的往返延迟时间</returns>
        public ulong GetCurrentRtt(uint clientId) => Master.GetCurrentRtt(clientId);

        /// <summary>
        ///     发送数据包
        /// </summary>
        /// <param name="id">客户端Id</param>
        /// <param name="obj">object</param>
        /// <typeparam name="T">类型</typeparam>
        public void Send<T>(uint id, T obj) where T : struct, INetworkMessage, IMemoryPackable<T> => Master.Send(id, obj);
    }
}