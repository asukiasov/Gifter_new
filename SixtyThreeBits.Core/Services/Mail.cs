using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace SixtyThreeBits.Services
{
    public class Email : SixtyThreeBitsDataObject
    {
        #region Properties
        readonly string SMTPUsername;
        readonly string SMTPPassword;
        readonly string SMTPAddress;
        readonly int SMTPPort;
        readonly bool SMTPUseSSL;
        readonly string SMTPFrom;
        #endregion Properties

        #region Constructors
        public Email(string SMTPAddress, int? SMTPPort , string SMTPUsername,string SMTPPassword, bool SMTPUseSSL, string SMTPFrom)
        {
            this.SMTPAddress = SMTPAddress;
            this.SMTPPort = SMTPPort ?? 0;
            this.SMTPUsername = SMTPUsername;
            this.SMTPPassword = SMTPPassword;            
            this.SMTPUseSSL = SMTPUseSSL;
            this.SMTPFrom = SMTPFrom;
        }
        #endregion

        #region Methods                
        public bool Send(string To, string Subject, string Body, string ReplyTo = null, IEnumerable<EmailAttachment> Attachments = null, IEnumerable<string> CCs = null, IEnumerable<string> BCCs = null)
        {
            return TryToReturn($"{nameof(Send)}({nameof(To)} = {To}, {nameof(Subject)} = {Subject}, {nameof(Body)} = {Body}, {nameof(ReplyTo)} = {ReplyTo}, {nameof(Attachments)} = {Attachments?.ToJSON()})", () =>
            {
                using (var Message = new MailMessage())
                {
                    Message.From = new MailAddress(SMTPUsername, SMTPFrom);
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
                        foreach(var Item in CCs)
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
                        Client.Send(Message);
                        Client.SendCompleted += (object sender, System.ComponentModel.AsyncCompletedEventArgs e) =>
                        {                            
                            Attachments?.OfType<IDisposable>().ToList().ForEach(Item =>
                            {
                                Item.Dispose();
                            });                            
                        };
                    }
                }

                return true;
            });
        }

        public Attachment CreateCalendarEventFileAsMailAttachment(DateTime? StartTime, DateTime? EndTime, string Subject, string Description)
        {
            string Contents =
           "BEGIN:VCALENDAR\n" +
           "PRODID:-//RA Media Inc.//Streaming Tutors//EN\n" +
           "BEGIN:VEVENT\n" +
           "DTSTART:" + StartTime.Value.ToUniversalTime().ToString("yyyyMMdd\\THHmmss\\Z") + "\n" +
           "DTEND:" + EndTime.Value.ToUniversalTime().ToString("yyyyMMdd\\THHmmss\\Z") + "\n" +
           "LOCATION: Streaming Tutors\n" +
           "DESCRIPTION;ENCODING=QUOTED-PRINTABLE:" + Description + "\n" +
           "SUMMARY:" + Subject + "PRIORITY:3\n" +
           "END:VEVENT" + "END:VCALENDAR\n";

            return new Attachment(Contents.ToStream(), $"{Subject} {string.Format(Constants.Formats.DateTimeEval,Subject)}.ics",  "text/calendar");
        }
        #endregion Methods

        #region Sub Classes
        public class EmailAttachment
        {
            #region Properties
            public string Filename { get; set; }
            public byte[] FileBytes { get; set; }
            #endregion
        }
        #endregion
    }
}
