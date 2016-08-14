using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBC_RRS_Feed
{
    class Article
    {
        public string title { get; set; }
        public string description { get; set; }
        public string link { get; set; }
        public string pubDate { get; set; }

        public Article(string title, string description, string link, string pubDate)
        {
            this.title = title;
            this.description = description;
            this.link = link;
            this.pubDate = pubDate;
        }
    }
}
