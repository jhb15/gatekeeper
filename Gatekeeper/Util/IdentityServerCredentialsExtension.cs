// Adapted from code at http://amilspage.com/signing-certificates-idsv4/

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Gatekeeper.Util
{
    public static class IdentityServerCredentialExtension
    {
        public static IIdentityServerBuilder AddSigningCredentialFromConfig(
            this IIdentityServerBuilder builder, IConfigurationSection gatekeeperConfig)
        {
            string certStorageType = gatekeeperConfig.GetValue<string>("CertStorageType");

            switch (certStorageType)
            {
                case "File":
                    AddCertificateFromFile(builder, gatekeeperConfig);
                    break;

                default:
                    builder.AddDeveloperSigningCredential();
                    break;
            }

            return builder;
        }

        private static void AddCertificateFromFile(IIdentityServerBuilder builder, IConfigurationSection gatekeeperConfig)
        {
            var cert = Path.Combine(gatekeeperConfig.GetValue<string>("CertsPath"), "is4cert.pfx");
            var certPassword = gatekeeperConfig.GetValue<string>("TokenCertPassword");

            if (File.Exists(cert))
            {
                builder.AddSigningCredential(new X509Certificate2(cert, certPassword));
            }
            else
            {
                Console.WriteLine($"IdentityServerCredentialExtension cannot find cert file {cert}");
            }
        }
    }
}