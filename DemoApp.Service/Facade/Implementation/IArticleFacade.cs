using DemoApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoApp.Service.Facade
{
    public interface IArticleFacade
    {
        Task<List<UserPost>> FetchPostsFromApiAsync();
    }
}
