using DemoApp.Service;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DemoApp.Functions
{
    public class ProcessPosts
    {
        private readonly ILogger<ProcessPosts> _logger;
        private readonly IArticleService _articleService;

        public ProcessPosts(ILogger<ProcessPosts> logger, IArticleService articleService)
        {
            _logger = logger;
            _articleService = articleService;
        }

        [Function("Function1")]
        public async Task Run([TimerTrigger("0 */10 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var status = await _articleService.ProcessUserPostAsync();
            _logger.LogInformation(status);

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
