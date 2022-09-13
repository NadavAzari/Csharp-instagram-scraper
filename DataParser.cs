using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Instagram_Bot
{
    internal class DataParser
    {
        private string data;
        public List<User> followers;
        public List<User> following;

        public DataParser() {data = ""; followers = new List<User>(); following = new List<User>();}
        
        public void LoadData(string data) { this.data = data; }

        public string nextPage = "";


        public int Analyze(int status)
        {
            try
            {
                string token = status == 0 ? "edge_followed_by" : "edge_follow";
                JObject jsonObj = JObject.Parse(data);

                int totalFollowers = jsonObj["data"]["user"][token]["count"].Value<int>();
                bool hasMoreInfo = jsonObj["data"]["user"][token]["page_info"]["has_next_page"].Value<bool>();
                string nextPage = jsonObj["data"]["user"][token]["page_info"]["end_cursor"].Value<string>();

                this.nextPage = nextPage;

                JToken followersToken = jsonObj["data"]["user"][token]["edges"].ToString();
                List<JToken> nodes = JArray.Parse(followersToken.ToString()).ToList();

                foreach (JToken node in nodes)
                {
                    if (status == 0)
                    {
                        followers.Add(new User
                        {
                            Id = node["node"]["id"].Value<string>(),
                            Username = node["node"]["username"].Value<string>(),
                            FullName = node["node"]["full_name"].Value<string>(),
                            ProfilePicUrl = node["node"]["profile_pic_url"].Value<string>()
                        });
                    }
                    else
                    {
                        following.Add(new User
                        {
                            Id = node["node"]["id"].Value<string>(),
                            Username = node["node"]["username"].Value<string>(),
                            FullName = node["node"]["full_name"].Value<string>(),
                            ProfilePicUrl = node["node"]["profile_pic_url"].Value<string>()
                        });
                    }
                }

                return hasMoreInfo ? 1 : 0;
            }
            catch { return -1; }


        }

    }
}
