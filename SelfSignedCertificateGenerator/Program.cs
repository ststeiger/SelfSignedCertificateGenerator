
namespace SelfSignedCertificateGenerator
{


    public class Program
    {


        public static async System.Threading.Tasks.Task Main(string[] args)
        {


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
        } // End Sub Test 


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
