using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CameraAPI.Services.Interface
{
    public interface IFileUploadService
    {
        Task<string> UploadFileToStorage(IFormFile file);
    }
}
