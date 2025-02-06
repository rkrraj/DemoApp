using System.Threading.Tasks;

namespace DemoApp.Service.Service.Implementation
{
    public interface IArticleService
    {
        Task<string> ProcessUserPostAsync();
    }
}
