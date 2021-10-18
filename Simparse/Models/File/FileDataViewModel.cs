using Simparse.Domain;
using Simparse.Enums;
using Simparse.Vision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simparse.Models.Words
{
    public class FileDataViewModel
    {

        public FileDataViewModel(FileDataDomain domain)
        {
            Width = (int)Math.Round(domain.Width);
            Height = (int)Math.Round(domain.Height);
            PageCount = domain.PageCount;
            Name = domain.Name;
            FileIconType = domain.IconFileType;
            Items = new List<FileDataItemViewModel>();
            domain.Words.ForEach(word => Items.Add(new FileDataItemViewModel(word)));
            Id = domain.Id;
        }

        public FileDataViewModel(FileListDataModel model)
        {
            Id = model.Id;
            Name = model.Name;
            FileIconType = model.IconFileType;
        }

        public string Id { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int PageCount { get; set; }

        public string Name { get; set; }
        public IconFileType FileIconType { get; set; }

        public List<FileDataItemViewModel> Items { get; set; }
    }

    public class FileDataItemViewModel
    {
        public FileDataItemViewModel(FileDataDomainWordItem item)
        {
            Size = item.TextSize;
            Text = item.Text;
            Font = item.FontStyle;
            X = decimal.ToInt32(item.X);
            Y = decimal.ToInt32(item.Y);
            Height = item.TextSize;
            Verts = item.Box;
        }

        public int Size { get; set; }


        public string Text { get; set; }

        public string Font { get; set; }

        public decimal Height { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public List<VisionVerticie> Verts { get; set; }
    }
}
