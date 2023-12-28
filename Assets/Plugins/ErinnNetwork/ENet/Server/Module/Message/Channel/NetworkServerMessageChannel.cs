//------------------------------------------------------------
// Erinn Network
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using MemoryPack;

#pragma warning disable CS8601

namespace Erinn
{
    /// <summary>
    ///     网络服务器信息通道
    /// </summary>
    public sealed class NetworkServerMessageChannel
    {
        /// <summary>
        ///     添加监听方法信息
        /// </summary>
        private static readonly MethodInfo RegisterMethodInfo = typeof(NetworkServerMessageChannel).GetMethod(nameof(RegisterHandler), BindingFlags.Instance | BindingFlags.Public);

        /// <summary>
        ///     移除监听方法信息
        /// </summary>
        private static readonly MethodInfo UnregisterMethodInfo = typeof(NetworkServerMessageChannel).GetMethod(nameof(UnregisterHandler), BindingFlags.Instance | BindingFlags.Public);

        /// <summary>
        ///     服务器事件句柄字典32
        /// </summary>
        private readonly Dictionary<uint, ServerCallbackHandler> _handlers32 = new();

        /// <summary>
        ///     服务器事件句柄字典64
        /// </summary>
        private readonly Dictionary<ulong, ServerCallbackHandler> _handlers64 = new();

        /// <summary>
        ///     哈希容量
        /// </summary>
        public RpcHashSize RpcHashSize { get; private set; } = RpcHashSize.VarIntFourBytes;

        /// <summary>
        ///     切换哈希容量
        /// </summary>
        /// <param name="hashSize">哈希容量</param>
        public void ChangeRpcHashSize(RpcHashSize hashSize) => RpcHashSize = hashSize;

        /// <summary>
        ///     绑定监听
        /// </summary>
        /// <param name="method">方法</param>
        /// <param name="firstArgument">调用者</param>
        private void BindMethod(MethodInfo method, object firstArgument)
        {
            var messageType = method.GetParameters()[1].ParameterType;
            var genericHandlerType = typeof(Action<,>).MakeGenericType(typeof(uint), messageType);
            var handler = Delegate.CreateDelegate(genericHandlerType, firstArgument, method);
            var genericRegisterMethod = RegisterMethodInfo.MakeGenericMethod(messageType);
            genericRegisterMethod.Invoke(this, new object[] { handler });
        }

        /// <summary>
        ///     移除监听
        /// </summary>
        /// <param name="method">方法</param>
        /// <param name="firstArgument">调用者</param>
        private void FreeMethod(MethodInfo method, object firstArgument)
        {
            var messageType = method.GetParameters()[1].ParameterType;
            var genericRegisterMethod = UnregisterMethodInfo.MakeGenericMethod(messageType);
            genericRegisterMethod.Invoke(this, null);
        }

        /// <summary>
        ///     注册命令句柄
        /// </summary>
        public void RegisterHandlers<T>(T listener) where T : IServerCallback => RegisterHandlers(listener, listener.GetType());

        /// <summary>
        ///     注册命令句柄
        /// </summary>
        public void RegisterHandlers<T>(T listener, Type type) where T : IServerCallback
        {
            foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var rpcAttribute = method.GetCustomAttribute<ServerCallbackAttribute>();
                if (rpcAttribute != null)
                    BindMethod(method, listener);
            }
        }

        /// <summary>
        ///     注册命令句柄
        /// </summary>
        public void RegisterHandlers(Type type)
        {
            foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var rpcAttribute = method.GetCustomAttribute<ServerCallbackAttribute>();
                if (rpcAttribute != null)
                    BindMethod(method, null);
            }
        }

        /// <summary>
        ///     注册命令句柄
        /// </summary>
        public void RegisterHandlers(Assembly assembly)
        {
            var types = Array.FindAll(assembly.GetTypes(), type => typeof(IServerCallback).IsAssignableFrom(type));
            foreach (var type in types)
                RegisterHandlers(type);
        }

        /// <summary>
        ///     注册命令句柄
        /// </summary>
        public void RegisterHandlers()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
                RegisterHandlers(assembly);
        }

        /// <summary>
        ///     注册命令句柄
        /// </summary>
        /// <param name="handler">句柄</param>
        /// <typeparam name="T">类型</typeparam>
        public void RegisterHandler<T>(Action<uint, T> handler) where T : struct, INetworkMessage, IMemoryPackable<T>
        {
            var (hash32, hash64) = NetworkHash.GetId<T>();
            var warpedHandler = ServerCallbackHandlerWrapper.WrapHandler(handler);
            _handlers32[hash32] = warpedHandler;
            _handlers64[hash64] = warpedHandler;
            Log.Info($"注册了一个命令句柄[{typeof(T).FullName}]");
        }

        /// <summary>
        ///     移除命令句柄
        /// </summary>
        public void UnregisterHandlers<T>(T listener) where T : IServerCallback => UnregisterHandlers(listener, listener.GetType());

        /// <summary>
        ///     移除命令句柄
        /// </summary>
        public void UnregisterHandlers<T>(T listener, Type type) where T : IServerCallback
        {
            foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var rpcAttribute = method.GetCustomAttribute<ServerCallbackAttribute>();
                if (rpcAttribute != null)
                    FreeMethod(method, listener);
            }
        }

        /// <summary>
        ///     移除命令句柄
        /// </summary>
        public void UnregisterHandlers(Type type)
        {
            foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var rpcAttribute = method.GetCustomAttribute<ServerCallbackAttribute>();
                if (rpcAttribute != null)
                    FreeMethod(method, null);
            }
        }

        /// <summary>
        ///     移除命令句柄
        /// </summary>
        public void UnregisterHandlers(Assembly assembly)
        {
            var types = Array.FindAll(assembly.GetTypes(), type => typeof(IServerCallback).IsAssignableFrom(type));
            foreach (var type in types)
                UnregisterHandlers(type);
        }

        /// <summary>
        ///     移除命令句柄
        /// </summary>
        public void UnregisterHandlers()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
                UnregisterHandlers(assembly);
        }

        /// <summary>
        ///     移除命令句柄
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        public void UnregisterHandler<T>() where T : struct, INetworkMessage, IMemoryPackable<T>
        {
            var (hash32, hash64) = NetworkHash.GetId<T>();
            if (_handlers32.Remove(hash32) || _handlers64.Remove(hash64))
                Log.Info($"移除了一个命令句柄[{typeof(T).FullName}]");
        }

        /// <summary>
        ///     清空命令句柄
        /// </summary>
        public void ClearHandlers()
        {
            _handlers32.Clear();
            _handlers64.Clear();
            Log.Info("清空了命令句柄");
        }

        /// <summary>
        ///     调用句柄
        /// </summary>
        /// <param name="id">客户端Id</param>
        /// <param name="networkPacket">网络数据包</param>
        public void InvokeHandler(uint id, ref NetworkPacket networkPacket)
        {
            switch (RpcHashSize)
            {
                case RpcHashSize.VarIntFourBytes:
                {
                    if (NetworkSerializer.Deserialize<uint>(networkPacket.Command, out var command))
                    {
                        if (_handlers32.TryGetValue(command, out var handler))
                            handler.Invoke(id, networkPacket.Payload);
                    }

                    break;
                }
                case RpcHashSize.VarIntEightBytes:
                {
                    if (NetworkSerializer.Deserialize<ulong>(networkPacket.Command, out var command))
                    {
                        if (_handlers64.TryGetValue(command, out var handler))
                            handler.Invoke(id, networkPacket.Payload);
                    }

                    break;
                }
            }
        }
    }
}