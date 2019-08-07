using System.Collections.Generic;
using System.IO;
using System.Linq;
using FancyTree.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace FancyTree.WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileServiceController : ControllerBase
    {
        private static string[] DEFUALT_FOLDER = { "logs", "UploadFiles" };
        private static string[] DEFUALT_MIME_TYPE = { };
        private readonly IHostingEnvironment _env;

        public FileServiceController(IHostingEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        [Route("HeadList")]
        public JsonResult HeadList()
        {
            List<FileServiceModel> fileModel = new List<FileServiceModel>();

            var contentRoot = _env.ContentRootPath;

            var dirnames = from fullDirname in Directory.EnumerateDirectories(contentRoot)
                            select fullDirname.Substring(fullDirname.LastIndexOf(Path.DirectorySeparatorChar) + 1);

            foreach (var dir in dirnames.Where(a => DEFUALT_FOLDER.Contains(a)))
            {
                FileServiceModel model = new FileServiceModel();
                model.key = dir;
                model.title = dir;
                model.lazy = true;
                model.folder = true;
                fileModel.Add(model);
            }

            //var filenames = from fullFilename in Directory.EnumerateFiles(contentRoot, "*")
            //                select Path.GetFileName(fullFilename);

            //foreach (string file in filenames)
            //{
            //    FileServiceModel model = new FileServiceModel();
            //    model.key = file;
            //    model.title = file;
            //    model.lazy = false;
            //    model.folder = false;
            //    fileModel.Add(model);
            //}

            return new JsonResult(fileModel);
        }

        [HttpGet]
        [Route("NodeList")]
        public JsonResult NodeList(string path)
        {
            List<FileServiceModel> fileModel = new List<FileServiceModel>();

            var contentRoot = _env.ContentRootPath;
            var searchPath = Path.Combine(contentRoot, path);

            var dirnames = from fullDirname in Directory.EnumerateDirectories(searchPath)
                select fullDirname.Substring(fullDirname.LastIndexOf(Path.DirectorySeparatorChar) + 1);

            foreach (var dir in dirnames)
            {
                FileServiceModel model = new FileServiceModel();
                model.key = path + "/" + dir;
                model.title = dir;
                model.lazy = true;
                model.folder = true;
                fileModel.Add(model);
            }

            var filenames = from fullFilename in Directory.EnumerateFiles(searchPath, "*")
                select Path.GetFileName(fullFilename);

            foreach (string file in filenames)
            {
                FileServiceModel model = new FileServiceModel();
                model.key = path + "/" + file;
                model.title = file;
                model.lazy = false;
                model.folder = false;
                fileModel.Add(model);
            }

            return new JsonResult(fileModel);
        }

        [HttpGet]
        [Route("DownloadFile")]
        public ActionResult DownloadFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            var contentRoot = _env.ContentRootPath;
            var pathFile = Path.Combine(contentRoot, path);

            if (!System.IO.File.Exists(pathFile))
            {
                return null;
            }

            var fileStream = new FileStream(pathFile, FileMode.Open, FileAccess.Read);
            var result = new FileStreamResult(fileStream, "application/octet-stream");
            result.FileDownloadName = Path.GetFileName(pathFile);
            return result;
        }
    }
}