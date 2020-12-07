
namespace TestApplicationHttps
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
        } // End Function FromPem 


    } // End Class CertHackStore 


} // End Namespace TestApplicationHttps 
