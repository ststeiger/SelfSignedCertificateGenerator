
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
            // Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair rootKeyPair = KeyGenerator.GenerateGhostKeyPair(4096, random);



            Org.BouncyCastle.X509.X509Certificate rootCertificate = GenerateRootCertificate(rootKeyPair, random);


            // Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair certKeyPair = KeyGenerator.GenerateEcKeyPair(curveName, random);
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair certKeyPair = KeyGenerator.GenerateRsaKeyPair(2048, random);
            // Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair certKeyPair = KeyGenerator.GenerateDsaKeyPair(1024, random);
            // Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair certKeyPair = KeyGenerator.GenerateDHKeyPair(1024, random);


            Org.BouncyCastle.X509.X509Certificate sslCertificate = SelfSignSslCertificate(random, rootCertificate, certKeyPair.Public, rootKeyPair.Private);

            bool val = CerGenerator.ValidateSelfSignedCert(sslCertificate, rootCertificate.GetPublicKey());
            
            
            // root 
            // PfxGenerator.CreatePfxFile(@"ca.pfx", caRoot, kp1.Private, null);
            // CerGenerator.WritePrivatePublicKey("issuer", caCertInfo.IssuerKeyPair);
            
            PfxFile.Create(@"obelix.pfx", sslCertificate, certKeyPair.Private, "");
            CerGenerator.WritePrivatePublicKey("obelix", certKeyPair.Private);
            
            System.IO.File.WriteAllText(fileName + "_priv.pem", keyPair.PrivateKey, System.Text.Encoding.ASCII);
            System.IO.File.WriteAllText(fileName + "_pub.pem", keyPair.PrivateKey, System.Text.Encoding.ASCII);you
            
            
            WriteCerAndCrt(@"ca", rootCertificate);
            WriteCerAndCrt(@"obelix", sslCertificate);
            
        } // End Sub Test 
        
        
        public static (string Private, string Public) GetPemKeyPair(Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair keyPair)
        {
            string k1 = KeyToString(keyPair.Private);
            string k2 = KeyToString(keyPair.Public);
            
            return new System.ValueTuple<string, string>(k1, k2);
        }

        public static string KeyToString(Org.BouncyCastle.Crypto.AsymmetricKeyParameter key)
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
        // They are Base64 encoded ASCII files.The DER format is the binary form of the certificate. 
        // DER formatted certificates do not contain the "BEGIN CERTIFICATE/END CERTIFICATE" statements. 
        // DER formatted certificates most often use the '.der' extension.
        // Note: 
        // https://stackoverflow.com/questions/642284/apache-with-ssl-how-to-convert-cer-to-crt-certificates
        // https://knowledge.digicert.com/solution/SO26449.html
        // https://info.ssl.com/how-to-der-vs-crt-vs-cer-vs-pem-certificates-and-how-to-conver-them/
        public static string ToPem(byte[] buf)
        {
            string cert_begin = "-----BEGIN CERTIFICATE-----\n";
            string end_cert = "\n-----END CERTIFICATE-----";
            string pem = System.Convert.ToBase64String(buf);

            string pemCert = cert_begin + pem + end_cert;
            return pemCert;
        } // End Function ToPem 
        

        
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
            string countryIso2Characters = "EA";
            string stateOrProvince = "Europe";
            string localityOrCity = "NeutralZone";
            string companyName = "Skynet Earth Inc.";
            string division = "Skynet mbH";
            string domainName = "Skynet";
            string email = "root@sky.net";


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
            string division = "NT (Neanderthal Technology)";
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
            "sql.guru", "*.sql.guru");

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

    } // End Class 


} // End Namespace 
