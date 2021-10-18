using Simparse.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simparse.Models.File
{
    public class SaveMappingsRequest
    {
        public List<FileMappingItem> Items { get; set; }
    }
}
