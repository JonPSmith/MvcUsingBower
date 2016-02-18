#region licence
// ======================================================================================
// Mvc5UsingBower - An example+library to allow an MVC project to use Bower and Grunt
// Filename: Test20CheckBundlesGood.cs
// Date Created: 2016/02/17
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// ======================================================================================
#endregion

using System.Linq;
using B4BCore;
using NUnit.Framework;
using Tests.Helpers;
using WebApplication.Mvc5;

namespace Tests.UnitTests
{
    public class Test20CheckBundlesGood
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
            var checker = new CheckBundles(typeof(Test20CheckBundlesGood), B4BSetupHelper.GetDirRelToTestDirectory("AbsBowerBundles\\"));

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

        [Test]
        public void TestCheckSingleBundleIsUpToDateWithCdnOk()
        {
            var checker = new CheckBundles(TestFileHelpers.GetTestDataFileDirectory(), B4BSetupHelper.GetDirRelToTestDirectory("WithCdn\\"));

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("standardLibsCdnJs");

            //VERIFY
            errors.Any().ShouldEqual(false, string.Join("\n", errors));
        }
    }
}