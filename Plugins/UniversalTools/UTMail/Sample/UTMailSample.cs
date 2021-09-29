using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// Namespace containing UTMail Sample scripts.
/// </summary>
namespace UT.MailSample
{
    /// <summary>
    /// Sample script (MonoBehaviour) demonstrating various UTMail features.
    /// </summary>
    public class UTMailSample : MonoBehaviour
    {
        [Header("SMTP Settings for Sample Sending")]
        [Tooltip("Sender's full email address, f.e. myaddress@gmail.com")]
        public string EmailAddress = "";
        [Tooltip("(Optional) Email account. Usually the same as EmailAddress (in that case can be left empty)")]
        public string EmailAccount = "";
        [Tooltip("Email account password")]
        public string Password = "";
        [Tooltip("SMTP server address, f.e. smtp.gmail.com")]
        public string SmtpServer = "";
        [Tooltip("(Optional) SMTP server port (usually 587 or 465 for a secure connection and 25 for a plain connection). If not specified, UTMail will try to detect it automatically")]
        public int SmtpPort = 0;
        [Tooltip("Whether to use SSL/TLS to establish a secure SMTP connection")]
        public bool EnableSSL = true;

        [Header("UI Controls")]
        public InputField To;
        public InputField Cc;
        public InputField Bcc;
        public InputField Attachment;
        public InputField Subject;
        public InputField Body;
        public Toggle HTMLBody;
        public Button ComposeButton;
        public Button SendButton;

        /// <summary>
        /// Demonstrates the email composing feature.
        /// </summary>
        public void Compose()
        {
            using (var message = PrepareMailMessage())
            {
                UT.Mail.Compose(message, annotation: "Choose an app to send email");
            }
        }

        /// <summary>
        /// Demonstrates the email sending feature.
        /// </summary>
        public void Send()
        {
            if (!IsSmtpConfigured())
            {
                // Please configure SMTP settings to send emails directly without composing.
                Debug.LogError(CantSendText);
                return;
            }
            
            using (var message = PrepareMailMessage())
            {
                Debug.Log("Sending \"" + message.Subject + "\"...");

                // Handles the result of sending
                UT.Mail.ResultHandler onSent = (mailMessage, success, errorMessage) =>
                {
                    if (success)
                    {
                        Debug.Log("Successfully sent \"" + mailMessage.Subject + "\"");
                    }
                    else
                    {
                        Debug.LogError("Failed to send \"" + mailMessage.Subject + "\": " + errorMessage);
                    }
                };

                if (SmtpPort > 0)
                {
                    if (!string.IsNullOrEmpty(EmailAccount))
                    {
                        // All settings are specified
                        UT.Mail.Send(message, SmtpServer, SmtpPort, EmailAddress, EmailAccount, Password, EnableSSL, onSent);
                    }
                    else
                    {
                        // EmailAccount is omitted.
                        UT.Mail.Send(message, SmtpServer, SmtpPort, EmailAddress, Password, EnableSSL, onSent);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(EmailAccount))
                    {
                        // SmtpPort is omitted.
                        UT.Mail.Send(message, SmtpServer, EmailAddress, EmailAccount, Password, EnableSSL, onSent);
                    }
                    else
                    {
                        // SmtpPort and EmailAccount are omitted.
                        UT.Mail.Send(message, SmtpServer, EmailAddress, Password, EnableSSL, onSent);
                    }
                }
            }
        }

        /// <summary>
        /// Builds UT.MailMessage which is then can be used for composing or sending.
        /// </summary>
        /// <returns>The ready to use UT.MailMessage.</returns>
        public UT.MailMessage PrepareMailMessage()
        {
            Save();

            var mailMessage = new UT.MailMessage()
                .SetSubject(Subject.text)
                .SetBody(Body.text)
                .SetBodyHtml(HTMLBody.isOn);

            foreach (string email in ToEmailsList(To.text))
            {
                mailMessage.AddTo(email);
            }

            foreach (string email in ToEmailsList(Cc.text))
            {
                mailMessage.AddCC(email);
            }

            foreach (string email in ToEmailsList(Bcc.text))
            {
                mailMessage.AddBcc(email);
            }

            if (!string.IsNullOrEmpty(Attachment.text))
            {
                mailMessage.AddAttachment(Attachment.text, "Attachment.txt");
            }

            return mailMessage;
        }

        /// <summary>
        /// Initializes the sample UI.
        /// </summary>
        private void Start()
        {
            Debug.Assert(To != null, "Please specify \"To\"!");
            Debug.Assert(Cc != null, "Please specify \"Cc\"!");
            Debug.Assert(Bcc != null, "Please specify \"Bcc\"!");
            Debug.Assert(Attachment != null, "Please specify \"Attachment\"!");
            Debug.Assert(Subject != null, "Please specify \"Subject\"!");
            Debug.Assert(Body != null, "Please specify \"Body\"!");
            Debug.Assert(HTMLBody != null, "Please specify \"HTMLBody\"!");
            Debug.Assert(ComposeButton != null, "Please specify \"ComposeButton\"!");
            Debug.Assert(SendButton != null, "Please specify \"SendButton\"!");

            Load();

            var moreButton = MoreButton.FindInstance();
            if (moreButton != null)
            {
                moreButton.MenuItems = new MoreButton.PopupMenuItem[] {new MoreButton.PopupMenuItem("EXIT", () => Application.Quit())};
            }

            ComposeButton.onClick.AddListener(Compose);
            SendButton.onClick.AddListener(Send);

            if (!IsSmtpConfigured())
            {
                SendButton.GetComponentInChildren<Text>().text = CantSendText;
            }

            To.onValueChanged.AddListener((text) => UpdateButtons());
            Cc.onValueChanged.AddListener((text) => UpdateButtons());
            Bcc.onValueChanged.AddListener((text) => UpdateButtons());
            Body.onValueChanged.AddListener((text) => UpdateButtons());
            UpdateButtons();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        private void OnApplicationQuit()
        {
            Save();
        }

    #if UNITY_EDITOR
        private void Reset()
        {
            this.To = null;
            this.Cc = null;
            this.Bcc = null;
            this.Attachment = null;
            this.Subject = null;
            this.Body = null;
            this.HTMLBody = null;
            this.ComposeButton = null;
            this.SendButton = null;
            
            OnValidate();
        }

        private void OnValidate()
        {
            FindObjectIfNotSet<InputField>("To", ref this.To);
            FindObjectIfNotSet<InputField>("Cc", ref this.Cc);
            FindObjectIfNotSet<InputField>("Bcc", ref this.Bcc);
            FindObjectIfNotSet<InputField>("Attachment", ref this.Attachment);
            FindObjectIfNotSet<InputField>("Subject", ref this.Subject);
            FindObjectIfNotSet<InputField>("Body", ref this.Body);
            FindObjectIfNotSet<Toggle>("HtmlBody", ref this.HTMLBody);
            FindObjectIfNotSet<Button>("Compose", ref this.ComposeButton);
            FindObjectIfNotSet<Button>("Send", ref this.SendButton);
        }

        private void FindObjectIfNotSet<T>(string name, ref T control)
        {
            if (control == null)
            {
                GameObject go = GameObject.Find(name);
                if (go != null)
                {
                    control = go.GetComponentInChildren<T>();

                    if (control != null)
                    {
                        UnityEditor.EditorUtility.SetDirty(this);
                    }
                }
            }
        }
    #endif

        private bool IsSmtpConfigured()
        {
            return !string.IsNullOrEmpty(SmtpServer) && !string.IsNullOrEmpty(EmailAddress) && !string.IsNullOrEmpty(Password);
        }

        private void UpdateButtons()
        {
            ComposeButton.interactable = true;
            ComposeButton.interactable &= CheckValidEmailList(To);
            ComposeButton.interactable &= CheckValidEmailList(Cc);
            ComposeButton.interactable &= CheckValidEmailList(Bcc);

            SendButton.interactable = ComposeButton.interactable && IsSmtpConfigured() && (!string.IsNullOrEmpty(To.text) || !string.IsNullOrEmpty(Cc.text) || !string.IsNullOrEmpty(Bcc.text)) && !string.IsNullOrEmpty(Body.text);
        }

        private static bool CheckValidEmailList(InputField field)
        {
            var incorrect = field.transform.Find("Incorrect");

            if (string.IsNullOrEmpty(field.text))
            {
                incorrect.gameObject.SetActive(false);
                return true;
            }
            else
            {
                bool correct = EmailRegex.IsMatch(field.text);
                incorrect.gameObject.SetActive(!correct);
                return correct;
            }
        }

        private void SaveInputField(string fieldName, InputField field)
        {
            PlayerPrefs.SetString("UTMailSample." + fieldName, field.text);
        }

        private void LoadInputField(string fieldName, InputField field, string defaultValue)
        {
            field.text = PlayerPrefs.GetString("UTMailSample." + fieldName, defaultValue);
        }

        private void SaveToggle(string fieldName, Toggle toggle)
        {
            PlayerPrefs.SetInt("UTMailSample." + fieldName, toggle.isOn ? 1 : 0);
        }

        private void LoadToggle(string fieldName, Toggle toggle)
        {
            toggle.isOn = PlayerPrefs.GetInt("UTMailSample." + fieldName, toggle.isOn ? 1 : 0) != 0;
        }

        private void Save()
        {
            SaveInputField("To", To);
            SaveInputField("Cc", Cc);
            SaveInputField("Bcc", Bcc);
            SaveInputField("Attachment", Attachment);
            SaveInputField("Subject", Subject);
            SaveInputField("Body", Body);
            SaveToggle("HTMLBody", HTMLBody);

            PlayerPrefs.Save();
        }

        private void Load()
        {
            LoadInputField("To", To, DefaultTo);
            LoadInputField("Cc", Cc, DefaultCC);
            LoadInputField("Bcc", Bcc, DefaultBcc);
            LoadInputField("Attachment", Attachment, DefaultAttachment);
            LoadInputField("Subject", Subject, DefaultSubject);
            LoadInputField("Body", Body, DefaultBody);
            LoadToggle("HTMLBody", HTMLBody);
        }

        private static string[] ToEmailsList(string list)
        {
            if (string.IsNullOrEmpty(list))
            {
                return new string[0];
            }
            else
            {
                string[] result = list.Split(',');
                for (int i = 0; i < result.Length; ++i)
                {
                    result[i] = result[i].Trim();
                }

                return result;
            }
        }

        private const string DefaultTo = "utmail-test@universal-tools.com";
        private const string DefaultCC = "";
        private const string DefaultBcc = "";
        private const string DefaultAttachment = "";
        private const string DefaultSubject = "Hi from UTMail!";
        private const string DefaultBody = "This is a test message. Seems to work well, as you're reading it!";
        private static readonly Regex EmailRegex = new Regex(@"^(([\w\.\-]+)@([\w\-]+)((\.\w+)+)\s*,*\s*)*$");
        private const string CantSendText = "Configure GameObject \"UTMail Sample\" to send!";
    }
}