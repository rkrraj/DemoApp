using DemoApp.Models;
using DemoApp.Service.Proxy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoApp.Service.Facade
{
    public class ArticleFacade : IArticleFacade
    {
        private readonly IHttpProxy _httpProxy;
        private readonly ILogger<ArticleFacade> _logger;
        public ArticleFacade(IHttpProxy httpProxy, ILogger<ArticleFacade> logger)
        {
            _httpProxy = httpProxy;
            _logger = logger;
        }
        public async Task<List<Post>> FetchPostsFromApiAsync()
        {
            try
            {
                return await _httpProxy.GetAsync<List<Post>>("https://jsonplaceholder.typicode.com/posts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching posts from API.");
                return null;
            }
        }
    }
}
