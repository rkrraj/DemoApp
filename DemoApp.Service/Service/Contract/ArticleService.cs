using Azure.Data.Tables;
using DemoApp.Models;
using DemoApp.Service.Facade;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoApp.Service
{
    public class ArticleService : IArticleService
    {
        private readonly TableClient _tableClient;
        private readonly ILogger<ArticleService> _logger;
        private readonly IArticleFacade _articleFacade;
        public ArticleService(TableClient tableClient, IArticleFacade articleFacade, ILogger<ArticleService> logger)
        {
            _articleFacade = articleFacade;
            _tableClient = tableClient;
            _logger = logger;
        }
        public async Task<string> ProcessUserPostAsync()
        {
            await _tableClient.CreateIfNotExistsAsync(); // Create table if it doesn't exist (do this once)
            var posts = await _articleFacade.FetchPostsFromApiAsync();
            var statusMessage = string.Empty;
            if (posts == null || posts.Count == 0) return statusMessage = "There is no posts found to process";
            var (storedCount, skippedCount) = await StorePostsAsync(posts);
            statusMessage = $"Successfully processed posts. Total: {posts.Count}, filteredPosts:{storedCount + skippedCount}, Stored: {storedCount}, Skipped: {skippedCount}";
            return statusMessage;
        }

        private async Task<(int storedCount, int skippedCount)> StorePostsAsync(List<UserPost> posts)
        {
            int storedCount = 0;
            int skippedCount = 0;

            var filteredPosts = posts.Where(p => p.UserId == 1).OrderBy(p => p.Id).ToList();

            foreach (var post in filteredPosts)
            {
                post.Title = post.Title.ToUpper();
                var entity = new PostEntity
                {
                    PartitionKey = "posts",
                    RowKey = post.Id.ToString(),
                    UserId = post.UserId,
                    Id = post.Id,
                    Title = post.Title,
                    Body = post.Body,
                    Timestamp = DateTimeOffset.UtcNow
                };

                try
                {
                    await _tableClient.UpsertEntityAsync(entity);
                    _logger.LogInformation("Upserted post with Id: {postId}", post.Id);
                    storedCount++;
                }
                catch (Exception ex) // Other exceptions during insert
                {
                    _logger.LogError(ex, $"Error inserting post with Id: {post.Id}");
                    skippedCount++;
                }

            }
            return (storedCount, skippedCount); // Return counts as a tuple
        }
    }
}
