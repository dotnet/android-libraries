using System;

namespace Square.OkHttp3.Internal.Connection
{
    public partial class RealConnection
    {
        // Implement IConnection.Route() by calling the existing InvokeRoute() method
        global::Square.OkHttp3.Route global::Square.OkHttp3.IConnection.Route()
        {
            return this.InvokeRoute();
        }

        // Implement IExchangeCodecCarrier.NoNewExchanges() by calling the existing InvokeNoNewExchanges() method
        void global::Square.OkHttp3.Internal.Http.IExchangeCodecCarrier.NoNewExchanges()
        {
            this.InvokeNoNewExchanges();
        }
    }
}

namespace Square.OkHttp3.Internal.Http2
{
    public partial class Http2Connection
    {
        public partial class ReaderRunnable
        {
            // Implement IFunction0.Invoke() by calling the existing Invoke() method and wrapping return type
            global::Java.Lang.Object global::Kotlin.Jvm.Functions.IFunction0.Invoke()
            {
                this.Invoke();
                return null; // Function0 in Kotlin represents a function that returns Unit (void in C#)
            }
        }
    }

    public partial class Http2Stream
    {
        // Implement ISocket.Sink property by wrapping the existing Sink property
        global::Square.OkIO.ISink global::Square.OkIO.ISocket.Sink 
        {
            get { return this.Sink; }
        }

        // Implement ISocket.Source property by wrapping the existing Source property  
        global::Square.OkIO.ISource global::Square.OkIO.ISocket.Source
        {
            get { return this.Source; }
        }
    }
}