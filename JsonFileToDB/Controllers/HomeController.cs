using JsonFileToDB.Models;
using JsonFileToDB.Storage;
using Pearson.ContentCompression.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JsonFileToDB.Controllers
{
    public class HomeController : Controller
    {
        delegate void Delegate_CopyToAzure(int FileID);
        public ActionResult Index()
        {
            // new SQLAccess().saveDataToDB();

            string unoptimizedPathPrefix = "D:\\unoptimized";
            string optimizedPrefix = "D:\\optimized";
            var movetouncompress = new UnoptimizedDataManagement();
            movetouncompress.copyToUnOptimizedDirectory(unoptimizedPathPrefix, unoptimizedPathPrefix);

            movetouncompress.scanUnOptimized(unoptimizedPathPrefix, optimizedPrefix);

            movetouncompress.zipFolderList.AddRange(movetouncompress.nonZipFolderList);
            new SQLAccess().SaveDataToDB(movetouncompress.zipFolderList);
            return View();

        }

        public ActionResult About()
        {
            List<ComparisonViewModel> model = new List<ComparisonViewModel>();
            model = new SQLAccess().GetComparisonData();
            return View(model);
        }

        [HttpPost]
        public JsonResult GetComparisionData(string id)
        {
            var data=new SQLAccess().GetComparisonData();

            var result = (from record in data
                                select new
                                {
                                    id = record.FileId,
                                    UnoptimizedFile=record.UnOptimizedFilePath,
                                    OptimizedFile=record.OptimizedFilePath,
                                    FileStatus=record.FileStatus,
                                    IsZip=record.IsZipFile,
                                }
                            ).ToArray();


                // Send the data to the jQGrid
               var jsonData = new
                {
                    current= 1,
                    rowCount= 10,
                    rows = result,
                    total= result.Count()
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public string ChangeStatus(bool accept, int FileID)
        {
            int data = new SQLAccess().ChangeStatus(accept, FileID);
            Delegate_CopyToAzure async = new Delegate_CopyToAzure(new SQLAccess().CopyToAzure);
            async.BeginInvoke(FileID, null, null);
            string result = data == 1 ? "Pending" : data == 3 ? "Approved" : data == 2 ? "Rejected" : "Not Optimized";

            return result;
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}