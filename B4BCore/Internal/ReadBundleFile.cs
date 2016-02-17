#region licence
// ======================================================================================
// Mvc5UsingBower - An example+library to allow an MVC project to use Bower and Grunt
// Filename: ReadBundleFile.cs
// Date Created: 2016/02/17
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// ======================================================================================
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace B4BCore.Internal
{
    internal class ReadBundleFile
    {
        private readonly JObject _settings;

        public ReadBundleFile(string bundleFilePath)
        {
            if (bundleFilePath == null)
                throw new ArgumentNullException(nameof(bundleFilePath));
            if (Path.GetExtension(bundleFilePath) != ".json")
                throw new ArgumentException("This needs a json file to read.");
            if (!File.Exists(bundleFilePath))
                throw new FileNotFoundException(
                    $"We could not find the bundle file using the file path {bundleFilePath}");

            _settings = JObject.Parse(File.ReadAllText(bundleFilePath));
            BundleNames = _settings.Properties().Select(x => x.Name).ToList().AsReadOnly();
        }

        /// <summary>
        /// This provides the list of property names that are present in the file settings file
        /// </summary>
        public ReadOnlyCollection<string> BundleNames { get; private set; }

        /// <summary>
        /// This returns an array of strings from the setting
        /// </summary>
        /// <param name="bundleName">The name of the setting</param>
        /// <param name="prefix">This string is added to the start of each string in the array</param>
        /// <param name="errorHandler">used by CheckBundles to get any errors rather than throwing exception</param>
        /// <returns></returns>
        public IEnumerable<string> GetBundleDebugFiles(string bundleName, string prefix = "~/", Action<string> errorHandler = null)
        {
            if (errorHandler == null) errorHandler = s => { throw new InvalidOperationException(s); };

            var bundle = _settings[bundleName];
            if (bundle == null)
                throw new ArgumentException($"Could not find the setting {bundleName}");
            if (bundle.Type != JTokenType.Array)
                throw new ArgumentException($"The setting [{bundleName}] is not an array");

            var result = new List<string>();
            var i = 0;
            foreach (var item in bundle)
            {
                switch (item.Type)
                {
                    case JTokenType.String:
                        result.Add(item.Value<string>());
                        break;
                    case JTokenType.Object:
                        var cdnInfo = new CdnInfo(bundleName, item.Value<JObject>());
                        if (cdnInfo.Development == null)
                        {
                            errorHandler(
                                $"The CDN bundle {bundleName}, array element {i}, is missing a property called '{CdnInfo.CdnObjectDevelopmentPropertyName}'.");
                        }
                        else
                        {
                            result.Add(cdnInfo.Development);
                        }
                        break;
                    default:
                        errorHandler($"The CDN bundle {bundleName}, array element {i}, contained an invalid type {item.Type}");
                        break;
                }
                i++;
            }

            return string.IsNullOrEmpty(prefix) ? result : result.Select(x => prefix + x);
        }


        /// <summary>
        /// This returns any Content Management Information in the bundle
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public ICollection<CdnInfo> GetBundleCdnInfo(string bundleName)
        {
            var bundle = _settings[bundleName];
            if (bundle == null)
                throw new ArgumentException($"Could not find the setting {bundleName}");
            if (bundle.Type != JTokenType.Array)
                throw new ArgumentException($"The setting [{bundleName}] is not an array");

            return bundle.Where(x => x.Type == JTokenType.Object).Select(x => new CdnInfo(bundleName, x.Value<JObject>())).ToList();
        }
    }
}