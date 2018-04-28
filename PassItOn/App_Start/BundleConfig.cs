using System.Web.Optimization;

namespace PassItOn
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/bootstrap-lumen.css",
                      "~/Content/css/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/appbootstrap").Include(
                      "~/Scripts/jquery-1.11.3.min.js",
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/bootbox.js",
                      "~/Scripts/toastr.js",
                      "~/Scripts/plugin.js",
                      "~/Scripts/variable.js",
                      "~/Scripts/map.js",
                      "~/Scripts/main.js",
                      "~/Scripts/demo.js",
                      "~/Scripts/app.js"));

            bundles.Add(new StyleBundle("~/Content/appcss").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/vendor.css",
                      "~/Content/toastr.css",
                      "~/Content/css/style.css",
                      "~/Content/css/demo.css",
                      "~/Content/css/custom.css"));

            bundles.Add(new StyleBundle("~/Bundles/css")
                .Include("~/admin-lte/css/bootstrap.css")
                .Include("~/admin-lte/css/select2.css")
                .Include("~/admin-lte/css/datepicker3.css")
                .Include("~/Content/datatables/css/dataTables.bootstrap.css")
                .Include("~/Content/toastr.css")
                .Include("~/admin-lte/css/AdminLTE.min.css")
                .Include("~/admin-lte/css/skins/skin-blue.css"));


            bundles.Add(new ScriptBundle("~/Bundles/js")
                .Include("~/admin-lte/js/plugins/jquery/jquery-2.2.4.min.js")
                .Include("~/admin-lte/js/plugins/bootstrap/bootstrap.min.js")
                .Include("~/Scripts/datatables/jquery.dataTables.js")
                .Include("~/Scripts/datatables/dataTables.bootstrap.js")
                .Include("~/admin-lte/js/plugins/fastclick/fastclick.js")
                .Include("~/admin-lte/js/plugins/slimscroll/jquery.slimscroll.js")
                .Include("~/admin-lte/js/plugins/select2/select2.full.js")
                .Include("~/admin-lte/js/plugins/moment/moment.js")
                .Include("~/admin-lte/js/plugins/datepicker/bootstrap-datepicker.js")
                .Include("~/admin-lte/js/plugins/icheck/icheck.js")
                .Include("~/admin-lte/js/plugins/validator.js")
                .Include("~/admin-lte/js/plugins/inputmask/jquery.inputmask.bundle.js")
                .Include("~/Scripts/bootbox.js")
                .Include("~/Scripts/toastr.js")
                .Include("~/admin-lte/js/app.js")
                .Include("~/admin-lte/js/init.js"));
        }
    }
}
