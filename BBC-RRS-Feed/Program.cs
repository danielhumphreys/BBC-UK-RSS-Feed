using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace BBC_RRS_Feed
{
    class Program
    {
        static void Main(string[] args)
        {

            XmlDocument rssDoc = new XmlDocument();
            rssDoc.Load("http://feeds.bbci.co.uk/news/uk/rss.xml");

            //getting xml feed details
            XmlNode feedNode = rssDoc.SelectSingleNode("rss/channel");
            XmlNode titleNode = feedNode.SelectSingleNode("title");
            XmlNode linkNode = feedNode.SelectSingleNode("link");
            XmlNode descNode = feedNode.SelectSingleNode("description");

            //getting app directory for file storage
            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //directory for storage of used articles
            var articleListsDirectory = appDirectory + @"\articlelists\";
            //datetime to timestamp files
            DateTime today = DateTime.Now;
            //file name for articles used on this day
            var todaysArticlesFileName = String.Format("{0}-{1}-{2}.txt", today.Year, today.Month, today.Day);

            //creating json object which will be converted to json file
            var jsonFile = new JsonFile
            {
                title = titleNode.InnerText,
                link = linkNode.InnerText,
                description = descNode.InnerText
            };

            //creating a list of articles which will then be converted to an array and added to json object
            var articles = new List<Article>();
            
            //getting xml articles
            XmlNodeList articleNodes = rssDoc.SelectNodes("rss/channel/item");
                     
            foreach (XmlNode article in articleNodes)
            {

                //getting article details
                XmlNode rssSubNode = article.SelectSingleNode("title");
                string title = rssSubNode != null ? rssSubNode.InnerText : "";
                rssSubNode = article.SelectSingleNode("description");
                string description = rssSubNode != null ? rssSubNode.InnerText : "";
                rssSubNode = article.SelectSingleNode("link");
                string link = rssSubNode != null ? rssSubNode.InnerText : "";
                rssSubNode = article.SelectSingleNode("pubDate");
                string pubDate = rssSubNode != null ? rssSubNode.InnerText : "";


                var articlesSavedToday = articleListsDirectory + todaysArticlesFileName;

                if (File.Exists(articlesSavedToday))
                {
                    //check file with todays saved articles for this article
                    if (!File.ReadAllText(articlesSavedToday).Contains(link))
                    {
                        Console.WriteLine("Recording " + title);
                        //recording article as saved today
                        recordArticleAsSaved(articlesSavedToday, link);
                        //adding to list of articles
                        articles.Add(new Article(title, description, link, pubDate));
                    }

                } else
                {
                    //creating file for storing articles saved today
                    Directory.CreateDirectory(articleListsDirectory);
                    File.Create(articlesSavedToday);
                    recordArticleAsSaved(articlesSavedToday, link);
                    articles.Add(new Article(title, description, link, pubDate));
                }

            }

            //setting json object's items to be the list of articles recorded
            jsonFile.items = articles.ToArray();
            //creating string with json
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = serializer.Serialize(jsonFile);
         
            //creating file name and getting directory to store
            string filename = String.Format("{0}-{1}-{2}-{3}.json", today.Year, today.Month, today.Day, today.Hour);
            var feedDirectory = appDirectory + @"\feed\";

            //creating .json file
            Directory.CreateDirectory(feedDirectory);
            File.WriteAllText(feedDirectory + filename, json);
        }

        static void recordArticleAsSaved(string filepath, string id)
        {
            using (StreamWriter w = File.AppendText(filepath))
            {
                w.WriteLine(id + "\n");
            }
        }
    }
}
