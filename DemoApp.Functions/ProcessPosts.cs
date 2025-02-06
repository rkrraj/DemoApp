using DemoApp.Service;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DemoApp.Functions
{
    public class ProcessPosts
    {
        private readonly ILogger<ProcessPosts> _logger;
        private readonly IUserPostService _blogPostService;

        public ProcessPosts(ILogger<ProcessPosts> logger, IUserPostService blogPostService)
        {
            _logger = logger;
            _blogPostService = blogPostService;
        }

        [Function("Function1")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await _blogPostService.ProcessUserPostAsync();

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
