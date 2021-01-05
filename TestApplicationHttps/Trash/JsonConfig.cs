
namespace TestApplicationHttps.Trash.JsonConfig 
{

    public class ConfigTest
    {
        public static void Test()
        {
            POCO poco = new POCO();
            poco.Endpoints.Http.AddRange(new string[] { "http://*:5000", "http://localhost:5000", "http://machinename:5000" });

            poco.Endpoints.Https.Add(new HttpsEndpoint()
            {
                Url = "https://*:5005",
                CertificateRegistryEntry = "ssl_cert",
                CertificateKeyRegistryEntry = "ssl_key"
            });


            poco.Endpoints.Https.Add(new HttpsEndpoint()
            {
                Url = "https://*:5006",
                CertificateRegistryEntry = "ssl_cert",
                CertificateKeyRegistryEntry = "ssl_key"
            });

            string json = System.Text.Json.JsonSerializer.Serialize(poco);
            System.Console.WriteLine(json);
        }

    }



    public class HttpsEndpoint
    {
        public string Url { get; set; }
        public string CertificateFile { get; set; }
        public string CertificateKey { get; set; }

        public string CertificateRegistryEntry { get; set; }
        public string CertificateKeyRegistryEntry { get; set; }

        public string AesKey { get; set; }
        public string AesIV { get; set; }


        public System.Security.Cryptography.X509Certificates.X509Certificate2 GetCertificate()
        {
            if (System.IO.File.Exists(this.CertificateFile) && System.IO.File.Exists(this.CertificateKey))
            {
                return System.Security.Cryptography.X509Certificates.X509Certificate2.CreateFromPemFile(this.CertificateFile, this.CertificateKey);
            }

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                if (!string.IsNullOrEmpty(this.CertificateRegistryEntry)
                    && !string.IsNullOrEmpty(this.CertificateKeyRegistryEntry)
                    )
                {
                    string ssl_cert = null;
                    string ssl_key = null;

                    using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser
      .OpenSubKey(@"Software\COR\All", false))
                    {
                        ssl_cert = (string)key.GetValue("ssl_cert");
                        ssl_key = (string)key.GetValue("ssl_key");
                        key.Close();
                    } // End Using key 

                    System.ReadOnlySpan<char> certa = System.MemoryExtensions.AsSpan(ssl_cert);
                    System.ReadOnlySpan<char> keya = System.MemoryExtensions.AsSpan(ssl_key);
                    return System.Security.Cryptography.X509Certificates.X509Certificate2
                        .CreateFromPem(certa, keya);
                } // End if CertEntriesExist 

            } // End if IsWindows 

            throw new System.IO.InvalidDataException("Need SSL-Key");
        } // End Function GetCertificate 


    } // End HttpsEndpoint  


    public class EndpointPOCO
    {
        public System.Collections.Generic.List<string> Http { get; set; }
        public System.Collections.Generic.List<HttpsEndpoint> Https { get; set; }


        public EndpointPOCO()
        {
            this.Http = new System.Collections.Generic.List<string>();
            this.Https = new System.Collections.Generic.List<HttpsEndpoint>();
        }

    }


    public class POCO
    {
        public EndpointPOCO Endpoints { get; set; }

        public POCO()
        {
            this.Endpoints = new EndpointPOCO();
        }


    }


}
