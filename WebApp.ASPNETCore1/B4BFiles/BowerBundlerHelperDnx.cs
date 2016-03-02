#region licence
// ======================================================================================
// MvcUsingBower - An example+library to allow an MVC project to use Bower and Grunt
// Filename: BowerBundlerHelper.cs
// Date Created: 2016/02/19
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// ======================================================================================
#endregion

using System;
using System.Collections.Concurrent;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;

namespace WebApp.ASPNETCore1.B4BFiles
{ 
    /// <summary>
    /// COPY OF enum from original BowerBundlerHelper
    /// </summary>
    public enum CssOrJs { Css, Js }

    /// <summary>
    /// This class contains extention methods to provide a the inclusings of CSS and JavaScript files in your views
    /// 1) @Html.HtmlCssCached("bundleName") is equivalent to @Styles.Render("~/Content/bundleName")
    /// 2) @Html.HtmlScriptsCached("bundleName") is equivalent to @Scripts.Render("~/bundles/bundleName")
    /// Whether it delivers individual files or the single minfied file that Grunt produced is defined by
    /// whether the code was compiled in DEBUG mode or not. This can be overridden on each method by the 
    /// optional 'forceState' parameter.
    /// 
    /// Both these methods have some small limitations:
    /// 1. These methods can do searches for files, e.g. ~/Scripts/*.js, or ~/Scripts/*/*.js 
    ///    However it does not support Grunt/Gulp's /**/ search all directories and subdirectories feature.
    ///    It is something that could be implemented, but I left it out for now. It throws an NotImplementedException if found
    /// 2. I suspect that the .NET search and the Grunt/Gulp search might have slightly different rules. 
    ///    see the Note under Remarks about a search like *.js https://msdn.microsoft.com/en-us/library/wz42302f%28v=vs.110%29.aspx
    /// 3. BundlerHelper has the same problem (feature) as BundleConfig. If you change or add files to the settings.json file 
    ///    then you need to rebuild your application because HtmlCssCached and HtmlCssCached deliver from a static cache. 
    ///    You can turn off caching by setting the forceState parameter in each methods to true.
    /// Finally it is very tempting to add comments to the settings.json file. While Json.Net handles comments Grunt doesn't, and fails silently!
    /// </summary>
    public static class BowerBundlerHelperDnx
    {
        private static readonly ConcurrentDictionary<string, HtmlString> IncludeCache = new ConcurrentDictionary<string, HtmlString>();

        /// <summary>
        /// This returns the CSS links using caching if forceState is null
        /// Note: this assumes that the CSS minified file is in the directory "~/css/"
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="bundleName">The name of the setting.json property containing the list of Css file to include. 
        /// defaults to main Css file</param>
        /// <param name="forceState">if not null then true forces into debug state and false forces production state</param>
        /// <returns></returns>
        public static HtmlString HtmlCssCached(this IUrlHelper urlHelper, string bundleName, bool? forceState = null)
        {
            return (forceState == null)
                ? IncludeCache.GetOrAdd(bundleName, setup => CreateHtmlIncludes(urlHelper, bundleName, CssOrJs.Css, forceState))
                : CreateHtmlIncludes(urlHelper, bundleName, CssOrJs.Css, forceState);
        }

        /// <summary>
        /// This returns the script includes for a specific group using caching if forceState is null
        /// Note: this assumes that the JavaScript minified file is in the directory "~/js/"
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="bundleName">The name of the setting.json property containing the list of JavaScript file to include. 
        /// defaults to main js file</param>
        /// <param name="forceState">if not null then true forces into debug state and false forces production state</param>
        /// <returns></returns>
        public static HtmlString HtmlScriptsCached(this IUrlHelper urlHelper, string bundleName, bool? forceState = null)
        {
            return (forceState == null)
                ? IncludeCache.GetOrAdd(bundleName, setup => CreateHtmlIncludes(urlHelper, bundleName, CssOrJs.Js, forceState))
                : CreateHtmlIncludes(urlHelper, bundleName, CssOrJs.Js, forceState);
        }

        //---------------------------------------
        //private methods

        /// <summary>
        /// This returns the html to include either CSS or JavaScript files
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="bundleName">The name of the bundle property and the name of the minified file</param>
        /// <param name="cssOrJs">This says if its css or javascript. NOTE: the enum string is used as the dir and the file type</param>
        /// <param name="forceState">if not null then true forces into debug state and false forces production state</param>
        /// <returns></returns>
        private static HtmlString CreateHtmlIncludes(this IUrlHelper urlHelper, string bundleName, CssOrJs cssOrJs, bool? forceState = null)
        {
            var isDebug = false;
#if DEBUG
            isDebug = true;
#endif
            if (forceState != null)
                isDebug = (bool)forceState;

            throw new NotImplementedException();
            //var bundler = new BundlerForBowerDnx(GetAbsDataDirectory(), urlHelper.Content);
            //return new HtmlString(bundler.CalculateHtmlIncludes(bundleName, cssOrJs, isDebug));
        }

        /// <summary>
        /// This is equivalent to MVC5's AppDomain.CurrentDomain.GetData("DataDirectory").ToString()
        /// This gets the absolute path to the directory containing the B4B files
        /// </summary>
        /// <returns></returns>
        private static string GetAbsDataDirectory()
        {
            return null;
        }
    }
}
