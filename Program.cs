using System;
using System.Collections.Generic;
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
         * RSSUpdate.exe [feed:url] [application path] {-f}
         * -l local file
         * -f : force update, even if the application is up to date it will be updated
         * 
        */
        static void Main(string[] args)
        {
            bool isNewer = false;
            if (args.Length < 2)
            {
                try
                {
                    if (args[0] == "-?" || args[0] == "/?")
                    {
                        help();
                        return;
                    }
                }
                catch
                {
                    Console.WriteLine("Invaild parameters.");
                    help();
                    return;
                }
            }
            else if (args.Length > 2)
            {
                if (args[2] == "-f" || args[2] == "/f")
                {
                    isNewer = true;
                }
            }
            if (args[1] == "NULL")
            {
                args[1] = System.IO.Directory.GetCurrentDirectory();
            }

            string[] nw = ReadXML(args[0]);
            string[] cur = ReadXML(args[1] + "\\version.xml");
            if (nw[0] != cur[0])
            {
                Console.WriteLine("Applications are not the same.");
                return;
            }

            string[] nwtmp = nw[1].Split('.');
            string[] curtmp = cur[1].Split('.');

            //skips if the -f switch was added.
            if (!isNewer)
            {
                for (int i = 0; i < cur.Length; i++)
                {
                    //cycles through each part of the version *.*.*.* can compares them.
                    //if the nw is greater in any of them it is decleared updated.
                    if (int.Parse(nwtmp[i]) > int.Parse(curtmp[i]))
                    {
                        isNewer = true;
                    }
                }
                if (!isNewer)
                {
                    Console.WriteLine("No Updates were found.");
                    return;
                }
                else
                {
                    Console.WriteLine("Update was found.");
                }
            }
            else
            {
                Console.WriteLine("Forcing Update");
            }



            try
            {
                //Download update(which is stored in an archive.)
                //Goto applictions dir
                System.IO.Directory.SetCurrentDirectory(args[1]);
                downloadFile(nw[2]);
                try
                {
                    extractArchive(nw[2].Substring(nw[2].LastIndexOf('/') + 1));
                }
                catch
                {
                    Console.WriteLine("7za.exe not found. Downloading it now.");
                    downloadFile(@"http://dl.dropbox.com/u/40387717/7za.exe");
                    Console.WriteLine("Application restart required.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + '\n' + ex.StackTrace);
            }

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
        static void help()
        {
            Console.WriteLine(@"RSSUpdate.exe [feed] [application path] {-f} {-?}
    [feed] : RSS feed where the update infomation is stored.
    [application path] : application's path to update.
    {-f} : forces update even if already up to date.
    {-?} : help

    e.g:
        RSSUpdate.exe -l update.xml RSSUpdate -f
            - A local file to update RSSUpdate forcably
        RSSUpdate.exe http://www.example.com/update.xml RSSUpdate
            - fetches update from the web server and updates RSSUpdate if there is an update");
        }
        #region FileDownload
        static void downloadFile(string URI)
        {
            Uri uri = new Uri(URI);
            WebClient webcli = new WebClient();
            string path = System.IO.Directory.GetCurrentDirectory() + '\\' + URI.Substring(URI.LastIndexOf('/') + 1);
            webcli.DownloadFile(URI,  path);
        }

        #endregion
        /*
         *ReadXML output:
         *  [0] Application name.
         *  [1] Application Version.
         *  [2] Download Location.
        */
        static string[] ReadXML(string path)
        {
            XmlTextReader xml = new XmlTextReader(path);
            string[] outp = new string[3];
            while (xml.Read())
            {
                if (xml.Name == "app")
                {
                    outp[0] = xml.ReadElementContentAsString();
                }
                else if (xml.Name == "ver")
                {
                    outp[1] = xml.ReadElementContentAsString();
                }
                else if (xml.Name == "loc")
                {
                    outp[2] = xml.ReadElementContentAsString();
                }
            }
            return outp;
        }
        static void extractArchive(string path)
        {
            System.Diagnostics.Process.Start("7za.exe", " e 7za920.zip -aoa");
        }
    }
}
