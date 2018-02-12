using System;

namespace Samarium.PluginFramework.Common {

    using MimeKit;
    using Newtonsoft.Json;
    using Samarium.PluginFramework.Plugin;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Represents an email in an indexable format.
    /// </summary>
    public class ElasticEmailDocument {

        private static Regex EmailRegex { get; } = new Regex("\\A[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\z");

        /// <summary>
        /// The slash replacement char.
        /// NEVER, I REPEAT, NEVER CHANGE THIS! IF THIS WORKS, LEAVE IT ALONE!
        /// CHANGING THIS WILL BREAK EVERYTHING! THE ENTIRE INDEX WOULD HAVE TO
        /// BE RE-CREATED!
        /// </summary>
        public const char SLASH_REPLACEMENT_CHAR = '-';
        /// <summary>
        /// The equals sign replacement char.
        /// NEVER, I REPEAT, NEVER CHANGE THIS! IF THIS WORKS, LEAVE IT ALONE!
        /// CHANGING THIS WILL BREAK EVERYTHING! THE ENTIRE INDEX WOULD HAVE TO
        /// BE RE-CREATED!
        /// </summary>
        public const char EQUALS_SIGN_REPLACEMENT_CHAR = '_';

        #region Properties
        /// <summary>
        /// User account information
        /// </summary>
        public DomainUser AccountInfo { get; set; }

        /// <summary>
        /// Attachments found in the email.
        /// </summary>
        public IEnumerable<ElasticEmailAttachmentDocument> Attachments { get; set; }

        /// <summary>
        /// BCC Recipients.
        /// </summary>
        public IEnumerable<string> BccRecipients { get; set; }

        /// <summary>
        /// CC Recipients
        /// </summary>
        public IEnumerable<string> CcRecipients { get; set; }

        /// <summary>
        /// The date the message was sent.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The date the message was sent as a timestamp
        /// </summary>
        public long DateAsTimestamp => Date.ToTimeStamp();

        /// <summary>
        /// Object representing the original file
        /// </summary>
        public FileInfo FileInfo { get; set; }

        /// <summary>
        /// The HTML body of the email
        /// </summary>
        public string HtmlBody { get; set; }

        /// <summary>
        /// The document's ID
        /// </summary>
        public string ID => Convert.ToBase64String(MessageID.GetBytes());

        /// <summary>
        /// The time the document was indexed.
        /// </summary>
        public DateTime IndexTime { get; set; }

        /// <summary>
        /// Whom the email was replying to.
        /// </summary>
        public string InReplyTo { get; set; }

        /// <summary>
        /// The user's internal ID.
        /// </summary>
        public string InternalUserID => AccountInfo.GetUserID;

        /// <summary>
        /// The message's ID
        /// </summary>
        public string MessageID { get; set; }

        /// <summary>
        /// The MIME version used to encode this message.
        /// </summary>
        public Version MimeVersion { get; set; }

        /// <summary>
        /// The message's references.
        /// </summary>
        public IEnumerable<string> References { get; set; }

        /// <summary>
        /// Reply-To
        /// </summary>
        public IEnumerable<string> ReplyTo { get; set; }

        /// <summary>
        /// Resent-Bcc
        /// </summary>
        public IEnumerable<string> ResentBcc { get; set; }

        /// <summary>
        /// Resent-Cc
        /// </summary>
        public IEnumerable<string> ResentCc { get; set; }

        /// <summary>
        /// Resent-From
        /// </summary>
        public IEnumerable<string> ResentFrom { get; set; }

        /// <summary>
        /// Resent-Reply-To
        /// </summary>
        public IEnumerable<string> ResentReplyTo { get; set; }

        /// <summary>
        /// Resent-Sender-List
        /// </summary>
        public IEnumerable<string> ResentSenderList { get; set; }

        /// <summary>
        /// The message's recipients.
        /// </summary>
        public IEnumerable<string> Recipients { get; set; }

        /// <summary>
        /// The sender of the message
        /// </summary>
        public IEnumerable<string> Senders { get; set; }

        /// <summary>
        /// The message's subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The raw (unformatted) text body of the message.
        /// </summary>
        public string RawTextBody { get; set; }
        #endregion

        /// <summary>
        /// Serialises this object
        /// </summary>
        /// <param name="serializationType">The serialisation format.</param>
        /// <param name="pretty">Pretty print.</param>
        /// <returns>The serialised data.</returns>
        public string Serialize(ConfigSerializationType serializationType = ConfigSerializationType.Json, bool pretty = false) {
            switch (serializationType) {
                case ConfigSerializationType.Json:
                    return JsonConvert.SerializeObject(this, pretty ? Formatting.Indented : Formatting.None);
                case ConfigSerializationType.Yaml:
                    return new SerializerBuilder().Build().Serialize(this);
                default:
                    throw new Exception("Only YAML and JSON serialization supported!");
            }
        }

        /// <summary>
        /// Generates an instance of this class from an email in MIME format.
        /// </summary>
        /// <param name="mimeMsg">The original MIME-formatted email</param>
        /// <param name="inFile">The input file.</param>
        /// <param name="receiveDate">The date the email was received.</param>
        /// <param name="indexTime">The date the email was indexed.</param>
        /// <param name="archivePath">The path to archive the email to.</param>
        /// <param name="usrInfo">User information.</param>
        /// <returns></returns>
        public static ElasticEmailDocument FromMimeMessage(MimeMessage mimeMsg, FileInfo inFile, DateTime receiveDate, DateTime indexTime, DirectoryInfo archivePath, DomainUser usrInfo) {
            var document = new ElasticEmailDocument {
                AccountInfo = usrInfo,
                Attachments = default, // Attachments are added later!
                BccRecipients = mimeMsg.Bcc.Select(InternetAddressSelector).Where(StringIsNotNullOrEmpty),
                CcRecipients = mimeMsg.Cc.Select(InternetAddressSelector).Where(StringIsNotNullOrEmpty),
                Date = receiveDate,
                FileInfo = MoveFile(inFile, archivePath),
                HtmlBody = mimeMsg.HtmlBody,
                IndexTime = indexTime,
                InReplyTo = mimeMsg.InReplyTo,
                MessageID = mimeMsg.MessageId,
                MimeVersion = mimeMsg.MimeVersion,
                RawTextBody = mimeMsg.TextBody,
                Recipients = mimeMsg.To.Select(InternetAddressSelector),
                References = mimeMsg.References.ToList(),
                ReplyTo = mimeMsg.ReplyTo.Select(InternetAddressSelector),
                ResentBcc = mimeMsg.ResentBcc.Select(InternetAddressSelector),
                ResentCc = mimeMsg.ResentCc.Select(InternetAddressSelector),
                ResentFrom = mimeMsg.ResentFrom.Select(InternetAddressSelector),
                ResentReplyTo = mimeMsg.ResentReplyTo.Select(InternetAddressSelector),
                ResentSenderList = mimeMsg.ResentReplyTo.Select(InternetAddressSelector),
                Senders = mimeMsg.From.Select(InternetAddressSelector).Concat(new[] { InternetAddressSelector(mimeMsg.Sender) }),
                Subject = mimeMsg.Subject
            };

            return document;
        }

        static string InternetAddressSelector(InternetAddress iAddr) {
            if (iAddr is MailboxAddress mbAddr)
                return string.Format("{0} <{1}>", mbAddr.Address, mbAddr.Name);
            else if (iAddr is GroupAddress grAddr)
                return string.Format("{0} <Group: {1}>", string.Join("; ", grAddr.Members.Select(InternetAddressSelector)), grAddr.Name); // WOHOO! RECURSION!
            return default;
        }

        static bool StringIsNotNullOrEmpty(string str) => !string.IsNullOrEmpty(str);

        static string PreIndexFileExtension { get; } = PluginRegistry.Instance.SystemConfig.GetString("pre_index_file_ext");

        static string PostIndexFileExtension { get; } = PluginRegistry.Instance.SystemConfig.GetString("post_index_file_ext");

        static FileInfo MoveFile(FileInfo file, DirectoryInfo directory) {
            if (file.Directory == directory) {
                file.MoveTo(string.Concat(file.Name.Replace(PreIndexFileExtension, string.Empty), PostIndexFileExtension));
                return file;
            }

            if (!directory.Exists)
                directory.Create();
            file.MoveTo(Path.Combine(directory.FullName, string.Concat(file.Name.Replace(PreIndexFileExtension, string.Empty), PostIndexFileExtension)));
            return file;
        }

    }

}
