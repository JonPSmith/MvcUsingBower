#region licence
// ======================================================================================
// Mvc5UsingBower - An example+library to allow an MVC project to use Bower and Grunt
// Filename: ConfigInfo.cs
// Date Created: 2016/02/17
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// ======================================================================================
#endregion

using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[assembly: InternalsVisibleTo("Tests")]

namespace B4BCore.Internal
{
    internal class ConfigInfo
    {
        /// <summary>
        /// Name of the json file that has the various bundle information
        /// </summary>
        public string BundlesFileName { get; set; }

        /// <summary>
        /// This holds the Directory path of where the concatenated and minified files are
        /// </summary>
        public string JsDirectory { get; set; }

        /// <summary>
        /// This holds the html to include the given file in the page when in debug mode 
        /// It contains one parameter called {fileUrl}, which takes the name of the file to include
        /// </summary>
        public string JsDebugHtmlFormatString { get; set; }

        /// <summary>
        /// This holds the html to include the given file in the page when in non-debug mode
        /// It contains two parameters:
        /// {fileUrl} which takes the name of the file to include
        /// an optional {cachebuster} property that, if present, places a cachebuster value in the 
        /// </summary>
        public string JsNonDebugHtmlFormatString { get; set; }

        /// <summary>
        /// This holds the html to include the given file in the page when in non-debug mode
        /// It contains four parameters:
        /// {cdnUrl} which is the url to try to get the file from
        /// {cdnSuccessTest} which is a JavaScript test which returns true if cnd loaded successfully
        /// {fileUrl} which takes the name of the file to include if the cdnSuccessTest fails
        /// an optional {cachebuster} property that, if present, places a cachebuster value on the non-cdn file
        /// </summary>
        public string JsCdnHtmlFormatString { get; set; }

        /// <summary>
        /// This holds the Directory path of where the concatenated and minified files are
        /// </summary>
        public string CssDirectory { get; set; }

        /// <summary>
        /// This holds the html to include the given file in the page when in debug mode 
        /// It contains one parameter called {fileUrl}, which takes the name of the file to include
        /// </summary>
        public string CssDebugHtmlFormatString { get; set; }

        /// <summary>
        /// This holds the html to include the given file in the page when in non-debug mode
        /// It contains two parameters:
        /// {fileUrl} which takes the name of the file to include
        /// an optional {cachebuster} property that, if present, places a cachebuster value in the 
        /// </summary>
        public string CssNonDebugHtmlFormatString { get; set; }

        /// <summary>
        /// This holds the html to include the given file in the page when in non-debug mode
        /// It contains four parameters:
        /// {cdnUrl} which is the url to try to get the file from
        /// {cdnSuccessTest} which is a JavaScript test which returns true if cnd loaded successfully
        /// {fileUrl} which takes the name of the file to include if the cdnSuccessTest fails
        /// an optional {cachebuster} property that, if present, places a cachebuster value on the non-cdn file
        /// </summary>
        public string CssCdnHtmlFormatString { get; set; }


        public FileTypeConfigInfo GetFileTypeData(CssOrJs cssOrJs)
        {
            return cssOrJs == CssOrJs.Css
                ? new FileTypeConfigInfo(CssDirectory, CssDebugHtmlFormatString, CssNonDebugHtmlFormatString, CssCdnHtmlFormatString)
                : new FileTypeConfigInfo(JsDirectory, JsDebugHtmlFormatString, JsNonDebugHtmlFormatString, JsCdnHtmlFormatString);
        }

        public FileTypeConfigInfo GetFileTypeData(string fileExtension)
        {
            if (fileExtension == null)
                throw new ArgumentNullException(nameof(fileExtension));
            if (!fileExtension.StartsWith("."))
                throw new InvalidOperationException("You must supply a file extension.");
            return GetFileTypeData(fileExtension.ToLowerInvariant() == ".css" ? CssOrJs.Css : CssOrJs.Js);
        }

        public override string ToString()
        {
            return $"BundlesFileName: {BundlesFileName}, JsDirectory: {JsDirectory}, JsDebugHtmlFormatString: {JsDebugHtmlFormatString}, JsNonDebugHtmlFormatString: {JsNonDebugHtmlFormatString}, JsCdnHtmlFormatString: {JsCdnHtmlFormatString}, CssDirectory: {CssDirectory}, CssDebugHtmlFormatString: {CssDebugHtmlFormatString}, CssNonDebugHtmlFormatString: {CssNonDebugHtmlFormatString}, CssCdnHtmlFormatString: {CssCdnHtmlFormatString}";
        }


        //-----------------------------------------------------------------------
        //public static

        /// <summary>
        /// This returns the config for the BundlerForBower.
        /// It forms this by reading the default setting from the manifest and then merges in any user config file, if present
        /// </summary>
        /// <param name="configFilePath"></param>
        /// <returns></returns>
        public static ConfigInfo ReadConfig(string configFilePath)
        {
            var defaultConfigJsonString = ReadDefaultConfigFileFromAssembly();

            if (configFilePath == null || !File.Exists(configFilePath))
                return JsonConvert.DeserializeObject<ConfigInfo>(defaultConfigJsonString);

            var defaultConfigJObject = JObject.Parse(defaultConfigJsonString);
            var userConfigJObject = JObject.Parse(File.ReadAllText(configFilePath));

            defaultConfigJObject.Merge(userConfigJObject);

            return defaultConfigJObject.ToObject<ConfigInfo>();
        }

        //-----------------------------------------------------------------
        //private

        private static string ReadDefaultConfigFileFromAssembly()
        {
            var assembly = Assembly.GetAssembly(typeof(ConfigInfo));
            var resourceName = assembly.GetName().Name + ".Internal.defaultConfig.json";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}