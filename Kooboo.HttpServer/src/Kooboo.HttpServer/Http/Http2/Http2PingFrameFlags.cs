// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Kooboo.HttpServer.Http.Http2
{
    [Flags]
    public enum Http2PingFrameFlags : byte
    {
        NONE = 0x0,
        ACK = 0x1
    }
}
