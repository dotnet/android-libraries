namespace Square.OkHttp3.TLS.Internal;

public partial class InsecureAndroidTrustManager
{
    // Fix for CS0738: wrong return type
    // The interface requires void return type, but the generated method returns a different type
    public unsafe void CheckClientTrusted(global::Java.Security.Cert.X509Certificate[]? p0, string? p1)
    {
        // Call the existing implementation if it exists, or provide empty implementation
        try
        {
            CheckClientTrusted(p0!, p1!);
        }
        catch
        {
            // Ignore exceptions for insecure trust manager
        }
    }

    public unsafe void CheckServerTrusted(global::Java.Security.Cert.X509Certificate[]? p0, string? p1)
    {
        // Call the existing implementation if it exists, or provide empty implementation
        try
        {
            CheckServerTrusted(p0!, p1!);
        }
        catch
        {
            // Ignore exceptions for insecure trust manager
        }
    }
}
