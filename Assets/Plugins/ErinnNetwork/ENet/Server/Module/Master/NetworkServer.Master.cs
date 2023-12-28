//------------------------------------------------------------
// Erinn Network
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using Cysharp.Threading.Tasks;

namespace Erinn
{
    /// <summary>
    ///     网络服务器
    /// </summary>
    partial class NetworkServer
    {
        /// <summary>
        ///     缓冲区容量
        /// </summary>
        public const int BufferSize = 16384;

        /// <summary>
        ///     缓冲区
        /// </summary>
        private readonly byte[] _buffer = new byte[BufferSize];

        /// <summary>
        ///     超时
        /// </summary>
        public int Timeout { get; private set; } = 15;

        /// <summary>
        ///     间隔
        /// </summary>
        public int Tick { get; private set; } = 1;

        /// <summary>
        ///     设置超时
        /// </summary>
        /// <param name="timeout">超时</param>
        public void SetTimeout(uint timeout) => Timeout = (int)timeout;

        /// <summary>
        ///     设置间隔
        /// </summary>
        /// <param name="tick">间隔</param>
        public void SetTick(uint tick) => Tick = (int)tick;

        /// <summary>
        ///     开始轮询
        /// </summary>
        private void StartPolling() => _ = UniTask.RunOnThreadPool(PollPeers);
    }
}