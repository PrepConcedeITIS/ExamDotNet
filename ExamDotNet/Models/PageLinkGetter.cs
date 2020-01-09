using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ExamDotNet.Models
{
    public class PageLinkGetter
    {
        private async static Task<string> LoadHtml(string url)
        {
            using(var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                if(response.IsSuccessStatusCode && response.Content!=null)
                {
                    var html = await response.Content.ReadAsStringAsync();
                    return html;
                }
                else
                {
                    throw new Exception("Can't get site");
                }
            }
        }

        public async static Task<List<string>> GetLinks(string url, List<string> currentLinks)
        {
            var uri = new Uri(url);
            var html = await LoadHtml(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var aTags = doc.DocumentNode.Descendants("a");
            var links = aTags.Select(node => node.GetAttributeValue("href", "no href"))
                .Where(str => str != "no href" && !str.Contains("mailto") && !str.Contains("@") && !str.Contains(@"javascript://"))
                .Where(str=>!currentLinks.Contains(str))
                .Where(str=>
                {
                    try
                    {
                        var test = new Uri(str);
                        if (test.Host == uri.Host)
                            return true;
                        return false;
                    }
                    catch(Exception)
                    {
                        return true;
                    }
                })
                .Take(10)
                .ToList();
            return links;
        }

        public async static Task<string> GetBody(string url)
        {
            var html = "";
            try
            {
                 html = await LoadHtml(url);
            }
            catch
            {
                return "Can't get content";
            }
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var body = doc.DocumentNode.InnerText;
            return string.Concat(body.Trim().Take(500)) + "...";
        }

        public static string ToCorrectFormat(string baseUrl, string link)
        {
            var goodLink = "";
            try
            {
                var test = new Uri(link);
                goodLink = link;
            }
            catch (Exception)
            {
                goodLink = baseUrl + link;
            }
            return goodLink;
        }
    }
}
