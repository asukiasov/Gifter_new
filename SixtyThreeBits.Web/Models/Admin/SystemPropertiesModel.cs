using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Services;
using SixtyThreeBits.Core.Libraries.EmailClients.Factory;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using SixtyThreeBits.Web.Models.Shared;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class SystemPropertiesModel : ModelBase
    {
        #region Methods
        public async Task<PageViewModel> GetPageViewModel()
        {
            var viewModel = new PageViewModel();
            var repository = RepositoriesFactory.GetSystemPropertiesRepository();
            var dbItem = await repository.SystemPropertiesGet();
            viewModel.ProjectName = dbItem.ProjectName;
            viewModel.ContactEmail = dbItem.ContactEmail;
            viewModel.ContactPhone = dbItem.ContactPhone;
            viewModel.ContactAddress = dbItem.ContactAddress;
            viewModel.FacebookUrl = dbItem.FacebookUrl;
            viewModel.TwitterUrl = dbItem.TwitterUrl;
            viewModel.InstagramUrl = dbItem.InstagramUrl;
            viewModel.YoutubeUrl = dbItem.YoutubeUrl;
            viewModel.LinkedInUrl = dbItem.LinkedInUrl;
            viewModel.GoogleMapsIFrame = dbItem.GoogleMapsIFrame;
            viewModel.ScriptsHeader = dbItem.ScriptsHeader;
            viewModel.ScriptsBodyStart = dbItem.ScriptsBodyStart;
            viewModel.ScriptsBodyEnd = dbItem.ScriptsBodyEnd;
            viewModel.IsEmailSmtpEnabled = dbItem.IsEmailSmtpEnabled;
            viewModel.SmtpAddress = dbItem.SmtpAddress;
            viewModel.SmtpPort = dbItem.SmtpPort;
            viewModel.SmtpUsername = dbItem.SmtpUsername;
            viewModel.SmtpPassword = dbItem.SmtpPassword;
            viewModel.SmtpUseSsl = dbItem.SmtpUseSsl;
            viewModel.SmtpFrom = dbItem.SmtpFrom;
            viewModel.IsEmailOffice365Enabled = dbItem.IsEmailOffice365Enabled;
            viewModel.MicrosoftGraphServiceTenant = dbItem.MicrosoftGraphServiceTenant;
            viewModel.MicrosoftGraphServiceClientID = dbItem.MicrosoftGraphServiceClientID;
            viewModel.MicrosoftGraphServiceClientSecret = dbItem.MicrosoftGraphServiceClientSecret;
            viewModel.MicrosoftGraphServiceUserID = dbItem.MicrosoftGraphServiceUserID;
            viewModel.IsEmailMailgunEnabled = dbItem.IsEmailMailgunEnabled;
            viewModel.MailgunBaseUrl = dbItem.MailgunBaseUrl;
            viewModel.MailgunApiKey = dbItem.MailgunApiKey;
            viewModel.MailgunDomain = dbItem.MailgunDomain;
            viewModel.MailgunFrom = dbItem.MailgunFrom;
            viewModel.MailgunWebhookWebhookSigningKey = dbItem.MailgunWebhookWebhookSigningKey;
            viewModel.AwsAccessKeyID = dbItem.AwsAccessKeyID;
            viewModel.AwsSecretAccessKey = dbItem.AwsSecretAccessKey;
            viewModel.AwsS3RegionSystemName = dbItem.AwsS3RegionSystemName;
            viewModel.AwsS3BucketNamePublic = dbItem.AwsS3BucketNamePublic;
            viewModel.AzureConnectionString = dbItem.AzureConnectionString;
            viewModel.AzureBlobStorageContainerName = dbItem.AzureBlobStorageContainerName;
            viewModel.UrlTestEmailSmtp = Url.RouteUrl(ControllerActionRouteNames.Admin.SystemProperties.TestEmailSmtp);
            viewModel.UrlTestEmailMailgun = Url.RouteUrl(ControllerActionRouteNames.Admin.SystemProperties.TestEmailMailgun);
            viewModel.UrlTestEmailOffice365 = Url.RouteUrl(ControllerActionRouteNames.Admin.SystemProperties.TestEmailOffice365);
            viewModel.UrlTestAws = Url.RouteUrl(ControllerActionRouteNames.Admin.SystemProperties.TestAws);
            return viewModel;
        }

        public async Task<AjaxResponse> TestEmailSmtp(EmailSmtpTestModel submitModel)
        {
            var viewModel = new AjaxResponse();


            var attachmentBytes = default(byte[]);
            using (var ms = new MemoryStream())
            {
                using (var tw = new StreamWriter(ms))
                {
                    tw.Write("This is test attachment");
                    tw.Flush();
                    ms.Position = 0;
                    attachmentBytes = ms.ToArray();
                }
            }

            var emailClient = EmailClientFactory.GetEmailClientBySystemProperties(new SystemPropertiesDTO
            {
                IsEmailSmtpEnabled = true,
                SmtpAddress = submitModel.SMTPAddress,
                SmtpPort = submitModel.SMTPPort,
                SmtpUsername = submitModel.SMTPUsername,
                SmtpPassword = submitModel.SMTPPassword,
                SmtpUseSsl = submitModel.SMTPUseSSL,
                SmtpFrom = submitModel.SMTPFrom
            });
            var sendEmailResult = await emailClient.SendEmail(
                emailTo: submitModel.EmailTo,
                subject: "Test Smtp",
                body: "Testing email send via SMTP channel",
                attachments: new List<EmailAttachmentDTO> { new EmailAttachmentDTO { Filename = "TestAttachment.txt", FileBytes = attachmentBytes } }
            );

            viewModel.IsSuccess = sendEmailResult.IsSent;
            viewModel.Data = sendEmailResult.ResponseMessage;

            return viewModel;
        }

        public async Task<AjaxResponse> TestEmailMailgun(EmailMailgunTestModel submitModel)
        {
            var viewModel = new AjaxResponse();

            var attachmentBytes = default(byte[]);
            using (var ms = new MemoryStream())
            {
                using (var tw = new StreamWriter(ms))
                {
                    tw.Write("This is test attachment");
                    tw.Flush();
                    ms.Position = 0;
                    attachmentBytes = ms.ToArray();
                }
            }

            var emailClient = EmailClientFactory.GetEmailClientBySystemProperties(new SystemPropertiesDTO
            {
                IsEmailMailgunEnabled = true,
                MailgunBaseUrl = submitModel.MailgunBaseUrl,
                MailgunApiKey = submitModel.MailgunApiKey,
                MailgunDomain = submitModel.MailgunDomain,
                MailgunFrom = submitModel.MailgunFrom
            });
            var sendEmailResult = await emailClient.SendEmail(
                emailTo: submitModel.EmailTo,
                subject: "Test Mailgun",
                body: "Testing email send via mailgun channel",
                attachments: new List<EmailAttachmentDTO> { new EmailAttachmentDTO { Filename = "TestAttachment.txt", FileBytes = attachmentBytes } }
            );

            viewModel.IsSuccess = sendEmailResult.IsSent;
            viewModel.Data = sendEmailResult.ResponseMessage;

            return viewModel;
        }

        public async Task<AjaxResponse> TestEmailOffice365(EmailOffice365TestModel submitModel)
        {
            var viewModel = new AjaxResponse();

            var attachmentBytes = default(byte[]);
            using (var ms = new MemoryStream())
            {
                using (var tw = new StreamWriter(ms))
                {
                    tw.Write("This is test attachment");
                    tw.Flush();
                    ms.Position = 0;
                    attachmentBytes = ms.ToArray();
                }
            }

            var emailClient = EmailClientFactory.GetEmailClientBySystemProperties(new SystemPropertiesDTO
            {
                IsEmailOffice365Enabled = true,
                MicrosoftGraphServiceTenant = submitModel.MicrosoftGraphServiceTenant,
                MicrosoftGraphServiceClientID = submitModel.MicrosoftGraphServiceClientID,
                MicrosoftGraphServiceClientSecret = submitModel.MicrosoftGraphServiceClientSecret,
                MicrosoftGraphServiceUserID = submitModel.MicrosoftGraphServiceUserID
            });
            var sendEmailResult = await emailClient.SendEmail(
                emailTo: submitModel.EmailTo,
                subject: "Test Office 365",
                body: "Testing email send via office365 channel",
                attachments: new List<EmailAttachmentDTO> { new EmailAttachmentDTO { Filename = "TestAttachment.txt", FileBytes = attachmentBytes } }
            );

            viewModel.IsSuccess = sendEmailResult.IsSent;
            viewModel.Data = sendEmailResult.ResponseMessage;

            return viewModel;
        }

        public async Task<AjaxResponse> TestAws()
        {
            var viewModel = new AjaxResponse();

            var repository = RepositoriesFactory.GetSystemPropertiesRepository();
            var dbItem = await repository.SystemPropertiesGet();

            var awsServiceClient = new AwsService(
                awsAccessKeyID: dbItem.AwsAccessKeyID,
                awsSecretAccessKey: dbItem.AwsSecretAccessKey,
                awsS3RegionSystemName: dbItem.AwsS3RegionSystemName,
                awsS3BucketName: dbItem.AwsS3BucketNamePublic
            );
            viewModel.IsSuccess = await awsServiceClient.Ping();
            return viewModel;
        }

        public async Task<PageViewModel> UpdateSystemProperties(PageViewModel submitModel)
        {
            if (submitModel.GoogleMapsIFrame != null && submitModel.GoogleMapsIFrame.Contains("<iframe") && !submitModel.GoogleMapsIFrame.Contains("width=\"100%\""))
            {
                submitModel.GoogleMapsIFrame = Regex.Replace(submitModel.GoogleMapsIFrame, "width=\"\\d+\"", "width=\"100%\"").Trim();
            }

            var sp = new SystemPropertiesDTO();
            sp.ProjectName = submitModel.ProjectName;
            sp.ContactEmail = submitModel.ContactEmail ?? Constants.NullValueFor.String;
            sp.ContactPhone = submitModel.ContactPhone ?? Constants.NullValueFor.String;
            sp.ContactAddress = submitModel.ContactAddress ?? Constants.NullValueFor.String;
            sp.FacebookUrl = submitModel.FacebookUrl ?? Constants.NullValueFor.String;
            sp.TwitterUrl = submitModel.TwitterUrl ?? Constants.NullValueFor.String;
            sp.InstagramUrl = submitModel.InstagramUrl ?? Constants.NullValueFor.String;
            sp.YoutubeUrl = submitModel.YoutubeUrl ?? Constants.NullValueFor.String;
            sp.LinkedInUrl = submitModel.LinkedInUrl ?? Constants.NullValueFor.String;
            sp.GoogleMapsIFrame = submitModel.GoogleMapsIFrame ?? Constants.NullValueFor.String;
            sp.ScriptsHeader = submitModel.ScriptsHeader ?? Constants.NullValueFor.String;
            sp.ScriptsBodyStart = submitModel.ScriptsBodyStart ?? Constants.NullValueFor.String;
            sp.ScriptsBodyEnd = submitModel.ScriptsBodyEnd ?? Constants.NullValueFor.String;
            sp.IsEmailSmtpEnabled = submitModel.EmailTypesSelectedOption == nameof(submitModel.IsEmailSmtpEnabled);
            sp.SmtpAddress = submitModel.SmtpAddress ?? Constants.NullValueFor.String;
            sp.SmtpPort = submitModel.SmtpPort ?? Constants.NullValueFor.Numeric;
            sp.SmtpUsername = submitModel.SmtpUsername ?? Constants.NullValueFor.String;
            sp.SmtpPassword = submitModel.SmtpPassword ?? Constants.NullValueFor.String;
            sp.SmtpUseSsl = submitModel.SmtpUseSsl;
            sp.SmtpFrom = submitModel.SmtpFrom ?? Constants.NullValueFor.String;
            sp.IsEmailMailgunEnabled = submitModel.EmailTypesSelectedOption == nameof(submitModel.IsEmailMailgunEnabled);
            sp.MailgunBaseUrl = submitModel.MailgunBaseUrl ?? Constants.NullValueFor.String;
            sp.MailgunApiKey = submitModel.MailgunApiKey ?? Constants.NullValueFor.String;
            sp.MailgunDomain = submitModel.MailgunDomain ?? Constants.NullValueFor.String;
            sp.MailgunFrom = submitModel.MailgunFrom ?? Constants.NullValueFor.String;
            sp.MailgunWebhookWebhookSigningKey = submitModel.MailgunWebhookWebhookSigningKey ?? Constants.NullValueFor.String;
            sp.IsEmailOffice365Enabled = submitModel.EmailTypesSelectedOption == nameof(submitModel.IsEmailOffice365Enabled);
            sp.MicrosoftGraphServiceTenant = submitModel.MicrosoftGraphServiceTenant ?? Constants.NullValueFor.String;
            sp.MicrosoftGraphServiceClientID = submitModel.MicrosoftGraphServiceClientID ?? Constants.NullValueFor.String;
            sp.MicrosoftGraphServiceClientSecret = submitModel.MicrosoftGraphServiceClientSecret ?? Constants.NullValueFor.String;
            sp.MicrosoftGraphServiceUserID = submitModel.MicrosoftGraphServiceUserID ?? Constants.NullValueFor.String;
            sp.AwsAccessKeyID = submitModel.AwsAccessKeyID ?? Constants.NullValueFor.String;
            sp.AwsSecretAccessKey = submitModel.AwsSecretAccessKey ?? Constants.NullValueFor.String;
            sp.AwsS3RegionSystemName = submitModel.AwsS3RegionSystemName ?? Constants.NullValueFor.String;
            sp.AwsS3BucketNamePublic = submitModel.AwsS3BucketNamePublic ?? Constants.NullValueFor.String;
            sp.AzureConnectionString = submitModel.AzureConnectionString ?? Constants.NullValueFor.String;
            sp.AzureBlobStorageContainerName = submitModel.AzureBlobStorageContainerName ?? Constants.NullValueFor.String;
            sp.IsEmailOffice365Enabled = submitModel.EmailTypesSelectedOption == nameof(submitModel.IsEmailOffice365Enabled);
            sp.MicrosoftGraphServiceTenant = submitModel.MicrosoftGraphServiceTenant ?? Constants.NullValueFor.String;
            sp.MicrosoftGraphServiceClientID = submitModel.MicrosoftGraphServiceClientID ?? Constants.NullValueFor.String;
            sp.MicrosoftGraphServiceClientSecret = submitModel.MicrosoftGraphServiceClientSecret ?? Constants.NullValueFor.String;
            sp.MicrosoftGraphServiceUserID = submitModel.MicrosoftGraphServiceUserID ?? Constants.NullValueFor.String;
            sp.IsEmailMailgunEnabled = submitModel.EmailTypesSelectedOption == nameof(submitModel.IsEmailMailgunEnabled);
            sp.AwsAccessKeyID = submitModel.AwsAccessKeyID ?? Constants.NullValueFor.String;
            sp.AwsSecretAccessKey = submitModel.AwsSecretAccessKey ?? Constants.NullValueFor.String;
            sp.AwsS3RegionSystemName = submitModel.AwsS3RegionSystemName ?? Constants.NullValueFor.String;
            sp.AwsS3BucketNamePublic = submitModel.AwsS3BucketNamePublic ?? Constants.NullValueFor.String;
            sp.AzureConnectionString = submitModel.AzureConnectionString ?? Constants.NullValueFor.String;
            sp.AzureBlobStorageContainerName = submitModel.AzureBlobStorageContainerName ?? Constants.NullValueFor.String;

            var repository = RepositoriesFactory.GetSystemPropertiesRepository();
            await repository.SystemPropertiesUpdate(sp);

            var viewModel = submitModel;
            viewModel.IsSaved = !repository.IsError;
            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class PageViewModel : FormViewModelBase
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

            public string EmailTypesSelectedOption { get; set; }

            public string AwsAccessKeyID { get; set; }
            public string AwsSecretAccessKey { get; set; }
            public string AwsS3RegionSystemName { get; set; }
            public string AwsS3BucketNamePublic { get; set; }

            public string AzureConnectionString { get; set; }
            public string AzureBlobStorageContainerName { get; set; }

            public string UrlTestEmailSmtp { get; set; }
            public string UrlTestEmailMailgun { get; set; }
            public string UrlTestEmailOffice365 { get; set; }
            public string UrlTestAws { get; set; }

            public readonly string TextSuccess = Resources.TextSuccess;
            public readonly string TextError = Resources.TextError;
            public readonly string TextGeneralProperties = Resources.TextGeneralProperties;
            public readonly string TextEmailProperties = Resources.TextEmailProperties;
            public readonly string TextCloudProperties = Resources.TextCloudProperties;
            public readonly string TextProjectName = Resources.TextProjectName;
            public readonly string TextContactPhone = Resources.TextContactPhone;
            public readonly string TextContactEmail = Resources.TextContactEmail;
            public readonly string TextContactAddress = Resources.TextContactAddress;
            public readonly string TextTestButton = Resources.TextTestButton;
            #endregion
        }

        public class EmailSmtpTestModel
        {
            #region Properties
            public string EmailTo { get; set; }
            public string SMTPAddress { get; set; }
            public int? SMTPPort { get; set; }
            public string SMTPUsername { get; set; }
            public string SMTPPassword { get; set; }
            public bool SMTPUseSSL { get; set; }
            public string SMTPFrom { get; set; }
            #endregion
        }

        public class EmailMailgunTestModel
        {
            #region Properties
            public string EmailTo { get; set; }
            public string MailgunBaseUrl { get; set; }
            public string MailgunApiKey { get; set; }
            public string MailgunDomain { get; set; }
            public string MailgunFrom { get; set; }
            #endregion
        }

        public class EmailOffice365TestModel
        {
            #region Properties
            public string EmailTo { get; set; }
            public string MicrosoftGraphServiceTenant { get; set; }
            public string MicrosoftGraphServiceClientID { get; set; }
            public string MicrosoftGraphServiceClientSecret { get; set; }
            public string MicrosoftGraphServiceUserID { get; set; }
            #endregion
        }
        #endregion
    }
}
