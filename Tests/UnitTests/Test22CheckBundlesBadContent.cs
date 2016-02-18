#region licence
// ======================================================================================
// Mvc5UsingBower - An example+library to allow an MVC project to use Bower and Grunt
// Filename: Test22CheckBundlesBadContent.cs
// Date Created: 2016/02/17
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// ======================================================================================
#endregion

using System.Linq;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test22CheckBundlesBadContent
    {

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            "BowerBundles".SetupFiles();    //we wipe all the temp files so that git doesn't keep trying to save them
        }

        [Test]
        public void Check1CheckBundlesHelperSetsUpFileOk()
        {
            //SETUP

            //ATTEMPT
            "BowerBundles".SetupFiles();

            //VERIFY
            var dirsWithFilesIn = CheckBundlesHelper.CheckFilesInDirs().ToList();
            dirsWithFilesIn.Count.ShouldEqual(1);
            dirsWithFilesIn.First().ShouldEqual("App_Data\\: BowerBundles.json");
        }

        [Test]
        public void Check2CheckBundlesHelperSetsUpFileOkOk()
        {
            //SETUP

            //ATTEMPT
            "BowerBundles, lib/myfile.js, js/simpleJs.js, js/simpleJs.min.js ".SetupFiles();

            //VERIFY
            var dirsWithFilesIn = CheckBundlesHelper.CheckFilesInDirs().ToList();
            dirsWithFilesIn.Count.ShouldEqual(3);
            dirsWithFilesIn[0].ShouldEqual("App_Data\\: BowerBundles.json");
            dirsWithFilesIn[1].ShouldEqual("js\\: simpleJs.js,simpleJs.min.js");
            dirsWithFilesIn[2].ShouldEqual("lib\\: myfile.js");
        }

        //-----------------------------------------
        //now use to test

        [Test]
        public void TestCheckBundlesSimpleJsOk()
        {
            //SETUP
            "BowerBundles, lib/myfile.js, js/simpleJs.js, js/simpleJs.min.js ".SetupFiles();
            var checker = CheckBundlesHelper.GetCheckBundlesWithCorrectDirs();

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("simpleJs");

            //VERIFY
            errors.Any().ShouldEqual(false);
        }

        [Test]
        public void TestCheckBundlesSimpleJsMissingLibFileOk()
        {
            //SETUP
            "BowerBundles, js/simpleJs.js, js/simpleJs.min.js ".SetupFiles();
            var checker = CheckBundlesHelper.GetCheckBundlesWithCorrectDirs();

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("simpleJs");

            //VERIFY
            errors.Count.ShouldEqual(1);
            errors.First().ShouldEqual("The following files were missing in the bundles called 'simpleJs':\n - lib/myfile.js");
        }

        [Test]
        public void TestCheckBundlesSimpleJsMissingConcatFileOk()
        {
            //SETUP
            "BowerBundles, lib/myfile.js, js/simpleJs.min.js ".SetupFiles();
            var checker = CheckBundlesHelper.GetCheckBundlesWithCorrectDirs();

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("simpleJs");

            //VERIFY
            errors.Count.ShouldEqual(1);
            errors.First().ShouldEqual("Warning: the concat file for 'simpleJs' is missing. Continuing test.");
        }

        [Test]
        public void TestCheckBundlesSimpleJsMissingMinifiedFileOk()
        {
            //SETUP
            "BowerBundles, lib/myfile.js, js/simpleJs.js ".SetupFiles();
            var checker = CheckBundlesHelper.GetCheckBundlesWithCorrectDirs();

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("simpleJs");

            //VERIFY
            errors.Count.ShouldEqual(1);
            errors.First().ShouldEqual("The minified file for 'simpleJs' is missing.");
        }

        [Test]
        public void TestCheckBundlesSimpleJsLibFileNewerThanConcatOk()
        {
            //SETUP
            "BowerBundles, js/simpleJs.js, lib/myfile.js, js/simpleJs.min.js ".SetupFiles();
            var checker = CheckBundlesHelper.GetCheckBundlesWithCorrectDirs();

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("simpleJs");

            //VERIFY
            errors.Count.ShouldEqual(1);
            errors.First().ShouldEqual("The concat file for 'simpleJs' is out of date. Newer files are:\n - lib/myfile.js");
        }

        [Test]
        public void TestCheckBundlesSimpleJsConcatFileNewerThanMinifiedOk()
        {
            //SETUP
            "BowerBundles, lib/myfile.js, js/simpleJs.min.js, js/simpleJs.js".SetupFiles();
            var checker = CheckBundlesHelper.GetCheckBundlesWithCorrectDirs();

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("simpleJs");

            //VERIFY
            errors.Count.ShouldEqual(1);
            errors.First().ShouldEqual("The concat file for 'simpleJs' is newer than the minified file.");
        }

        //---------------------------------------
        //SimpleJs - ignore concat

        [Test]
        public void TestCheckBundlesIgnoreConcatSimpleJsMissingConcatFileOk()
        {
            //SETUP
            "BowerBundles, lib/myfile.js, js/simpleJs.min.js ".SetupFiles();
            var checker = CheckBundlesHelper.GetCheckBundlesWithCorrectDirs(false);

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("simpleJs");

            //VERIFY
            errors.Count.ShouldEqual(0);
        }

        [Test]
        public void TestCheckBundlesIgnoreConcatSimpleJsMissingMinifiedFileOk()
        {
            //SETUP
            "BowerBundles, lib/myfile.js".SetupFiles();
            var checker = CheckBundlesHelper.GetCheckBundlesWithCorrectDirs(false);

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("simpleJs");

            //VERIFY
            errors.Count.ShouldEqual(1);
            errors.First().ShouldEqual("The minified file for 'simpleJs' is missing.");
        }

        [Test]
        public void TestCheckBundlesIgnoreConcatSimpleJsJsFileNewerThanMinifiedOk()
        {
            //SETUP
            "BowerBundles, js/simpleJs.min.js, lib/myfile.js".SetupFiles();
            var checker = CheckBundlesHelper.GetCheckBundlesWithCorrectDirs(false);

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("simpleJs");

            //VERIFY
            errors.Count.ShouldEqual(1);
            errors.First().ShouldEqual("The minified file for 'simpleJs' is out of date. Newer files are:\n - lib/myfile.js");
        }

        //---------------------------------------------------------
        //now test CND

        [Test]
        public void TestCheckBundlesCdnJsOk()
        {
            //SETUP
            "BowerBundles, lib/myfile.js, js/myfile.min.js ".SetupFiles();
            var checker = CheckBundlesHelper.GetCheckBundlesWithCorrectDirs();

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("cdnJs");

            //VERIFY
            errors.Any().ShouldEqual(false);
        }

        [Test]
        public void TestCheckBundlesCdnJsMissingDevelopmentFileOk()
        {
            //SETUP
            "BowerBundles, js/myfile.min.js ".SetupFiles();
            var checker = CheckBundlesHelper.GetCheckBundlesWithCorrectDirs();

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("cdnJs");

            //VERIFY
            errors.Count.ShouldEqual(1);
            errors.First().ShouldEqual("The following files in 'development' were missing in the bundles called 'cdnJs':\n - lib/myfile.js");
        }

        [Test]
        public void TestCheckBundlesCdnJsMissingProductionFileOk()
        {
            //SETUP
            "BowerBundles, lib/myfile.js ".SetupFiles();
            var checker = CheckBundlesHelper.GetCheckBundlesWithCorrectDirs();

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("cdnJs");

            //VERIFY
            errors.Count.ShouldEqual(1);
            errors.First().ShouldEqual("The following files in 'production' were missing in the bundles called 'cdnJs':\n - js/myfile.min.js");
        }

        //------------------------------------------------------------------
        //now the CheckBundleFileIsNotNewerThanMinifiedFiles

        [Test]
        public void TestCheckBundleFileIsNotNewerThanMinifiedFilesAllOk()
        {
            //SETUP
            "BowerBundles, lib/myfile.min.js, js/simpleJs.min.js".SetupFiles();
            var checker = CheckBundlesHelper.GetCheckBundlesWithCorrectDirs();

            //ATTEMPT
            var errors = checker.CheckBundleFileIsNotNewerThanMinifiedFiles();

            //VERIFY
            errors.ShouldEqual(null);
        }

        [Test]
        public void TestCheckBundleFileIsNotNewerThanMinifiedFilesBadDateOk()
        {
            //SETUP
            "lib/myfile.min.js, js/simpleJs.min.js, BowerBundles".SetupFiles();
            var checker = CheckBundlesHelper.GetCheckBundlesWithCorrectDirs();

            //ATTEMPT
            var errors = checker.CheckBundleFileIsNotNewerThanMinifiedFiles();

            //VERIFY
            errors.ShouldEqual("The following minified files have not been updated since the change in the bundle file:\n - js/simpleJs.min.js");
        }

        [Test]
        public void TestCheckBundleFileIsNotNewerThanMinifiedFilesMissingFileOk()
        {
            //SETUP
            "BowerBundles, lib/myfile.min.js".SetupFiles();
            var checker = CheckBundlesHelper.GetCheckBundlesWithCorrectDirs();

            //ATTEMPT
            var errors = checker.CheckBundleFileIsNotNewerThanMinifiedFiles();

            //VERIFY
            errors.ShouldEqual("The following minified files have not been updated since the change in the bundle file:\n - js/simpleJs.min.js");
        }
    }
}