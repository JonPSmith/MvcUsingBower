#region licence
// ======================================================================================
// Mvc5WithBowerAndGrunt - An example of how to change a MVC5 project to Bower and Grunt
// Filename: Test01ReadConfig.cs
// Date Created: 2016/02/04
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// ======================================================================================
#endregion

using System.IO;
using System.Linq;
using B4BCore.Internal;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test05RelPathSearcher
    {

        [Test]
        public void CheckGetActualFilePathFromVirtualPathFileOk()
        {
            //SETUP 
            var func = B4BSetupHelper.GetActualFilePathFromVirtualPath();
            const string relPath = "Scripts\\Script1.js";

            //ATTEMPT
            var path = func("~/" + relPath);

            //VERIFY
            path.ShouldEqual(TestFileHelpers.GetTestFileFilePath(relPath));
        }

        [Test]
        public void CheckGetActualFilePathFromVirtualPathDirOk()
        {
            //SETUP 
            var func = B4BSetupHelper.GetActualFilePathFromVirtualPath();
            const string relPath = "Scripts\\";

            //ATTEMPT
            var path = func("~/" + relPath);

            //VERIFY
            path.ShouldEqual(Path.Combine(TestFileHelpers.GetTestDataFileDirectory(), relPath));
        }

        [TestCase("~/fred.txt", false)]
        [TestCase("~/*.txt", true)]
        [TestCase("~/?red.txt", true)]
        public void TestContainsSearchCharsOk(string relFilePath, bool shouldBeSearch)
        {
            //SETUP 

            //ATTEMPT
            var isSearch = RelPathSearcher.ContainsSearchChars(relFilePath);

            //VERIFY
            isSearch.ShouldEqual(shouldBeSearch);
        }

        [TestCase("~/fred.txt", false)]
        [TestCase("~/*.txt", false)]
        [TestCase("~/**/fred.txt", true)]
        public void TestContainsInvalidSearchCharsOk(string relFilePath, bool shouldBeBad)
        {
            //SETUP 

            //ATTEMPT
            var isBadSearch = RelPathSearcher.ContainsInvalidSearchChars(relFilePath);

            //VERIFY
            isBadSearch.ShouldEqual(shouldBeBad);
        }

        [TestCase("~/*.js", "~/TopLevelScript.js")]
        [TestCase("~/SearchDir1/Script*.js", "~/SearchDir1/Script1.js")]
        [TestCase("~/SearchDir1/*", "~/SearchDir1/Script1.js", "~/SearchDir1/Text1.txt")]
        [TestCase("~/SearchDir1/SearchDir2a/Script*.js", "~/SearchDir1/SearchDir2a/Script2a.js")]
        [TestCase("~/SearchDir1/SearchDir2a/*", "~/SearchDir1/SearchDir2a/Script2a.js", "~/SearchDir1/SearchDir2a/Text2a.txt")]
        public void TestFindAllFilesOnlyFileNameSearchOk(string relFilePath, params string [] expectedFiles)
        {
            //SETUP 
            var searcher = new RelPathSearcher(B4BSetupHelper.GetActualFilePathFromVirtualPath());

            //ATTEMPT
            var foundFiles = searcher.FindAllFiles(relFilePath);

            //VERIFY
            CollectionAssert.AreEqual(expectedFiles, foundFiles);
        }


        [TestCase("~/SearchDir1/SearchDir2*/Script*.js", "~/SearchDir1/SearchDir2a/Script2a.js", "~/SearchDir1/SearchDir2b/Script2b.js")]
        [TestCase("~/SearchDir1/SearchDir2?/Script*.js", "~/SearchDir1/SearchDir2a/Script2a.js", "~/SearchDir1/SearchDir2b/Script2b.js")]
        [TestCase("~/SearchDir1/*/Script*.js", "~/SearchDir1/SearchDir2a/Script2a.js", "~/SearchDir1/SearchDir2b/Script2b.js")]
        [TestCase("~/SearchDir1/*/*/Script*.js", "~/SearchDir1/SearchDir2a/SearchDir3/Script3.js")]
        public void TestFindAllFilesOnlyDirectorySearchOk(string relFilePath, params string[] expectedFiles)
        {
            //SETUP 
            var searcher = new RelPathSearcher(B4BSetupHelper.GetActualFilePathFromVirtualPath());

            //ATTEMPT
            var foundFiles = searcher.FindAllFiles(relFilePath);

            //VERIFY
            CollectionAssert.AreEqual(expectedFiles, foundFiles);
        }

        [TestCase("~/SearchDir1/**/Script*.js", "'~/SearchDir1/**/Script*.js contains a search string that BundlerForBower does not currently support.")]
        [TestCase("~/BadDir/SearchDir2*/Script*.js", "The directory '~\\BadDir' in ~/BadDir/SearchDir2*/Script*.js does not exist.")]
        [TestCase("~/SearchDir1/BadDir/Script*.js", "The fileRef '~/SearchDir1/BadDir/Script*.js' contains a directory that does not exist.")]
        public void TestFindAllFilesBadDirSearchOk(string relFilePath, string expectedMessage)
        {
            //SETUP 
            var searcher = new RelPathSearcher(B4BSetupHelper.GetActualFilePathFromVirtualPath());
            string errorMessage = null;

            //ATTEMPT
            var foundFiles = searcher.FindAllFiles(relFilePath, s => errorMessage = s);

            //VERIFY
            foundFiles.Count().ShouldEqual(0);
            errorMessage.ShouldEqual(expectedMessage);
        }

    }
}