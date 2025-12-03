using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Core.Infrastructure.Repositories;
using SixtyThreeBits.Core.Libraries.EmailClients.DTO;
using SixtyThreeBits.Core.Libraries.Validation;
using SixtyThreeBits.Core.Utilities;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Libraries
{
    public class NotificationManager
    {
        #region Properties
        readonly EmailTemplatesRepository _emailTemplatesRepository;
        readonly UtilityCollection _utilities;
        readonly string _websiteHttpPath;
        string _languageCultureCode;
        readonly IEmailClient _email;

        readonly Dictionary<NotificationPlaceHolders, string> _notificationPlaceHoldersDictionary = new Dictionary<NotificationPlaceHolders, string>
        {
            {NotificationPlaceHolders.Email, "[Email]"},
            {NotificationPlaceHolders.Firstname, "[Firstname]"},
            {NotificationPlaceHolders.Fullname, "[Fullname]"},
            {NotificationPlaceHolders.Lastname, "[Lastname]"},
            {NotificationPlaceHolders.Message, "[Message]"},
            {NotificationPlaceHolders.OrderID, "[OrderID]"},
            {NotificationPlaceHolders.OrderDetails, "[OrderDetails]"},
            {NotificationPlaceHolders.Password, "[Password]"},
            {NotificationPlaceHolders.Phone, "[Phone]"},
            {NotificationPlaceHolders.RedirectUrl, "[RedirectUrl]"},
            {NotificationPlaceHolders.Subject, "[Subject]"},
            {NotificationPlaceHolders.VerificationCode, "[VerificationCode]"}
        };
        #endregion

        #region Contructors
        public NotificationManager(EmailTemplatesRepository emailTemplatesRepository, UtilityCollection utilities, IEmailClient email, string websiteHttpPath, string languageCultureCode)
        {
            _emailTemplatesRepository = emailTemplatesRepository;
            _email = email;
            _websiteHttpPath = websiteHttpPath;
            _utilities = utilities;
            _languageCultureCode = languageCultureCode;
        }
        #endregion

        #region Methods
        public void SetCulture(string languageCultureCode)
        {
            _languageCultureCode = languageCultureCode;
        }

        public async Task<bool> SendSignUpVerificationCodeEmail(string userFirstname, string verificationCode, string emailTo)
        {
            var emailTemplate = await _emailTemplatesRepository.EmailTemplatesGetSingleByID(Enums.EmailTemplates.SignUpVerification);
            var subject = emailTemplate?.EmailTemplateSubject;
            var body = await _emailTemplatesRepository.EmailTemplatesWrapInLayout(_websiteHttpPath, _languageCultureCode, emailTemplate?.EmailTemplateBody);
            var valuesToReplace = new Dictionary<NotificationPlaceHolders, string>
            {
                { NotificationPlaceHolders.Firstname, userFirstname },
                { NotificationPlaceHolders.VerificationCode, verificationCode }
            };
            body = ReplacePlaceHolders(body, valuesToReplace);
            var Result = await SendNotification(emailTo, subject, body);
            return Result?.IsSent == true;
        }

        public async Task<bool> SendWelcomeEmail(string emailTo, string userFirstname, string userEmail, string userPassword)
        {
            var emailTemplate = await _emailTemplatesRepository.EmailTemplatesGetSingleByID(Enums.EmailTemplates.SignUpWelcome);
            if (emailTemplate != null)
            {
                var subject = emailTemplate.EmailTemplateSubject;
                var body = await _emailTemplatesRepository.EmailTemplatesWrapInLayout(_websiteHttpPath, _languageCultureCode, emailTemplate.EmailTemplateBody);
                var ValuesToReplace = new Dictionary<NotificationPlaceHolders, string>
                {
                    { NotificationPlaceHolders.Firstname, userFirstname },
                    { NotificationPlaceHolders.Email, userEmail },
                    { NotificationPlaceHolders.Password, userPassword }
                };

                body = ReplacePlaceHolders(body, ValuesToReplace);
                var result = await SendNotification(emailTo, subject, body);
                return result.IsSent;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SendEmailChangeNotification(string userFirstname, string verificationCode, string emailTo)
        {
            var emailTemplate = await _emailTemplatesRepository.EmailTemplatesGetSingleByID(Enums.EmailTemplates.EmailChange);
            var subject = emailTemplate?.EmailTemplateSubject;
            var body = await _emailTemplatesRepository.EmailTemplatesWrapInLayout(_websiteHttpPath, _languageCultureCode, emailTemplate?.EmailTemplateBody);
            var valuesToReplace = new Dictionary<NotificationPlaceHolders, string>
            {
                { NotificationPlaceHolders.Firstname, userFirstname },
                { NotificationPlaceHolders.VerificationCode, verificationCode }
            };
            body = ReplacePlaceHolders(body, valuesToReplace);
            var result = await SendNotification(emailTo, subject, body);
            return result.IsSent;
        }

        public async Task<bool> SendPasswordResetNotification(string userFirstname, string verificationCode, string emailTo)
        {
            var emailTemplate = await _emailTemplatesRepository.EmailTemplatesGetSingleByID(Enums.EmailTemplates.PasswordReset);
            var subject = emailTemplate?.EmailTemplateSubject;
            var body = await _emailTemplatesRepository.EmailTemplatesWrapInLayout(_websiteHttpPath, _languageCultureCode, emailTemplate?.EmailTemplateBody);
            var valuesToReplace = new Dictionary<NotificationPlaceHolders, string>
            {
                { NotificationPlaceHolders.Firstname, userFirstname },
                { NotificationPlaceHolders.VerificationCode, verificationCode }
            };
            body = ReplacePlaceHolders(body, valuesToReplace);
            var result = await SendNotification(emailTo, subject, body);
            return result?.IsSent == true;
        }

        public async Task<bool> SendOrderNotificationToUser(string emailTo, byte[] file, string filename, string userFirstname)
        {
            var emailTemplate = await _emailTemplatesRepository.EmailTemplatesGetSingleByID(Enums.EmailTemplates.OrderToUser);
            var subject = emailTemplate?.EmailTemplateSubject;
            var body = await _emailTemplatesRepository.EmailTemplatesWrapInLayout(_websiteHttpPath, _languageCultureCode, emailTemplate?.EmailTemplateBody);
            var valuesToReplace = new Dictionary<NotificationPlaceHolders, string>
            {
                { NotificationPlaceHolders.Firstname, userFirstname }
            };
            body = ReplacePlaceHolders(body, valuesToReplace);
            var result = await SendNotification(
                    emailTo: emailTo,
                    subject: subject,
                    body: body,
                    attachments: new List<EmailAttachmentDTO>
                    {
                        new EmailAttachmentDTO
                        {
                            Filename = filename,
                            FileBytes = file
                        }
                    }
                );
            return result.IsSent;
        }

        public async Task SendContactUsEmailToAdmins(string name, string email, string message, string emailsTo)
        {
            var emailTemplate = await _emailTemplatesRepository.EmailTemplatesGetSingleByID(Enums.EmailTemplates.ContactFormToAdmins);
            var subject = emailTemplate?.EmailTemplateSubject;
            var body = await _emailTemplatesRepository.EmailTemplatesWrapInLayout(_websiteHttpPath, _languageCultureCode, emailTemplate?.EmailTemplateBody);

            var valuesToReplace = new Dictionary<NotificationPlaceHolders, string>
            {
                { NotificationPlaceHolders.Firstname, name },
                { NotificationPlaceHolders.Email, email },
                { NotificationPlaceHolders.Message, message }
            };
            body = ReplacePlaceHolders(body, valuesToReplace);

            if (!string.IsNullOrWhiteSpace(emailsTo))
            {
                var split = emailsTo.Split(',');
                foreach (var emailTo in split)
                {
                    var isEmailFormatValid = Validation63.IsEmailFormatValid(emailTo);
                    if (isEmailFormatValid)
                    {
                        var result = await SendNotification(
                           emailTo: emailTo,
                           subject: subject,
                           body: body
                       );
                    }
                }
            }
        }

        async Task<EmailSendResultDTO> SendNotification(string emailTo, string subject, string body, string replyTo = null, string emailGuid = null, List<EmailAttachmentDTO> attachments = null)
        {
            var result = new EmailSendResultDTO();
            var isEmailFormatValid = Validation63.IsEmailFormatValid(emailTo);
            //var isEmailSent = false;
            if (isEmailFormatValid)
            {
                result = await _email.SendEmail(
                    emailTo: emailTo,
                    subject: subject,
                    body: body,
                    replyTo: replyTo,
                    attachments: attachments
                );
                //isEmailSent = result?.IsSent;
                //result.EmailLogID = await RepositoryFactory.Logs.EmailLogsIUD(
                //     databaseAction: Enums.DatabaseActions.CREATE,
                //     emailTo: emailTo,
                //     emailSubject: subject,
                //     emailBody: body,
                //     emailIDApi: result.emailMessageID,
                //     emailResultApi: result.responseContent,
                //     emailIsSent: result.isSent,
                //     emailGuid: emailGuid
                // );
            }
            return result;
        }

        public string ReplacePlaceHolders(string body, Dictionary<NotificationPlaceHolders, string> valuesToReplace)
        {
            foreach (var key in valuesToReplace.Keys)
            {
                var value = valuesToReplace[key];
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = "";
                }

                var placeHolder = _notificationPlaceHoldersDictionary[key];
                if (key == NotificationPlaceHolders.Firstname && string.IsNullOrWhiteSpace(value))
                {
                    value = "Friend";
                }
                body = body.Replace(placeHolder, value);
            }

            return body;
        }

        string GetOrderDetailsListHtml(List<OrderDetailItem> orderDetails)
        {
            var html = new StringBuilder();
            html.Append("<h2>Order Details</h2>");
            html.Append("<div style=\"position: relative; width: 100%; padding-left: 0; box-sizing: border-box;\">");
            html.Append("<table style=\"width:100%;\">");
            html.Append("<tr>");
            html.Append("<th style=\"border: 1px solid black;\">Description</th>");
            html.Append("<th style=\"border: 1px solid black;\">Price Unit</th>");
            html.Append("<th style=\"border: 1px solid black;\">Quantity</th>");
            html.Append("<th style=\"border: 1px solid black;\">Price</th>");
            html.Append("</tr>");
            foreach (var item in orderDetails)
            {
                html.Append("<tr>");
                html.Append($"<td style=\"border: 1px solid black;\">{item.OrderDetailProductCaption}</td>");
                html.Append($"<td style=\"border: 1px solid black;\">{item.OrderDetailProductPriceUnit}</td>");
                html.Append($"<td style=\"border: 1px solid black;\">{item.OrderDetailProductCount}</td>");
                html.Append($"<td style=\"border: 1px solid black;\">{item.OrderDetailProductPricePaid}</td>");
                html.Append("</tr>");
            }
            html.Append("</table>");
            html.Append("</div>");
            return html.ToString();
        }
        #endregion

        #region Nested Classes
        public enum NotificationPlaceHolders
        {
            Email,
            Firstname,
            Fullname,
            Lastname,
            Message,
            OrderID,
            OrderDetails,
            Password,
            Phone,
            RedirectUrl,
            Subject,
            VerificationCode
        }

        public class OrderDetailItem
        {
            #region Properties
            public string OrderDetailProductCaption { get; set; }
            public string OrderDetailProductCount { get; set; }
            public string OrderDetailProductPriceUnit { get; set; }
            public string OrderDetailProductPricePaid { get; set; }
            #endregion
        }
        #endregion
    }
}