//------------------------------------------------------------
// Erinn Network
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using System;

namespace Erinn
{
    /// <summary>
    ///     远程调用属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ServerCallbackAttribute : Attribute
    {
    }
}