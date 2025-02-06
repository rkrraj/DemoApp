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
        private readonly IApiClient _httpProxy;
        private readonly ILogger<ArticleFacade> _logger;
        public ArticleFacade(IApiClient httpProxy, ILogger<ArticleFacade> logger)
        {
            _httpProxy = httpProxy;
            _logger = logger;
        }
        public async Task<List<UserPost>> FetchPostsFromApiAsync()
        {
            try
            {
                return await _httpProxy.GetAsync<List<UserPost>>("https://jsonplaceholder.typicode.com/posts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching posts from API.");
                return null;
            }
        }
    }
}
