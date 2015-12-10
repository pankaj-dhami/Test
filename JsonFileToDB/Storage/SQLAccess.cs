using JsonFileToDB.Models;
using Newtonsoft.Json;
using Pearson.ContentCompression.Business;
using Pearson.ContentCompression.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace JsonFileToDB.Storage
{
    public class SQLAccess
    {
        Model ctx;
        public SQLAccess()
        {
            ctx = new Model();
            //  new ModelDBInitializer().InitializeDatabase(ctx);
        }

        public void SaveDataToDB(List<FoldersMetadata> FolderList)
        {

            foreach (var item in FolderList)
            {
                ctx.FoldersMetadata.Add(item);
            }
            ctx.SaveChanges();
        }

        public List<ComparisonViewModel> GetComparisonData()
        {
            var comparisonList = (from mfm in ctx.MediaFilesMetadata
                                  join fm in ctx.FoldersMetadata on mfm.FolderMetaData.ID equals fm.ID

                                  select new
                                  {
                                      Id = mfm.ID,
                                      LocalPath = fm.LocalPath,
                                      FileName = mfm.UnCompressedFilePath,
                                      IsZip = fm.IsZip,
                                      FolderStatus = fm.Status,
                                      FileStatus = mfm.Status

                                  }).AsEnumerable().Select(a => new

                                  ComparisonViewModel
                                  {
                                      FileId = a.Id,
                                      UnOptimizedFilePath = a.LocalPath + "\\" + a.FileName,
                                      OptimizedFilePath = a.LocalPath.Replace("unoptimized", "optimized") + "\\" + a.FileName,
                                      IsZipFile = a.IsZip,
                                      FileStatus = a.FileStatus == 1 ? "Pending" : a.FileStatus == 3 ? "Approved" : a.FileStatus == 2 ? "Rejected" : "Not Optimized"

                                  }).ToList();

            return comparisonList;
        }

        public int ChangeStatus(bool accept, int fileId)
        {
            var filesMetaData = (from item in ctx.MediaFilesMetadata
                                 where item.ID == fileId
                                 select item).FirstOrDefault();
            if (filesMetaData != null)
            {
                filesMetaData.Status = accept == true ? 3 : 2;
                ctx.Entry(filesMetaData).State = System.Data.Entity.EntityState.Modified;
                ctx.SaveChanges();
                return filesMetaData.Status;
            }
            else
            {
                return 0;
            }
        }

        public void CopyToAzure(int fileId)
        {

            var folderData = ctx.MediaFilesMetadata.Where(a => a.ID == fileId).Select(a => a.FolderMetaData).FirstOrDefault();

            if (folderData.IsZip)
            {
                var filesData = ctx.MediaFilesMetadata.Where(a => a.FolderMetaData.ID == folderData.ID).ToList();
                int approvedfileCount = filesData.Where(a => a.Status > (int)mediaFileStatusType.pending).Count();
                if (approvedfileCount == filesData.Count)
                {

                    //to do azure copy and create zip
                    folderData.Status = 1;
                    ctx.SaveChanges();
                    new OptimizedFileManagement().CreateZipAndCopyToFinal(filesData, folderData);

                }
            }
            else
            {

                // todo non zip items
               

                var filesData = ctx.MediaFilesMetadata.Where(a => a.FolderMetaData.ID == folderData.ID).ToList();
                int approvedfileCount = filesData.Where(a => a.Status > (int)mediaFileStatusType.pending).Count();
                if (approvedfileCount == filesData.Count)
                {
                    folderData.Status = 1;
                    ctx.SaveChanges();
                }
                new OptimizedFileManagement().CopyToFinal(filesData, folderData, fileId);
            }
        }

    }


}