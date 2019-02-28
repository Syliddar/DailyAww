using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using DailyAww.Interfaces;
using HtmlAgilityPack;
using RedditSharp;
using RedditSharp.Things;

namespace DailyAww.Services
{
    public class AwwService : IAwwService
    {
        private readonly Subreddit _aww;

        public AwwService()
        {
            var reddit = new Reddit();
            _aww = reddit.GetSubreddit("/r/aww");
        }

        public string GetAwws(FromTime fromTime)
        {
            var list = GetManualAww();
            list.AddRange(_aww.GetTop(fromTime).Where(x =>
                x.Url.AbsoluteUri.EndsWith(".jpg") ||
                x.Url.AbsoluteUri.EndsWith(".png") ||
                x.Url.AbsoluteUri.EndsWith(".webm") ||
                x.Url.AbsoluteUri.EndsWith(".gif")).Take(25));
            list = FilterUndesirableAwws(list);

            return ParsePostsIntoEmailBody(list, 10);
        }

        private static List<Post> FilterUndesirableAwws(IEnumerable<Post> list)
        {
            return list.Where(PassesCuteFilter).ToList();
        }

        private static bool PassesCuteFilter(Post post)
        {
            bool result = !(post.Title.IndexOf("snake", StringComparison.OrdinalIgnoreCase) >= 0);
            if (post.Title.IndexOf("snek", StringComparison.OrdinalIgnoreCase) >= 0) result = false;
            if (post.Title.IndexOf("noodle", StringComparison.OrdinalIgnoreCase) >= 0) result = false;
            return result;
        }

        private static string ParsePostsIntoEmailBody(List<Post> list, int awwCount)
        {
            var path = HttpContext.Current.Server.MapPath("~/Views/Aww/EmailTemplate.html");
            var result = File.ReadAllText(path);
            var awwTable = "";
            //get that good shit first. Fukken GIFs
            var finalAwwList = list.Where(x => x.Url.AbsoluteUri.EndsWith(".gif") || x.Url.AbsoluteUri.EndsWith(".webm")).ToList();
            finalAwwList.AddRange(list.Take(awwCount - finalAwwList.Count).ToList());
            foreach (var post in finalAwwList.OrderBy(l => Guid.NewGuid()))
            {
                if (post.Url.AbsoluteUri.EndsWith(".webm"))
                {
                    awwTable += "<h3>" + post.Title + "</h3><img src='" + post.Url.AbsoluteUri.Replace("webm", "gif") + "' /><hr />";
                }
                else
                {
                    awwTable += "<h3>" + post.Title + "</h3><img src='" + post.Url + "' /><hr />";
                }
            }
            awwTable.Remove(awwTable.LastIndexOf("<hr />", StringComparison.Ordinal));
            //EmailTemplate.Html includes an <AWWS /> tag, which we are replacing with our content
            result = result.Replace("<AWWS />", awwTable);
            return result;
        }

        private static List<Post> GetManualAww()
        {
            var result = new List<Post>();
            //            var url = WebConfigurationManager.AppSettings["ManualAwwUrl"];
            //            var web = new HtmlWeb();
            //            var doc = web.Load(url);
            //            var nodes = doc.DocumentNode.SelectNodes("//a")
            //                .Where(n => n.InnerText.Contains("Parent Directory") == false);
            //            if (nodes.Any())
            //                result.AddRange(nodes.Select(htmlNode => new Post
            //                {
            //                    Title = htmlNode.InnerText,
            //                    Url = new Uri(url + htmlNode.Attributes["href"].Value)
            //                }));

            return result;
        }
    }
}