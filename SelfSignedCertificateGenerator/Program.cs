
namespace SelfSignedCertificateGenerator
{


    public class Program
    {


        public static async System.Threading.Tasks.Task Main(string[] args)
        {

            // chrome://settings/certificates?search=certifi
            Test();

            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();

            await System.Threading.Tasks.Task.CompletedTask;
        }




        public static void Test()
        {
            Org.BouncyCastle.Security.SecureRandom random = new Org.BouncyCastle.Security.SecureRandom(NonBackdooredPrng.Create());


            // Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair rootKeyPair = KeyGenerator.GenerateEcKeyPair(curveName, random);
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair rootKeyPair = KeyGenerator.GenerateRsaKeyPair(2048, random);
            // Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair rootKeyPair = KeyGenerator.GenerateDsaKeyPair(1024, random);
            // Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair rootKeyPair = KeyGenerator.GenerateDHKeyPair(1024, random);
            // Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair rootKeyPair = KeyGenerator.GenerateGostKeyPair(4096, random);


            Org.BouncyCastle.X509.X509Certificate rootCertificate = GenerateRootCertificate(rootKeyPair, random);


            string curveName = "curve25519"; curveName = "secp256k1";
            // IIS does not support Elliptic Curve...
            // Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair certKeyPair = KeyGenerator.GenerateEcKeyPair(curveName, random);
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair certKeyPair = KeyGenerator.GenerateRsaKeyPair(2048, random);
            // Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair certKeyPair = KeyGenerator.GenerateDsaKeyPair(1024, random);
            // Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair certKeyPair = KeyGenerator.GenerateDHKeyPair(1024, random);
            // Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair certKeyPair = KeyGenerator.GenerateGostKeyPair(4096, random);



            Org.BouncyCastle.X509.X509Certificate sslCertificate = SelfSignSslCertificate(random, rootCertificate, certKeyPair.Public, rootKeyPair.Private);

            bool val = CerGenerator.ValidateSelfSignedCert(sslCertificate, rootCertificate.GetPublicKey());


            // root 
            (string Private, string Public) rootKeys = KeyPairToPem(rootKeyPair);
            PfxFile.Create(@"skynet.pfx", rootCertificate, rootKeyPair.Private, "");
            WriteCerAndCrt(rootCertificate, @"skynet");
            System.IO.File.WriteAllText(@"skynet_private.key", rootKeys.Private, System.Text.Encoding.ASCII);
            // System.IO.File.WriteAllText(@"ca_public.key", rootKeys.Public, System.Text.Encoding.ASCII);


            // SSL 
            (string Private, string Public) certKeys = KeyPairToPem(certKeyPair);
            PfxFile.Create(@"obelix.pfx", sslCertificate, certKeyPair.Private, "");
            WriteCerAndCrt(sslCertificate, @"obelix");
            System.IO.File.WriteAllText(@"obelix_private.key", certKeys.Private, System.Text.Encoding.ASCII);
            // System.IO.File.WriteAllText(@"obelix_public.key", certKeys.Public, System.Text.Encoding.ASCII);



            string pemCert = ToPem(sslCertificate);
            System.IO.File.WriteAllText(@"obelix.pem", pemCert, System.Text.Encoding.ASCII);

            System.ReadOnlySpan<char> certSpan = System.MemoryExtensions.AsSpan(pemCert);
            System.ReadOnlySpan<char> keySpan = System.MemoryExtensions.AsSpan(certKeys.Private);

            System.Security.Cryptography.X509Certificates.X509Certificate2 certSslLoaded = System.Security.Cryptography.X509Certificates.X509Certificate2.CreateFromPem(certSpan, keySpan);
            System.Console.WriteLine(sslCertificate);
            System.Console.WriteLine(certSslLoaded);
            Org.BouncyCastle.X509.X509Certificate certly = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(certSslLoaded);
            bool b = certly.Equals(rootCertificate);
            System.Console.WriteLine(b);

            // certly.GetPublicKey()


            Org.BouncyCastle.X509.X509Certificate cert = ReadCertificate("obelix.pem");
            System.Console.WriteLine(cert);


            PfxData pfx = PfxFile.Read("obelix.pfx");

            System.Console.WriteLine(pfx.Certificate);
            System.Console.WriteLine(pfx.PrivateKey);
            
        } // End Sub Test 


        public static (string Private, string Public) KeyPairToPem(Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair keyPair)
        {
            string k1 = KeyToPemString(keyPair.Private);
            string k2 = KeyToPemString(keyPair.Public);

            return new System.ValueTuple<string, string>(k1, k2);
        }

        public static string KeyToPemString(Org.BouncyCastle.Crypto.AsymmetricKeyParameter key)
        {
            string retValue = null;

            // id_rsa
            using (System.IO.TextWriter textWriter = new System.IO.StringWriter())
            {
                Org.BouncyCastle.OpenSsl.PemWriter pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(textWriter);
                pemWriter.WriteObject(key);
                pemWriter.Writer.Flush();

                retValue = textWriter.ToString();
            } // End Using textWriter 

            return retValue;
        }




        // Most systems accept both formats, but if you need to you can convert one to the other via openssl 
        // Certificate file should be PEM-encoded X.509 Certificate file:
        // openssl x509 -inform DER -in certificate.cer -out certificate.pem

        // Note: The PEM format is the most common format used for certificates. 
        // Extensions used for PEM certificates are cer, crt, and pem. 
        // They are Base64 encoded ASCII files. The DER format is the binary form of the certificate. 
        // DER formatted certificates do not contain the "BEGIN CERTIFICATE/END CERTIFICATE" statements. 
        // DER formatted certificates most often use the '.der' extension.
        // Note: 
        // https://stackoverflow.com/questions/642284/apache-with-ssl-how-to-convert-cer-to-crt-certificates
        // https://knowledge.digicert.com/solution/SO26449.html
        // https://info.ssl.com/how-to-der-vs-crt-vs-cer-vs-pem-certificates-and-how-to-conver-them/
        public static string ToPem(byte[] derEncodedBytes)
        {
            string cert_begin = "-----BEGIN CERTIFICATE-----\n";
            string end_cert = "\n-----END CERTIFICATE-----";
            string pem = System.Convert.ToBase64String(derEncodedBytes);

            string pemCert = cert_begin + pem + end_cert;
            return pemCert;
        } // End Function ToPem 


        public static string ToPem(Org.BouncyCastle.X509.X509Certificate cert)
        {
            byte[] buf = cert.GetEncoded();
            return ToPem(buf);
        }



        public static void WriteCerAndCrt(
              Org.BouncyCastle.X509.X509Certificate certificate
            , string fileName
        )
        {

            using (System.IO.Stream fs = System.IO.File.Open(fileName + ".cer", System.IO.FileMode.Create))
            {
                byte[] buf = certificate.GetEncoded();
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            } // End Using fs 

            // new System.Text.ASCIIEncoding(false)
            // new System.Text.UTF8Encoding(false)
            using (System.IO.Stream fs = System.IO.File.Open(fileName + ".crt", System.IO.FileMode.Create))
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.ASCII))
                {
                    byte[] buf = certificate.GetEncoded();
                    string pem = ToPem(buf);

                    sw.Write(pem);
                    sw.Flush();
                    fs.Flush();
                } // End Using sw 

            } // End Using fs 

        } // End Sub WriteCerAndCrt 



        // https://stackoverflow.com/questions/51703109/nginx-the-ssl-directive-is-deprecated-use-the-listen-ssl
        public static Org.BouncyCastle.X509.X509Certificate GenerateRootCertificate(
              Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair rootKeyPair
            , Org.BouncyCastle.Security.SecureRandom sr)
        {
            string countryIso2Characters = "Laniakea Supercluster";
            string stateOrProvince = "Milky Way Galaxy";
            string localityOrCity = "Planet Earth";
            string companyName = "Skynet Earth Inc.";
            string division = "Skynet Ltd.";
            string domainName = "sky.net";
            string email = "t.800@sky.net";


            Org.BouncyCastle.X509.X509Certificate caRoot = null;
            Org.BouncyCastle.X509.X509Certificate caSsl = null;

            // string curveName = "curve25519"; curveName = "secp256k1";


            CertificateInfo caCertInfo = new CertificateInfo(
                  countryIso2Characters, stateOrProvince
                , localityOrCity, companyName
                , division, domainName, email
                , System.DateTime.UtcNow
                , System.DateTime.UtcNow.AddYears(5)
            );



            caRoot = CerGenerator.GenerateRootCertificate(caCertInfo, sr, rootKeyPair.Public, rootKeyPair.Private);


            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair certificateKeyPair = KeyGenerator.GenerateRsaKeyPair(2048, sr);



            // PfxGenerator.CreatePfxFile(@"ca.pfx", caRoot, kp1.Private, null);
            // CerGenerator.WritePrivatePublicKey("issuer", caCertInfo.IssuerKeyPair);

            return caRoot;
        } // End Sub GenerateRootCertificate 



        public static Org.BouncyCastle.X509.X509Certificate SelfSignSslCertificate(
              Org.BouncyCastle.Security.SecureRandom random
            , Org.BouncyCastle.X509.X509Certificate caRoot
            , Org.BouncyCastle.Crypto.AsymmetricKeyParameter subjectPublicKey
            , Org.BouncyCastle.Crypto.AsymmetricKeyParameter rootCertPrivateKey
        ) // PrivatePublicPemKeyPair subjectKeyPair)
        {
            Org.BouncyCastle.X509.X509Certificate caSsl = null;

            string countryIso2Characters = "GA";
            string stateOrProvince = "Aremorica";
            string localityOrCity = "Erquy, Bretagne";
            string companyName = "Coopérative Ménhir Obelix Gmbh & Co. KGaA";
            string division = "Neanderthal Technology Group (NT)";
            string domainName = "localhost";
            domainName = "*.sql.guru";
            domainName = "localhost";
            string email = "webmaster@localhost";


            CertificateInfo ci = new CertificateInfo(
                  countryIso2Characters, stateOrProvince
                , localityOrCity, companyName
                , division, domainName, email
                , System.DateTime.UtcNow
                , System.DateTime.UtcNow.AddYears(5)
            );

            ci.AddAlternativeNames("localhost", System.Environment.MachineName, "127.0.0.1",
            "sql.guru", "*.sql.guru", "example.int", "foo.int", "bar.int", "foobar.int", "*.com");

            // Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair kp1 = KeyGenerator.GenerateEcKeyPair(curveName, random);
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair kp1 = KeyGenerator.GenerateRsaKeyPair(2048, random);
            // Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair kp1 = KeyGenerator.GenerateDsaKeyPair(1024, random);
            // Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair kp1 = KeyGenerator.GenerateDHKeyPair(1024, random);


            caSsl = CerGenerator.GenerateSslCertificate(
                  ci
                , subjectPublicKey
                , rootCertPrivateKey
                , caRoot
                , random
            );

            /*
            PfxGenerator.CreatePfxFile(@"obelix.pfx", caSsl, kp1.Private, "");
            CerGenerator.WritePrivatePublicKey("obelix", ci.SubjectKeyPair);


            CerGenerator.WriteCerAndCrt(@"ca", caRoot);
            CerGenerator.WriteCerAndCrt(@"obelix", caSsl);
            */

            return caSsl;
        } // End Sub SelfSignSslCertificate 


        public static Org.BouncyCastle.X509.X509Certificate ReadCertificate(string pemLocation)
        {
            Org.BouncyCastle.X509.X509Certificate bouncyCertificate = null;

            Org.BouncyCastle.X509.X509CertificateParser certParser = new Org.BouncyCastle.X509.X509CertificateParser();
            // Org.BouncyCastle.X509.X509Certificate bouncyCertificate = certParser.ReadCertificate(mycert.GetRawCertData());


            using (System.IO.Stream fs = System.IO.File.OpenRead(pemLocation))
            {
                bouncyCertificate = certParser.ReadCertificate(fs);
            } // End Using fs 


            // Org.BouncyCastle.Crypto.AsymmetricKeyParameter pubKey = bouncyCertificate.GetPublicKey();

            return bouncyCertificate;
        } // End Function ReadCertificate 


    } // End Class 


} // End Namespace 
