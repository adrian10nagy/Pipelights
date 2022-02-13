using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Pipelights.Website.Services.Interfaces
{
    public interface IBlobService
    {
        Task<bool> MoveFileToBlobStorage(string sourceContainerName, string sourceFileName,
            string destinationContainerName, string destName, ILogger logger);

        Task<bool> SaveFileToBlobStorage(Stream stream, string fileName, string container, ILogger logger);

        Task<MemoryStream> SaveFileFromBlobStorage(string fileName, string container,
            ILogger logger);

        Task<Stream> OpenFile(string container, string fileName, ILogger logger);

        List<string> GetAllResourcesFromFolder(string folder);
        Task<bool> RemoveFile(string fileName, string container, ILogger logger);
    }
}
