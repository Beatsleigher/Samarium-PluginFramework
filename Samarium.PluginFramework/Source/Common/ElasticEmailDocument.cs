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

    public class ElasticEmailDocument {

        private static Regex EmailRegex { get; } = new Regex("\\A[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a - z0 - 9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a - z0 - 9])?\\z");

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
        public DomainUser AccountInfo { get; set; }

        public IEnumerable<ElasticEmailAttachmentDocument> Attachments { get; set; }

        public IEnumerable<string> BccRecipients { get; set; }

        public IEnumerable<string> CcRecipients { get; set; }

        public DateTime Date { get; set; }

        public long DateAsTimestamp => Date.ToTimeStamp();

        public FileInfo FileInfo { get; set; }

        public string HtmlBody { get; set; }

        public string ID => Convert.ToBase64String(MessageID.GetBytes());

        public DateTime IndexTime { get; set; }

        public string InReplyTo { get; set; }

        public string InternalUserID => AccountInfo.GetUserID;

        public string MessageID { get; set; }

        public Version MimeVersion { get; set; }

        public IEnumerable<string> References { get; set; }

        public IEnumerable<string> ReplyTo { get; set; }

        public IEnumerable<string> ResentBcc { get; set; }

        public IEnumerable<string> ResentCc { get; set; }

        public IEnumerable<string> ResentFrom { get; set; }

        public IEnumerable<string> ResentReplyTo { get; set; }

        public IEnumerable<string> ResentSenderList { get; set; }

        public IEnumerable<string> Recipients { get; set; }

        public IEnumerable<string> Senders { get; set; }

        public string Subject { get; set; }

        public string RawTextBody { get; set; }
        #endregion

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
                return string.Format("{0} <Group: {1}>", string.Join("; ", grAddr.Members.Select(InternetAddressSelector)), grAddr.Name; // WOHOO! RECURSION!
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
