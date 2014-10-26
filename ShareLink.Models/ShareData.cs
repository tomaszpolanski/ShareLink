using System;

namespace ShareLink.Models
{
    public class ShareData
    {
        public string Title { get; set; }
        public Uri Uri { get; set; }
        public string ApplicationName { get; set; }

        public ShareData()
        {
            
        }

        public ShareData(string title, string url)
        {
            Title = title;
            Uri = new Uri(url);
        }

        public override string ToString()
        {
            return "Title: " + Title + ", Url: " + Uri + ", Application: " + ApplicationName
;
        }
    }
}