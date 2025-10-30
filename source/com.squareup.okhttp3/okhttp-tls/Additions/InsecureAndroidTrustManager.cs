namespace Square.OkHttp3.TLS.Internal;

public partial class InsecureAndroidTrustManager
{
    // Implement interface methods with void return type by calling base methods
    public unsafe void CheckClientTrusted(global::Java.Security.Cert.X509Certificate[]? chain, string? authType)
    {
        base.CheckClientTrusted(chain, authType);
    }

    public unsafe void CheckServerTrusted(global::Java.Security.Cert.X509Certificate[]? chain, string? authType)
    {
        base.CheckServerTrusted(chain, authType);
    }
}
