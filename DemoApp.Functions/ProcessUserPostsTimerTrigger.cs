using DemoApp.Service.Service.Implementation;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DemoApp.Functions
{
    public class ProcessUserPostsTimerTrigger
    {
        private readonly ILogger<ProcessUserPostsTimerTrigger> _logger;
        private readonly IArticleService _articleService;

        public ProcessUserPostsTimerTrigger(ILogger<ProcessUserPostsTimerTrigger> logger, IArticleService articleService)
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

        }
    }
}
