
namespace SelfSignedCertificateGenerator
{
    
    
    public class KeyGenerator
    {
        
        
        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateGostKeyPair(
            int length
           , Org.BouncyCastle.Security.SecureRandom secureRandom
           )
        {
            Org.BouncyCastle.Crypto.KeyGenerationParameters keygenParam =
                new Org.BouncyCastle.Crypto.KeyGenerationParameters(secureRandom, length);
            
            Org.BouncyCastle.Crypto.Generators.Gost3410KeyPairGenerator keyGenerator =
                new Org.BouncyCastle.Crypto.Generators.Gost3410KeyPairGenerator();
            keyGenerator.Init(keygenParam);
            
            return keyGenerator.GenerateKeyPair();
        } // End Function GenerateGostKeyPair 
        
        
        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateRsaKeyPair(
             int length
            , Org.BouncyCastle.Security.SecureRandom secureRandom
            )
        {
            Org.BouncyCastle.Crypto.KeyGenerationParameters keygenParam =
                new Org.BouncyCastle.Crypto.KeyGenerationParameters(secureRandom, length);
            
            Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator keyGenerator =
                new Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator();
            keyGenerator.Init(keygenParam);
            
            return keyGenerator.GenerateKeyPair();
        } // End Function GenerateRsaKeyPair 
        
        
        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateDsaKeyPair(
            int length
            , Org.BouncyCastle.Security.SecureRandom secureRandom
        )
        {
            Org.BouncyCastle.Crypto.Generators.DsaParametersGenerator paramgen = 
                new Org.BouncyCastle.Crypto.Generators.DsaParametersGenerator();
            paramgen.Init(length, 100, secureRandom);
            
            Org.BouncyCastle.Crypto.Parameters.DsaKeyGenerationParameters param = new Org.BouncyCastle.Crypto.Parameters.DsaKeyGenerationParameters(secureRandom, paramgen.GenerateParameters());
            
            Org.BouncyCastle.Crypto.Generators.DsaKeyPairGenerator keyGenerator =
                new Org.BouncyCastle.Crypto.Generators.DsaKeyPairGenerator();
            keyGenerator.Init(param);
            
            return keyGenerator.GenerateKeyPair();
        } // End Function GenerateDsaKeyPair 
        
        
        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateElGamalKeyPair(
            int length
            , Org.BouncyCastle.Security.SecureRandom secureRandom
        )
        {
            Org.BouncyCastle.Crypto.Generators.ElGamalParametersGenerator pg = 
                new Org.BouncyCastle.Crypto.Generators.ElGamalParametersGenerator();
            pg.Init(length, 100, secureRandom);
            
            Org.BouncyCastle.Crypto.Parameters.ElGamalParameters egp = pg.GenerateParameters();
            
            
            Org.BouncyCastle.Crypto.Parameters.ElGamalKeyGenerationParameters pars = 
                new Org.BouncyCastle.Crypto.Parameters.ElGamalKeyGenerationParameters(secureRandom, egp);
            
            
            Org.BouncyCastle.Crypto.Generators.ElGamalKeyPairGenerator keyGenerator = 
                new Org.BouncyCastle.Crypto.Generators.ElGamalKeyPairGenerator();
            keyGenerator.Init(pars);
            
            return keyGenerator.GenerateKeyPair();
        } // End Function GenerateElGamalKeyPair 
        
        
        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateDHKeyPair(
            int length
            , Org.BouncyCastle.Security.SecureRandom secureRandom
        )
        {
            // Org.BouncyCastle.Math.BigInteger p = new BigInteger("123", 10);
            // Org.BouncyCastle.Math.BigInteger g = new BigInteger("456", 10);
            
            // Org.BouncyCastle.Crypto.Parameters.DHParameters dhParams = 
            //     new Org.BouncyCastle.Crypto.Parameters.DHParameters(p, g);
            
            Org.BouncyCastle.Crypto.Generators.DHParametersGenerator pg =
                new Org.BouncyCastle.Crypto.Generators.DHParametersGenerator();
            pg.Init(length, 100, secureRandom);
            
            Org.BouncyCastle.Crypto.Parameters.DHParameters dhp = pg.GenerateParameters();
            
            Org.BouncyCastle.Crypto.Parameters.DHKeyGenerationParameters pars = 
                new Org.BouncyCastle.Crypto.Parameters.DHKeyGenerationParameters(secureRandom, dhp);
            
            Org.BouncyCastle.Crypto.Generators.DHKeyPairGenerator keyGenerator =
                new Org.BouncyCastle.Crypto.Generators.DHKeyPairGenerator();
            keyGenerator.Init(pars);
            
            return keyGenerator.GenerateKeyPair();
        } // End Function GenerateDHKeyPair 
        
        
        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateEcKeyPair(
              Org.BouncyCastle.Asn1.X9.X9ECParameters ecParam
            , Org.BouncyCastle.Security.SecureRandom secureRandom
            )
        {
            Org.BouncyCastle.Crypto.Parameters.ECDomainParameters ecDomain =
                new Org.BouncyCastle.Crypto.Parameters.ECDomainParameters(ecParam.Curve, ecParam.G, ecParam.N);
            
            Org.BouncyCastle.Crypto.Parameters.ECKeyGenerationParameters keygenParam =
                new Org.BouncyCastle.Crypto.Parameters.ECKeyGenerationParameters(ecDomain, secureRandom);
            
            Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator keyGenerator =
                new Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator();
            
            keyGenerator.Init(keygenParam);
            return keyGenerator.GenerateKeyPair();
        } // End Function GenerateEcKeyPair 
        
        
        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateEcKeyPair(
              string curveName
            , Org.BouncyCastle.Security.SecureRandom secureRandom
            )
        {
            Org.BouncyCastle.Asn1.X9.X9ECParameters ecParam = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName(curveName);
            
            if (ecParam == null)
                ecParam = Org.BouncyCastle.Crypto.EC.CustomNamedCurves.GetByName(curveName);
            
            return GenerateEcKeyPair(ecParam, secureRandom);
        } // End Function GenerateEcKeyPair 




        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateGostKeyPair(string curveName, Org.BouncyCastle.Security.SecureRandom random)
        {
            Org.BouncyCastle.Asn1.X9.X9ECParameters ecParam = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName(curveName);

            if (ecParam == null)
                ecParam = Org.BouncyCastle.Crypto.EC.CustomNamedCurves.GetByName(curveName);


            Org.BouncyCastle.Crypto.Parameters.ECDomainParameters parameters = new Org.BouncyCastle.Crypto.Parameters.ECDomainParameters(ecParam);
            Org.BouncyCastle.Crypto.Parameters.ECKeyGenerationParameters keyGenerationParameters = new Org.BouncyCastle.Crypto.Parameters.ECKeyGenerationParameters(parameters, random);

            Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator keygenerator = new Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator("ECGOST3410");
            keygenerator.Init(keyGenerationParameters);
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair pair = keygenerator.GenerateKeyPair();

            // Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters validatorPrivate = (Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters)pair.Private;
            // Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters validatorPublic = (Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters)pair.Public;

            return pair; 
        } // End Function GenerateGostKeyPair 


    } // End Class KeyGenerator 


} // End Namespace RedmineMailService.CertSSL 
