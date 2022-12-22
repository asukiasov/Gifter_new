using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Services
{
    public class SMTP : SixtyThreeBitsDataObject, IEmail
    {
        #region Properties
        readonly string SMTPUsername;
        readonly string SMTPPassword;
        readonly string SMTPAddress;
        readonly int SMTPPort;
        readonly bool SMTPUseSSL;
        readonly string SMTPFromName;
        #endregion Properties

        #region Constructors
        public SMTP(string SMTPAddress, int? SMTPPort, string SMTPUsername, string SMTPPassword, bool? SMTPUseSSL, string SMTPFromName)
        {
            this.SMTPAddress = SMTPAddress;
            this.SMTPPort = SMTPPort ?? 587;
            this.SMTPUsername = SMTPUsername;
            this.SMTPPassword = SMTPPassword;
            this.SMTPUseSSL = SMTPUseSSL == true;
            this.SMTPFromName = SMTPFromName;
        }
        #endregion

        #region Methods                
        public async Task<EmailSendResult> Send(string To, string Subject, string Body, string ReplyTo = null, IEnumerable<EmailAttachment> Attachments = null, IEnumerable<string> CCs = null, IEnumerable<string> BCCs = null)
        {
            return await TryToReturnAsyncTask($"{nameof(Send)}({nameof(To)} = {To}, {nameof(Subject)} = {Subject}, {nameof(Body)} = {Body}, {nameof(ReplyTo)} = {ReplyTo})", async () =>
            {
                using (var Message = new MailMessage())
                {
                    Message.From = new MailAddress(SMTPUsername, SMTPFromName);
                    Message.To.Add(To);
                    Message.Subject = Subject;
                    Message.Body = Body;
                    Message.IsBodyHtml = true;
                    Message.BodyEncoding = Encoding.UTF8;
                    Message.SubjectEncoding = Encoding.UTF8;

                    if (!string.IsNullOrWhiteSpace(ReplyTo))
                    {
                        Message.ReplyToList.Add(ReplyTo);
                    }

                    if (CCs != null)
                    {
                        foreach (var Item in CCs)
                        {
                            Message.CC.Add(Item);
                        }
                    }

                    if (BCCs != null)
                    {
                        foreach (var Item in BCCs)
                        {
                            Message.Bcc.Add(Item);
                        }
                    }

                    if (Attachments != null)
                    {
                        foreach (var Item in Attachments)
                        {
                            Message.Attachments.Add(new Attachment(
                                contentStream: new MemoryStream(Item.FileBytes),
                                name: Item.Filename
                            ));
                        }
                    }

                    using (var Client = new SmtpClient(SMTPAddress, SMTPPort))
                    {
                        Client.EnableSsl = SMTPUseSSL;
                        Client.Credentials = new NetworkCredential(SMTPUsername, SMTPPassword);
                        await Client.SendMailAsync(Message);
                        Client.SendCompleted += (object sender, System.ComponentModel.AsyncCompletedEventArgs e) =>
                        {
                            Attachments?.OfType<IDisposable>().ToList().ForEach(Item =>
                            {
                                Item.Dispose();
                            });
                        };
                    }
                }

                return new EmailSendResult { IsSent = true };
            });
        }
        #endregion Methods        
    }
}
