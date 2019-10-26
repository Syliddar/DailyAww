using System;
using System.Net.Http;
using System.Web;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using DailyAww.Interfaces;
using HtmlAgilityPack;
using RedditSharp;
using RedditSharp.Things;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

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
            var result = new List<Post>();
            foreach (var post in list)
            {
                if (!IsAcceptableFileType(post)) continue;
                if (PassesCuteFilterAsync(post)) result.Add(post);
            }

            return result;
        }

        private static bool PassesCuteFilterAsync(Post post)
        {
            if (post.Title.IndexOf("snake", StringComparison.OrdinalIgnoreCase) >= 0) return false;
            if (post.Title.IndexOf("snek", StringComparison.OrdinalIgnoreCase) >= 0) return false;
            if (post.Title.IndexOf("noodle", StringComparison.OrdinalIgnoreCase)>= 0) return false;
            else return true;
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
            //var url = WebConfigurationManager.AppSettings["ManualAwwUrl"];
            //var web = new HtmlWeb();
            //var doc = web.Load(url);
            //var nodes = doc.DocumentNode.SelectNodes("//a")
            //    .Where(n => n.InnerText.Contains("Parent Directory") == false);
            //if (nodes.Any())
            //    result.AddRange(nodes.Select(htmlNode => new Post
            //    {
            //        Title = htmlNode.InnerText,
            //        Url = new Uri(url + htmlNode.Attributes["href"].Value)
            //    }));

            return result;
        }


        static string MakeRequest(string url)
        {
            //var cflient = new HttpClient();
            //cflient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "5f02fb70e86244879c529dd39157a348");

            //object data = new
            //{
            //    url
            //};
            //var myContent = JsonConvert.SerializeObject(data);
            //var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            //var byteContent = new ByteArrayContent(buffer);
            //byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //var fresponse = cflient.PostAsync("https://southcentralus.api.cognitive.microsoft.com/vision/v1.0/tag", byteContent).Result;



            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "5f02fb70e86244879c529dd39157a348");

            var uri = "https://southcentralus.api.cognitive.microsoft.com/vision/v1.0/tag?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("\"url\":\"" + url + "\"");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = client.PostAsync(uri, content).Result;
            }
            client.Dispose();
            return response.Content.ToString();
        }
    }
}