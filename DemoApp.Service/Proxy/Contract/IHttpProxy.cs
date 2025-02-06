using System.Net.Http;
using System.Threading.Tasks;

namespace DemoApp.Service.Proxy
{
    public interface IHttpProxy
    {
        Task<T> SendAsync<T>(HttpMethod method, string apiUrl, object data = null) where T : class;
        Task<T> GetAsync<T>(string apiUrl) where T : class;
        Task<T> PostAsync<T>(string apiUrl, object data) where T : class;
        Task<T> PutAsync<T>(string apiUrl, object data) where T : class;
        Task<T> DeleteAsync<T>(string apiUrl) where T : class;

    }
}