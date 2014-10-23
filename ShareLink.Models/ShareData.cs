using System;

namespace ShareLink.Models
{
    public class ShareData
    {
        public string Title { get; set; }
        public Uri Uri { get; set; }
        public Exception Exception { get; set; }

        public ShareData()
        {
            
        }

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

        public override string ToString()
        {
            return "Title: " + Title + ", Url: " + Uri;
        }
    }
}