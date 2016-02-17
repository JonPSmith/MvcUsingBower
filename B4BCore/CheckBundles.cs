#region licence
// ======================================================================================
// Mvc5WithBowerAndGrunt - An example of how to change a MVC5 project to Bower and Grunt
// Filename: CheckBundles.cs
// Date Created: 2016/02/03
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
using B4BCore.Internal;

namespace B4BCore
{
    public class CheckBundles
    {
        private const string DefaultAppDataDirName = "App_Data\\";

        private readonly string _mvcAppPath;
        private readonly bool _checkForConcatFile;
        private readonly string _bundleFilePath;
        private readonly ConfigInfo _config;
        private readonly ReadBundleFile _reader;
        private readonly RelPathSearcher _searcher;

        /// <summary>
        /// This will setup checks the BundlesForBower in the assembly that the given type is in.
        /// It uses the data directory to find the BowerBundles.json and the bundlerForBower.json user config file
        /// </summary>
        /// <param name="classToFindProjectDirOf"></param>
        /// <param name="appDataDir">optional: if App_Data directory is not at the top level then you need to supply
        /// AppDomain.CurrentDomain.GetData("DataDirectory").ToString() or an equivalent absolute directory path</param>
        /// <param name="checkForConcatFile">optional: by default it checks the concat file, 
        /// but if you go straight to the minified file then set this to false</param>
        public CheckBundles(Type classToFindProjectDirOf, string appDataDir = null, bool checkForConcatFile = true)
            : this(GetProjectDirectoryFromType(classToFindProjectDirOf), appDataDir, checkForConcatFile)
        { }

        /// <summary>
        /// This requires the path to the directory of the project where all the files are stored
        /// It uses the data directory to find the BowerBundles.json and the bundlerForBower.json user config file
        /// </summary>
        /// <param name="mvcAppPath">The absolute path to the project you want to test.</param>
        /// <param name="appDataDir">optional: if App_Data directory is not at the top level then you need to supply
        /// AppDomain.CurrentDomain.GetData("DataDirectory").ToString() or an equivalent absolute directory path</param>
        /// <param name="checkForConcatFile">optional: by default it checks the concat file, 
        /// but if you go straight to the minified file then set this to false</param>
        public CheckBundles(string mvcAppPath, string appDataDir = null, bool checkForConcatFile = true)
        {
            _mvcAppPath = mvcAppPath;
            _checkForConcatFile = checkForConcatFile;
            var appDataPath = appDataDir ?? Path.Combine(_mvcAppPath, DefaultAppDataDirName);
            _config = ConfigInfo.ReadConfig(Path.Combine(appDataPath, BundlerForBower.B4BConfigFileName));
            _bundleFilePath = Path.Combine(appDataPath, _config.BundlesFileName);
            _reader = new ReadBundleFile(_bundleFilePath);
            _searcher = new RelPathSearcher(s => Path.Combine(_mvcAppPath, s));
        }


        /// <summary>
        /// Thic checks a specific bundle by name
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns>empty list if no error, otherwise a list of errors</returns>
        public ReadOnlyCollection<string> CheckSingleBundleIsValid(string bundleName)
        {
            var errors = new List<string>();
            var allFilesWithDate = AllFilesWithDate(bundleName, s => errors.Add(s));

            if (!allFilesWithDate.Any())
            {
                errors.Add($"The bundle called {bundleName} did not contain any files to work on.");
                return errors.AsReadOnly();
            }

            var fileExtension = Path.GetExtension(allFilesWithDate.First().FileRelPath);
            var fileTypeInfo = _config.GetFileTypeData(fileExtension);

            var missingFiles = allFilesWithDate.Where(x => !x.Exists).Select(x => x.FileRelPath).ToList();
            if (missingFiles.Any())
            {
                errors.Add(
                    $"The following files were missing for the bundles called '{bundleName}':\n - {string.Join("\n - ", missingFiles)}");
                return errors.AsReadOnly();
            }

            FileWithDateUpdated concatFileInfo = null;
            if (_checkForConcatFile)
            {
                concatFileInfo = new FileWithDateUpdated(_mvcAppPath, Path.Combine(_mvcAppPath, fileTypeInfo.Directory, bundleName + fileExtension));
                if (!concatFileInfo.Exists)
                {
                    errors.Add($"Warning: the concat file '{bundleName}' is missing. Continuing test.");
                    concatFileInfo = null;
                }
            }
            var minfiedFileInfo =
                new FileWithDateUpdated(_mvcAppPath, Path.Combine(_mvcAppPath, fileTypeInfo.Directory, bundleName + ".min" + fileExtension));
            if (!minfiedFileInfo.Exists)
            {
                errors.Add($"The minified file '{bundleName}' is missing");
                return errors.AsReadOnly();
            }

            var newerThanConcat = allFilesWithDate.Where(x => x.LastWrittenUtc > (concatFileInfo ?? minfiedFileInfo).LastWrittenUtc)
                .Select(x => x.FileRelPath).ToList();
            if (newerThanConcat.Any())
                errors.Add(
                    $"The concat file '{bundleName}' is out of date. Newer files are:\n - {string.Join("\n - ", newerThanConcat)}");

            if (_checkForConcatFile && concatFileInfo != null && minfiedFileInfo.LastWrittenUtc < concatFileInfo.LastWrittenUtc)
                errors.Add($"The minified file '{bundleName}' is out of date");

            return errors.AsReadOnly();
        }

        /// <summary>
        /// This checks all the bundles 
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<string> CheckAllBundlesAreValid()
        {
            var errors = new List<string>();
            foreach (var bundleName in _reader.BundleNames)
            {
                errors.AddRange(CheckSingleBundleIsValid(bundleName));
            }

            return errors.AsReadOnly();
        }

        /// <summary>
        /// This checks that the minified files for each bundle is older than the 
        /// BowerBundles.json file that defines what is in a bundle.
        /// </summary>
        /// <returns></returns>
        public string CheckBundleFileIsNotNewerThanMinifiedFiles()
        {
            var bundleInfo = new FileWithDateUpdated(_mvcAppPath, _bundleFilePath);
            var badFiles = (from bundleName in _reader.BundleNames
                            let fileExtension = Path.GetExtension(_reader.GetSettingAsStringArray(bundleName, "").First())
                            let fileTypeInfo = _config.GetFileTypeData(fileExtension)
                            let minfiedFile = new FileWithDateUpdated(_mvcAppPath, Path.Combine(fileTypeInfo.Directory,
                                    bundleName + ".min" + fileExtension))
                            where !minfiedFile.Exists || minfiedFile.LastWrittenUtc < bundleInfo.LastWrittenUtc
                            select minfiedFile.FileRelPath).ToList();

            return badFiles.Any()
                ? $"The following minified files have not been updated since the change in the bundle file:\n - {string.Join("\n - ", badFiles)}"
                : null;
        }

        //------------------------------------------------------------------
        //private methods

        private static string GetProjectDirectoryFromType(Type classToFindProjectDirOf)
        {
            const string debugEnding = @"\bin\debug";
            const string releaseEnding = @"\bin\release";

            var pathToManipulate = Environment.CurrentDirectory;

            string projectDir = null;
            if (pathToManipulate.EndsWith(debugEnding, StringComparison.InvariantCultureIgnoreCase))
                projectDir = pathToManipulate.Substring(0, pathToManipulate.Length - debugEnding.Length);
            if (pathToManipulate.EndsWith(releaseEnding, StringComparison.InvariantCultureIgnoreCase))
                projectDir = pathToManipulate.Substring(0, pathToManipulate.Length - releaseEnding.Length);

            if (projectDir == null)
                throw new InvalidOperationException($"Expected directory ending in {debugEnding} or {releaseEnding} but got {pathToManipulate}" +
                    "Please use one of the other forms of the CheckBundles ctor and provide the absolute path to the project.");

            return Path.Combine(projectDir.Substring(0, projectDir.LastIndexOf("\\", StringComparison.Ordinal)),
                classToFindProjectDirOf.Assembly.GetName().Name);
        }

        private List<FileWithDateUpdated> AllFilesWithDate(string bundleName, Action<string> errorHandler)
        {
            return _searcher.UnpackBundle(_reader.GetSettingAsStringArray(bundleName, ""), errorHandler)
                .Select(x => new FileWithDateUpdated(_mvcAppPath, x)).ToList();
        }

    }
}