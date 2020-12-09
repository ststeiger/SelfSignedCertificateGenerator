// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


namespace TestApplicationHttps.ProxyProtocol
{
    public class ProxyProtocolFeature
    {
        public System.Net.IPAddress SourceIp { get; internal set; }
        public System.Net.IPAddress DestinationIp { get; internal set; }
        public int SourcePort { get; internal set; }
        public int DestinationPort { get; internal set; }
        public long LinkId { get; internal set; }
    }
}