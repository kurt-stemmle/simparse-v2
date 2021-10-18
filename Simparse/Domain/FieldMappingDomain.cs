using Simparse.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Simparse.Vision;
using Simparse.Enums;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Drawing;

namespace Simparse.Domain
{

    public interface IFieldMappingDomain
    {
        Task ProcessNewFile(Guid fileId, string folderId, string fileExtension, ExtensionType type, string bucketName, string fileName, int size, string userId, bool isImage, int width, int height);

        Task<FileDataDomain> GetFileDataForCanvas(string fileId, int pageNumber, decimal zoom);

        Task<List<FileListDataModel>> GetFileList(string folderId);

        Task<List<FolderDataDomain>> GetFolderList(string userId);

        Task CreateFolder(string folderName, string userId);

        Task DeleteFolder(string folderId);

        Task DeleteFile(string fileId);

        Task InsertMapping(FileMappingItem item);

        Task DeleteMapping(string id);

        Task<List<MappingDomainItem>> GetMappingsByFolderId(string folderId, string fileId, int pageNumber);

        Task UpdateFolderMappings(List<FileMappingItem> mappings);

        Task<MemoryStream> FileResponseJSON(string fileId);

        Task<MemoryStream> FileListResponseJSON(string folderId);

        Task<MemoryStream> FileListResponseXLS(string folderId);

    }


    public class FieldMappingDomain : IFieldMappingDomain
    {
        private readonly IFolderCollection _folderCollection;
        private readonly IFileCollection _fileCollection;
        private readonly IGoogleVisionHandler _googleVisionHandler;
        private readonly IFileMappingCollection _mappingCollection;

        public FieldMappingDomain(IFolderCollection folderCollection, IFileCollection fileCollection, IFileMappingCollection mappingCollection, IGoogleVisionHandler handler)
        {
            _fileCollection = fileCollection;
            _folderCollection = folderCollection;
            _googleVisionHandler = handler;
            _mappingCollection = mappingCollection;
        }

        public async Task ProcessNewFile(Guid fileId, string folderId, string fileExtension, ExtensionType type, string bucketName, string fileName, int size, string userId, bool isImage, int width, int height)
        {
            VisionExtract visionData =
                type == ExtensionType.PDF ? await _googleVisionHandler.PDFRunner(fileId, fileExtension, bucketName) : await _googleVisionHandler.ImageRunner(fileId, fileExtension, bucketName, width, height);

            FileDataModel fileData = new FileDataModel()
            {
                Name = fileName,
                DateCreated = DateTime.UtcNow,
                Extension = fileExtension,
                Size = size,
                FileId = fileId,
                FolderId = folderId,
                PageCount = visionData.PageCount
            };

            foreach (var page in visionData.Pages)
            {
                PageItemDataModel pageModel = new PageItemDataModel()
                {
                    Height = page.Height,
                    PageNumber = page.PageNumber,
                    Width = page.Width,
                };

                foreach (var word in page.Words)
                {
                    WordItemDataModel wordModel = new WordItemDataModel()
                    {
                        XAxis = (decimal)word.CenterPointOfWord.X,
                        YAxis = (decimal)word.CenterPointOfWord.Y,
                        FontStyle = "Times New Roman",
                        Text = word.Text,
                        TextSize = word.TextSize,
                        Vertices = word.BoundingPointsOfWord
                    };

                    pageModel.WordItems.Add(wordModel);
                }

                fileData.Pages.Add(pageModel);
            }

            if (type == ExtensionType.PDF)
            {
                fileData.IconFileType = IconFileType.PDF;
            }
            else
            {
                fileData.IconFileType = IconFileType.Image;
            }

            await _fileCollection.InsertNewFile(fileData).ConfigureAwait(false);
        }

        public async Task<FileDataDomain> GetFileDataForCanvas(string fileId, int pageNumber, decimal zoom)
        {
            var file = await _fileCollection.GetFileById(fileId).ConfigureAwait(false);

            if (pageNumber != 0)
            {
                file.Pages = file.Pages.Where(m => m.PageNumber == pageNumber).ToList();
            }

            var domain = new FileDataDomain(file);

            domain.Resize(zoom);

            return domain;
        }

        public async Task<List<FileListDataModel>> GetFileList(string folderId)
        {
            var files = await _fileCollection.GetFileListByFolder(folderId, true).ConfigureAwait(false);
            var response = new List<FileListDataModel>();
            foreach (var file in files)
            {
                response.Add(new FileListDataModel(file));
            }
            return response;
        }

        public async Task<List<FolderDataDomain>> GetFolderList(string userId)
        {
            var folders = await _folderCollection.GetFolderList(userId).ConfigureAwait(false);
            var response = new List<FolderDataDomain>();
            foreach (var folder in folders)
            {
                response.Add(new FolderDataDomain(folder));
            }
            return response;
        }

        public async Task CreateFolder(string folderName, string userId)
        {
            await _folderCollection.CreateFolder(userId, folderName);
        }

        public async Task DeleteFolder(string folderId)
        {
            var files = await GetFileList(folderId).ConfigureAwait(false);

            foreach (var file in files)
            {
                await DeleteFile(file.Id).ConfigureAwait(false);
            }

            await _folderCollection.DeleteFolder(folderId).ConfigureAwait(false);
        }

        public async Task DeleteFile(string fileId)
        {
            await _fileCollection.DeleteFile(fileId).ConfigureAwait(false);
        }

        public async Task InsertMapping(FileMappingItem item)
        {
            await _mappingCollection.Insert(item);
        }

        public async Task DeleteMapping(string id)
        {
            await _mappingCollection.Delete(id);
        }

        public async Task<List<MappingDomainItem>> GetMappingsByFolderId(string folderId, string fileId, int pageNumber)
        {
            var mappings = await _mappingCollection.GetByFolderId(folderId, pageNumber);
            var fileData = await GetFileDataForCanvas(fileId, pageNumber, 100);
            return Extractor(mappings, fileData);
        }

        private List<MappingDomainItem> Extractor(List<FileMappingItem> response, FileDataDomain fileData)
        {
            var result = new List<MappingDomainItem>();

            foreach (var mapping in response)
            {
                StringBuilder build = new StringBuilder();
                MappingDomainItem map = new MappingDomainItem(mapping);

                foreach (var word in fileData.Words)
                {
                    bool didFindWord = false;

                    foreach (var mapV in mapping.Items)
                    {
                        didFindWord = DoesSegmentContainPoint(word.Box, mapV);

                        if (didFindWord)
                        {
                            break;
                        }
                    }

                    if (didFindWord == false)
                    {
                        foreach (var wordV in word.Box)
                        {
                            didFindWord = DoesSegmentContainPoint(mapping.Items, wordV);

                            if (didFindWord)
                            {
                                break;
                            }
                        }
                    }

                    if (didFindWord)
                    {
                        build.Append(word.Text);
                        build.Append(" ");
                    }
                }

                map.TextExtract = build.ToString();

                result.Add(map);
            }


            return result;
        }

        public async Task UpdateFolderMappings(List<FileMappingItem> mappings)
        {
            foreach (var mapping in mappings)
            {
                await _mappingCollection.UpdateFileMappingItem(mapping);
            }
        }

        public bool DoesSegmentContainPoint(List<VisionVerticie> polygon, VisionVerticie testPoint)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
                {
                    if (polygon[i].X + (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        public async Task<MemoryStream> FileListResponseXLS(string folderId)
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var extract = await GetFolderLevelDataExtract(folderId);

            using (var package = new ExcelPackage(new FileInfo("Folder.xlsx")))
            {
                package.Workbook.Properties.Author = "Simparse";
                package.Workbook.Properties.Title = "Simparse Extract";
                package.Workbook.Properties.Subject = "Folder level data extract";
                package.Workbook.Properties.Created = DateTime.Now;

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet 1");

                int rowCounter = 1;
                int columnCounter = 1;

                foreach (var row in extract)
                {


                    foreach (var field in row.Fields)
                    {
                        if (rowCounter == 1)
                        {
                            worksheet.Cells[rowCounter, columnCounter].Value = field.FieldName;
                            worksheet.Cells[rowCounter + 1, columnCounter].Value = field.FieldValue;
                        }
                        else
                        {
                            worksheet.Cells[rowCounter + 1, columnCounter].Value = field.FieldValue;
                        }

                        columnCounter++;
                    }

                    columnCounter = 1;
                    rowCounter++;

                }

                var stream = new MemoryStream(package.GetAsByteArray());

                return stream;
            }
        }

        public async Task<MemoryStream> FileResponseJSON(string fileId)
        {
            var extract = await GetFileLevelDataExtract(fileId);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartObject();

                foreach (var field in extract.Fields)
                {
                    writer.WritePropertyName(field.FieldName);
                    writer.WriteValue(field.FieldValue);
                }

                writer.WriteEndObject();
            }


            byte[] bytes = Encoding.ASCII.GetBytes(sb.ToString());

            var stream = new MemoryStream(bytes);

            return stream;
        }

        public async Task<MemoryStream> FileListResponseJSON(string folderId)
        {
            var extract = await GetFolderLevelDataExtract(folderId);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartObject();
                writer.WritePropertyName("Items");
                writer.WriteStartArray();

                foreach (var row in extract)
                {
                    writer.WriteStartObject();

                    foreach (var field in row.Fields)
                    {
                        writer.WritePropertyName(field.FieldName);
                        writer.WriteValue(field.FieldValue);
                    }

                    writer.WriteEnd();

                }

                writer.WriteEnd();
                writer.WriteEndObject();
            }


            byte[] bytes = Encoding.ASCII.GetBytes(sb.ToString());

            var stream = new MemoryStream(bytes);

            return stream;
        }

        private async Task<ExtractionRow> GetFileLevelDataExtract(string fileId)
        {
            var file = await _fileCollection.GetFileById(fileId);
            var mappings = await _mappingCollection.GetByFolderId(file.FolderId);

            ExtractionRow row = new ExtractionRow();

            var fileDomainItem = new FileDataDomain(file);
            var extractionItems = Extractor(mappings, fileDomainItem);

            foreach (var field in extractionItems)
            {
                row.Fields.Add(new ExtractionItem(field.Name, field.TextExtract, field.Type));
            }

            return row;
        }

        private async Task<List<ExtractionRow>> GetFolderLevelDataExtract(string folderId)
        {
            var mappings = await _mappingCollection.GetByFolderId(folderId);
            var fileData = await _fileCollection.GetFileListByFolder(folderId, false);

            List<ExtractionRow> rows = new List<ExtractionRow>();

            foreach (var fileItem in fileData)
            {
                ExtractionRow row = new ExtractionRow();

                var fileDomainItem = new FileDataDomain(fileItem);
                var extractionItems = Extractor(mappings, fileDomainItem);

                foreach (var field in extractionItems)
                {
                    row.Fields.Add(new ExtractionItem(field.Name, field.TextExtract, field.Type));
                }

                rows.Add(row);
            }

            return rows;

        }
    }

    public class ExtractionRow
    {
        public ExtractionRow()
        {
            Fields = new List<ExtractionItem>();
        }

        public List<ExtractionItem> Fields { get; set; }
    }

    public class ExtractionItem
    {

        public ExtractionItem(string fieldName, string value, FieldValueType type)
        {
            FieldName = fieldName;
            Type = type;
            FieldValue = value;
        }

        public string FieldName { get; set; }

        public string FieldValue { get; set; }

        public FieldValueType Type { get; set; }
    }

    public class FileListDataModel
    {
        public FileListDataModel(FileDataModel model)
        {
            Id = model.Id;
            FolderId = model.FolderId;
            Name = model.Name;
            IconFileType = model.IconFileType;
            Extension = model.Extension;
            DateCreated = model.DateCreated;
        }

        public string Id { get; set; }

        public string FolderId { get; set; }

        public string Name { get; set; }

        public IconFileType IconFileType { get; set; }

        public string Extension { get; set; }

        public DateTime DateCreated { get; set; }


    }

    public class FileDataDomain
    {
        public FileDataDomain(FileDataModel model)
        {
            var page = model.Pages.FirstOrDefault();
            Id = model.Id;
            Width = page.Width;
            Height = page.Height;
            PageNumber = page.PageNumber;
            PageCount = model.PageCount;
            Words = new List<FileDataDomainWordItem>();
            foreach (var word in page.WordItems)
            {
                Words.Add(new FileDataDomainWordItem(word));
            }
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public int PageNumber { get; set; }

        public int PageCount { get; set; }

        public IconFileType IconFileType { get; set; }

        public List<FileDataDomainWordItem> Words { get; set; }

        internal void Resize(decimal zoom)
        {
            float scale = (float)(zoom / 100m);

            if (scale == 1)
            {
                return;
            }

            Width = Width * scale;
            Height = Height * scale;

            foreach (var word in Words)
            {
                word.TextSize = (int)Math.Round(word.TextSize * scale);

                word.Box[1].X = (word.Box[1].X * scale);

                word.Box[2].X = word.Box[2].X * scale;
                word.Box[2].Y = word.Box[2].Y * scale;

                word.Box[3].Y = word.Box[3].Y * scale;


                var center = GetCentroid(word.Box);

                word.X = (decimal)center.X;
                word.Y = (decimal)center.Y;
            }

        }

        private PointF GetCentroid(List<VisionVerticie> poly)
        {
            float accumulatedArea = 0.0f;
            float centerX = 0.0f;
            float centerY = 0.0f;

            for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
            {
                float temp = poly[i].X * poly[j].Y - poly[j].X * poly[i].Y;
                accumulatedArea += temp;
                centerX += (poly[i].X + poly[j].X) * temp;
                centerY += (poly[i].Y + poly[j].Y) * temp;
            }

            if (Math.Abs(accumulatedArea) < 1E-7f)
                return PointF.Empty;  // Avoid division by zero

            accumulatedArea *= 3f;
            return new PointF(centerX / accumulatedArea, centerY / accumulatedArea);
        }


    }

    public class FileDataDomainWordItem
    {

        public FileDataDomainWordItem(WordItemDataModel model)
        {
            TextSize = model.TextSize;
            Text = model.Text;
            X = model.XAxis;
            Y = model.YAxis;
            FontStyle = model.FontStyle;
            Box = model.Vertices;
        }

        public string FontStyle { get; set; }

        public int TextSize { get; set; }

        public string Text { get; set; }

        public decimal X { get; set; }

        public decimal Y { get; set; }

        public List<VisionVerticie> Box { get; set; }
    }

    public class FolderDataDomain
    {
        public FolderDataDomain(FolderDataModel folder)
        {
            Id = folder.Id;
            Name = folder.Name;
            DateCreated = folder.DateCreated;
            UserId = folder.UserId;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public string UserId { get; set; }
    }

    public class MappingDomainItem
    {
        public MappingDomainItem(FileMappingItem data)
        {
            Id = data.Id;
            PageNumber = data.PageNumber;
            Items = data.Items;
            Type = data.Type;
            Name = data.Name;
            DateCreated = data.DateCreated;
        }

        public string Id { get; set; }

        public int PageNumber { get; set; }

        public List<VisionVerticie> Items { get; set; }

        public FieldValueType Type { get; set; }

        public string Name { get; set; }

        public string TextExtract { get; set; }

        public DateTime DateCreated { get; set; }
    }


}
