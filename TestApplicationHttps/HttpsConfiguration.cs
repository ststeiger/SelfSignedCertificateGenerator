
using Microsoft.AspNetCore.Hosting; // for UseHttps


namespace TestApplicationHttps.Configuration.Kestrel 
{


    public class CertHackStore
    {

        protected bool m_isNotWindows;
        protected System.Security.Cryptography.X509Certificates.X509Certificate2 m_certificate;
        protected byte[] m_bkcs12Bytes;


        public CertHackStore(System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            this.m_isNotWindows = !System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
            this.m_certificate = cert;
            this.m_bkcs12Bytes = cert.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Pkcs12);
        } // End Constructor 


        public System.Security.Cryptography.X509Certificates.X509Certificate2 Certificate
        {
            get
            {
                if (this.m_isNotWindows)
                    return this.m_certificate;

                // Hack for 2017 Windoze Bug "No credentials are available in the security package" 
                // SslStream is not working with ephemeral keys ... 
                return new System.Security.Cryptography.X509Certificates.X509Certificate2(this.m_bkcs12Bytes);
            }
        } // End Property Certificate 


        public static CertHackStore FromPem(string cert, string key)
        {
            System.ReadOnlySpan<char> certa = System.MemoryExtensions.AsSpan(cert);
            System.ReadOnlySpan<char> keya = System.MemoryExtensions.AsSpan(key);
            System.Security.Cryptography.X509Certificates.X509Certificate2 sslCert = 
                System.Security.Cryptography.X509Certificates.X509Certificate2.CreateFromPem(certa, keya);

            return new CertHackStore(sslCert);
        }


    } // End Class CertHackStore


    public static class Https
    {


        public static void HttpsDefaults(Microsoft.AspNetCore.Server.Kestrel.Https.HttpsConnectionAdapterOptions listenOptions)
        {
            listenOptions.OnAuthenticate =
                delegate (Microsoft.AspNetCore.Connections.ConnectionContext connectionContext, System.Net.Security.SslServerAuthenticationOptions sslOptions)
                {
                    #if NO_NGINX_FUCKUP
                        sslOptions.CipherSuitesPolicy = new System.Net.Security.CipherSuitesPolicy(
                           new System.Net.Security.TlsCipherSuite[]
                           {
                                System.Net.Security.TlsCipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256,
                                System.Net.Security.TlsCipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384,
                                System.Net.Security.TlsCipherSuite.TLS_CHACHA20_POLY1305_SHA256,
                               // ...
                           });
                    #endif

                }
            ; // End OnAuthenticate 

        } // End Sub HttpsDefaults 


        public static void CertificateFileChanged(
              System.Collections.Concurrent.ConcurrentDictionary<string, CertHackStore> certs
            , object sender
            , System.IO.FileSystemEventArgs e
            )
        {
            System.Console.WriteLine(e.FullPath.ToString() + " is changed!");
            // TODO: Swap certificate...
            // certs["localhost"] = localhostCert;
        } // End Sub CertificateFileChanged 


        public static System.Security.Cryptography.X509Certificates.X509Certificate2 ServerCertificateSelector(
              System.Collections.Concurrent.ConcurrentDictionary<string, CertHackStore> certs
            , Microsoft.AspNetCore.Connections.ConnectionContext connectionContext
            , string name)
        {
            if (certs != null && certs.Count > 0)
            {
                // return certs.GetEnumerator().Current.Value;
                // return System.Linq.Enumerable.FirstOrDefault(certs);
                foreach (System.Collections.Generic.KeyValuePair<string, CertHackStore> thisCert in certs)
                {
                    System.Console.WriteLine("SNI Name: {0}", name);
                    
                    return thisCert.Value.Certificate;
                } // Next thisCert 

            } // End if (certs != null && certs.Count > 0) 


            // CertHackStore cert;
            // if (name != null && certs.TryGetValue(name, out cert))
            // {
            //     return cert.NewCertificate;
            // }
            
            throw new System.IO.InvalidDataException("No certificate for name \"" + name + "\".");
        } // End Function ServerCertificateSelector 


        public static void ListenAnyIP(
              Microsoft.AspNetCore.Server.Kestrel.Core.ListenOptions listenOptions
            , System.IO.FileSystemWatcher watcher 
        )
        {
            System.Collections.Concurrent.ConcurrentDictionary<string, CertHackStore> certs = 
                new System.Collections.Concurrent.ConcurrentDictionary<string, CertHackStore>(
                    System.StringComparer.OrdinalIgnoreCase
            );

            // watcher.Filters.Add("localhost.yml");
            // watcher.Filters.Add("example.com.yaml");
            // watcher.Filters.Add("sub.example.com.yaml");

            System.IO.FileSystemEventHandler onChange = delegate (object sender, System.IO.FileSystemEventArgs e)
            {
                CertificateFileChanged(certs, sender, e);
            };


            watcher.Changed += new System.IO.FileSystemEventHandler(onChange);
            watcher.Created += new System.IO.FileSystemEventHandler(onChange);
            watcher.Deleted += new System.IO.FileSystemEventHandler(onChange);
            // watcher.Renamed += new System.IO.RenamedEventHandler(OnRenamed);

            if(!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                watcher.EnableRaisingEvents = true;


            listenOptions.UseHttps(
                delegate (Microsoft.AspNetCore.Server.Kestrel.Https.HttpsConnectionAdapterOptions httpsOptions)
                {
                    UseHttps(certs, httpsOptions);

                    httpsOptions.ServerCertificateSelector =
                        delegate (Microsoft.AspNetCore.Connections.ConnectionContext connectionContext, string name)
                        {
                            return ServerCertificateSelector(certs, connectionContext, name);
                        };

                }
            ); // End ListenOptions.UseHttps

        } // End Sub ListenAnyIP 


        public static void UseHttps(
              System.Collections.Concurrent.ConcurrentDictionary<string, CertHackStore> certs
            , Microsoft.AspNetCore.Server.Kestrel.Https.HttpsConnectionAdapterOptions httpsOptions)
        {
            /*
            System.Security.Cryptography.X509Certificates.X509Certificate2 localhostCert = Microsoft.AspNetCore.Server.Kestrel.Https.CertificateLoader.LoadFromStoreCert(
                "localhost", "My", System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser,
                allowInvalid: true);

            System.Security.Cryptography.X509Certificates.X509Certificate2 exampleCert = Microsoft.AspNetCore.Server.Kestrel.Https.CertificateLoader.LoadFromStoreCert(
                "example.com", "My", System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser,
                allowInvalid: true);

            System.Security.Cryptography.X509Certificates.X509Certificate2 subExampleCert = Microsoft.AspNetCore.Server.Kestrel.Https.CertificateLoader.LoadFromStoreCert(
                "sub.example.com", "My", System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser,
                allowInvalid: true);

            certs["localhost"] = localhostCert;
            certs["example.com"] = exampleCert;
            certs["sub.example.com"] = subExampleCert;
            */

            string cert = SecretManager.GetSecret<string>("ssl_cert");
            string key = SecretManager.GetSecret<string>("ssl_key");
            certs["localhost"] = CertHackStore.FromPem(cert, key);

            httpsOptions.ServerCertificateSelector =
                delegate (Microsoft.AspNetCore.Connections.ConnectionContext connectionContext, string name)
                {
                    return ServerCertificateSelector(certs, connectionContext, name);
                }
            ;

        } // End Sub UseHttps 


    } // End Class Https 


} // End Namespace TestApplicationHttps.Configuration.Kestrel 
