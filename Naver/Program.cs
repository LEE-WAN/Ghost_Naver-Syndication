using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using HtmlAgilityPack;
using System;

namespace Naver
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load("http://blog.iwanhae.ga/sitemap-posts.xml");
            XmlNodeList xnl = xml.GetElementsByTagName("url");

            foreach(XmlNode xn in xnl)
            {
                string url = xn["loc"].InnerText;
                WebClient client = new WebClient();             
                string html = Encoding.UTF8.GetString(client.DownloadData(url));

                string pattern = "<section.*?>(.*?)<\\/section>";
                var a=  Regex.Matches(html, pattern);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                HtmlNode parent = doc.DocumentNode.SelectSingleNode("/html/body/div[@class='site-wrapper']/main[@class='content']/article/section");
                Console.WriteLine(parent.InnerText);
                Console.WriteLine("#####################################");
            }
            Console.ReadLine();
        }
    }
}
