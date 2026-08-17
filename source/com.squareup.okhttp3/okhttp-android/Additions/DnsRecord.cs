namespace Square.OkHttp3
{
    public abstract partial class DnsRecord
    {
        public sealed partial class IpAddress
        {
            public override string InvokeHostname() => InvokeHostnameCore();
        }

        public sealed partial class ServiceMetadata
        {
            public override string InvokeHostname() => InvokeHostnameCore();
        }
    }
}
