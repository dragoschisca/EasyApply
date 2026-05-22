using System.IO;
using System.Threading.Tasks;

namespace EasyApply.BusinessLayer.Interfaces.Services;

public interface ISupabaseStorageService
{
    Task<string> UploadFileAsync(string bucket, string fileName, Stream content);
    Task<Stream> DownloadFileAsync(string bucket, string fileName);
    Task DeleteFileAsync(string bucket, string fileName);
    string GetPublicUrl(string bucket, string fileName);
}
