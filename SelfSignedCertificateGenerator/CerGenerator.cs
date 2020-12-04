
namespace SelfSignedCertificateGenerator
{


    class CerGenerator
    {


        public static Org.BouncyCastle.Crypto.ISignatureFactory CreateSignatureFactory(
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter privateKey)
        {
            // https://github.com/bcgit/bc-csharp/blob/master/crypto/src/crypto/operators/Asn1Signature.cs
            // https://github.com/kerryjiang/BouncyCastle.Crypto/blob/master/Crypto/x509/X509Utilities.cs
            Org.BouncyCastle.Crypto.ISignatureFactory signatureFactory = null;
            // Org.BouncyCastle.X509.X509Utilities.GetAlgorithmOid("algorithm");

            // new Asn1SignatureFactory("SHA512WITHRSA", issuerKeyPair.Private, rand
            // erObjectIdentifier sigOid = X509Utilities.GetAlgorithmOid(algorithm);
            // this.algID = X509Utilities.GetSigAlgID(sigOid, algorithm);

            if (privateKey is Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters)
            {
#if false
                // Values from Org.BouncyCastle.Crypto.Operators.X509Utilities // in Asn1Signature.cs 
                // GOST3411WITHECGOST3410-2001r

                signatureFactory = new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory(
                      Org.BouncyCastle.Asn1.CryptoPro.CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001.ToString()
                    , privateKey
                );
                
                return signatureFactory;
#endif

                // Org.BouncyCastle.Asn1.X9.X9ObjectIdentifiers.ECDsaWithSha512
                signatureFactory = new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory(
                      Org.BouncyCastle.Asn1.X9.X9ObjectIdentifiers.ECDsaWithSha256.ToString()
                    // Org.BouncyCastle.Asn1.X9.X9ObjectIdentifiers.ECDsaWithSha512.ToString()
                    , privateKey
                );
            }
            else if (privateKey is Org.BouncyCastle.Crypto.Parameters.Gost3410KeyParameters)
            {
                signatureFactory = new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory(
                       Org.BouncyCastle.Asn1.CryptoPro.CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94.ToString()
                     , privateKey
                 );
            }
            else if (privateKey is Org.BouncyCastle.Crypto.Parameters.DsaPrivateKeyParameters)
            {
                signatureFactory = new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory(
                     // Org.BouncyCastle.Asn1.Nist.NistObjectIdentifiers.DsaWithSha256.ToString()
                     Org.BouncyCastle.Asn1.Nist.NistObjectIdentifiers.DsaWithSha512.ToString()
                    , privateKey
                );
            }
            else if (privateKey is Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters)
            {
                // Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Sha512WithRsaEncryption
                signatureFactory = new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory(
                      Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Sha256WithRsaEncryption.ToString()
                    //Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Sha512WithRsaEncryption.ToString()
                    , privateKey
                );
            }

            return signatureFactory;
        } // End Function CreateSignatureFactory 


        public static void AddExtensions(Org.BouncyCastle.X509.X509V3CertificateGenerator certificateGenerator
            , CertificateInfo certificateInfo)
        {
            foreach (System.Collections.Generic.KeyValuePair<string, Org.BouncyCastle.Asn1.Asn1Encodable> kvp
                     in certificateInfo.CriticalExtensions)
            {
                certificateGenerator.AddExtension(kvp.Key, true, kvp.Value);
            } // Next kvp 

            foreach (System.Collections.Generic.KeyValuePair<string, Org.BouncyCastle.Asn1.Asn1Encodable> kvp
                in certificateInfo.NonCriticalExtensions)
            {
                certificateGenerator.AddExtension(kvp.Key, false, kvp.Value);
            } // Next kvp 

        } // End Sub AddExtensions 


        public static Org.BouncyCastle.X509.X509Certificate GenerateSslCertificate(
              CertificateInfo certificateInfo1
            , Org.BouncyCastle.Crypto.AsymmetricKeyParameter subjectPublicKey
            , Org.BouncyCastle.Crypto.AsymmetricKeyParameter issuerPrivateKey
            , Org.BouncyCastle.X509.X509Certificate rootCertificate
            , Org.BouncyCastle.Security.SecureRandom secureRandom
        )
        {
            // The Certificate Generator
            Org.BouncyCastle.X509.X509V3CertificateGenerator certificateGenerator =
                new Org.BouncyCastle.X509.X509V3CertificateGenerator();

            certificateGenerator.SetSubjectDN(certificateInfo1.Subject);
            certificateGenerator.SetIssuerDN(rootCertificate.IssuerDN);


            Org.BouncyCastle.Math.BigInteger serialNumber =
                Org.BouncyCastle.Utilities.BigIntegers.CreateRandomInRange(
                Org.BouncyCastle.Math.BigInteger.One,
                Org.BouncyCastle.Math.BigInteger.ValueOf(System.Int64.MaxValue), secureRandom
            );


            certificateGenerator.SetSerialNumber(Org.BouncyCastle.Math.BigInteger.ValueOf(1));


            certificateGenerator.SetNotBefore(certificateInfo1.ValidFrom);
            certificateGenerator.SetNotAfter(certificateInfo1.ValidTo);

            certificateGenerator.AddExtension(Org.BouncyCastle.Asn1.X509.X509Extensions.SubjectAlternativeName.Id
                , false
                , certificateInfo1.SubjectAlternativeNames
            );


            certificateGenerator.SetPublicKey(subjectPublicKey);




            Org.BouncyCastle.Asn1.X509.SubjectKeyIdentifier subjectKeyIdentifierExtension =
                new Org.BouncyCastle.Asn1.X509.SubjectKeyIdentifier(
                Org.BouncyCastle.X509.SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(subjectPublicKey)
            );

            certificateGenerator.AddExtension(
                  Org.BouncyCastle.Asn1.X509.X509Extensions.SubjectKeyIdentifier.Id
                , false
                , subjectKeyIdentifierExtension
            );


            certificateGenerator.AddExtension(Org.BouncyCastle.Asn1.X509.X509Extensions.ExtendedKeyUsage.Id, false,
                new Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage(Org.BouncyCastle.Asn1.X509.KeyPurposeID.IdKPServerAuth)
            );


            // rootCertificate.GetPublicKey():
            // rootCertificate.GetEncoded()
            // System.Security.Cryptography.X509Certificates.X509Certificate2 srp = 
            //    new System.Security.Cryptography.X509Certificates.X509Certificate2(rootCertificate.GetEncoded());

            // srp.PrivateKey
            // srp.HasPrivateKey
            // Org.BouncyCastle.Crypto.AsymmetricKeyParameter Akp = Org.BouncyCastle.Security.DotNetUtilities.GetKeyPair(srp.PrivateKey).Private;

            // Org.BouncyCastle.Crypto.IDigest algorithm = Org.BouncyCastle.Security.DigestUtilities.GetDigest(rootCertificate.SigAlgOid);
            // var signature = new X509Certificate2Signature(cert, algorithm);

            // rootCertificate.SigAlgOid
            // rootCertificate.GetSignature();
            // srp.GetRawCertData()

            // X509CertificateParser certParser = new X509CertificateParser();
            // X509Certificate privateCertBouncy = certParser.ReadCertificate(mycert.GetRawCertData());
            // AsymmetricKeyParameter pubKey = privateCertBouncy.GetPublicKey();


            AddExtensions(certificateGenerator, certificateInfo1);

            Org.BouncyCastle.Crypto.ISignatureFactory signatureFactory = CreateSignatureFactory(issuerPrivateKey);
            return certificateGenerator.Generate(signatureFactory);
        } // End Function GenerateSslCertificate 


        public static bool ValidateSelfSignedCert(
          Org.BouncyCastle.X509.X509Certificate cert,
          Org.BouncyCastle.Crypto.ICipherParameters pubKey
        )
        {
            cert.CheckValidity(System.DateTime.UtcNow);
            byte[] tbsCert = cert.GetTbsCertificate(); // (TBS is short for To Be Signed), see RFC5280 for all the gory details.
            byte[] sig = cert.GetSignature();

            Org.BouncyCastle.Crypto.ISigner signer = Org.BouncyCastle.Security.SignerUtilities.GetSigner(
                cert.SigAlgName
            );

            signer.Init(false, pubKey);
            signer.BlockUpdate(tbsCert, 0, tbsCert.Length);
            return signer.VerifySignature(sig);
        } // End Function ValidateSelfSignedCert 




        public static Org.BouncyCastle.X509.X509Certificate GenerateRootCertificate(
              CertificateInfo certificateInfo
            , Org.BouncyCastle.Security.SecureRandom secureRandom
            , Org.BouncyCastle.Crypto.AsymmetricKeyParameter publicKey
            , Org.BouncyCastle.Crypto.AsymmetricKeyParameter privateKey
            )
        {
            // The Certificate Generator
            Org.BouncyCastle.X509.X509V3CertificateGenerator certificateGenerator =
                new Org.BouncyCastle.X509.X509V3CertificateGenerator();

            Org.BouncyCastle.Asn1.X509.X509Name subjectDn = certificateInfo.Subject;
            Org.BouncyCastle.Asn1.X509.X509Name issuerDn = certificateInfo.Subject;

            certificateGenerator.SetSubjectDN(issuerDn);
            certificateGenerator.SetIssuerDN(issuerDn);


            certificateGenerator.SetNotBefore(certificateInfo.ValidFrom);
            certificateGenerator.SetNotAfter(certificateInfo.ValidTo);


            AddExtensions(certificateGenerator, certificateInfo);
            Org.BouncyCastle.Crypto.ISignatureFactory signatureFactory = CreateSignatureFactory(privateKey);


            certificateGenerator.SetPublicKey(publicKey);


            // Serial Number
            Org.BouncyCastle.Math.BigInteger serialNumber =
                Org.BouncyCastle.Utilities.BigIntegers.CreateRandomInRange(
                      Org.BouncyCastle.Math.BigInteger.One
                    , Org.BouncyCastle.Math.BigInteger.ValueOf(long.MaxValue)
                    , secureRandom
            );

            certificateGenerator.SetSerialNumber(serialNumber);


            certificateGenerator.AddExtension(Org.BouncyCastle.Asn1.X509.X509Extensions.KeyUsage, true
                , new Org.BouncyCastle.Asn1.X509.KeyUsage(
                      Org.BouncyCastle.Asn1.X509.KeyUsage.DigitalSignature
                    | Org.BouncyCastle.Asn1.X509.KeyUsage.KeyCertSign
                    | Org.BouncyCastle.Asn1.X509.KeyUsage.CrlSign
                    | Org.BouncyCastle.Asn1.X509.KeyUsage.KeyEncipherment
                    | Org.BouncyCastle.Asn1.X509.KeyUsage.DataEncipherment
                    | Org.BouncyCastle.Asn1.X509.KeyUsage.KeyAgreement
                    | Org.BouncyCastle.Asn1.X509.KeyUsage.NonRepudiation
                )
            );



            Org.BouncyCastle.Asn1.X509.AuthorityKeyIdentifier authorityKeyIdentifierExtension =
                new Org.BouncyCastle.Asn1.X509.AuthorityKeyIdentifier(
                Org.BouncyCastle.X509.SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey)
            );



            Org.BouncyCastle.Asn1.X509.SubjectKeyIdentifier subjectKeyIdentifierExtension =
                new Org.BouncyCastle.Asn1.X509.SubjectKeyIdentifier(
                Org.BouncyCastle.X509.SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey)
            );

            certificateGenerator.AddExtension(
                  Org.BouncyCastle.Asn1.X509.X509Extensions.SubjectKeyIdentifier.Id
                , false
                , subjectKeyIdentifierExtension
            );

            certificateGenerator.AddExtension(
                 Org.BouncyCastle.Asn1.X509.X509Extensions.AuthorityKeyIdentifier.Id
               , false
               , authorityKeyIdentifierExtension
           );



            // Set certificate intended purposes to only Server Authentication
            //certificateGenerator.AddExtension(Org.BouncyCastle.Asn1.X509.X509Extensions.ExtendedKeyUsage.Id
            //    , true 
            //    , new Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage(Org.BouncyCastle.Asn1.X509.KeyPurposeID.IdKPServerAuth)
            //);


            certificateGenerator.AddExtension(Org.BouncyCastle.Asn1.X509.X509Extensions.ExtendedKeyUsage.Id, true
                , new Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage(new[] {
                      Org.BouncyCastle.Asn1.X509.KeyPurposeID.IdKPClientAuth,
                      Org.BouncyCastle.Asn1.X509.KeyPurposeID.IdKPServerAuth
                })
            );


            // Only if we generate a root-Certificate 
            certificateGenerator.AddExtension(Org.BouncyCastle.Asn1.X509.X509Extensions.BasicConstraints.Id
                , true
                , new Org.BouncyCastle.Asn1.X509.BasicConstraints(true)
            );

            return certificateGenerator.Generate(signatureFactory);
        } // End Function GenerateRootCertificate 


    }
}
