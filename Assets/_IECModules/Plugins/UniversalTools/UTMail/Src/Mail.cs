using System.Net;
using UnityEngine;
using UT.System.Net.Mail;

namespace UT
{
    /// <summary>
    /// Provides access to all UTMail features.
    /// </summary>
    public class Mail
    {
        /// <summary>
        /// The asset version.
        /// </summary>
        public const string Version = "1.1.2";
        
        /// <summary>
        /// Displays a default system email client application. Allows users to modify a preconfigured (by you) message and send it from their own email address.
        /// </summary>
        /// <param name="message">Preconfigured UT.MailMessage (may include To, CC, Bcc, Body, Text, Attachments, ...) to be displayed in an email client app.</param>
        /// <param name="annotation">Is used only on Android as a title of an email application choosing popup.</param>
        public static void Compose(MailMessage message, string annotation)
        {
            MailImpl.Compose(message, annotation);
        }

        /// <summary>
        /// Sends the specified message with SMTP.
        /// </summary>
        /// <param name="message">UT.MailMessage to send.</param>
        /// <param name="host">SMTP server address, f.e. smtp.gmail.com.</param>
        /// <param name="senderEmail">Sender's full email address, f.e. myaddress@gmail.com.</param>
        /// <param name="password">Email account password.</param>
        /// <param name="enableSsl">Whether to use SSL/TLS to establish a secure SMTP connection.</param>
        /// <param name="resultHandler">(Optional) Delegate to be called in the main Unity thread after the sending operation is complete (successfully or not).</param>
        /// <remarks>
        /// UTMail will try to detect the SMTP port automatically based on enableSsl argument (587 for secure connection, 25 otherwise).
        /// Full email address will be used as authentication account name.
        /// </remarks>
        public static void Send(MailMessage message, string host, string senderEmail, string password, bool enableSsl, ResultHandler resultHandler = null)
        {
            Send(message, host, senderEmail, senderEmail, password, enableSsl, resultHandler);
        }

        /// <summary>
        /// Sends the specified message with SMTP.
        /// </summary>
        /// <param name="message">UT.MailMessage to send.</param>
        /// <param name="host">SMTP server address, f.e. smtp.gmail.com.</param>
        /// <param name="senderEmail">Sender's full email address, f.e. myaddress@gmail.com.</param>
        /// <param name="account">SMTP authentication account name.</param>
        /// <param name="password">Email account password.</param>
        /// <param name="enableSsl">Whether to use SSL/TLS to establish a secure SMTP connection.</param>
        /// <param name="resultHandler">(Optional) Delegate to be called in the main Unity thread after the sending operation is complete (successfully or not).</param>
        /// <remarks>
        /// UTMail will try to detect the SMTP port automatically based on enableSsl argument (587 for secure connection, 25 otherwise).
        /// </remarks>
        public static void Send(MailMessage message, string host, string senderEmail, string account, string password, bool enableSsl, ResultHandler resultHandler = null)
        {
            Send(message, host, DefaultPort(enableSsl), senderEmail, account, password, enableSsl, resultHandler);
        }

        /// <summary>
        /// Sends the specified message with SMTP.
        /// </summary>
        /// <param name="message">UT.MailMessage to send.</param>
        /// <param name="host">SMTP server address, f.e. smtp.gmail.com.</param>
        /// <param name="port">SMTP server port (usually 587 or 465 for secure connection, 25 otherwise).</param>
        /// <param name="senderEmail">Sender's full email address, f.e. myaddress@gmail.com.</param>
        /// <param name="password">Email account password.</param>
        /// <param name="enableSsl">Whether to use SSL/TLS to establish a secure SMTP connection.</param>
        /// <param name="resultHandler">(Optional) Delegate to be called in the main Unity thread after the sending operation is complete (successfully or not).</param>
        /// <remarks>
        /// Full email address will be used as authentication account name.
        /// </remarks>
        public static void Send(MailMessage message, string host, int port, string senderEmail, string password, bool enableSsl, ResultHandler resultHandler = null)
        {
            Send(message, host, port, senderEmail, senderEmail, password, enableSsl, resultHandler);
        }

        /// <summary>
        /// Sends the specified message with SMTP.
        /// </summary>
        /// <param name="message">UT.MailMessage to send.</param>
        /// <param name="host">SMTP server address, f.e. smtp.gmail.com.</param>
        /// <param name="port">SMTP server port (usually 587 or 465 for secure connection, 25 otherwise).</param>
        /// <param name="senderEmail">Sender's full email address, f.e. myaddress@gmail.com.</param>
        /// <param name="account">SMTP authentication account name.</param>
        /// <param name="password">Email account password.</param>
        /// <param name="enableSsl">Whether to use SSL/TLS to establish a secure SMTP connection.</param>
        /// <param name="resultHandler">(Optional) Delegate to be called in the main Unity thread after the sending operation is complete (successfully or not).</param>
        public static void Send(MailMessage message, string host, int port, string senderEmail, string account, string password, bool enableSsl, ResultHandler resultHandler = null)
        {
            if (port == 465)
            {
                Debug.LogWarning("Nonstandard port 465 is often used by SMTP servers to work with deprecated non-STARTTLS SMTPS clients. " +
                    "It's not supported by UTMail. Try using standardized port 587 instead. " +
                    "For more details see https://en.wikipedia.org/wiki/SMTPS");
            }

            if (resultHandler != null)
            {
                // Make sure there is MailUtil instance
                // It's important to call in advance as GameObjects can be created only in the main thread
                MailUtil.InstanceRequired();
            }

#if UNITY_EDITOR || !UNITY_WSA
            bool messageCloned = false;
            try
            {
                SmtpClient smtpClient = new SmtpClient(host, port);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new CCredentialsByHost(account, password);
                smtpClient.EnableSsl = enableSsl;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                message = new MailMessage(message);
                messageCloned = true;
                message.From = new MailAddress(senderEmail);

                smtpClient.SendCompleted += (object sender, global::System.ComponentModel.AsyncCompletedEventArgs e) =>
                {
                    if (e.Error != null)
                    {
                        Debug.LogException(e.Error);
                    }

                    if (resultHandler != null)
                    {
                        string errorMessage = e.Error != null ? e.Error.Message : null;
                        MailUtil.Instance.DispatchToMainThread(() =>
                        {
                            try
                            {
                                resultHandler(message, errorMessage == null, errorMessage);
                            }
                            finally
                            {
                                message.Dispose();
                            }
                        });
                    }
                    else
                    {
                        message.Dispose();
                    }

                    smtpClient.Dispose();
                };

                smtpClient.SendAsync(message, null);
            }
            catch (global::System.Exception e)
            {
                try
                {
                    UnityEngine.Debug.LogException(e);
                    if (resultHandler != null)
                    {
                        resultHandler(message, false, e.Message);
                    }
                }
                finally
                {
                    if (messageCloned)
                    {
                        message.Dispose();
                    }
                }
            }
#else
            MailImpl.Send(message, host, port, senderEmail, account, password, enableSsl, resultHandler);
#endif
        }

        /// <summary>
        /// Returns a default port depending on enableSsl setting.
        /// </summary>
        /// <returns>The port number.</returns>
        /// <param name="enableSsl">Whether to use SSL/TLS to establish a secure SMTP connection.</param>
        public static int DefaultPort(bool enableSsl)
        {
            return enableSsl ? 587 : 25;
        }

        public delegate void ResultHandler(MailMessage mailMessage, bool success, string errorMessage);

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoad]
        public class MailHelper : ScriptableObject
        {
            static MailHelper()
            {
                UnityEditor.EditorApplication.update += Update;
            }

            private static void Update()
            {
                UnityEditor.EditorApplication.update -= Update;

                CheckAssetVersionUpdated();
            }
        }

        private static void CheckAssetVersionUpdated()
        {
            string versionPrefName = "UTMailVersion" + Application.dataPath.GetHashCode();
            string assetVersionSaved = UnityEditor.EditorPrefs.GetString(versionPrefName);

            if (!string.IsNullOrEmpty(assetVersionSaved) && assetVersionSaved == Version)
            {
                return;
            }

            const string wouldYouLikeMessage = "Would you like to open the docs?";

            if (!string.IsNullOrEmpty(assetVersionSaved))
            {
                string updateMessage = "";
                int savedVersionIndex = -1;
                for (int i = 0; i < assetUpdateMessages.Length; ++i)
                {
                    if (assetUpdateMessages[i].version == assetVersionSaved)
                    {
                        savedVersionIndex = i;
                        break;
                    }
                }

                for (int i = savedVersionIndex + 1; i < assetUpdateMessages.Length; ++i)
                {
                    updateMessage += assetUpdateMessages[i].text + "\n";
                }

                if (updateMessage.Length > 0)
                {
                    const string lastLine = "\n" + wouldYouLikeMessage;
                    if (UnityEditor.EditorUtility.DisplayDialog("UTMail has been updated to version " + Version, updateMessage + lastLine, "Yes", "No"))
                    {
                        OpenDocs();
                    }
                }
            }
            else
            {
                if (UnityEditor.EditorUtility.DisplayDialog("UTMail " + Version + " found.", wouldYouLikeMessage, "Yes", "No"))
                {
                    OpenDocs();
                }
            }

            UnityEditor.EditorPrefs.SetString(versionPrefName, Version);
        }

        private static void OpenDocs()
        {
            Application.OpenURL("https://universal-tools.github.io/UTMail/html_1.1/index.html");
        }

        private class UpdateMessage
        {
            public UpdateMessage(string version, string text)
            {
                this.version = version;
                this.text = text;
            }

            public readonly string version;
            public readonly string text;
        }

        private static readonly UpdateMessage[] assetUpdateMessages =
        {
            new UpdateMessage("1.1",
@"- All versions of Send method now optionally accept a callback to handle the result of sending.
- Android: No need to target API Level 23 or lower anymore for composing. Any API levels >= 9 are supported."),
            new UpdateMessage("1.1.2",
@"- Better handling of latest versions of Unity and minor improvements.")
        };
#endif
    }
}