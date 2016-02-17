
#region licence
// ======================================================================================
// Mvc5WithBowerAndGrunt - An example of how to change a MVC5 project to Bower and Grunt
// Filename: ReadBundleFile.cs
// Date Created: 2016/02/03
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// ======================================================================================
#endregion

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace B4BCore.Internal
{
    internal class ReadBundleFile
    {
        private readonly JObject _settings;

        /// <summary>
        /// This provides the list of property names that are present in the file settings file
        /// </summary>
        public ReadOnlyCollection<string> BundleNames { get; private set; }

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
        /// This returns an array of strings from the setting
        /// </summary>
        /// <param name="bundleName">The name of the setting</param>
        /// <param name="prefix">This string is added to the start of each string in the array</param>
        /// <returns></returns>
        public string[] GetSettingAsStringArray(string bundleName, string prefix = "~/")
        {
            var setting = _settings[bundleName];
            if (setting == null)
                throw new ArgumentException($"Could not find the setting {bundleName}");
            if (setting.Type != JTokenType.Array)
                throw new ArgumentException($"The setting [{bundleName}] is not an array");

            return setting.Values<string>().Select(x => prefix + x).ToArray();
        }
    }
}