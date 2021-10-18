using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Simparse.FileStorage
{
    public interface IFileAccessStore
    {
        Task InsertFile(Stream stream, Guid objectKey, string bucketName, string fileExtension);

        Task GetFile(Guid objectKey, string bucketName, Stream stream, string fileExtension);

        Task DeleteFile(Guid objectKey, string bucketName, string fileExtension);
    }


    public class FileAccessStore : IFileAccessStore
    {
        private readonly StorageClient _client;
        private readonly string _googleProjectId;

        public FileAccessStore(StorageClient client, string googleProjectId)
        {
            _client = client;
            _googleProjectId = googleProjectId;
        }

        private async Task CreateBucket(string bucketName)
        {
            try
            {
                await _client.CreateBucketAsync(_googleProjectId, bucketName);
            }
            catch (Google.GoogleApiException e)
            when (e.Error.Code == 409)
            {

            }
        }

        public async Task InsertFile(Stream stream, Guid objectKey, string bucketName, string fileExtension)
        {
            await CreateBucket(bucketName);
            string objectKeyname = $"{objectKey}{fileExtension}";
            await _client.UploadObjectAsync(bucketName, objectKeyname, null, stream);
        }

        public async Task GetFile(Guid objectKey, string bucketName, Stream stream, string fileExtension)
        {
            string objectKeyname = $"{objectKey}{fileExtension}";
            await _client.DownloadObjectAsync(bucketName, objectKeyname, stream);
            stream.Position = 0;
        }

        public async Task DeleteFile(Guid objectKey, string bucketName, string fileExtension)
        {
            string objectKeyname = $"{objectKey}{fileExtension}";
            var thing = await _client.GetObjectAsync(bucketName, objectKeyname);
            await _client.DeleteObjectAsync(thing);
        }
    }
}
