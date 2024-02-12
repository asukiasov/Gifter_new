using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Core.Infrastructure.DTO;
using SixtyThreeBits.Core.Infrastructure.Libraries.EmailClients;
using SixtyThreeBits.Core.Infrastructure.Services;

namespace SixtyThreeBits.Core.Infrastructure.Factories
{
    public class EmailClientFactory
    {
        #region Methods
        public static IEmailClient GetEmailClientBySystemProperties(SystemPropertiesDTO systemProperties)
        {
            if (systemProperties.IsEmailSmtpEnabled)
            {
                return new SmtpEmailClient(
                    smtpAddress: systemProperties.SmtpAddress,
                    smtpPort: systemProperties.SmtpPort,
                    smtpUsername: systemProperties.SmtpUsername,
                    smtpPassword: systemProperties.SmtpPassword,
                    smtpUseSSL: systemProperties.SmtpUseSsl,
                    smtpFromName: systemProperties.SmtpFrom
                );
            }
            else if (systemProperties.IsEmailMailgunEnabled)
            {
                return new MailgunEmailClient(
                    baseUrl: systemProperties.MailgunBaseUrl,
                    mailgunApiKey: systemProperties.MailgunApiKey,
                    mailgunDomain: systemProperties.MailgunDomain,
                    mailgunFrom: systemProperties.MailgunFrom
                );
            }
            else if (systemProperties.IsEmailOffice365Enabled)
            {
                return new Office365EmailClient(
                    tenant: systemProperties.MicrosoftGraphServiceTenant,
                    clientID: systemProperties.MicrosoftGraphServiceClientID,
                    clientSecret: systemProperties.MicrosoftGraphServiceClientSecret,
                    microsoftUserID: systemProperties.MicrosoftGraphServiceUserID
                );
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
