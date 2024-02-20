namespace SixtyThreeBits.Core.DTO
{
    public class SystemPropertiesDTO
    {
        #region Properties
        public string ProjectName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string ContactAddress { get; set; }
        public string FacebookUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string YoutubeUrl { get; set; }
        public string LinkedInUrl { get; set; }
        public string GoogleMapsIFrame { get; set; }
        public string ScriptsHeader { get; set; }
        public string ScriptsBodyStart { get; set; }
        public string ScriptsBodyEnd { get; set; }

        public bool IsEmailSmtpEnabled { get; set; }
        public string SmtpAddress { get; set; }
        public int? SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public bool SmtpUseSsl { get; set; }
        public string SmtpFrom { get; set; }

        public bool IsEmailMailgunEnabled { get; set; }
        public string MailgunBaseUrl { get; set; }
        public string MailgunApiKey { get; set; }
        public string MailgunDomain { get; set; }
        public string MailgunFrom { get; set; }
        public string MailgunWebhookWebhookSigningKey { get; set; }

        public bool IsEmailOffice365Enabled { get; set; }
        public string MicrosoftGraphServiceTenant { get; set; }
        public string MicrosoftGraphServiceClientID { get; set; }
        public string MicrosoftGraphServiceClientSecret { get; set; }
        public string MicrosoftGraphServiceUserID { get; set; }

        public string AwsAccessKeyID { get; set; }
        public string AwsSecretAccessKey { get; set; }
        public string AwsS3RegionSystemName { get; set; }
        public string AwsS3BucketNamePublic { get; set; }

        public string AzureConnectionString { get; set; }
        public string AzureBlobStorageContainerName { get; set; }

        public string ReCaptchaSiteKey { get; set; }
        public string ReCaptchaSecretKey { get; set; }
        #endregion
    }
}
