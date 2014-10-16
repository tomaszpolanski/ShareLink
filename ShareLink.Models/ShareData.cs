using System;

namespace ShareLink.Models
{
    public class ShareData
    {
        public readonly string Title;
        public readonly Uri Uri;
        public readonly Exception Exception;

        public ShareData(string title, string url)
        {
            Title = title;
            Uri = new Uri(url);
        }

        public ShareData(string title, string url, Exception exception) :
            this(title, url)
        {
            Exception = exception;
        }
    }
}