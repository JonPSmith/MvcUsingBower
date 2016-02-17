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

using System;
using System.IO;
using System.Linq;
using B4BCore;
using Mvc5WithBowerAndGrunt;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test20CheckBundles
    {

        [Test]
        public void TestCheckSingleBundleCssOk()
        {
            //SETUP
            var checker = new CheckBundles(TestFileHelpers.GetTestDataFileDirectory(), B4BSetupHelper.GetDirRelToTestDirectory("NoConfig\\"));

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("mainCss");

            //VERIFY
            errors.Any().ShouldEqual(false, string.Join("\n", errors));
        }

        [Test]
        public void TestCheckSingleBundleJsOk()
        {
            //SETUP
            var checker = new CheckBundles(TestFileHelpers.GetTestDataFileDirectory(), B4BSetupHelper.GetDirRelToTestDirectory("NoConfig\\"));

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("appLibsJs");

            //VERIFY
            errors.Any().ShouldEqual(false, string.Join("\n", errors));
        }

        [Test]
        public void TestCheckSingleBundleJsViaTypeOk()
        {
            //SETUP
            var checker = new CheckBundles(typeof(Test20CheckBundles), B4BSetupHelper.GetDirRelToTestDirectory("AbsBowerBundles\\"));

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("appLibsJs");

            //VERIFY
            errors.Any().ShouldEqual(false, string.Join("\n", errors));
        }

        [Test]
        public void TestCheckAllBundlesAreUpToDateOk()
        {
            var checker = new CheckBundles(TestFileHelpers.GetTestDataFileDirectory(), B4BSetupHelper.GetDirRelToTestDirectory("MinCssAndJs\\"));

            //ATTEMPT
            var errors = checker.CheckAllBundlesAreValid();

            //VERIFY
            errors.Any().ShouldEqual(false, string.Join("\n", errors));
        }

        [Test]
        public void TestCheckAllBundlesAreUpToDateInMvcAppOk()
        {
            //SETUP
            var checker = new CheckBundles(typeof(BowerBundlerHelper));

            //ATTEMPT
            var errors = checker.CheckAllBundlesAreValid();

            //VERIFY
            errors.Any().ShouldEqual(false, string.Join("\n", errors));
        }

        [Test]
        public void TestCheckBundleFileIsNotNewerThanMinifiedFilesOk()
        {
            //SETUP
            var checker = new CheckBundles(TestFileHelpers.GetTestDataFileDirectory(), B4BSetupHelper.GetDirRelToTestDirectory("MinCssAndJs\\"));

            //ATTEMPT
            var error = checker.CheckBundleFileIsNotNewerThanMinifiedFiles();

            //VERIFY
            error.ShouldEqual(null, error);
        }

        [Test]
        public void TestCheckBundleFileIsNotNewerThanMinifiedFilesMvcAppOk()
        {
            //SETUP
            var checker = new CheckBundles(typeof(BowerBundlerHelper));

            //ATTEMPT
            var error = checker.CheckBundleFileIsNotNewerThanMinifiedFiles();

            //VERIFY
            error.ShouldEqual(null, error);
        }

        //------------------------------------------------------------
        //Check with cdns

        //------------------------------------------------------------
        //FAILURE TESTS NEED TO BE WRITTEN!

        //todo: write method to write/delete the files in a certain order so that failures can be tested
    }
}