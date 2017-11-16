using System;

namespace Samarium.PluginFramework {

    using Newtonsoft.Json;
    using Samarium.PluginFramework.Rest;
    using ServiceStack.Text;

    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    public static class Extensions {

        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;
        
        static Regex YamlBoolRegex { get; }

        static Regex YamlYesRegex { get; }

        static Regex YamlNoRegex { get; }

        static Extensions() {
            YamlBoolRegex = new Regex(
                @"y|Y|yes|Yes|YES|n|N|no|No|NO|true|True|TRUE|false|False|FALSE|on|On|ON|off|Off|OFF",
                RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant
            );

            YamlYesRegex = new Regex(
                @"yes|Yes|YES|y|Y",
                RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant
            );

            YamlNoRegex = new Regex(
                @"no|No|NO|n|N",
                RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant
            );
        }

        const string TrueString = "true";
        const string FalseString = "false";

        /// <summary>
        /// Gets the raw bytes contained in a string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this string str) => Encoding.UTF8.GetBytes(str);

        /// <summary>
        /// Splits a string in to command-line-type arguments.
        /// </summary>
        /// <param name="commandLine">The string to split.</param>
        /// <returns>The split string.</returns>
        /// <remarks >
        /// Thanks to Stackoverflow user Daniel Earwicker!
        /// </remarks>
        public static IEnumerable<string> SplitCommandLine(this string commandLine) {
            bool inQuotes = false;

            return commandLine.Split(c => {
                if (c == '"')
                    inQuotes = !inQuotes;

                return !inQuotes && c == ' ';
            }).Select(arg => arg.Trim().TrimMatchingQuotes())
              .Where(arg => !string.IsNullOrEmpty(arg));
        }

        /// <summary>
        /// Splits the specified controller.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="controller">The controller.</param>
        /// <returns></returns>
        /// <remarks >
        /// Thanks to Stackoverflow user Daniel Earwicker!
        /// </remarks>
        public static IEnumerable<string> Split(this string str, Func<char, bool> controller) {
            int nextPiece = 0;

            for (int c = 0; c < str.Length; c++) {
                if (controller(str[c])) {
                    yield return str.Substring(nextPiece, c - nextPiece);
                    nextPiece = c + 1;
                }
            }

            yield return str.Substring(nextPiece);
        }

        /// <summary>
        /// Trims matching quotes.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="quote">The quote.</param>
        /// <returns></returns>
        /// <remarks >
        /// Thanks to Stackoverflow user Daniel Earwicker!
        /// </remarks>
        public static string TrimMatchingQuotes(this string input, char quote = '"') {
            if ((input.Length >= 2) &&
                (input[0] == quote) && (input[input.Length - 1] == quote))
                return input.Substring(1, input.Length - 2);

            return input;
        }

        /// <summary>
        /// Create excerpt string with n characters
        /// </summary>
        /// <param name="longString">The long string</param>
        /// <param name="delimiter">How many chars there should be</param>
        /// <returns>Shortened string</returns>
        public static string Excerpt(this string longString, int delimiter = 30) {
            string shortened = "";
            int i = 0;
            if (!string.IsNullOrEmpty(longString)) {
                if (longString.Length > delimiter) {
                    foreach (var c in longString) {
                        if (i < delimiter) shortened += c;
                        if (i == delimiter) shortened += "...";
                        i++;
                    }
                } else {
                    return longString;
                }
            }
            return shortened;
        }

        /// <summary>
        /// Get a range of elements from any given array
        /// </summary>
        /// <param name="array">The original array</param>
        /// <param name="from">Starting point</param>
        /// <param name="length">Length of items to get</param>
        /// <returns></returns>
        public static T[] GetRange<T>(this T[] array, int from, int length) {
            var list = new List<T>();

            for (int i = from; i < (length + from); i++) {
                list.Add(array[i]);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Gets the size (on disk) of a given directory
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static long DirectorySize(this DirectoryInfo directory) {
            long size = 0;
            // Add file sizes.
            var files = directory.GetFiles();
            foreach (var file in files) {
                size += file.Length;
            }
            // Add subdirectory sizes.
            var directories = directory.GetDirectories();
            foreach (var dir in directories) {
                size += dir.DirectorySize();
            }
            return size;
        }

        /// <summary>
        /// Counts the amount of files in a given directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static long CountFiles(this DirectoryInfo directory) {
            long _files = 0;
            // Add file sizes.
            var files = directory.GetFiles();
            _files += files.Length;
            // Add subdirectory sizes.
            var directories = directory.GetDirectories();
            foreach (var dir in directories)
                _files += dir.CountFiles();

            return _files;
        }

        /// <summary>
        /// Encrypts a plaintext string using AES256 bit encryption.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="passPhrase"></param>
        /// <returns></returns>
        public static string Encrypt(this string plainText, string passPhrase) {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations)) {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged()) {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.Zeros;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    using (var memoryStream = new MemoryStream())
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                        var cipherTextBytes = saltStringBytes;
                        cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                        cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                        memoryStream.Close();
                        cryptoStream.Close();
                        return Convert.ToBase64String(cipherTextBytes);
                    }

                }
            }
        }

        /// <summary>
        /// Decrypts an encrypted string.
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="passPhrase"></param>
        /// <returns></returns>
        public static string Decrypt(this string cipherText, string passPhrase) {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations)) {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged()) {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.Zeros;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    using (var memoryStream = new MemoryStream(cipherTextBytes))
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)) {
                        var plainTextBytes = new byte[cipherTextBytes.Length];
                        var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                        memoryStream.Close();
                        cryptoStream.Close();
                        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                    }


                }
            }
        }
        /// <summary>
        /// Encryptes a plaintext string, and writes the encrypted string as a byte stream to a file.
        /// </summary>
        /// <param name="outputFile"></param>
        /// <param name="plainText"></param>
        /// <param name="passPhrase"></param>
        public static void WriteEncryptedString(this FileInfo outputFile, string plainText, string passPhrase) {
            using (var oStream = outputFile.OpenWrite()) {
                var encryptedText = plainText.Encrypt(passPhrase);
                var buffer = Convert.FromBase64String(encryptedText);
                oStream.Write(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// Reads and decrypts data from an encrypted file.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="passPhrase"></param>
        /// <returns></returns>
        public static string ReadDecryptedString(this FileInfo inputFile, string passPhrase) {
            using (var iStream = inputFile.OpenRead())
            using (var memStream = new MemoryStream()) {
                iStream.CopyTo(memStream);
                return Convert.ToBase64String(memStream.ToArray()).Decrypt(passPhrase);
            }
        }

        /// <summary>
        /// Generates 256 bits of random data.
        /// </summary>
        /// <returns></returns>
        private static byte[] Generate256BitsOfRandomEntropy() {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider()) {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
        
        /// <summary>
        /// Parses a string from a human-readable timespan (e.g.: 10m, 30s, 5hr) to a <see cref="TimeSpan"/> object
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static TimeSpan ParseHumanReadableTimeSpan(this string dateTime) {
            TimeSpan ts = TimeSpan.Zero;
            string currentString = ""; string currentNumber = "";
            foreach (char ch in dateTime) {
                if (ch == ' ') continue;
                if (Regex.IsMatch(ch.ToString(), @"\d")) { currentNumber += ch; continue; }
                currentString += ch;
                if (Regex.IsMatch(currentString, @"^(days|day|d)", RegexOptions.IgnoreCase)) { ts = ts.Add(TimeSpan.FromDays(int.Parse(currentNumber))); currentString = ""; currentNumber = ""; continue; }
                if (Regex.IsMatch(currentString, @"^(hours|hour|h)", RegexOptions.IgnoreCase)) { ts = ts.Add(TimeSpan.FromHours(int.Parse(currentNumber))); currentString = ""; currentNumber = ""; continue; }
                if (Regex.IsMatch(currentString, @"^(mins|min|m)", RegexOptions.IgnoreCase)) { ts = ts.Add(TimeSpan.FromMinutes(int.Parse(currentNumber))); currentString = ""; currentNumber = ""; continue; }
                if (Regex.IsMatch(currentString, @"^(secs|sec|s)", RegexOptions.IgnoreCase)) { ts = ts.Add(TimeSpan.FromSeconds(int.Parse(currentNumber))); currentString = ""; currentNumber = ""; continue; }
                if (Regex.IsMatch(currentString, @"^(ms)", RegexOptions.IgnoreCase)) { ts = ts.Add(TimeSpan.FromMilliseconds(int.Parse(currentNumber))); currentString = ""; currentNumber = ""; continue; }
            }
            return ts;
        }

        /// <summary>
        /// Converts a <see cref="TimeSpan"/> to a human-readable string
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static string ToHumanReadable(this TimeSpan timeSpan) {
            var days = -1;
            var hours = -1;
            var minutes = -1;
            var seconds = -1;
            var milliseconds = -1;

            days = timeSpan.Days;
            hours = timeSpan.Hours;
            minutes = timeSpan.Minutes;
            seconds = timeSpan.Seconds;
            milliseconds = timeSpan.Milliseconds;

            return string.Format(
                "{0} {1} {2} {3} {4}",
                days > 0 ? $"{ days }days" : "", hours > 0 ? $"{ hours }h" : "",
                minutes > 0 ? $"{ minutes }min" : "", seconds > 0 ? $"{ seconds }s" : "",
                milliseconds > 0 ? $"{ milliseconds }ms" : ""
            );
        }

        /// <summary>
        /// Removes items from a given list that match a certain criteria.
        /// </summary>
        /// <typeparam name="T">The type inside of the list.</typeparam>
        /// <param name="list">The list to remove elements from</param>
        /// <param name="pred">The predicate defining the criteria to delete the items.</param>
        /// <returns>The list without the items matching the criteria in <paramref name="pred"/></returns>
        public static IEnumerable<T> Remove<T>(this IEnumerable<T> list, Predicate<T> pred) {
            var _list = list.ToList();
            foreach (var elem in list)
                if (pred(elem))
                    _list.Remove(elem);
            return _list;
        }

        /// <summary>
        /// Determines whether a given IP address is a v4 link local (private) address.
        /// </summary>
        /// <param name="ipAddress">The IP address to check.</param>
        /// <returns><code>true</code> if IP address is v4 link-local or not.</returns>
        public static bool IsIPv4LinkLocal(this IPAddress ipAddress) {

            var addrBytes = ipAddress.GetAddressBytes();

            if (addrBytes[0] == 192 && addrBytes[1] == 168) {
                // Is link-local
                return true;
            } else if (addrBytes[0] == 127) {
                // Is link-local
                return true;
            } else if (addrBytes[0] == 10) {
                // Is link-local
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a new element to an existing array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="elem"></param>
        /// <returns></returns>
        public static T[] Add<T>(this T[] array, T elem) {
            var tList = array.ToList();
            tList.Add(elem);
            return tList.ToArray();
        }

        /// <summary>
        /// Adds a range of new elements to an existing array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="elems"></param>
        /// <returns></returns>
        public static T[] AddRange<T>(this T[] array, params T[] elems) {
            var tList = array.ToList();
            tList.AddRange(elems);
            return tList.ToArray();
        }
        
        /// <summary>
        /// Sets the status of an <see cref="OutgoingWebResponseContext"/> to HTTP 400/Bad Request.
        /// </summary>
        /// <param name="context">The context to apply the changes to.</param>
        /// <param name="noContent">If set to <code>true</code>, the content-length header will be set to 0.</param>
        /// <param name="requestDescription" >Optional request description. Set to <code>null</code> for empty description.</param>
        /// <param name="suppressBody" >Determines whether the entity body will be suppressed or not. If <code >true</code>, an empty response will be sent.</param>
        public static OutgoingWebResponseContext SetStatusAsBadRequest(this OutgoingWebResponseContext context, bool noContent = true, string requestDescription = "Invalid request or bad args/format!", bool suppressBody = true) {
            context.ContentLength = noContent ? 0 : context.ContentLength;
            context.StatusCode = HttpStatusCode.BadRequest;
            context.StatusDescription = requestDescription ?? "";
            context.SuppressEntityBody = suppressBody;
            return context;
        }

        /// <summary>
        /// Sets the status of an <see cref="OutgoingWebResponseContext"/> to HTTP 302/Redirect (or however defined).
        /// </summary>
        /// <param name="context">The context this change applies to.</param>
        /// <param name="newLocation">The new location / the location to redirect to.</param>
        /// <param name="statusCode">The status code for the redirect - default: 302 - Found/Redirect</param>
        /// <returns></returns>
        public static OutgoingWebResponseContext SetStatusAsRedirect(this OutgoingWebResponseContext context, Uri newLocation, HttpStatusCode statusCode = (HttpStatusCode)302) {
            context.Location = newLocation.AbsoluteUri;
            context.StatusCode = statusCode;
            context.StatusDescription = $"{ (int)statusCode } redirect to { newLocation.AbsoluteUri }";
            return context;
        }

        /// <summary>
        /// Adds a range of elements to an existing list.
        /// </summary>
        /// <typeparam name="T">The type of element to add to the list.</typeparam>
        /// <param name="enumerable">The list.</param>
        /// <param name="objs">The new elements.</param>
        /// <returns>The updated list.</returns>
        public static List<T> AddRange<T>(this List<T> enumerable, params T[] objs) {
            enumerable.AddRange(objs);

            return enumerable;
        }

        public static string Serialize<T>(this T obj, ConfigSerializationType serializationType = ConfigSerializationType.Yaml) {
            switch (serializationType) {
                case ConfigSerializationType.Yaml:
                    return new SerializerBuilder().WithNamingConvention(new UnderscoredNamingConvention()).Build().Serialize(obj);
                case ConfigSerializationType.Json:
                    return JsonConvert.SerializeObject(obj, Formatting.Indented);
                case ConfigSerializationType.XML:
                    return XmlSerializer.SerializeToString(obj);
                default:
                    throw new ArgumentOutOfRangeException(nameof(serializationType), "Invalid serialization type!");
            }
        }

        public static string ConvertYamlBool(this string @string) {

            if (YamlYesRegex.IsMatch(@string))
                return YamlYesRegex.Replace(@string, TrueString);
            else if (YamlNoRegex.IsMatch(@string))
                return YamlNoRegex.Replace(@string, FalseString);

            throw new ArgumentException(nameof(@string), "Given string must be a YAML-serialized boolean!");
        }

        /// <summary>
        /// Gets the incoming web request.
        /// </summary>
        /// <param name="routesContainer"></param>
        /// <returns></returns>
        public static IncomingWebRequestContext GetIncomingWebRequest(this IRoutesContainer _) => WebOperationContext.Current.IncomingRequest;

        /// <summary>
        /// Gets the outgoing web response.
        /// </summary>
        /// <param name="routesContainer"></param>
        /// <returns></returns>
        public static OutgoingWebResponseContext GetOutgoingWebResponse(this IRoutesContainer _) => WebOperationContext.Current.OutgoingResponse;

        /// <summary>
        /// Gets a <see cref="Stream"/> of bytes containing HTML code
        /// </summary>
        /// <param name="routesContainer"></param>
        /// <param name="htmlString">The string to convert.</param>
        /// <returns>A <see cref="MemoryStream"/></returns>
        public static Stream GetHtmlStream(this IRoutesContainer _, string htmlString) => new MemoryStream(Encoding.UTF8.GetBytes(htmlString));

        /// <summary>
        /// Gets a <see cref="Stream"/> of bytes containing JSON text.
        /// </summary>
        /// <param name="routesContainer"></param>
        /// <param name="objs">The objects to serialise.</param>
        /// <returns>A <see cref="MemoryStream"/></returns>
        public static Stream GetJsonStream(this IRoutesContainer _, params object[] objs) {
            return new MemoryStream(JsonConvert.SerializeObject(objs).GetBytes());
        }

        /// <summary>
        /// Gets a <see cref="Stream"/> of bytes containing pretty (indented) JSON text.
        /// </summary>
        /// <param name="routesContainer"></param>
        /// <param name="objs">The objects to serialise.</param>
        /// <returns>A <see cref="MemoryStream"/></returns>
        public static Stream GetPrettyJsonStream(this IRoutesContainer _, params object[] objs) {
            return new MemoryStream(JsonConvert.SerializeObject(objs, Formatting.Indented).GetBytes());
        }

        /// <summary>
        /// Gets a <see cref="Stream"/> of bytes containing YAML text.
        /// </summary>
        /// <param name="routesContainer"></param>
        /// <param name="objs">The objects to serialise.</param>
        /// <returns>A <see cref="MemoryStream"/></returns>
        public static Stream GetYamlStream(this IRoutesContainer _, params object[] objs) {
            var yamlSerializer = new SerializerBuilder().Build();
            return new MemoryStream(yamlSerializer.Serialize(objs).GetBytes());
        }

        public static void SetAsNotFound(this OutgoingWebResponseContext context, string description = "", long contentLength = 0, string contentType = "application/x-empty") {
            context.ContentLength = contentLength;
            context.ContentType = contentType;
            context.StatusDescription = description;
            context.StatusCode = (HttpStatusCode)404;
        }

        public static void SetAsNoContent(this OutgoingWebResponseContext context, string description = "") {
            context.ContentLength = 0;
            context.StatusDescription = description;
            context.StatusCode = (HttpStatusCode)204;
        }

        public static void SetAsOk(this OutgoingWebResponseContext context, string description = "", long contentLength = 0, string contentType = "application/x-empty") {
            context.ContentLength = contentLength;
            context.ContentType = contentType;
            context.StatusDescription = description;
            context.StatusCode = (HttpStatusCode)200;
        }

        /// <summary>
        /// Shuffles (randomly organizes) a given list.
        /// </summary>
        /// <typeparam name="T">The type contained within the list</typeparam>
        /// <param name="list"></param>
        /// <remarks >
        /// Thanks to StackOverflow user grenade!
        /// </remarks>
        public static IList<T> Shuffle<T>(this IList<T> list) {
            using (var provider = new RNGCryptoServiceProvider()) {
                var n = list.Count;

                while (n > 1) {
                    var box = new byte[1];
                    do
                        provider.GetBytes(box);
                    while (!(box[0] < n * (Byte.MaxValue / n)));

                    var k = (box[0] % n);
                    n--;
                    T value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
            }
            return list;
        }

        /// <summary>
        /// Generates a new unique ID
        /// </summary>
        /// <param name="length">The length of the ID</param>
        /// <returns>A new, unique identifier.</returns>
        /// <remarks >
        /// If a length &lt; 8 is passed, the length will automatically upped to eight.
        /// Eight is the minimum number of characters.
        /// </remarks>
        public static string GenerateUniqueId(int length = 16) {

            const string abc = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789-_";

            using (var randomNumberGenerator = new RNGCryptoServiceProvider()) {
                var sBuilder = new StringBuilder();
                var buffer = new byte[1];

                for (var i = 0; i < length; i++) {
                    do
                        randomNumberGenerator.GetBytes(buffer);
                    while (buffer[0] > abc.Length);

                    sBuilder.Append(abc[buffer[0]]);
                }

                return sBuilder.ToString();

            }

        }

    }
}
