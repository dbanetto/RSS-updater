using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;

namespace rssupdate
{
    class Program
    {
        /*
         * Key:
         * [*] needed 
         * {*} optional
         * 
         * RSSUpdate.exe [feed:url] [application] -f
         * -f : force update, even if the application is up to date it will be updated
         * 
        */
        static void Main(string[] args)
        {
            //Get RSS feed
            //getRSSFeed(args[0]);
            XmlTextReader xml = new XmlTextReader("temp.xml");
            string Version = "M.m.p.b";
            while (xml.Read())
            {
                if (xml.Name == "ver")
                {
                    Version = xml.ReadElementContentAsString();
                }
            }
            Console.WriteLine(Version);
            Console.Read();
        }

        static byte[] getRSSFeed(string URI)
        {
            WebRequest req = HttpWebRequest.Create(URI);
            WebResponse res = req.GetResponse();
            byte[] b = new byte[res.ContentLength];
            res.GetResponseStream().Read(b, 0, (int)(res.ContentLength));
            res.Close();
            return b;
        }

    }
}
