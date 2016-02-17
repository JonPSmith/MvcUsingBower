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

using B4BCore.Internal;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test02ReadBundleFile
    {

        [Test]
        public void TestReadBundleFileOk()
        {
            //SETUP 

            //ATTEMPT
            var reader = new ReadBundleFile(TestFileHelpers.GetTestFileFilePath("BowerBundles01*.json"));

            //VERIFY
            CollectionAssert.AreEqual(new[] { "mainCss", "standardLibsJs", "appLibsJs", "jqueryval" }, reader.BundleNames);
        }

        [TestCase("mainCss", "bootstrap.css", "site.css")]
        [TestCase("standardLibsJs", "jquery.js", "bootstrap.js")]
        [TestCase("appLibsJs", "Script*.js")]
        [TestCase("jqueryval", "jquery.validate.js", "jquery.validate.unobtrusive.js")]
        public void TestReadBundleFileBundleContentOk(string bundleName, params string[] expectedfiles)
        {
            //SETUP 
            var reader = new ReadBundleFile(TestFileHelpers.GetTestFileFilePath("BowerBundles01*.json"));

            //ATTEMPT
            var files = reader.GetSettingAsStringArray(bundleName, "");

            //VERIFY
            CollectionAssert.AreEqual(expectedfiles, files);
        }

    }
}