namespace Simparse.Models.File
{
    internal class FileUploadRequest
    {
        public int TotalCount { get; set; }
        public string FileName { get; set; }
        public int Index { get; set; }
        public string FolderId { get; set; }

    }
}