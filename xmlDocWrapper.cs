using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace update
{
    /*xml(XmlTextReader/XmlDocument) wrapper for getting elements via xPath*/
    public class XmlDocWrapper
    {
        /*Locals*/
        private XmlDocument xmlDoc = null;

        /*init*/
        public XmlDocWrapper(string path)
        {
            XmlTextReader xmlReader = new XmlTextReader(path);
            xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlReader);
            xmlReader.Close();
        }
        public XmlDocWrapper(System.IO.Stream input)
        {
            XmlTextReader xmlReader = new XmlTextReader(input);
            xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlReader);
            xmlReader.Close();
        }

        /*Methods*/
        public XmlNode getNode(string xPath)
        {
            return xmlDoc.SelectSingleNode(xPath);
        }
        public string getNodeInnerText(string xPath)
        {
            return xmlDoc.SelectSingleNode(xPath).InnerText;
        }
        public string getNodeInnerXml(string xPath)
        {
            return xmlDoc.SelectSingleNode(xPath).InnerXml;
        }
        public XmlNodeList getNodes(string xPath)
        {
            return xmlDoc.SelectNodes(xPath);
        }
        public string getNodeAttribute(string xPath, string AttribName)
        {
            foreach (XmlAttribute i in xmlDoc.SelectSingleNode(xPath).Attributes)
            {
                if (i.Name == AttribName)
                {
                    return i.Value;
                }
            }
            throw new Exception("Attribute not found.");
        }

        /*Methods static*/
        public static XmlNode getChild(XmlNodeList children, string name)
        {
            foreach (XmlNode i in children)
            {
                if (i.Name == name)
                {
                    return i;
                }

            }
            throw new Exception("Child by the name " + name + " not found.");
        }

        /*Propreties*/
        public XmlDocument XmlDocument
        {
            get { return this.xmlDoc; }
        }

    }

    public class RssWrapper : XmlDocWrapper
    {
        public RssWrapper(string path)
            : base(path)
        {
            CheckFeedVersion();
        }
        public RssWrapper(System.IO.Stream input)
            : base(input)
        {
            CheckFeedVersion();
        }

        /*RSS*/
        private void CheckFeedVersion()
        {
            if (base.getNodeAttribute("/rss", "version") != "2.0")
            {
                throw new Exception("RSS feed is an unsupported version " + base.getNodeAttribute("/rss", "version"));
            }
        }
        public XmlNode getNewestItem()
        {
            XmlNodeList nodes = base.getNodes("/rss/channel/item");
            XmlNode newestNode = null;
            DateTime time = DateTime.MinValue;
            foreach (XmlNode i in nodes)
            {
                if (!i.HasChildNodes)
                    continue;

                if (newestNode == null)
                {
                    newestNode = i;
                    time = DateTime.Parse(getChild(newestNode.ChildNodes, "pubDate").InnerText);
                    continue;
                }

                DateTime dt = DateTime.Parse(getChild(newestNode.ChildNodes, "pubDate").InnerText);
                int t = DateTime.Compare(time, dt);
                if (t > 1)
                {
                    newestNode = i;
                    time = dt;
                }
            }
            return newestNode;
        }

    }
}
