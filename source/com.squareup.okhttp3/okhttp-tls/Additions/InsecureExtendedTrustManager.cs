namespace Square.OkHttp3.TLS.Internal;

public partial class InsecureExtendedTrustManager
{
    // Fix for CS0534: missing abstract member implementations
    
    public override unsafe void CheckClientTrusted(global::Java.Security.Cert.X509Certificate[]? p0, string? p1, global::Javax.Net.Ssl.SSLEngine? p2)
    {
        // Insecure implementation - trust all certificates
    }

    public override unsafe void CheckClientTrusted(global::Java.Security.Cert.X509Certificate[]? p0, string? p1, global::Java.Net.Socket? p2)
    {
        // Insecure implementation - trust all certificates
    }

    public override unsafe void CheckClientTrusted(global::Java.Security.Cert.X509Certificate[]? p0, string? p1)
    {
        // Insecure implementation - trust all certificates
    }

    public override unsafe void CheckServerTrusted(global::Java.Security.Cert.X509Certificate[]? p0, string? p1)
    {
        // Insecure implementation - trust all certificates
    }
}
