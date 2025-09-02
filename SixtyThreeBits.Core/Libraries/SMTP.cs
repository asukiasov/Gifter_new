using SixtyThreeBits.Core.Libraries.EmailClients.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Libraries
{
    public class SMTP
    {
        #region Properties
        readonly string _smtpUsername;
        readonly string _smtpPassword;
        readonly string _smtpAddress;
        readonly int _smtpPort;
        readonly bool _smtpUseSSL;
        readonly string _smtpFromName;
        #endregion Properties

        #region Constructors
        public SMTP(string smtpAddress, int? smtpPort, string smtpUsername, string smtpPassword, bool? smtpUseSSL, string smtpFromName)
        {
            _smtpAddress = smtpAddress;
            _smtpPort = smtpPort ?? 587;
            _smtpUsername = smtpUsername;
            _smtpPassword = smtpPassword;
            _smtpUseSSL = smtpUseSSL == true;
            _smtpFromName = smtpFromName;
        }
        #endregion

        #region Methods                
        public async Task<SendEmailResult> SendEmail(string emailTo, string subject, string body, string replyTo = null, IEnumerable<EmailAttachmentDTO> attachments = null, IEnumerable<string> cc = null, IEnumerable<string> bcc = null)
        {
            var result = new SendEmailResult();
            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(_smtpUsername, _smtpFromName);
                    message.To.Add(emailTo);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;
                    message.BodyEncoding = Encoding.UTF8;
                    message.SubjectEncoding = Encoding.UTF8;

                    if (!string.IsNullOrWhiteSpace(replyTo))
                    {
                        message.ReplyToList.Add(replyTo);
                    }

                    if (cc != null)
                    {
                        foreach (var Item in cc)
                        {
                            message.CC.Add(Item);
                        }
                    }

                    if (bcc != null)
                    {
                        foreach (var Item in bcc)
                        {
                            message.Bcc.Add(Item);
                        }
                    }

                    if (attachments != null)
                    {
                        foreach (var Item in attachments)
                        {
                            message.Attachments.Add(new Attachment(
                                contentStream: new MemoryStream(Item.FileBytes),
                                name: Item.Filename
                            ));
                        }
                    }

                    using (var client = new SmtpClient(_smtpAddress, _smtpPort))
                    {
                        client.SendCompleted += (sender, e) =>
                        {
                            attachments?.OfType<IDisposable>().ToList().ForEach(Item =>
                            {
                                Item.Dispose();
                            });
                        };

                        client.EnableSsl = _smtpUseSSL;
                        client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                        await client.SendMailAsync(message);
                    }
                }

                result.IsSent = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;

        }
        #endregion Methods        

        #region Nested Classes
        public class SendEmailResult
        {
            #region Properties
            public bool IsSent { get; set; }
            public string ErrorMessage { get; set; }
            #endregion
        }
        #endregion
    }
}
