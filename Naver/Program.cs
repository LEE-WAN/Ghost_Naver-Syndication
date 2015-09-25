using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using HtmlAgilityPack;
using System;
using System.Xml.Linq;

namespace Naver
{
    class Program
    {
        static void Main(string[] args)
        {
            Naver_Syndication q = new Naver_Syndication();
            q.init(null, null, null, null, null);
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
    class Naver_Syndication
    {
        XmlDocument xml = new XmlDocument();
        /// <summary>
        /// 네이버 신디케이션 첫 초기화
        /// </summary>
        /// <param name="id">피드문서를 구분하는 피드의URL, 특정ID로 구분하기 어렵다면, 네이버 신디케이션 문서를 얻을수있는 url 사용을 권장한다.</param>
        /// <param name="title">피드 문서를 나타내는 제목</param>
        /// <param name="name">필수, 사이트이름</param>
        /// <param name="email">선택, 관리자 이메일, 넣기싫으면 null값 입력</param>
        /// <param name="url">사이트 주소</param>
        public void init(string id, string title, string name, string email, string url)
        {
            //http://webmastertool.naver.com/syndi/naver_syndication_document_sample.xml
            //http://cc.naver.com/cc?a=sgu.guide&r=&i=&bw=1263&px=410&py=882&sx=410&sy=619&m=1&nsc=wmt.all&u=http%3A%2F%2Fwebmastertool.naver.com%2Ftools%2Fdownfile.naver%3Ffilename%3DNaver_Syndication_User_Guide_v1.2.pdf
            XElement node;
            node =
                new XElement("feed",
                new XElement("id", id),
                new XElement("title", title),
                new XElement("author", new XElement("name", name), new XElement("email", email)));

        }

        public void add_post()
        {

        }
    }
}

