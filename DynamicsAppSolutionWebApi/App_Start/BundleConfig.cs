using System.Web;
using System.Web.Optimization;

namespace DynamicsAppSolutionWebApi
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            #region Layout
            bundles.Add(new StyleBundle("~/Content/css/layout").Include(
                    "~/Content/css/layout/layout-style.css",
                    "~/Content/css/layout/left-fixed-menu-bar-style.css"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/js/layout").Include(
                  "~/Scripts/jquery-{version}.js",
                  "~/Content/js/layout/left-menu.js",
                  "~/Content/js/layout/layout.js"
                  ));
            #endregion

            #region Home
            bundles.Add(new StyleBundle("~/Content/css/home").Include(
                 "~/Content/css/home/right-fixed-menu-bar-style.css",
                 "~/Content/css/home/home-page-style.css"
                 ));
            bundles.Add(new ScriptBundle("~/bundles/js/home").Include(
                   "~/Scripts/jquery-{version}.js",
                   "~/Content/js/home/home-page.js"
                   ));
            #endregion

            #region worklist
            bundles.Add(new ScriptBundle("~/bundles/js/worklist").Include(
                   "~/Scripts/jquery-{version}.js",
                  "~/Content/js/worklist/worklist.js"
                  ));
            #endregion
        }
    }
}
