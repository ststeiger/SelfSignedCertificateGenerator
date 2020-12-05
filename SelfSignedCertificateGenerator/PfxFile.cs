
namespace SelfSignedCertificateGenerator
{


    public class PfxData
    {
        public Org.BouncyCastle.X509.X509Certificate Certificate;
        public Org.BouncyCastle.Crypto.AsymmetricKeyParameter PrivateKey;
    }


    public class PfxFile
    {


        // System.Security.Cryptography.X509Certificates.X509Certificate2.Import (string fileName);

        // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.x509certificate2.import?view=netframework-4.7.2
        // https://gist.github.com/yutopio/a217a4af63cf6bcf0a530c14c074cf8f
        // https://gist.githubusercontent.com/yutopio/a217a4af63cf6bcf0a530c14c074cf8f/raw/42b2f8cb27f6d22b7e22d65da5bbd0f1ce9b2fff/cert.cs
        // https://stackoverflow.com/questions/44755155/store-pkcs12-container-pfx-with-bouncycastle
        // https://github.com/Worlaf/RSADemo/blob/328692e28e48db92340d55563480c8724d916384/RSADemo_WinForms/frmRsaDemo.cs
        public static void Create(
              string fileName
            , Org.BouncyCastle.X509.X509Certificate certificate
            , Org.BouncyCastle.Crypto.AsymmetricKeyParameter privateKey
              , string password = "")
        {
            // create certificate entry
            Org.BouncyCastle.Pkcs.X509CertificateEntry certEntry =
                new Org.BouncyCastle.Pkcs.X509CertificateEntry(certificate);
            string friendlyName = certificate.SubjectDN.ToString();

            if (!friendlyName.Contains("obelix", System.StringComparison.InvariantCultureIgnoreCase))
                friendlyName = "Skynet Certification Authority";
            else
                friendlyName = "Coopérative Ménhir Obelix Gmbh & Co. KGaA";


            // get bytes of private key.
            Org.BouncyCastle.Asn1.Pkcs.PrivateKeyInfo keyInfo = Org.BouncyCastle.Pkcs.PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);
            //byte[] keyBytes = keyInfo.ToAsn1Object().GetEncoded();

            Org.BouncyCastle.Pkcs.Pkcs12StoreBuilder builder = new Org.BouncyCastle.Pkcs.Pkcs12StoreBuilder();
            builder.SetUseDerEncoding(true);



            Org.BouncyCastle.Pkcs.Pkcs12Store store = builder.Build();

            store.SetCertificateEntry(friendlyName, certEntry);

            // create store entry
            store.SetKeyEntry(
                  //keyFriendlyName
                  friendlyName
                , new Org.BouncyCastle.Pkcs.AsymmetricKeyEntry(privateKey)
                , new Org.BouncyCastle.Pkcs.X509CertificateEntry[] { certEntry }
            );


            byte[] pfxBytes = null;

            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                // Cert is contained in store
                // null: no password, "": an empty passwords
                // note: Linux needs empty password on null...
                store.Save(stream, password == null ? "".ToCharArray() : password.ToCharArray(), new Org.BouncyCastle.Security.SecureRandom());
                // stream.Position = 0;
                pfxBytes = stream.ToArray();
            } // End Using stream 


#if WITH_MS_PFX
            WithMsPfx(pfxBytes, fileName, password);
#else
            byte[] result = Org.BouncyCastle.Pkcs.Pkcs12Utilities.ConvertToDefiniteLength(pfxBytes);
            // this.StoreCertificate(System.Convert.ToBase64String(result));

            using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(System.IO.File.Open(fileName, System.IO.FileMode.Create)))
            {
                writer.Write(result);
            } // End Using writer 
#endif

        } // End Sub Create


        private static void WithMsPfx(byte[] pfxBytes, string fileName, string password)
        {
            System.Security.Cryptography.X509Certificates.X509Certificate2 convertedCertificate =
                new System.Security.Cryptography.X509Certificates.X509Certificate2(pfxBytes,
                    "", // PW
                    System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.PersistKeySet | System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.Exportable);

            byte[] bytes = convertedCertificate.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Pfx, password);
            System.IO.File.WriteAllBytes(fileName, bytes);
        } // End Sub WithMsPfx 



        public static PfxData Read(string pfxFilePath, string password = "")
        {
            Org.BouncyCastle.Pkcs.Pkcs12Store store = null;

            using (System.IO.Stream pfxStream = System.IO.File.OpenRead(pfxFilePath))
            {
                store = new Org.BouncyCastle.Pkcs.Pkcs12Store(pfxStream, password.ToCharArray());
            }

            // System.Console.WriteLine(store);


            foreach (string alias in store.Aliases)
            {
                Org.BouncyCastle.Pkcs.X509CertificateEntry certEntry = store.GetCertificate(alias);
                Org.BouncyCastle.X509.X509Certificate cert = certEntry.Certificate;

                // Org.BouncyCastle.Crypto.AsymmetricKeyParameter publicKey = cert.GetPublicKey();
                // System.Console.WriteLine(publicKey);

                // https://7thzero.com/blog/bouncy-castle-convert-a-bouncycastle-asymmetrickeyentry-to-a-.ne
                if (store.IsKeyEntry(alias))
                {
                    Org.BouncyCastle.Pkcs.AsymmetricKeyEntry keyEntry = store.GetKey(alias);
                    Org.BouncyCastle.Crypto.AsymmetricKeyParameter privateKey = keyEntry.Key;

                    if (privateKey.IsPrivate)
                        return new PfxData() 
                               { 
                                   Certificate = cert, 
                                   PrivateKey = privateKey 
                               };
                } // End if (store.IsKeyEntry((string)alias))

            } // Next alias 

            return null;
        } // End Sub Read


        public static System.Security.Cryptography.X509Certificates.X509Certificate2 
            MicrosoftCertificateFromPfx(string pfxFilePath, string password = "")
        {
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert =
                  new System.Security.Cryptography.X509Certificates.X509Certificate2(
                  pfxFilePath
                , password
            );

            return cert;
        }


    }


}