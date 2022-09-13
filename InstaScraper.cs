using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Instagram_Bot
{

    internal class InstaScraper
    {
        private string Username;
        private string Password;
        private string UserId;

        public string csrfToken;

        public CookieContainer cookiesSession;   

        //Sending GET request to instagram homepage for collecting cookies values
        private void FirstInitialize()
        {
            string baseUrl = "https://www.instagram.com";
            HttpWebResponse response = HttpRequests.Get(baseUrl,null, cookiesSession);
            

            var cookies = cookiesSession.GetCookies(new Uri("https://www.instagram.com"));
            for (int i = 0; i < cookies.Count; i++)
            {
                if (cookies[i].Name == "csrftoken")
                {
                    csrfToken = cookies[i].Value;
                    break;
                }
            }


        }

        //Set Credentials [Username, Password]
        public void SetCredentials(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        //Trying to login -> returns userID if succeed and null if failed
        public string Login()
        {
            if (Username.Trim().Length == 0 || Password.Trim().Length == 0)
                return null;


            //Creating body data to send for the instagram web server
            string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            string data = "username=" + Username + "&enc_password=#PWD_INSTAGRAM_BROWSER:0:" + timeStamp + ":" + Password + "&queryParams={}&optIntoOneTap=false&stopDeletionNonce=&trustedDeviceRecords={}";
            

            //Setting headers to the request
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                {"Content-Type" , "application/x-www-form-urlencoded"},
                {"x-csrftoken", csrfToken },
                {"Referer","https://www.instagram.com/accounts/login/" },
                {"X-Requested-With", "XMLHttpRequest" }
            };

            //sending the request and recieveing response
            HttpWebResponse response = HttpRequests.Post("https://www.instagram.com/accounts/login/ajax/", data, headers, cookiesSession);
            string respSTR = HttpRequests.ReadResponse(response);
            int indexResp = respSTR.IndexOf("authenticated") + "authenticated".Length + 2;
            if(indexResp == -1)
                return null;


            //start with t means "true" else mean "false"
            bool succeed = respSTR[indexResp] == 't';

            //pooling the user if if authenticated
            string id = "";
            if (succeed)
            {
                Console.WriteLine(".\n.\n.");
                Console.WriteLine(respSTR);
                int idIndex = respSTR.IndexOf("userId") + "userId".Length + 3;
                for (int i = idIndex; i < respSTR.Length; i++)
                {
                    if (respSTR[i] == '\"')
                        break;
                    id += respSTR[i];
                }
            }
            UserId = id;

            return succeed ? id : null;
        }



        //fetch the followers data about the account
        public DataParser FetchData()
        {
            DataParser dParser = new DataParser();
            int status = -1;


            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Fetching User Followers List...");

            do
            {
                string variables = "{\"id\":\"" + UserId + "\",\"include_reel\":true,\"fetch_mutual\":true,\"first\":50";
                variables += dParser.nextPage == "" ? "}" : ",\"after\":" + "\"" + dParser.nextPage + "\"}";
                string encoded = Uri.EscapeDataString(variables);
                string queryFollowers = "37479f2b8209594dde7facb0d904896a";


                string url = $"https://www.instagram.com/graphql/query/?query_hash={queryFollowers}&variables={encoded}";
                string jsonResp = (HttpRequests.ReadResponse(HttpRequests.Get(url, null, cookiesSession)));
                dParser.LoadData(jsonResp);

                status = dParser.Analyze(0);
                if (status == -1)
                {
                    Console.WriteLine("Failed To Fetch User Data");
                    return null;
                }
            } while (status != 0);

            status = -1;

            Console.WriteLine("Fetching User Following List...");
            do
            {
                string variables = "{\"id\":\"" + UserId + "\",\"include_reel\":true,\"fetch_mutual\":true,\"first\":50";
                variables += dParser.nextPage == "" ? "}" : ",\"after\":" + "\"" + dParser.nextPage + "\"}";
                string encoded = Uri.EscapeDataString(variables);
                string queryFollowing = "58712303d941c6855d4e888c5f0cd22f";


                string url = $"https://www.instagram.com/graphql/query/?query_hash={queryFollowing}&variables={encoded}";
                string jsonResp = (HttpRequests.ReadResponse(HttpRequests.Get(url, null, cookiesSession)));
                dParser.LoadData(jsonResp);

                status = dParser.Analyze(1);
                if (status == -1)
                {
                    Console.WriteLine("Failed To Fetch User Data");
                    return null;
                }
            } while (status != 0);

            return dParser;

        }


        public InstaScraper()
        {
            Username = "";
            Password = "";
            cookiesSession = new CookieContainer();
            FirstInitialize();
        }
    }
}
