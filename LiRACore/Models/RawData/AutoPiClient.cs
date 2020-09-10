using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiRACore.Models.RawData
{
    public class AutoPiClient
    {
        private static MongoClient _mongoClient = null;
        private static MongoClientSettings _mongoClientSetting = null;

        static AutoPiClient()
        {

        }

        public void CredentialSetting( Credential credential)
        {


            MongoClientSettings settings = new MongoClientSettings();
            settings.Server = new MongoServerAddress(credential.Host, Convert.ToInt32(credential.Port));
            MongoIdentity internalIdentity = new MongoInternalIdentity(credential.DatabaseName, credential.Username);
            PasswordEvidence passwordEvidence = new PasswordEvidence(credential.Password);
            string mongoDbAuthMechanism = "SCRAM-SHA-1";
            MongoCredential mongoCredential =
                    new MongoCredential(mongoDbAuthMechanism, internalIdentity, passwordEvidence);
            settings.Credential = mongoCredential;
            settings.UseTls = true;
            settings.SslSettings = new SslSettings();
            settings.SslSettings.CheckCertificateRevocation = false;
            settings.ConnectionMode = MongoDB.Driver.ConnectionMode.Automatic;

            _mongoClientSetting = settings;

            _mongoClient = new MongoClient(_mongoClientSetting);
        }

        public static MongoClient MongoClient { get { return _mongoClient; } }
    }


}
