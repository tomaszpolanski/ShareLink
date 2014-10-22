using System;

namespace ShareLink.Models
{
    public class ShareData
    {
        public readonly string Title;
        public readonly Uri Uri;
        public readonly Uri Icon;
        public readonly Exception Exception;

        public ShareData(string title, string url, Uri icon)
        {
            Title = title;
            Uri = new Uri(url);
            Icon = icon;
        }

        public ShareData(string title, string url, Exception exception) :
            this(title, url, new Uri(url))
        {
            Exception = exception;
        }
    }
}