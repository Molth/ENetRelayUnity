//------------------------------------------------------------
// Erinn Network
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using ENet;

namespace Erinn
{
    /// <summary>
    ///     网络拓展
    /// </summary>
    public static class NetworkExtensions
    {
        /// <summary>
        ///     端点名称
        /// </summary>
        /// <param name="netEvent">Event</param>
        /// <returns>端点名称</returns>
        public static string EndPointString(this Event netEvent)
        {
            return netEvent.Peer.EndPointString();
        }

        /// <summary>
        ///     端点名称
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <returns>端点名称</returns>
        public static string EndPointString(this Peer peer)
        {
            return $"[{peer.IP}:{peer.Port}]";
        }
    }
}