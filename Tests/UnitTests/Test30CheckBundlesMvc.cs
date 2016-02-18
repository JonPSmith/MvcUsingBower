#region licence
// ======================================================================================
// Mvc5UsingBower - An example+library to allow an MVC project to use Bower and Grunt
// Filename: Test30CheckBundlesMvc.cs
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
    //This checks the MVC application's Bundles 
    public class Test30CheckBundlesMvc
    {

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
        //Check with cdns (already checked by CheckAllBundlesAreValid, but done again to ensure there is no bugs in the checking code

        [Test]
        public void TestCheckSingleBundleIsUpToDateWithCdnOk()
        {
            var checker = new CheckBundles(typeof(BowerBundlerHelper));

            //ATTEMPT
            var errors = checker.CheckSingleBundleIsValid("standardLibsCndJs");

            //VERIFY
            errors.Any().ShouldEqual(false, string.Join("\n", errors));
        }
    }
}