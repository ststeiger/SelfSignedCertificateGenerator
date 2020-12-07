
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace TestApplicationHttps
{


    // 1. add  <AspNetCoreHostingModel>InProcess</AspNetCoreHostingModel> to project file 
    public class Program
    {


        // https://github.com/Tondas/LetsEncrypt
        // https://blogs.akamai.com/2018/10/best-practices-for-ultra-low-latency-streaming-using-chunked-encoded-and-chunk-transferred-cmaf.html
        // https://www.monitis.com/blog/how-to-log-to-postgresql-with-syslog-ng/
        // https://github.com/sshnet/SSH.NET
        // https://labs.rebex.net/syslog
        // https://github.com/moovspace/HotSsl
        // https://github.com/jchristn/WatsonSyslogServer
        // https://github.com/dotnet/aspnetcore/discussions/28238#discussioncomment-142844
        // https://learn.akamai.com/en-us/webhelp/media-services-live/media-services-live-encoder-compatibility-testing-and-qualification-guide-v4.0/GUID-A7D10A31-F4BC-49DD-92B2-8A5BA409BAFE.html#:~:text=The%20transmission%20ends%20when%20a%20zero%2Dlength%20chunk%20is%20received.&text=The%20Content%2DLength%20header%20is,regular%20chunk%20with%20zero%20length.
        // https://www.drunkcode.net/en/posts/2020/6/10/parsing-with-ReadOnlySpan-and-first-try-of-OBJ-reader
        public static void Main(string[] args)
        {
            // https://medium.com/@mvuksano/how-to-properly-configure-your-nginx-for-tls-564651438fe0
            
            // ln -s /etc/nginx/sites-available/example.int example.int
            // cat cert.pem ca.pem > fullchain.pem
            // cat ./obelix.pem ./../skynet/skynet.crt > fullchain.pem
            using (System.IO.FileSystemWatcher watcher = new System.IO.FileSystemWatcher())
            {
                // listenOptions.UseHttps("testCert.pfx", "testPassword");                                
                watcher.NotifyFilter = System.IO.NotifyFilters.Size
                    | System.IO.NotifyFilters.LastWrite
                    | System.IO.NotifyFilters.CreationTime
                    | System.IO.NotifyFilters.FileName // Needed if text-file is changed with Visual Studio ...
                ;

                CreateHostBuilder(args, watcher).Build().Run();
            } // End Using watcher 

        } // End Sub Main 


        // https://github.com/dotnet/aspnetcore/discussions/28238
        // https://github.com/aspnet/KestrelHttpServer/issues/2103
        // https://ayende.com/blog/181281-A/building-a-lets-encrypt-acme-v2-client
        // https://weblog.west-wind.com/posts/2016/feb/22/using-lets-encrypt-with-iis-on-windows
        // https://medium.com/@MaartenSikkema/automatically-request-and-use-lets-encrypt-certificates-in-dotnet-core-9d0d152a59b5
        // https://github.com/dotnet/aspnetcore/issues/1190
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-5.0#code-try-30
        public static IHostBuilder CreateHostBuilder(string[] args, System.IO.FileSystemWatcher watcher)
        {
            // Microsoft.AspNetCore.Server.IIS

            return Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(
                    delegate (IWebHostBuilder webBuilder)
                    {

#if true 

                        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-5.0#code-try-30
                        webBuilder.ConfigureKestrel(
                            delegate (
                                 WebHostBuilderContext builderContext
                                , Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions serverOptions)
                            {
                                // https://github.com/dotnet/aspnetcore/pull/24286
                                // From config-file with reload on change 
                                // serverOptions.Configure(builderContext.Configuration.GetSection("Kestrel"), reloadOnChange: false);

                                // On Linux, CipherSuitesPolicy can be used to filter TLS handshakes on a per-connection basis:
                                serverOptions.ConfigureHttpsDefaults(Configuration.Kestrel.Https.HttpsDefaults); // End ConfigureHttpsDefaults 

                                // serverOptions.Listen(System.Net.IPAddress.Loopback, 5001,
                                serverOptions.ListenAnyIP(5005,
                                    delegate (Microsoft.AspNetCore.Server.Kestrel.Core.ListenOptions listenOptions)
                                    {
                                        Configuration.Kestrel.Https.ListenAnyIP(listenOptions, watcher);
                                    }
                                ); // End ListenAnyIp 

                            }
                        ); // End ConfigureKestrel 
#endif

                        // https://developers.redhat.com/blog/2018/07/24/improv-net-core-kestrel-performance-linux/
                        // webBuilder.UseLinuxTransport();

                        
                        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                        {
                            webBuilder.UseIISIntegration();
                        }
                        else webBuilder.UseKestrel();
                        

                        webBuilder.UseStartup<Startup>()
                        // .UseApplicationInsights()
                        ;

                    }); // End ConfigureWebHostDefaults 

        } // End Function CreateHostBuilder 


    } // End Class Program 


} // End Namespace TestApplicationHttps 
