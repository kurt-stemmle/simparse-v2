using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Simparse.Collections;
using Simparse.Models.File;
using Simparse.Enums;
using Simparse.FileStorage;
using Simparse.Identity;
using Simparse.Models;
using System.Linq;
using Simparse.Domain;
using Simparse.Models.Words;
using System.Text.RegularExpressions;

namespace Simparse.Backend.Controllers
{

    /// <summary>
    /// CRUD Entrypoints for File Directories and File Items.
    /// The File Directories and File Items are stored in a nested Parent - Children file structure.
    /// Each node has it owns OCR Vector data or inherits that data from its parent node.
    /// When nodes are moved or deleted this action is always applied to the children nodes as well.
    /// </summary>
    [Route("api/file")]
    [ApiController]
    public class FileController : ControllerBase
    {

        private readonly ISimparseUserStore _userStore;
        private readonly IUserCollection _userCollection;
        private readonly ILogger<FileController> _logger;
        private readonly IFileAccessStore _fileStore;
        private readonly IFieldMappingDomain _fieldMappingDomain;


        /// <summary>
        /// Constructor CRUD File entry points
        /// </summary>
        /// <param name="userStore"></param>
        /// <param name="userCollection"></param>
        /// <param name="logger"></param>
        /// <param name="filestore"></param>
        /// <param name="fieldMappingDomain"></param>
        public FileController(ISimparseUserStore userStore,
            IUserCollection userCollection,
            ILogger<FileController> logger,
            IFileAccessStore filestore,
            IFieldMappingDomain fieldMappingDomain)
        {
            _userStore = userStore;
            _userCollection = userCollection;
            _logger = logger;
            _fileStore = filestore;
            _fieldMappingDomain = fieldMappingDomain;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            string token = Request.Headers["SToken"];
            var refreshData = await _userStore.ValidateToken(token).ConfigureAwait(false);
            ApplicationUser user = await _userCollection.GetUserByIdentityId(refreshData.user_id).ConfigureAwait(false);
            return user;
        }

        /// <summary>
        /// Get a list of items that live under the umbrella of this parent Id.
        /// If no parent Id is provided we will return all items from the root level.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetFolders"), ProducesDefaultResponseType(typeof(List<FolderDataViewModel>))]
        public async Task<IActionResult> GetFolders()
        {
            try
            {
                var user = await GetCurrentUser().ConfigureAwait(false);

                if (user == null)
                {
                    return BadRequest();
                }

                List<FolderDataViewModel> response = new List<FolderDataViewModel>();

                var folderDomainData = await _fieldMappingDomain.GetFolderList(user.Id).ConfigureAwait(false);

                foreach (var folder in folderDomainData)
                {
                    response.Add(new FolderDataViewModel(folder));
                }

                return Ok(response);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTreeItem failure");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get a list of items that live under the umbrella of this parent Id.
        /// If no parent Id is provided we will return all items from the root level.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetFiles/{folderId}"), ProducesDefaultResponseType(typeof(List<FileDataViewModel>))]
        public async Task<IActionResult> GetFiles(string folderId)
        {

            try
            {
                var user = await GetCurrentUser().ConfigureAwait(false);

                if (user == null)
                {
                    return BadRequest();
                }

                if (folderId.Contains("null"))
                {
                    folderId = null;
                }

                if (!string.IsNullOrWhiteSpace(folderId))
                {
                    var files = await _fieldMappingDomain.GetFileList(folderId).ConfigureAwait(false);

                    List<FileDataViewModel> response = new List<FileDataViewModel>();

                    foreach (var file in files)
                    {
                        response.Add(new FileDataViewModel(file));
                    }

                    return Ok(response);
                }


                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTreeItem failure");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Create a new Directory in the tree.
        /// If the Parent Id is not supplied this folder will go into the root directory.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("CreateFolder"), ProducesDefaultResponseType(typeof(bool))]
        public async Task<IActionResult> CreateFolder(CreateFolderRequest request)
        {
            try
            {
                var user = await GetCurrentUser().ConfigureAwait(false);

                if (user == null || string.IsNullOrWhiteSpace(request.Name))
                {
                    return BadRequest();
                }

                await _fieldMappingDomain.CreateFolder(request.Name, user.Id);

                return Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTreeItem failure");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Delete a file or director item and all its potential children
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpGet, Route("DeleteFile/{fileId}"), ProducesDefaultResponseType(typeof(bool))]
        public async Task<IActionResult> DeleteFile(string fileId)
        {
            try
            {
                var user = await GetCurrentUser().ConfigureAwait(false);

                if (user == null)
                {
                    return BadRequest();
                }

                await _fieldMappingDomain.DeleteFile(fileId).ConfigureAwait(false);

                return Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTreeItem failure");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Delete a folder or director item and all its potential children
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        [HttpGet, Route("DeleteFolder/{folderId}"), ProducesDefaultResponseType(typeof(bool))]
        public async Task<IActionResult> DeleteFolder(string folderId)
        {
            try
            {
                var user = await GetCurrentUser().ConfigureAwait(false);

                if (user == null)
                {
                    return BadRequest();
                }

                await _fieldMappingDomain.DeleteFolder(folderId).ConfigureAwait(false);

                return Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTreeItem failure");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Get the data for a individul file upload
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        [HttpGet, Route("GetFileDataForCanvas/{fileId}/{pageNumber}/{zoom}"), ProducesDefaultResponseType(typeof(FileDataViewModel))]
        public async Task<IActionResult> GetFileData(string fileId, int pageNumber, int zoom)
        {
            var fileData = await _fieldMappingDomain.GetFileDataForCanvas(fileId, pageNumber, zoom).ConfigureAwait(false);

            FileDataViewModel response = new FileDataViewModel(fileData);

            return Ok(response);
        }

        /// <summary>
        /// File Uploader
        /// </summary>
        /// <param name="chunkMetadata"></param>
        /// <returns></returns>
        [HttpPost, Route("UploadFile/{chunkMetadata}")]
        public async Task<IActionResult> UploadFile(string chunkMetadata)
        {

            try
            {

                var user = await GetCurrentUser().ConfigureAwait(false);

                if (user == null)
                {
                    return BadRequest();
                }

                if (!string.IsNullOrEmpty(chunkMetadata))
                {
                    var metaDataObject = JsonConvert.DeserializeObject<FileUploadRequest>(chunkMetadata);

                    foreach (var formFile in Request.Form.Files)
                    {

                        var extension = Path.GetExtension(formFile.FileName);

                        var fileName = Path.GetFileNameWithoutExtension(formFile.FileName);

                        ExtensionType dataCheck = ExtensionDataCheck(extension);

                        if (dataCheck == ExtensionType.Unsupported)
                        {
                            return BadRequest("Unsupported media type");
                        }



                        Guid fileId = Guid.NewGuid();

                        using var stream = formFile.OpenReadStream();

                        bool isImage = false;
                        int height = 0;
                        int width = 0;
                        if (dataCheck == ExtensionType.JPEG || dataCheck == ExtensionType.PNG || dataCheck == ExtensionType.TIFF)
                        {
                            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                            isImage = true;
                            width = image.Width;
                            height = image.Height;
                        }

                        string uploadBucket = "simparse-upload";

                        await _fileStore.InsertFile(stream, fileId, uploadBucket, extension).ConfigureAwait(false);

                        await _fieldMappingDomain.ProcessNewFile(fileId, metaDataObject.FolderId, extension, dataCheck, uploadBucket, fileName, (int)formFile.Length, user.Id, isImage, width, height).ConfigureAwait(false);

                    }

                }
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(new { Success = true });
        }

        /// <summary>
        /// Helper method to get files extension as an enum
        /// </summary>
        /// <param name="extensionData"></param>
        /// <returns></returns>
        private ExtensionType ExtensionDataCheck(string extensionData)
        {
            return extensionData switch
            {
                ".png" => ExtensionType.PNG,
                ".jpeg" => ExtensionType.JPEG,
                ".jpg" => ExtensionType.JPEG,
                ".tiff" => ExtensionType.TIFF,
                ".pdf" => ExtensionType.PDF,
                ".zip" => ExtensionType.ZIP,
                ".docx" => ExtensionType.Word,
                ".xlsx" => ExtensionType.Excel,
                ".csv" => ExtensionType.CSV,
                ".txt" => ExtensionType.TXT,
                ".json" => ExtensionType.JSON,
                _ => ExtensionType.Unsupported,
            };
        }


        [HttpPost, Route("InsertMapping")]
        public async Task<IActionResult> InsertMapping(FileMappingItem item)
        {
            await _fieldMappingDomain.InsertMapping(item);
            return Ok(new { Success = true });
        }

        [HttpGet, Route("DeleteMapping/{id}")]
        public async Task<IActionResult> DeleteMapping(string id)
        {
            await _fieldMappingDomain.DeleteMapping(id);
            return Ok(new { Success = true });
        }


        [HttpGet, Route("GetMappings/{folderId}/file/{fileId}/page/{page:int}")]
        public async Task<IActionResult> GetMappings(string folderId, string fileId, int page)
        {
            var response = await _fieldMappingDomain.GetMappingsByFolderId(folderId, fileId, page);
            return Ok(response);
        }

        [HttpPost, Route("UpdateMappings")]
        public async Task<IActionResult> UpdateMappings(SaveMappingsRequest request)
        {
            await _fieldMappingDomain.UpdateFolderMappings(request.Items);
            return Ok(new { Success = true });
        }

        [HttpGet, Route("GetFileJSON/{id}")]
        public async Task<IActionResult> GetFileJSON(string id)
        {
            var stream = await _fieldMappingDomain.FileResponseJSON(id);
            return File(stream, "application/octet-stream", "file.json");
        }

        [HttpGet, Route("GetFolderJSON/{id}")]
        public async Task<IActionResult> GetFolderJSON(string id)
        {
            var stream = await _fieldMappingDomain.FileListResponseJSON(id);
            return File(stream, "application/octet-stream", "file.json");
        }

        [HttpGet, Route("GetFolderXLS/{id}")]
        public async Task<IActionResult> GetFolderXLS(string id)
        {
            var stream = await _fieldMappingDomain.FileListResponseXLS(id);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "file.xlsx");
        }


    }
}