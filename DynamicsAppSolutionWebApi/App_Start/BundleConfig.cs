using DynamicsAppSolutionWebApi.Enums;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Optimization;

namespace DynamicsAppSolutionWebApi
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            AddToBundles(bundles, ContentStatus.css);
            AddToBundles(bundles, ContentStatus.js);
        }

        private static void AddToBundles(BundleCollection bundles, ContentStatus contentStatus)
        {
            // Get Domain Path
            var currentDirectory = HttpRuntime.AppDomainAppPath + "\\Content";

            // Get content folder. Depends on content status
            var directories = Directory.GetDirectories(currentDirectory).Where(x => x.Contains(contentStatus.ToString()));

            foreach (var folder in directories)
            {
                var subFolders = Directory.GetDirectories(folder);

                // Get content sub folders
                foreach (var subFolder in subFolders)
                {
                    string subFolderName = subFolder.Split('\\').Last();

                    // Create bundle
                    Bundle bundle = null;
                    if (contentStatus.ToString() == "css")
                    {
                        bundle = new Bundle("~/Content/" + contentStatus.ToString() + "/" + subFolderName + "");
                    }
                    else if (contentStatus.ToString() == "js")
                    {
                        bundle = new Bundle("~/bundles/" + contentStatus.ToString() + "/" + subFolderName + "");
                    }

                    // Get sub folder files and include to bundle
                    foreach (var files in Directory.GetFiles(subFolder))
                    {
                        string fileName = files.Split('\\').Last();
                        bundle.Include("~/Content/" + contentStatus.ToString() + "/" + subFolderName + "/" + fileName + "");
                    }

                    // Add to bundles
                    bundles.Add(bundle);
                }
            }
        }
    }
}
