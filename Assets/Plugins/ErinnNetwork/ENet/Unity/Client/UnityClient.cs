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
    public sealed class UnityClient : MonoBehaviour
    {
        /// <summary>
        ///     单例
        /// </summary>
        public static UnityClient Singleton { get; private set; }

        /// <summary>
        ///     主管
        /// </summary>
        public NetworkClient Master { get; private set; }

        /// <summary>
        ///     已经连接
        /// </summary>
        public bool Connected => Master.Connected;

        /// <summary>
        ///     加载时调用
        /// </summary>
        private void Awake()
        {
            Application.runInBackground = true;
            Master = new NetworkClient();
            Singleton = this;
        }

        /// <summary>
        ///     销毁时释放
        /// </summary>
        private void OnDestroy() => Disconnect();

        /// <summary>
        ///     退出应用程序时释放
        /// </summary>
        private void OnApplicationQuit() => Disconnect();

        /// <summary>
        ///     连接事件
        /// </summary>
        public event Action OnConnectedCallback
        {
            add => Master.OnConnectedCallback += value;
            remove => Master.OnConnectedCallback -= value;
        }

        /// <summary>
        ///     断开事件
        /// </summary>
        public event Action OnDisconnectedCallback
        {
            add => Master.OnDisconnectedCallback += value;
            remove => Master.OnDisconnectedCallback -= value;
        }

        /// <summary>
        ///     启动客户端
        /// </summary>
        /// <param name="ipAddress">服务器Ip地址</param>
        /// <param name="port">服务器端口</param>
        public void StartClient(string ipAddress, ushort port) => Master.Start(ipAddress, port);

        /// <summary>
        ///     断开连接
        /// </summary>
        public void Disconnect() => Master.Disconnect();

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
        public void RegisterHandlers<T>(T listener) where T : IClientCallback => Master.RegisterHandlers(listener);

        /// <summary>
        ///     注册命令句柄
        /// </summary>
        public void RegisterHandlers<T>(T listener, Type type) where T : IClientCallback => Master.RegisterHandlers(listener, type);

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
        public void RegisterHandler<T>(Action<T> handler) where T : struct, INetworkMessage, IMemoryPackable<T> => Master.RegisterHandler(handler);

        /// <summary>
        ///     移除命令句柄
        /// </summary>
        public void UnregisterHandlers<T>(T listener) where T : IClientCallback => Master.UnregisterHandlers(listener);

        /// <summary>
        ///     移除命令句柄
        /// </summary>
        public void UnregisterHandlers<T>(T listener, Type type) where T : IClientCallback => Master.UnregisterHandlers(listener, type);

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
        /// <returns>获得的往返延迟时间</returns>
        public ulong GetCurrentRtt() => Master.GetCurrentRtt();

        /// <summary>
        ///     发送数据包
        /// </summary>
        /// <param name="obj">object</param>
        /// <typeparam name="T">类型</typeparam>
        public void Send<T>(T obj) where T : struct, INetworkMessage, IMemoryPackable<T> => Master.Send(obj);
    }
}