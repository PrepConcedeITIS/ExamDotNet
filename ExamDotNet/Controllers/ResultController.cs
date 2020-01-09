using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamDotNet.Controllers
{
    public class ResultController:Controller
    {
        private readonly DotNetExamContext context;

        public ResultController(DotNetExamContext context)
        {
            this.context = context;
        }

        public IActionResult Results()
        {
            return View();
        }

        public async Task<IActionResult> Search(string domain)
        {
            var domainLocal = domain;
            try
            {
                var uri = new Uri(domain);
                domainLocal = uri.Host;
            }
            catch { }
            var result = await GetContent(domain);
            return View(result);
        }

        private async Task<List<Tuple<string, string>>> GetContent(string domainStr)
        {
            var domain = context.Domain.FirstOrDefault(domain => domain.Name == domainStr);
            if (domain == null)
                return new List<Tuple<string, string>>();

            var content = context.UrlContent
                .Where(cont => cont.DomainId == domain.Id)
                .Select(cont => Tuple.Create(cont.Url, cont.Content))
                .ToList();
            return content;
        }

    }
}
