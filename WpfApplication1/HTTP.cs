using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;
using System.Web;
using System.Net.Sockets;

using System.ComponentModel;

namespace WpfApplication1
{
    class HTTP
    {
        public static Window1.RequestQuery requestLobbyTrigger;

        private static string URL;// = "http://localhost:8080/";

        private static HttpListener listener;

        private static Thread _chatThread;
        public static Thread chatThread
        {
            get { return _chatThread; }
            set { _chatThread = value; }
        }

        public HTTP(Window1.RequestQuery trigger)
        {
            requestLobbyTrigger = trigger;
            chatThread = new Thread(this.startListening);
            //this.startListening();
            // Start the worker thread.
            chatThread.Start();
            Console.WriteLine("main thread: Starting worker thread...");
        }

        public HTTP(Window1.RequestQuery trigger,string newURL)
        {
            requestLobbyTrigger = trigger;
            URL = "http://" + newURL + "/" ;
            chatThread = new Thread(this.startListening);
            //this.startListening();
            // Start the worker thread.
            chatThread.Start();
            Console.WriteLine("main thread: Starting worker thread...");
        }

        private void startListening()
        {
            if (URL == null)
                URL = "http://localhost:8080/";//"http://" + getIP() + ":8080/";
            Console.WriteLine(URL);
            while (true)
            {
                HTTP.StartServer();
            }
        }

        public static void StartServer()
        {
            String[] prefixes = new String[1];
            prefixes[0] = URL;
            Console.WriteLine(prefixes[0]);
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            // URI prefixes are required, 
            // for example "http://contoso.com:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // Create a listener.
            //HttpListener listener = 
            listener = new HttpListener();
            // Add the prefixes. 
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }
            listener.Start();
            Console.WriteLine("Listening...");
            // Note: The GetContext method blocks while waiting for a request. 
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            Console.WriteLine(request.Url.ToString());
            if (request.Url.ToString() != null)
                decipherURL(request);
                //requestTrigger.Invoke(request.Url.ToString());

            //decipherURL(request.Url.ToString(), );
            //string query = request.RawUrl;
            //Console.WriteLine(request.Url.ToString());
            //Console.WriteLine(query);
            //Console.WriteLine(request.QueryString);

            // Obtain a response object.
            //HttpListenerResponse response = context.Response;
            
            // Construct a response. 
            //string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            //response.ContentLength64 = buffer.Length;
            //System.IO.Stream output = response.OutputStream;
            //output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            //output.Close();
            context.Response.Close();
            listener.Stop();
        }

        private static void decipherURL(HttpListenerRequest request)
        {
            string url = request.Url.ToString();
            string data = url.Split('?')[1];
            String[] args = data.Split('&');
            String[] line = args[0].Split('=');
            if(line[1] == "mainlobby")
            {
                requestLobbyTrigger.Invoke(request.Url.ToString());
            }
            else if (line[1] == "gamelobby")
            {
                //requestTrigger.Invoke(request.Url.ToString());
            }

        }

        public static void sendToServer(string URLparameters)
        {
            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create("http://localhost:8008");//http://domain.com/page.aspx");

            ASCIIEncoding encoding = new ASCIIEncoding();
            Console.WriteLine(URLparameters);
            byte[] data = encoding.GetBytes(URLparameters);//postData);

            httpWReq.Method = "POST";
            httpWReq.ContentType = "application/x-www-form-urlencoded";
            httpWReq.ContentLength = data.Length;
            try
            {
                using (Stream stream = httpWReq.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            catch { }

            //HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();

            //string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        public static string GETfromServer(string URLparameters)
        {
            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create("http://localhost:8008/get");//http://domain.com/page.aspx");

            ASCIIEncoding encoding = new ASCIIEncoding();
            Console.WriteLine(URLparameters);
            byte[] data = encoding.GetBytes(URLparameters);//postData);

            httpWReq.Method = "POST";
            httpWReq.ContentType = "application/x-www-form-urlencoded";
            httpWReq.ContentLength = data.Length;
            try
            {
                using (Stream stream = httpWReq.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString.Split('\r')[0];
            }
            catch { }
            return "";
        }

        public static void closeConnection()
        {
            listener.Close();
        }

        public static String buildParameters(String pre, String name, String value)
        {
            if (pre != "")
            { pre += "&"; }
            pre += name + "=" + value;
            return null;
        }
        public static string getIP()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }
    }
}