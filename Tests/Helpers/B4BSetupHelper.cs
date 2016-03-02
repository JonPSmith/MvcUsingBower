#region licence
// ======================================================================================
// Mvc5UsingBower - An example+library to allow an MVC project to use Bower and Grunt
// Filename: B4BSetupHelper.cs
// Date Created: 2016/02/17
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// ======================================================================================
#endregion

using System;
using System.IO;
using WebApplication.Mvc5;

namespace Tests.Helpers
{
    public static class B4BSetupHelper
    {
        public static string GetDirRelToTestDirectory(string relativeDir)
        {
            if (!relativeDir.EndsWith("\\")) relativeDir += "\\";
            var dirPath = Path.Combine(TestFileHelpers.GetTestDataFileDirectory(), relativeDir);

            return dirPath;
        }

        public static Func<string, string> GetActualFilePathFromVirtualPath(string testSubDirToUse = null)
        {
            var topDir = testSubDirToUse != null
                ? Path.Combine(TestFileHelpers.GetTestDataFileDirectory(), testSubDirToUse)
                : TestFileHelpers.GetTestDataFileDirectory();

            return s => Path.Combine(topDir, s.Length <2 ? "" : s.Substring(2));
        }

        public static Func<string, string> GetChecksumFromRelPath(string testSubDirToUse = null)
        {
            var lookupFunc = GetActualFilePathFromVirtualPath(testSubDirToUse);
            return s => BowerBundlerHelper.GetChecksumBasedOnFileContent(lookupFunc(s));
        }
    }
}