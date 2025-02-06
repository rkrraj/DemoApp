using System.Threading.Tasks;

namespace DemoApp.Service
{
    public interface IArticleService
    {
        Task<string> ProcessUserPostAsync();
    }
}
