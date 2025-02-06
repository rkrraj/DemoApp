using Azure.Data.Tables;
using DemoApp.Models;
using DemoApp.Service.Proxy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DemoApp.Service
{
    public class UserPostService : IUserPostService
    {
        private readonly IHttpProxy _httpProxy;
        private readonly TableClient _tableClient;
        private readonly ILogger<UserPostService> _log;
        public UserPostService(IHttpProxy httpProxy, TableClient tableClient, ILogger<UserPostService> log)
        {
            _httpProxy = httpProxy;
            _tableClient = tableClient;
            _log = log;
        }
        public async Task ProcessUserPostAsync()
        {
            await _tableClient.CreateIfNotExistsAsync(); // Create table if it doesn't exist (do this once)
            try
            {
                var posts = await FetchPostsFromApi();
                if (posts == null || posts.Count == 0) return; // Early exit if no posts

                var (storedCount, skippedCount) = await StorePosts(posts);
                var successMessage = $"Successfully processed posts. Total: {posts.Count}, filteredPosts:{storedCount + skippedCount}, Stored: {storedCount}, Skipped: {skippedCount}";
                _log.LogInformation(successMessage);

            }
            catch (Exception ex)
            {
                _log.LogError(ex, "An error occurred in the function.");
            }
        }

        private async Task<List<Post>> FetchPostsFromApi()
        {
            try
            {
                return await _httpProxy.GetAsync<List<Post>>("https://jsonplaceholder.typicode.com/posts");
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error fetching posts from API.");
                return null;
            }
        }

        private async Task<(int storedCount, int skippedCount)> StorePosts(List<Post> posts)
        {
            int storedCount = 0;
            int skippedCount = 0;

            var filteredPosts = posts.Where(p => p.UserId == 1).OrderBy(p => p.Id).ToList();

            foreach (var post in filteredPosts)
            {
                try
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
                        _log.LogInformation("Upserted post with Id: {postId}", post.Id);
                        storedCount++;
                    }
                    catch (Exception ex) // Other exceptions during insert
                    {
                        _log.LogError(ex, $"Error inserting post with Id: {post.Id}");
                        skippedCount++;
                    }
                }
                catch (Exception ex) // Exceptions during post processing
                {
                    _log.LogError(ex, $"Error processing post with Id: {post.Id}");
                }
            }
            return (storedCount, skippedCount); // Return counts as a tuple
        }
    }
}
