
namespace TestApplicationHttps
{


    internal class KestrelOptionsSetup 
        : Microsoft.Extensions.Options.IConfigureOptions<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>
    {

        protected readonly Microsoft.Extensions.Logging.ILogger<KestrelOptionsSetup> m_logger;
        protected System.IO.FileSystemWatcher m_watcher;

        public KestrelOptionsSetup(Microsoft.Extensions.Logging.ILogger<KestrelOptionsSetup> logger)
        {
            this.m_logger = logger;
            this.m_watcher = new System.IO.FileSystemWatcher();
        } // End Constructor 


        ~KestrelOptionsSetup()
        {
            if (this.m_watcher != null)
            {
                this.m_watcher.Dispose();
                this.m_watcher = null;
            } // End if (this.m_watcher != null) 

        } // End Destructor 


        void Microsoft.Extensions.Options.IConfigureOptions<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>.Configure(
            Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions options)
        {
            System.Console.WriteLine(options);

            options.AddServerHeader = false;

            options.ConfigureHttpsDefaults(
                delegate (Microsoft.AspNetCore.Server.Kestrel.Https.HttpsConnectionAdapterOptions listenOptions)
                {
                    Configuration.Kestrel.Https.HttpsDefaults(listenOptions, this.m_watcher);
                }
            );

            options.ConfigureEndpointDefaults(Configuration.Kestrel.Https.ConfigureEndpointDefaults);
        } // End Sub Configure 


    } // End Class KestrelOptionsSetup 


} // End Namespace TestApplicationHttps 
