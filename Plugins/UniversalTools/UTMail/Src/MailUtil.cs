using System.IO;
using System.Collections.Generic;
using UT.System.Net.Mail;
using UnityEngine;
#if !UNITY_EDITOR && UNITY_WSA
using AttachmentCollection = global::System.Collections.Generic.IList<UT.System.Net.Mail.Attachment>;
using MailAddress = Windows.ApplicationModel.Email.EmailRecipient;
using MailAddressCollection = global::System.Collections.Generic.ICollection<Windows.ApplicationModel.Email.EmailRecipient>;
#endif

namespace UT
{
    /// <summary>
    /// Various utility functions used by UTMail.
    /// </summary>
    public class MailUtil : MonoBehaviour
    {
        /// <summary>
        /// Returns the only instance of MailUtil.
        /// </summary>
        public static MailUtil Instance
        {
            get
            {
                InstanceRequired();
                return instance;
            }
        }

        /// <summary>
        /// Converts MailAddressCollection to string[].
        /// </summary>
        public static string[] MailAddressCollectionToStrings(MailAddressCollection collection)
        {
            if (collection == null || collection.Count == 0)
            {
                return null;
            }

            string[] result = new string[collection.Count];
            int index = 0;
            foreach (var it in collection)
            {
                result[index++] = it.Address;
            }

            return result;
        }

        /// <summary>
        /// Writes a stream into a temporary file.
        /// </summary>
        public static string StreamToFilePath(Stream stream, string fileName)
        {
            if (!Directory.Exists(TempAttachmentDirectory))
            {
                Directory.CreateDirectory(TempAttachmentDirectory);
            }

            string targetPath = Path.Combine(TempAttachmentDirectory, fileName);

    #if UNITY_WSA || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            targetPath = targetPath.Replace('/', '\\');
    #endif

            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }

            using (Stream file = File.Create(targetPath))
            {
                CopyStream(stream, file);
            }

            return targetPath;
        }

#if UNITY_EDITOR || !UNITY_WSA
        /// <summary>
        /// Writes an attachment into a temporary file.
        /// </summary>
        public static string AttachmentToFilePath(Attachment attachment)
        {
            return StreamToFilePath(attachment.ContentStream, attachment.Name);
        }

        /// <summary>
        /// Writes a collection of attachments into temporary files.
        /// </summary>
        public static string[] AttachmentsToFilePaths(AttachmentCollection attachments)
        {
            if (attachments == null)
            {
                return null;
            }
            
            string[] paths = new string[attachments.Count];
            int i = 0;
            foreach (var it in attachments)
            {
                paths[i++] = AttachmentToFilePath(it);
            }

            return paths;
        }
#endif

        /// <summary>
        /// Cleans the temporary attachments directory.
        /// </summary>
        public static void CleanTempAttachmentDirectory()
        {
#if (UNITY_EDITOR || !UNITY_WSA) && (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_WEBGL)
            if (Directory.Exists(TempAttachmentDirectory))
            {
                Directory.Delete(TempAttachmentDirectory, true);
            }
#endif
        }

        /// <summary>
        /// Dispatches the action to be executed in the main Unity thread (in LateUpdate stage).
        /// </summary>
        public void DispatchToMainThread(global::System.Action action)
        {
            lock (this.mainThreadActions)
            {
                this.mainThreadActions.Add(action);
            }
        }

        /// <summary>
        /// Makes sure the only instance of MailUtil exists.
        /// </summary>
        public static void InstanceRequired()
        {
            if (!instance && !destroyed)
            {
                GameObject gameObject = new GameObject("__UTMailUtil");
                instance = gameObject.AddComponent<MailUtil>();
                DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// Copies stream data to a new memory stream.
        /// </summary>
        public static MemoryStream CopyToMemoryStream(Stream stream)
        {
            MemoryStream outStream = new MemoryStream();
            CopyStream(stream, outStream);
            stream.Seek(0, SeekOrigin.Begin);
            outStream.Seek(0, SeekOrigin.Begin);

            return outStream;
        }

        static MailUtil()
        {
            // Clean the temp directory on the next access after an app restart.
            CleanTempAttachmentDirectory();
        }

        private void OnDestroy()
        {
            instance = null;
            destroyed = true;
        }

        private void LateUpdate()
        {
            lock (this.mainThreadActions)
            {
                foreach (var action in this.mainThreadActions)
                {
                    try
                    {
                        action();
                    }
                    catch (global::System.Exception e)
                    {
                        UnityEngine.Debug.LogException(e);
                    }
                }

                this.mainThreadActions.Clear();
            }
        }
        
        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        private static string GetTempAttachmentDirectory()
        {
    #if !UNITY_EDITOR && UNITY_ANDROID
            using (AndroidJavaClass manager = new AndroidJavaClass("universal.tools.mail.Manager"))
            {
                string applicationIdentifier =
        #if UNITY_5_6_OR_NEWER
                    Application.identifier;
        #else
                    Application.bundleIdentifier;
        #endif
                return Path.Combine(manager.CallStatic<string>("getTemporaryDirectory"), "UTMailAttachments." + applicationIdentifier);
            }
    #else
            return Path.Combine(Application.temporaryCachePath, "UTMailAttachments");
    #endif
        }

        private static MailUtil instance;
        private static bool destroyed = false;

        private static readonly string TempAttachmentDirectory = GetTempAttachmentDirectory();

        private readonly List<global::System.Action> mainThreadActions = new List<global::System.Action>();
    }
}