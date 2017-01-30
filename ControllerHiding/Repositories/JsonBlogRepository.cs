using System;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using ControllerHiding.DTO;

namespace ControllerHiding.Repositories
{
    public class JsonBlogRepository
    {
        private const string JsonFileName = "BlogEntries.js";
        private BlogEntry[] _blogEntries;

        protected BlogEntry[] BlogEntries()
        {
            if (_blogEntries != null)
            {
                return _blogEntries;
            }

            var jsonEntries = JsonEntries();
            return _blogEntries = new JavaScriptSerializer().Deserialize<BlogEntry[]>(jsonEntries);
        }

        private string JsonEntries()
        {
            var appdatafolder = GetAppDataPath();
            var filePath = Path.Combine(appdatafolder, JsonFileName);

            return File.ReadAllText(filePath);
        }

        private static string GetAppDataPath()
        {
            var physicalApplicationPath = HttpContext.Current.Request.PhysicalApplicationPath;
            if (physicalApplicationPath != null)
            {
                return Path.Combine(physicalApplicationPath, "App_Data");
            }
            throw new Exception("Could not receive PhysicalApplicationPath!");
        }
    }
}