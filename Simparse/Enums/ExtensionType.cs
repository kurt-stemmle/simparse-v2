using System;
using System.Collections.Generic;
using System.Text;

namespace Simparse.Enums
{
    public enum ExtensionType
    {
        Unsupported = 0,
        PNG = 1,
        JPEG = 2,
        TIFF = 3,
        PDF = 4,
        ZIP = 5,
        Word = 6,
        Excel = 7,
        CSV = 8,
        TXT = 9,
        JSON = 10,
    }

    public enum IconFileType
    {
        None = 0,
        Image = 1,
        PDF = 2
    }
}
