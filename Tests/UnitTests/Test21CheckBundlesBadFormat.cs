#region licence
// ======================================================================================
// Mvc5WithBowerAndGrunt - An example of how to change a MVC5 project to Bower and Grunt
// Filename: Test21CheckBundlesBadFormat.cs
// Date Created: 2016/02/16
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

namespace Tests.UnitTests
{
    public class Test21CheckBundlesBadFormat
    {

        //------------------------------------------------------------
        //FAILURE TESTS NEED TO BE WRITTEN!

        [Test]
        public void TestCheckSingleBundleIsValidBadCndContainsMixOk()
        {
            //SETUP
            var checker = new CheckBundles(TestFileHelpers.GetTestDataFileDirectory(), B4BSetupHelper.GetDirRelToTestDirectory("BadBowerBundlesFormat\\"));

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("BadCndContainsMix");

            //VERIFY
            errors.Count.ShouldEqual(1);
            errors.First().ShouldEqual("The Bundle called BadCndContainsMix contained both cdn and non cdn entries, which is not supported.");
        }

        [Test]
        public void TestCheckSingleBundleIsValidMissingDevProdOk()
        {
            //SETUP
            var checker = new CheckBundles(TestFileHelpers.GetTestDataFileDirectory(), B4BSetupHelper.GetDirRelToTestDirectory("BadBowerBundlesFormat\\"));

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("BadCndMissingDev");

            //VERIFY
            errors.Count.ShouldEqual(1);
            errors.First().ShouldEqual("The CDN bundle BadCndMissingDev, array element 0, is missing a property called 'development'.");
        }

        [Test]
        public void TestCheckSingleBundleIsValidMissingProdProdOk()
        {
            //SETUP
            var checker = new CheckBundles(TestFileHelpers.GetTestDataFileDirectory(), B4BSetupHelper.GetDirRelToTestDirectory("BadBowerBundlesFormat\\"));

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("BadCndMissingProd");

            //VERIFY
            errors.Count.ShouldEqual(1);
            errors.First().ShouldEqual("In the bundle called BadCndMissingProd, array element 1, the following properties were missing: production");
        }

        [Test]
        public void TestCheckSingleBundleIsValidBadCndMissingCdnSuccessTestOk()
        {
            //SETUP
            var checker = new CheckBundles(TestFileHelpers.GetTestDataFileDirectory(), B4BSetupHelper.GetDirRelToTestDirectory("BadBowerBundlesFormat\\"));

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("BadCndMissingCdnSuccessTest");

            //VERIFY
            errors.Count.ShouldEqual(1);
            errors.First().ShouldEqual("In the bundle called BadCndMissingCdnSuccessTest, array element 0, the following properties were missing: cdnSuccessTest");
        }

        [Test]
        public void TestCheckSingleBundleIsValidBadCndBadCndCssNotSupportedOk()
        {
            //SETUP
            var checker = new CheckBundles(TestFileHelpers.GetTestDataFileDirectory(), B4BSetupHelper.GetDirRelToTestDirectory("BadBowerBundlesFormat\\"));

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("BadCndCssNotSupported");

            //VERIFY
            errors.Count.ShouldEqual(1);
            errors.First().ShouldEqual("This configuration of BundlerForBower does not support CDN bundles for .css files. Bad bundle is BadCndCssNotSupported.");
        }

        [Test]
        public void TestCheckSingleBundleIsValidBadCndCdnBadFormatOk()
        {
            //SETUP
            var checker = new CheckBundles(TestFileHelpers.GetTestDataFileDirectory(), B4BSetupHelper.GetDirRelToTestDirectory("BadBowerBundlesFormat\\"));

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("CdnBadFormat");

            //VERIFY
            errors.Count.ShouldEqual(2);
            errors.First().ShouldEqual("The CDN bundle CdnBadFormat, array element 0, contained an invalid type Integer");
            errors.Last().ShouldEqual("The CDN bundle CdnBadFormat, array element 1, contained an invalid type Boolean");
        }

        [Test]
        public void TestCheckSingleBundleIsValidBadCndContaintsSearchTestOk()
        {
            //SETUP
            var checker = new CheckBundles(TestFileHelpers.GetTestDataFileDirectory(), B4BSetupHelper.GetDirRelToTestDirectory("BadBowerBundlesFormat\\"));

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("BadCndContaintsSearchTest");

            //VERIFY
            errors.Count.ShouldEqual(2);
            errors.First().ShouldEqual("The following files had 'development' with search strings in the bundles called 'BadCndContaintsSearchTest':\n - lib/jquery/dist/*jquery.js");
            errors.Last().ShouldEqual("The following files had 'production' with search strings in the bundles called 'BadCndContaintsSearchTest':\n - lib/jquery/dist/*jquery.js");
        }
    }
}