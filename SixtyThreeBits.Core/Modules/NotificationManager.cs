using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Services;
using SixtyThreeBits.Core.Utilities;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Modules
{
    public class NotificationManager
    {
        #region Properties
        DataAccessFactory DataAccessFactory;
        UtilityCollection Utilities;
        string FacebookUrl;
        string ContactPhone;
        string ContactEmail;
        string WebsiteHttpPath;
        string Culture;
        IEmail Email;
        public readonly Dictionary<NotificationPlaceHolders, string> NotificationPlaceHoldersDictionary = new Dictionary<NotificationPlaceHolders, string>
        {
            {NotificationPlaceHolders.Firstname , "[Firstname]"},
            {NotificationPlaceHolders.Lastname, "[Lastname]"},
            {NotificationPlaceHolders.Fullname , "[Fullname]"},
            {NotificationPlaceHolders.Email, "[Email]"},
            {NotificationPlaceHolders.Password, "[Password]"},
            {NotificationPlaceHolders.VerificationCode, "[VerificationCode]"},
            {NotificationPlaceHolders.CourseCaption, "[CourseCaption]"},
            {NotificationPlaceHolders.SectionCaption, "[SectionCaption]"},
            {NotificationPlaceHolders.Courses, "[Courses]"},
            {NotificationPlaceHolders.ContactPhone, "[ContactPhone]"},
            {NotificationPlaceHolders.ContactEmail, "[ContactEmail]"},
            {NotificationPlaceHolders.RedirectUrl, "[RedirectUrl]"},
            {NotificationPlaceHolders.FacebookUrl, "[FacebookUrl]"},
            {NotificationPlaceHolders.WebsiteHttpPath , "[WebsiteHttpPath]"},
            {NotificationPlaceHolders.PageUrl , "[PageUrl]"},
            {NotificationPlaceHolders.ErrorMessage, "[ErrorMessage]" },
            {NotificationPlaceHolders.OrderDetails, "[OrderDetails]" }
        };
        #endregion

        #region Contructors
        public NotificationManager(DataAccessFactory DataAccessFactory, UtilityCollection Utilities, IEmail Email, string WebsiteHttpPath, string FacebookUrl, string ContactPhone, string ContactEmail, string Culture)
        {
            this.Email = Email;
            this.WebsiteHttpPath = WebsiteHttpPath;
            this.DataAccessFactory = DataAccessFactory;
            this.Utilities = Utilities;
            this.FacebookUrl = FacebookUrl;
            this.ContactPhone = ContactPhone;
            this.ContactEmail = ContactEmail;
            this.Culture = Culture;
        }
        #endregion

        #region Methods
        public void SetCulture(string Culture)
        {
            this.Culture = Culture;
        }                                        

        public async Task<bool> SendSignUpVerificationCodeEmail(string UserFirstname, string VerificationCode, string EmailTo)
        {
            var EmailTemplate = await DataAccessFactory.EmailTemplates.GetSingleEmailTemplateByID(Enums.EmailTemplates.SignUpVerification);
            var Subject = EmailTemplate?.EmailTemplateSubject;
            var Body = await DataAccessFactory.NotificationManager.GetLayoutWrappedBody(WebsiteHttpPath, EmailTemplate?.EmailTemplateBody);
            var ValuesToReplace = new Dictionary<NotificationPlaceHolders, string>
            {
                { NotificationPlaceHolders.Firstname, UserFirstname },
                { NotificationPlaceHolders.VerificationCode, VerificationCode }
            };
            Body = ReplacePlaceHolders(Body, ValuesToReplace);
            var Result = await SendNotification(EmailTo, Subject, Body);
            return Result?.IsSent == true;
        }

        public async Task<bool> SendWelcomeEmail(string EmailTo, string UserFirstname, string UserEmail, string UserPassword)
        {
            var EmailTemplate = await DataAccessFactory.EmailTemplates.GetSingleEmailTemplateByID(Enums.EmailTemplates.SignUpWelcome);
            if (EmailTemplate != null)
            {
                var Subject = EmailTemplate.EmailTemplateSubject;
                var Body = await DataAccessFactory.NotificationManager.GetLayoutWrappedBody(WebsiteHttpPath, EmailTemplate.EmailTemplateBody);
                var ValuesToReplace = new Dictionary<NotificationPlaceHolders, string>
                {
                    { NotificationPlaceHolders.WebsiteHttpPath, WebsiteHttpPath },
                    { NotificationPlaceHolders.Firstname, UserFirstname },
                    { NotificationPlaceHolders.Email, UserEmail },
                    { NotificationPlaceHolders.Password, UserPassword },
                    { NotificationPlaceHolders.FacebookUrl, FacebookUrl },
                    { NotificationPlaceHolders.ContactPhone, ContactPhone },
                    { NotificationPlaceHolders.ContactEmail, ContactEmail }
                };

                Body = ReplacePlaceHolders(Body, ValuesToReplace);
                var Result = await SendNotification(EmailTo, Subject, Body);
                return Result.IsSent;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SendEmailChangeNotification(string UserFirstname, string VerificationCode, string EmailTo)
        {
            var EmailTemplate = await DataAccessFactory.EmailTemplates.GetSingleEmailTemplateByID(Enums.EmailTemplates.EmailChange);

            var Subject = EmailTemplate?.EmailTemplateSubject;
            var Body = await DataAccessFactory.NotificationManager.GetLayoutWrappedBody(WebsiteHttpPath, EmailTemplate?.EmailTemplateBody);
            var ValuesToReplace = new Dictionary<NotificationPlaceHolders, string>
            {
                { NotificationPlaceHolders.Firstname, UserFirstname },
                { NotificationPlaceHolders.VerificationCode, VerificationCode }
            };
            Body = ReplacePlaceHolders(Body, ValuesToReplace);
            var Result = await SendNotification(EmailTo, Subject, Body);
            return Result.IsSent;
        }

        public async Task<bool> SendPasswordResetNotification(string UserFirstname, string VerificationCode, string EmailTo)
        {
            var EmailTemplate = await DataAccessFactory.EmailTemplates.GetSingleEmailTemplateByID(Enums.EmailTemplates.PasswordReset);

            var Subject = EmailTemplate?.EmailTemplateSubject;
            var Body = await DataAccessFactory.NotificationManager.GetLayoutWrappedBody(WebsiteHttpPath, EmailTemplate?.EmailTemplateBody);
            var ValuesToReplace = new Dictionary<NotificationPlaceHolders, string>
            {
                { NotificationPlaceHolders.Firstname, UserFirstname },
                { NotificationPlaceHolders.VerificationCode, VerificationCode }
            };
            Body = ReplacePlaceHolders(Body, ValuesToReplace);
            var Result = await SendNotification(EmailTo, Subject, Body);
            return Result?.IsSent == true;
        }

        public async Task<bool> SendOrderNotificationToUser(string EmailTo, byte[] File, string Filename, string UserFirstname)
        {
            var EmailTemplate = await DataAccessFactory.EmailTemplates.GetSingleEmailTemplateByID(Enums.EmailTemplates.OrderToUser);

            var Subject = EmailTemplate?.EmailTemplateSubject;
            var Body = await DataAccessFactory.NotificationManager.GetLayoutWrappedBody(WebsiteHttpPath, EmailTemplate?.EmailTemplateBody);
            var ValuesToReplace = new Dictionary<NotificationPlaceHolders, string>
            {
                { NotificationPlaceHolders.Firstname, UserFirstname }
            };
            Body = ReplacePlaceHolders(Body, ValuesToReplace);
            var Result = await SendNotification(
                    EmailTo: EmailTo,
                    Subject: Subject,
                    Body: Body,
                    Attachments: new List<EmailAttachment>
                    {
                        new EmailAttachment
                        {
                            Filename = Filename,
                            FileBytes = File
                        }
                    }
                );
            return Result.IsSent;
        }        
        
        public async Task<EmailSendResult> SendContactUsEmailToAdmins(string EmailTo, string Subject, string Body)
        {
            Body = await DataAccessFactory.NotificationManager.GetLayoutWrappedBody(WebsiteHttpPath, Body);

            var Result = await SendNotification(
               EmailTo: EmailTo,
               Subject: Subject,
               Body: Body
           );
            return Result;
        }

        async Task<EmailSendResult> SendNotification(string EmailTo, string Subject, string Body, string ReplyTo = null, string EmailGuid = null, List<EmailAttachment> Attachments = null)
        {
            var Result = new EmailSendResult();
            var IsEmailFormatValid = Validation.IsEmailFormatValid(EmailTo);
            //var IsEmailSent = false;
            if (IsEmailFormatValid)
            {
                Result = await Email.Send(
                    To: EmailTo,
                    Subject: Subject,
                    Body: Body,
                    ReplyTo: ReplyTo,
                    Attachments: Attachments,
                    CCs: null,
                    BCCs: null
                );
                //IsEmailSent = Result?.IsSent;
                //Result.EmailLogID = await DataAccessFactory.Logs.EmailLogsIUD(
                //     DatabaseAction: Enums.DatabaseActions.CREATE,
                //     EmailTo: EmailTo,
                //     EmailSubject: Subject,
                //     EmailBody: Body,
                //     EmailIDApi: Result.EmailMessageID,
                //     EmailResultApi: Result.ResponseContent,
                //     EmailIsSent: Result.IsSent,
                //     EmailGuid: EmailGuid
                // );
            }
            return Result;
        }

        public string ReplacePlaceHolders(string Body, Dictionary<NotificationPlaceHolders, string> ValuesToReplace)
        {
            foreach (var Key in ValuesToReplace.Keys)
            {
                var Value = ValuesToReplace[Key];
                if (string.IsNullOrWhiteSpace(Value))
                {
                    Value = "";
                }

                var PlaceHolder = NotificationPlaceHoldersDictionary[Key];
                if (Key == NotificationPlaceHolders.Firstname && string.IsNullOrWhiteSpace(Value))
                {
                    Value = "Friend";
                }
                Body = Body.Replace(PlaceHolder, Value);
            }

            return Body;
        }        

        string GetOrderDetailsListHtml(List<OrderDetailItem> OrderDetails)
        {
            var Html = new StringBuilder();
            Html.Append("<h2>Order Details</h2>");
            Html.Append("<div style=\"position: relative; width: 100%; padding-left: 0; box-sizing: border-box;\">");
            Html.Append("<table style=\"width:100%;\">");
            Html.Append("<tr>");
            Html.Append("<th style=\"border: 1px solid black;\">Description</th>");
            Html.Append("<th style=\"border: 1px solid black;\">Quantity</th>");
            Html.Append("<th style=\"border: 1px solid black;\">Price</th>");
            Html.Append("<th style=\"border: 1px solid black;\">Amount</th>");
            Html.Append("</tr>");
            foreach (var Item in OrderDetails)
            {
                Html.Append("<tr>");
                Html.Append($"<td style=\"border: 1px solid black;\">{Item.OrderDetailProductCaption}</td>");
                Html.Append($"<td style=\"border: 1px solid black;\">{Item.OrderDetailProductCount}</td>");
                Html.Append($"<td style=\"border: 1px solid black;\">{Item.OrderDetailProductPriceUnit}</td>");
                Html.Append($"<td style=\"border: 1px solid black;\">{Item.OrderDetailProductPricePaid}</td>");
                Html.Append("</tr>");
            }
            Html.Append("</table>");
            Html.Append("</div>");
            return Html.ToString();
        }        
        #endregion

        #region Sub Classes
        public enum NotificationPlaceHolders
        {
            Firstname,
            Lastname,
            Fullname,
            Email,
            Password,
            VerificationCode,
            CourseCaption,
            SectionCaption,
            Courses,
            ContactPhone,
            ContactEmail,
            RedirectUrl,
            FacebookUrl,
            WebsiteHttpPath,
            PageUrl,
            ErrorMessage,
            OrderDetails
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

    public class NotificationManagerDataAccess : DataAccessBase
    {
        #region Constructors
        public NotificationManagerDataAccess(ConnectionFactory ConnectionFactory) : base(ConnectionFactory) { }
        #endregion

        #region Methods
        public IEmail GetEmailServiceBySystemProperties(SystemProperties SystemProperties)
        {
            var MailService = default(IEmail);
            if (SystemProperties.IsMailgunEnabled == true)
            {
                MailService = new Mailgun(
                    BaseUrl: SystemProperties.MailgunBaseUrl,
                    MailgunApiKey: SystemProperties.MailgunApiKey,
                    MailgunDomain: SystemProperties.MailgunDomain,
                    MailgunFrom: SystemProperties.MailgunFrom
                );
            }
            else
            {
                MailService = new SMTP(
                    SMTPAddress: SystemProperties.SMTPAddress,
                    SMTPPort: SystemProperties.SMTPPort,
                    SMTPUsername: SystemProperties.SMTPUsername,
                    SMTPPassword: SystemProperties.SMTPPassword,
                    SMTPUseSSL: SystemProperties.SMTPUseSSL,
                    SMTPFromName: SystemProperties.SMTPFrom
                );
            }
            return MailService;
        }
        public async Task<string> GetLayoutWrappedBody(string WebsiteHttpPath = null, string BodyText = null, string UrlUnsubscribe = null)
        {
            return await TryToReturnAsyncTask($"{nameof(GetLayoutWrappedBody)}({nameof(WebsiteHttpPath)} = {WebsiteHttpPath},{nameof(BodyText)} = {BodyText}, {nameof(UrlUnsubscribe)} = {UrlUnsubscribe})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.EmailsLayout(WebsiteHttpPath, BodyText, UrlUnsubscribe);
                }

            });
        }
        #endregion
    }
}
