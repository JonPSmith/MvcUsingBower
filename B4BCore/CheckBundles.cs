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
using System.Runtime.InteropServices;
using B4BCore.Internal;

namespace B4BCore
{
    /// <summary>
    /// This is a class that contains various tests to ensure that your bundles are correctly formed and
    /// they are up to date.
    /// </summary>
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
        public CheckBundles(Type classToFindProjectDirOf, string appDataDir = null,  bool checkForConcatFile = true)
            : this(GetProjectDirectoryFromType(classToFindProjectDirOf), appDataDir, checkForConcatFile) {}

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
            var allBundleDebugLines = _reader.GetBundleDebugFiles(bundleName, "", s => errors.Add(s));
            var allCdns = _reader.GetBundleCdnInfo(bundleName);
            if (errors.Any())
                return errors.AsReadOnly();

            if (!allCdns.Any())
                return CheckNonCdnBundle(bundleName, allBundleDebugLines).AsReadOnly();
            
            //It has Cdns 
            if (allBundleDebugLines.Count() != allCdns.Count())
                return new List<string>
                    { $"The Bundle called {bundleName} contained both cdn and non cdn entries, which is not supported." }
                    .AsReadOnly();

            return CheckCdnBundle(bundleName, allCdns).AsReadOnly();

        }

        /// <summary>
        /// This checks all the bundles in the bundle file are valid
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
            var errors = new List<string>();

            var bundleInfo = new FileWithDateUpdated(_mvcAppPath, _bundleFilePath);

            //Note: we exclude the cdn bundles as the minified file is not normally created by the build process
            var badFiles = (from bundleName in _reader.BundleNames.Where(x => !_reader.GetBundleCdnInfo(x).Any())
                let fileExtension = Path.GetExtension(_reader.GetBundleDebugFiles(bundleName, "", s => errors.Add(s)).First())
                let fileTypeInfo = _config.GetFileTypeData(fileExtension)
                let minfiedFile = new FileWithDateUpdated(_mvcAppPath, Path.Combine(fileTypeInfo.Directory,
                        bundleName + ".min" + fileExtension))
                where !minfiedFile.Exists || minfiedFile.LastWrittenUtc < bundleInfo.LastWrittenUtc
                select minfiedFile.FileRelPath).ToList();

            if (errors.Any())
                return $"Errors in the BowerBundles file: {string.Join("\n", errors)}.";

            return badFiles.Any()
                ? $"The following minified files have not been updated since the change in the bundle file:\n - {string.Join("\n - ", badFiles)}"
                : null;
        }

        //------------------------------------------------------------------
        //private methods

        private List<string> CheckCdnBundle(string bundleName, ICollection<CdnInfo> allCdns)
        {
            var errors = new List<string>();
            var productionFileName = allCdns.FirstOrDefault(x => x.Production != null)?.Production;
            if (productionFileName == null)
            {
                return new List<string> { $"In the CDN bundle called {bundleName} we need a property called {nameof(CdnInfo.Production)}"};
            }

            var fileExtension = Path.GetExtension(allCdns.First().Production);
            var fileTypeInfo = _config.GetFileTypeData(fileExtension);

            if (string.IsNullOrEmpty(fileTypeInfo.CdnHtmlFormatString))
            {
                return new List<string> { $"This configuration of BundlerForBower does not support CDN bundles for {fileExtension} files. Bad bundle is {bundleName}." };
            }

            var i = 0;
            foreach (var properties in allCdns.Select(cdnInfo => cdnInfo.FindMissingPropertiesNeededByHtmlInclude(fileTypeInfo.CdnHtmlFormatString).ToList()))
            {
                if (properties.Any())
                    errors.Add($"In the bundle called {bundleName}, array element {i}, the following properties were missing: {string.Join(", ", properties)}");
                i++;
            }
            if (errors.Any())
                return errors;

            AddErrorIfAnyFilesInBadFiles(bundleName, 
                allCdns.Where(x => x.Development != null && RelPathSearcher.ContainsSearchChars(x.Development))
                .Select(x => x.Development).ToList(), "had 'development' with search strings", errors);
            AddErrorIfAnyFilesInBadFiles(bundleName,
                allCdns.Where(x => RelPathSearcher.ContainsSearchChars(x.Production))
                .Select(x => x.Development).ToList(), "had 'production' with search strings", errors);

            if (errors.Any())
                //If any errors so far then not worth continuing
                return errors;

            var allDevelopmentWithDate = allCdns
                .Select(x => new FileWithDateUpdated(_mvcAppPath, x.Development)).ToList();

            AddErrorIfAnyFilesInBadFiles(bundleName,
                allDevelopmentWithDate.Where(x => !x.Exists).Select(x => x.FileRelPath).ToList(), "in 'development' were missing", errors);

            var allProductionWithDate = allCdns
                .Select(x => new FileWithDateUpdated(_mvcAppPath, Path.Combine(fileTypeInfo.Directory, x.Production))).ToList();

            AddErrorIfAnyFilesInBadFiles(bundleName,
                allProductionWithDate.Where(x => !x.Exists).Select(x => x.FileRelPath).ToList(), "in 'production' were missing", errors);

            return errors;
        }

        private List<string> CheckNonCdnBundle(string bundleName, IEnumerable<string> allBundleDebugLines)
        {
            var errors = new List<string>();
            var allFilesWithDate = _searcher.UnpackBundle(allBundleDebugLines, s => errors.Add(s))
                .Select(x => new FileWithDateUpdated(_mvcAppPath, x)).ToList();

            if (!allFilesWithDate.Any())
            {
                errors.Add($"The bundle called {bundleName} did not contain any files to work on.");
                return errors;
            }

            var fileExtension = Path.GetExtension(allFilesWithDate.First().FileRelPath);
            var fileTypeInfo = _config.GetFileTypeData(fileExtension);

            AddErrorIfAnyFilesInBadFiles(bundleName,
                allFilesWithDate.Where(x => !x.Exists).Select(x => x.FileRelPath).ToList(), "were missing", errors);

            FileWithDateUpdated concatFileInfo = null;
            if (_checkForConcatFile)
            {
                concatFileInfo = new FileWithDateUpdated(_mvcAppPath,
                    Path.Combine(_mvcAppPath, fileTypeInfo.Directory, bundleName + fileExtension));
                if (!concatFileInfo.Exists)
                {
                    errors.Add($"Warning: the concat file for '{bundleName}' is missing. Continuing test.");
                    concatFileInfo = null;
                }
            }
            var minfiedFileInfo =
                new FileWithDateUpdated(_mvcAppPath,
                    Path.Combine(_mvcAppPath, fileTypeInfo.Directory, bundleName + ".min" + fileExtension));
            if (!minfiedFileInfo.Exists)
            {
                errors.Add($"The minified file for '{bundleName}' is missing.");
                return errors;
            }

            var newerFiles =
                allFilesWithDate.Where(x => x.LastWrittenUtc > (concatFileInfo ?? minfiedFileInfo).LastWrittenUtc)
                    .Select(x => x.FileRelPath).ToList();
            if (newerFiles.Any())
            {
                var concatOrMinfied = concatFileInfo == null ? "minified" : "concat";
                errors.Add(
                    $"The {concatOrMinfied} file for '{bundleName}' is out of date. Newer files are:\n - {string.Join("\n - ", newerFiles)}");
                    
            }

            if (_checkForConcatFile && concatFileInfo != null && minfiedFileInfo.LastWrittenUtc < concatFileInfo.LastWrittenUtc)
                errors.Add($"The concat file for '{bundleName}' is newer than the minified file.");

            return errors;
        }



        private void AddErrorIfAnyFilesInBadFiles(string bundleName, IList<string> badFiles, string errorMessage, ICollection<string> errors)
        {
            if (badFiles.Any())
                errors.Add(
                    $"The following files {errorMessage} in the bundles called '{bundleName}':\n - {string.Join("\n - ", badFiles)}");
        }


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
                throw new InvalidOperationException($"Expected directory ending in {debugEnding} or {releaseEnding} but got {pathToManipulate}"+
                    "Please use one of the other forms of the CheckBundles ctor and provide the absolute path to the project.");

            return Path.Combine(projectDir.Substring(0, projectDir.LastIndexOf("\\", StringComparison.Ordinal)),
                classToFindProjectDirOf.Assembly.GetName().Name);
        }

    }
}