#region licence
// ======================================================================================
// Mvc5WithBowerAndGrunt - An example of how to change a MVC5 project to Bower and Grunt
// Filename: AppDataHelper.cs
// Date Created: 2016/02/04
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// ======================================================================================
#endregion

using System;
using System.IO;

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
    }
}