
namespace SelfSignedCertificateGenerator
{


    class TODO
    {


        public static void ReadPkFromCertificate()
        {
            string pfxLocation = @"obelix.pfx";


            Org.BouncyCastle.Pkcs.Pkcs12Store store = null;

            using (System.IO.Stream pfxStream = System.IO.File.OpenRead(pfxLocation))
            {
                store = new Org.BouncyCastle.Pkcs.Pkcs12Store(pfxStream, "".ToCharArray());
            }

            System.Console.WriteLine(store);

            foreach (string alias in store.Aliases)
            {
                System.Console.WriteLine(alias);

                // https://7thzero.com/blog/bouncy-castle-convert-a-bouncycastle-asymmetrickeyentry-to-a-.ne
                if (store.IsKeyEntry(alias))
                {
                    Org.BouncyCastle.Pkcs.AsymmetricKeyEntry keyEntry = store.GetKey(alias);
                    System.Console.WriteLine(keyEntry);
                    Org.BouncyCastle.Crypto.AsymmetricKeyParameter privateKey = keyEntry.Key;
                    System.Console.WriteLine(privateKey.IsPrivate);
                } // End if (store.IsKeyEntry((string)alias))


                Org.BouncyCastle.Pkcs.X509CertificateEntry certEntry = store.GetCertificate(alias);
                Org.BouncyCastle.X509.X509Certificate cert = certEntry.Certificate;
                System.Console.WriteLine(cert);

                Org.BouncyCastle.Crypto.AsymmetricKeyParameter publicKey = cert.GetPublicKey();
                System.Console.WriteLine(publicKey);



                System.Security.Cryptography.X509Certificates.X509Certificate2 cert2 = new System.Security.Cryptography.X509Certificates.X509Certificate2(pfxLocation);
                // cert2.PrivateKey = null;

                if (cert2.HasPrivateKey)
                {
                    System.Console.WriteLine(cert2.PrivateKey);
                }

            }

        } // End Sub ReadPkFromCertificate


    }


}
