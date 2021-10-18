using Simparse.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simparse.Models.File
{
    public class FolderDataViewModel
    {
        public FolderDataViewModel(FolderDataDomain item)
        {
            Id = item.Id;
            Name = item.Name;
            DateCreated = item.DateCreated;
            UserId = item.UserId;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime DateCreated { get; set; }

        //UI helper
        public bool IsDirectory { get { return true; } }

        public string UserId { get; set; }

    }
}
