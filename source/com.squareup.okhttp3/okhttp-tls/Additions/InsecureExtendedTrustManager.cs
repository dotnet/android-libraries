namespace Square.OkHttp3.TLS.Internal;

public partial class InsecureExtendedTrustManager
{
    // Implement abstract methods by calling base implementations
    public override unsafe void CheckClientTrusted(global::Java.Security.Cert.X509Certificate[]? chain, string? authType, global::Javax.Net.Ssl.SSLEngine? engine)
    {
        base.CheckClientTrusted(chain, authType, engine);
    }

    public override unsafe void CheckClientTrusted(global::Java.Security.Cert.X509Certificate[]? chain, string? authType, global::Java.Net.Socket? socket)
    {
        base.CheckClientTrusted(chain, authType, socket);
    }

    public override unsafe void CheckClientTrusted(global::Java.Security.Cert.X509Certificate[]? chain, string? authType)
    {
        base.CheckClientTrusted(chain, authType);
    }

    public override unsafe void CheckServerTrusted(global::Java.Security.Cert.X509Certificate[]? chain, string? authType)
    {
        base.CheckServerTrusted(chain, authType);
    }
}
