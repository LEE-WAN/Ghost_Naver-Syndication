using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using HtmlAgilityPack;
using System;
using System.IO;
namespace Naver
{
    class Program
    {
        static void Main(string[] args)
        {

            Naver_Syndication q;

            FileInfo FI = new FileInfo("Naver.xml");
            if (FI.Exists == true)
            {
                XmlDocument Syndication = new XmlDocument();
                StreamReader sr = new StreamReader("Naver.xml");
                Syndication.LoadXml(sr.ReadToEnd());
                q = new Naver_Syndication(Syndication);
            }
            else
            {
                q = new Naver_Syndication("http://blog.iwanhae.ga/", "iWan", "LEEWANHAE", "puppytrain96@naver.com", "http://blog.iwanhae.ga");
            }


            XmlDocument xml = new XmlDocument();
            xml.Load("http://blog.iwanhae.ga/sitemap-posts.xml");
            XmlNodeList xnl = xml.GetElementsByTagName("url");
            foreach(XmlNode xn in xnl)
            {
                string url = xn["loc"].InnerText;
                WebClient client = new WebClient();             
                string html = Encoding.UTF8.GetString(client.DownloadData(url));
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                /////////
                string title = doc.DocumentNode.SelectSingleNode("/html/body/div/main/article/header/h1[@class='post-title']").InnerText;
                /////////
                string author = doc.DocumentNode.SelectSingleNode("/html/body/div/main/article/footer/section[@class='author']/h4").InnerText;
                /////////
                string updated = xn["lastmod"].InnerText;
                /////////
                string content = doc.DocumentNode.SelectSingleNode("/html/body/div[@class='site-wrapper']/main[@class='content']/article/section").InnerHtml;
                /////////
                string summary = doc.DocumentNode.SelectSingleNode("/html/head/meta[@property='og:description']").OuterHtml.Remove(0,41);
                ////////
                q.add_post(url, title, author, updated, "http://blog.iwanhae.ga", content, summary);
            }
            Console.WriteLine(q.get_xml());
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
        public Naver_Syndication(string id, string title, string name, string email, string url)
        {
            //http://webmastertool.naver.com/syndi/naver_syndication_document_sample.xml
            //http://cc.naver.com/cc?a=sgu.guide&r=&i=&bw=1263&px=410&py=882&sx=410&sy=619&m=1&nsc=wmt.all&u=http%3A%2F%2Fwebmastertool.naver.com%2Ftools%2Fdownfile.naver%3Ffilename%3DNaver_Syndication_User_Guide_v1.2.pdf

            string now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss") + "+09:00";

            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", "yes"));
            XmlElement root = xml.CreateElement("feed");
            XmlElement temp;
            root.SetAttribute("xmlns", "http://webmastertool.naver.com");

            root.AppendChild(CreateNode(xml, "id", id));
            root.AppendChild(CreateNode(xml, "title", title));
            root.AppendChild(CreateNode(xml, "author", 
                innerxml(xml,"name",name) + innerxml(xml,"email",email)));
            root.AppendChild(CreateNode(xml, "updated", now));

            temp = xml.CreateElement("link");
            temp.SetAttribute("rel", "site");
            temp.SetAttribute("href", url);
            temp.SetAttribute("title", title);
            root.AppendChild(temp);
            temp = null;



            xml.AppendChild(root);

        }
        public Naver_Syndication(XmlDocument input)
        {
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", "yes"));
            string now = DateTime.UtcNow.ToString("yyyy-MM-dd-THH:mm:ss") + "+09:00";
            XmlNode FN = input.DocumentElement;
            XmlElement sub = (XmlElement)FN.SelectSingleNode("updated");
            sub.InnerText = now;

            FN.ReplaceChild(FN.SelectSingleNode("updated"), sub);
            xml.AppendChild(FN);
        }


        /// <summary>
        /// entry
        /// </summary>
        /// <param name="id">게시물 주소</param>
        /// <param name="title">게시물 제목</param>
        /// <param name="author">작성자</param>
        /// <param name="updated">업데이트날자</param>
        /// <param name="root_url">블로그 주소</param>
        /// <param name="html">내용</param>
        /// <param name="summary">메타데이터</param>
        public void add_post(string id, string title, string author, string updated, string root_url, string html, string summary)
        {
            XmlNode entry = xml.CreateElement("entry");
            XmlElement temp;
            entry.AppendChild(CreateNode(xml, "id", id));
            entry.AppendChild(CreateNode(xml, "title", title));
            entry.AppendChild(CreateNode(xml, "author", innerxml(xml, "name", author)));
            entry.AppendChild(CreateNode(xml, "updated", updated));
            //
            XmlNodeList xnl = xml.GetElementsByTagName("entry");
            foreach (XmlNode xn in xnl)
            {
                if(xn["id"].InnerText == id)
                {                  
                    updated = xn["id"]["published"].InnerText;
                    xml.RemoveChild(xn);
                    break;
                }
            }
            entry.AppendChild(CreateNode(xml, "published", updated));
            //
            temp = xml.CreateElement("link");
            temp.SetAttribute("rel", "via");
            temp.SetAttribute("href", root_url);
            temp.SetAttribute("title", "Main Page");
            entry.AppendChild(temp);
            temp = null;
            //
            temp = xml.CreateElement("link");
            temp.SetAttribute("rel", "mobile");
            temp.SetAttribute("href", id);
            entry.AppendChild(temp);
            temp = null;
            //
            temp = xml.CreateElement("content");
            temp.SetAttribute("type", "html");
            temp.InnerXml = "<![CDATA["+html+ "]]>";
            entry.AppendChild(temp);
            temp = null;
            //
            temp = xml.CreateElement("summary");
            temp.SetAttribute("type", "text");
            temp.InnerXml = "<![CDATA[" + summary + "]]>";
            entry.AppendChild(temp);
            temp = null;
            //


            XmlNode FN = xml.DocumentElement;
            FN.AppendChild(entry);
            xml.AppendChild(FN);
        }


        public string get_xml()
        {
            return xml.OuterXml;
        }
        XmlNode CreateNode(XmlDocument xml, string name, string innerxml)
        {
            XmlNode output = xml.CreateElement(null, name, null);
            output.InnerXml = innerxml;
            return output;
        }
        string innerxml(XmlDocument xml, string name, string innerxml, string att_name, string att_value)
        {
            XmlElement output = xml.CreateElement(null, name, null);
            output.InnerXml = innerxml;
            output.SetAttribute(att_name, att_value);
            return output.OuterXml;
        }
        string innerxml(XmlDocument xml, string name, string innerxml)
        {
            XmlElement output = xml.CreateElement(null, name, null);
            output.InnerXml = innerxml;
            return output.OuterXml;
        }
    }
}

