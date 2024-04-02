namespace SixtyThreeBits.Core.DTO
{
    public record SystemPropertiesDTO
    {
        #region Properties
        public string ProjectName { get; init; }
        public string ContactEmail { get; init; }
        public string ContactPhone { get; init; }
        public string ContactAddress { get; init; }
        public string ContactAddressEng { get; init; }
        public string FacebookUrl { get; init; }
        public string InstagramUrl { get; init; }
        public string TwitterUrl { get; init; }
        public string YoutubeUrl { get; init; }
        public string LinkedInUrl { get; init; }
        public string GoogleMapsIFrame { get; init; }
        public string ScriptsHeader { get; init; }
        public string ScriptsBodyStart { get; init; }
        public string ScriptsBodyEnd { get; init; }

        public bool IsEmailSmtpEnabled { get; init; }
        public string SmtpAddress { get; init; }
        public int? SmtpPort { get; init; }
        public string SmtpUsername { get; init; }
        public string SmtpPassword { get; init; }
        public bool SmtpUseSsl { get; init; }
        public string SmtpFrom { get; init; }

        public bool IsEmailMailgunEnabled { get; init; }
        public string MailgunBaseUrl { get; init; }
        public string MailgunApiKey { get; init; }
        public string MailgunDomain { get; init; }
        public string MailgunFrom { get; init; }
        public string MailgunWebhookWebhookSigningKey { get; init; }

        public bool IsEmailOffice365Enabled { get; init; }
        public string MicrosoftGraphServiceTenant { get; init; }
        public string MicrosoftGraphServiceClientID { get; init; }
        public string MicrosoftGraphServiceClientSecret { get; init; }
        public string MicrosoftGraphServiceUserID { get; init; }

        public string AwsAccessKeyID { get; init; }
        public string AwsSecretAccessKey { get; init; }
        public string AwsS3RegionSystemName { get; init; }
        public string AwsS3BucketNamePublic { get; init; }

        public string AzureConnectionString { get; init; }
        public string AzureBlobStorageContainerName { get; init; }

        public string ReCaptchaSiteKey { get; init; }
        public string ReCaptchaSecretKey { get; init; }
        #endregion
    }

    public record SystemPropertiesIudDTO
    {
        #region Properties
        public string ProjectName { get; init; }
        public string ContactEmail { get; init; }
        public string ContactPhone { get; init; }
        public string ContactAddress { get; init; }
        public string ContactAddressEng { get; init; }
        public string FacebookUrl { get; init; }
        public string InstagramUrl { get; init; }
        public string TwitterUrl { get; init; }
        public string YoutubeUrl { get; init; }
        public string LinkedInUrl { get; init; }
        public string GoogleMapsIFrame { get; init; }
        public string ScriptsHeader { get; init; }
        public string ScriptsBodyStart { get; init; }
        public string ScriptsBodyEnd { get; init; }

        public bool? IsEmailSmtpEnabled { get; init; }
        public string SmtpAddress { get; init; }
        public int? SmtpPort { get; init; }
        public string SmtpUsername { get; init; }
        public string SmtpPassword { get; init; }
        public bool? SmtpUseSsl { get; init; }
        public string SmtpFrom { get; init; }

        public bool? IsEmailMailgunEnabled { get; init; }
        public string MailgunBaseUrl { get; init; }
        public string MailgunApiKey { get; init; }
        public string MailgunDomain { get; init; }
        public string MailgunFrom { get; init; }
        public string MailgunWebhookWebhookSigningKey { get; init; }

        public bool? IsEmailOffice365Enabled { get; init; }
        public string MicrosoftGraphServiceTenant { get; init; }
        public string MicrosoftGraphServiceClientID { get; init; }
        public string MicrosoftGraphServiceClientSecret { get; init; }
        public string MicrosoftGraphServiceUserID { get; init; }

        public string AwsAccessKeyID { get; init; }
        public string AwsSecretAccessKey { get; init; }
        public string AwsS3RegionSystemName { get; init; }
        public string AwsS3BucketNamePublic { get; init; }

        public string AzureConnectionString { get; init; }
        public string AzureBlobStorageContainerName { get; init; }

        public string ReCaptchaSiteKey { get; init; }
        public string ReCaptchaSecretKey { get; init; }
        #endregion
    }
}
