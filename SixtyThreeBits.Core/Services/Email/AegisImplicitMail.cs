using AegisImplicitMail;
using Core.Abstractions;
using Core.Shared;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Core.Services
{
    public class AegisImplicitMailService : SixtyThreeBitsDataObject, IEmail
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
        public AegisImplicitMailService(string SMTPAddress, int? SMTPPort, string SMTPUsername, string SMTPPassword, bool? SMTPUseSSL, string SMTPFromName)
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
        public EmailSendResult Send(string To, string Subject, string Body, string ReplyTo = null, IEnumerable<EmailAttachment> Attachments = null, IEnumerable<string> CCs = null, IEnumerable<string> BCCs = null)
        {
            var AttachmentPhysicalFiles = new List<string>();

            return TryToReturn(
                Logger: $"{nameof(Send)}({nameof(To)} = {To}, {nameof(Subject)} = {Subject}, {nameof(SMTPAddress)} = {SMTPAddress}, {nameof(SMTPPort)} = {SMTPPort}, {nameof(SMTPUsername)} = {SMTPUsername}, {nameof(SMTPPassword)} = {SMTPPassword}, {nameof(SMTPUseSSL)} = {SMTPUseSSL})",
                ActionToTry: () =>
                {
                    var MailMessage = new MimeMailMessage();
                    MailMessage.From = new MimeMailAddress(SMTPUsername);
                    MailMessage.To.Add(To);
                    MailMessage.Subject = Subject;
                    MailMessage.Body = Body;
                    MailMessage.IsBodyHtml = true;
                    MailMessage.BodyEncoding = Encoding.UTF8;

                    if (CCs != null)
                    {
                        foreach (var Item in CCs)
                        {
                            MailMessage.CC.Add(Item);
                        }
                    }


                    if (BCCs != null)
                    {
                        foreach (var Item in BCCs)
                        {
                            MailMessage.Bcc.Add(Item);
                        }
                    }

                    if (Attachments != null)
                    {
                        foreach (var Item in Attachments)
                        {
                            var FilePhysicalPath = $"{AppDomain.CurrentDomain.BaseDirectory}App_Data\\{Item.Filename}";
                            AttachmentPhysicalFiles.Add(FilePhysicalPath);
                            File.WriteAllBytes(FilePhysicalPath, Item.FileBytes);
                            MailMessage.Attachments.Add(new MimeAttachment(FilePhysicalPath));
                        }
                    }

                    var Mailer = new MimeMailer(SMTPAddress, SMTPPort);
                    {
                        Mailer.User = SMTPUsername;
                        Mailer.Password = SMTPPassword;
                        Mailer.SslType = SslMode.Ssl;
                        Mailer.AuthenticationMode = AuthenticationType.Base64;
                        Mailer.Send(MailMessage);

                        Mailer.SendCompleted += (object sender, AsyncCompletedEventArgs e) =>
                        {
                            foreach (var Item in AttachmentPhysicalFiles)
                            {
                                if (File.Exists(Item))
                                {
                                    File.Delete(Item);
                                }
                            }
                        };
                    }

                    return new EmailSendResult { IsSent = true };
                },
                ActionForCatch: () =>
                {
                    foreach (var Item in AttachmentPhysicalFiles)
                    {
                        if (File.Exists(Item))
                        {
                            File.Delete(Item);
                        }
                    }
                    return new EmailSendResult { IsSent = false };
                }
            );
        }
        #endregion
    }
}
