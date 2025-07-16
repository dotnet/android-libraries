namespace AndroidX.WebKit.ChromiumLibBoundary;

public partial class WebViewBuilderBoundaryInterfaceConfig
{
    public virtual void Accept (Java.Lang.Object? chromiumConfig) =>
        this.Accept ((Java.Util.Functions.IBiConsumer?) chromiumConfig);
}
