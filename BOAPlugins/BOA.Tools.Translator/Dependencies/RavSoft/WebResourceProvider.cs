// Decompiled with JetBrains decompiler
// Type: RavSoft.WebResourceProvider
// Assembly: WebResourceProvider, Version=1.0.2206.29497, Culture=neutral, PublicKeyToken=null
// MVID: 4CACA863-D821-4BB8-A0DC-2D29A3B801E6
// Assembly location: D:\Projeler\GoogleTranslator\Dependencies\WebResourceProvider.dll

using System;
using System.Net;
using System.Threading;

namespace RavSoft
{
    public abstract class WebResourceProvider
    {
        private string m_strAgent;
        private string m_strReferer;
        private string m_strError;
        private string m_strContent;
        private HttpStatusCode m_httpStatusCode;
        private int m_nPause;
        private int m_nTimeout;
        private DateTime m_tmFetchTime;

        public string Agent
        {
            get
            {
                return this.m_strAgent;
            }
            set
            {
                this.m_strAgent = value == null ? "" : value;
            }
        }

        public string Referer
        {
            get
            {
                return this.m_strReferer;
            }
            set
            {
                this.m_strReferer = value == null ? "" : value;
            }
        }

        public int Pause
        {
            get
            {
                return this.m_nPause;
            }
            set
            {
                this.m_nPause = value;
            }
        }

        public int Timeout
        {
            get
            {
                return this.m_nTimeout;
            }
            set
            {
                this.m_nTimeout = value;
            }
        }

        public string Content
        {
            get
            {
                return this.m_strContent;
            }
        }

        public DateTime FetchTime
        {
            get
            {
                return this.m_tmFetchTime;
            }
        }

        public string ErrorMsg
        {
            get
            {
                return this.m_strError;
            }
        }

        public WebResourceProvider()
        {
            this.reset();
        }

        public void reset()
        {
            this.m_strAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2377.0 Safari/537.36";
            this.m_strReferer = "";
            this.m_strError = "";
            this.m_strContent = "";
            this.m_httpStatusCode = HttpStatusCode.OK;
            this.m_nPause = 0;
            this.m_nTimeout = 0;
            this.m_tmFetchTime = DateTime.MinValue;
        }

        public void fetchResource()
        {
            if (!this.init())
                return;
            bool flag;
            do
            {
                this.getContent(this.getFetchUrl());
                flag = this.m_httpStatusCode == HttpStatusCode.OK;
                if (flag)
                    this.parseContent();
            }
            while (flag && this.continueFetching());
        }

        protected virtual bool init()
        {
            return true;
        }

        protected abstract string getFetchUrl();

        protected virtual string getPostData()
        {
            return (string)null;
        }

        protected virtual void parseContent()
        {
        }

        protected virtual bool continueFetching()
        {
            return false;
        }

        WebProxy WebProxy => new WebProxy("http://hybrid-web.global.blackspider.com:8082/proxy.pac?p=4hsmb8gt",false);
        private void getContent(string url)
        {
            if (this.m_nPause > 0)
            {
                int num = 0;
                do
                {
                    if (num == 0 && this.m_tmFetchTime != DateTime.MinValue)
                        num = (int)(this.m_tmFetchTime - DateTime.Now).TotalMilliseconds;
                    int millisecondsTimeout = 100;
                    if (num < this.m_nPause)
                    {
                        Thread.Sleep(millisecondsTimeout);
                        num += millisecondsTimeout;
                    }
                }
                while (num < this.m_nPause);
            }
            string requestUriString = url;
            if (!requestUriString.StartsWith("http://"))
                requestUriString = "http://" + requestUriString;



            using (var wc = new WebClient())
            {
                wc.Headers.Add("Accept", "application/json");
                this.m_strContent = wc.DownloadString(url);
                return;
            }
           
        }
    }
}
