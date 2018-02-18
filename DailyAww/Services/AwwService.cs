using DailyAww.Interfaces;
using System;
using RedditSharp;
using RedditSharp.Things;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Configuration;
using HtmlAgilityPack;

namespace DailyAww.Services
{
    public class AwwService : IAwwService
    {
        private HttpClient _client;
        private Subreddit _aww;
        public AwwService()
        {
            _client = new HttpClient();
            var reddit = new Reddit();
            _aww = reddit.GetSubreddit("/r/aww");
        }

        public string GetDailyAwws()
        {
            var list = GetManualAww();
            list.AddRange(_aww.GetTop(FromTime.Day).Where(x => x.Url.ToString().Contains(".jpg")).Take(25));
            list = FilterUndesirableAwws(list);
            return ParsePostsIntoEmailBody(list, 10); 
        }
        public string GetHourlyAwws()
        {
            var list = GetManualAww();
            list.AddRange(_aww.Hot.Where(x => x.Url.ToString().Contains(".jpg")).Take(25));
            list = FilterUndesirableAwws(list);
            return ParsePostsIntoEmailBody(list, 5);
        }

        public string GetWeeklyAwws()
        {
            var list = GetManualAww();
            list.AddRange(_aww.GetTop(FromTime.Week).Where(x => x.Url.ToString().Contains(".jpg")).Take(25));
            var filteredList = FilterUndesirableAwws(list);
            return ParsePostsIntoEmailBody(filteredList, 12);
        }

        private List<Post> FilterUndesirableAwws(List<Post> list)
        {
            var result = new List<Post>();
            foreach (var post in list)
            {
                if (PassesCuteFilter(post))
                {
                    result.Add(post);
                }
            }
            return result;
        }
        private static bool PassesCuteFilter(Post post)
        {
            if (post.Title.IndexOf("snake", StringComparison.OrdinalIgnoreCase) >= 0) return false;
            if (post.Title.IndexOf("snek", StringComparison.OrdinalIgnoreCase) >= 0) return false;
            if (post.Title.IndexOf("noodle", StringComparison.OrdinalIgnoreCase) >= 0) return false;
            return true;
        }
        
        private string ParsePostsIntoEmailBody(List<Post> list, int awwCount)
        {
            var path = System.Web.HttpContext.Current.Server.MapPath("~/Views/Aww/EmailTemplate.html");
            var result = File.ReadAllText(path);
            var awwTable = "";
            foreach (var post in list.Take(awwCount).OrderBy(l => Guid.NewGuid()).ToList())
            {
                awwTable += "<h3>" + post.Title + "</h3><img src='" + post.Url + "' /><hr />";
            }
            awwTable.Remove(awwTable.LastIndexOf("<hr />", StringComparison.Ordinal));
            //EmailTemplate.Html includes an <AWWS /> tag, which we are replacing with our content
            result = result.Replace("<AWWS />", awwTable);
            return result;
        }

        private List<Post> GetManualAww()
        {
            var result = new List<Post>();
            var url = WebConfigurationManager.AppSettings["ManualAwwUrl"];
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var nodes = doc.DocumentNode.SelectNodes("//a").Where(n=>n.InnerText.Contains("Parent Directory") == false);
            foreach (var htmlNode in nodes)
            {
                var post = new Post()
                {
                    //Title = htmlNode.InnerText,
                    Title = "This is a Test of the Manual Aww Input system. Also, this is Lucy.  -Jason",
                    Url = new Uri(url + htmlNode.Attributes["href"].Value)
                };
                result.Add(post);
            }
            return result;
        }

    }
}