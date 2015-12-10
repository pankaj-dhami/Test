using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JsonFileToDB.Models
{
    public class ComparisonViewModel
    {
        public int FileId { get; set; }
        public string UnOptimizedFilePath { get; set; }
        public string OptimizedFilePath { get; set; }
        public string FileStatus { get; set; }
        public bool IsZipFile { get; set; }
    }
}