#region licence
// ======================================================================================
// Mvc5UsingBower - An example+library to allow an MVC project to use Bower and Grunt
// Filename: CheckBundlesHelper.cs
// Date Created: 2016/02/17
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// ======================================================================================
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using B4BCore;

namespace Tests.Helpers
{

    public static class CheckBundlesHelper
    {
        private const string BowerBundlesMatchName = "BowerBundles";
        private const string BowerBundlesFileName = "BowerBundles.json";

        private static readonly List<string> DirectoriesToWipe = new List<string> { "App_Data\\", "css\\", "js\\", "lib\\"};

        public static readonly string MvcSetDir = B4BSetupHelper.GetDirRelToTestDirectory("CheckBundlesTests\\");
        public static readonly string BowerBundlesDir = B4BSetupHelper.GetDirRelToTestDirectory("CheckBundlesTests\\App_Data\\");

        private static readonly string OriginalBowerBundlesFilePath = 
            Path.Combine( B4BSetupHelper.GetDirRelToTestDirectory("CheckBundlesTests\\OriginalBowerBundle\\"), BowerBundlesFileName);


        public static CheckBundles GetCheckBundlesWithCorrectDirs(bool checkForConcatFile = true)
        {
            return new CheckBundles(MvcSetDir, BowerBundlesDir, checkForConcatFile);
        }

        public static void SetupFiles(this string filesToCreate)
        {
            var dirAndFiles = filesToCreate.Split(',').Select(x => new DirAndFile(x.Trim())).ToList();

            if(!dirAndFiles.Any(x => x.IsBowerBundle))
                throw new InvalidOperationException($"Must contain '{BowerBundlesMatchName}'");

            //now wipe all files in the given directories
            DirectoriesToWipe.ForEach(DeleteAllFilesInDir);
            //and add the requested files in the order requested
            dirAndFiles.ForEach(x => x.AddFileToAbsDir());
        }

        public static IEnumerable<string> CheckFilesInDirs()
        {
           return from dir in DirectoriesToWipe
                let files = Directory.GetFiles(Path.Combine(MvcSetDir, dir))
                let fileStr = string.Join(",", files.Select(Path.GetFileName))
                where files.Length > 0
                select $"{dir}: {fileStr}";
        }

        //---------------------------------------------------------------
        //private helpers

        private static void DeleteAllFilesInDir( string relDir)
        {
            var di = new DirectoryInfo(Path.Combine(MvcSetDir, relDir));

            foreach (var file in di.GetFiles())
            {
                file.Delete();
            }
        }

        private class DirAndFile
        {
            public DirAndFile(string fileDefn)
            {
                if (fileDefn == BowerBundlesMatchName)
                {
                    IsBowerBundle = true;
                    AbsDir = BowerBundlesDir;
                    FileName = BowerBundlesFileName;
                }
                else
                {
                    var dirFile = fileDefn.Split('/');
                    if (dirFile.Length != 2)
                        throw new InvalidOperationException($"The string '{fileDefn}' does not match the dir/file.ext format.");
                    AbsDir = Path.Combine(MvcSetDir, dirFile[0]);
                    FileName = dirFile[1];
                }
            }

            public bool IsBowerBundle { get; private set; }
            public string AbsDir { get; private set; }
            public string FileName { get; private set; }

            public void AddFileToAbsDir()
            {
                Thread.Sleep(10);           //need delay to ensure files are written at the right time 
                var fileContent = IsBowerBundle
                    ? File.ReadAllText(OriginalBowerBundlesFilePath)
                    : "//This is a dummy file content";

                File.WriteAllText(Path.Combine(AbsDir,FileName), fileContent);
            }
        }
    }
}