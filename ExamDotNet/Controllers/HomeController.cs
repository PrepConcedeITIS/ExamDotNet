using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ExamDotNet.Models;
using Newtonsoft.Json;

namespace ExamDotNet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DotNetExamContext context;

        public HomeController(ILogger<HomeController> logger, DotNetExamContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> GetUrls(string url, int depth)
        {
            int i = 1;
            url = NormilizeLink(url);
            var list = new List<string>();
            var baseLinks = new List<string>();
            try
            {
                baseLinks = await PageLinkGetter.GetLinks(url, list);
            }
            catch
            {
                return View(new List<string>());
            }
            baseLinks = baseLinks.Select(str => PageLinkGetter.ToCorrectFormat(url, str)).ToList();
            list.AddRange(baseLinks);
            while (i != depth)
            {
                var newBaseLinks = new List<string>();
                foreach (var link in baseLinks)
                {
                    var inLinks = new List<string>();
                    try
                    {
                        inLinks = await PageLinkGetter.GetLinks(link, list);
                    }
                    catch { }

                    foreach (var inLink in inLinks)
                    {
                        if (!newBaseLinks.Contains(inLink))
                            newBaseLinks.Add(PageLinkGetter.ToCorrectFormat(url, inLink));
                    }
                }

                foreach (var link in newBaseLinks)
                {
                    if (!list.Contains(link))
                        list.Add(link);
                }
                i++;
            }
            return View(list.Distinct().ToList());
        }


        public async Task<IActionResult> SaveToDb(string links)
        {
            var listLinks = JsonConvert.DeserializeObject<List<string>>(links);
            var body = await PageLinkGetter.GetBody(listLinks[0]);
            await SaveToDb(listLinks);
            return View("~/Views/Result/Results.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string NormilizeLink(string str)
        {
            if (str.Last().ToString() == "/")
                return str.Remove(str.Length - 1, 1);
            else
                return str;
        }

        private async Task<bool> SaveToDb(List<string> links)
        {
            var host = new Uri(links.FirstOrDefault()).Host;
            var domain = context.Domain.Where(domain => domain.Name == host).FirstOrDefault();
            if (domain == default)
            {
                context.Domain.Add(new Domain() { Name = host });
                context.SaveChanges();
            }
            domain = context.Domain.Where(domain => domain.Name == host).FirstOrDefault();
            foreach (var link in links)
            {
                var urlContent = new UrlContent()
                { DomainId = domain.Id, Url = link, Content = await PageLinkGetter.GetBody(link) };

                context.UrlContent.Add(urlContent);
            }
            context.UrlContent.Distinct();
            context.SaveChanges();
            return true;
        }
    }
}
