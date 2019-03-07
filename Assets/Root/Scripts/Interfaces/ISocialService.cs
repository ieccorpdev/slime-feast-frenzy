using System;

using UnityEngine;

namespace Root
{
    public interface ISocialService
    {
        void ShareImage(string message, string path);
        
        void ShareImageFacebook(string message, string path);

        void ShareImageTwitter(string message, string path);

        void ShareScreenShot(string message);

        void ShareScreenShotFacebook(string message);

        void ShareScreenShotTwitter(string message);

        void ShareSocial(string title, string content, string url);

        void ShareSocialFacebook(string title, string content, string url);

        void ShareSocialTwitter(string title, string content, string url);

    }
}

