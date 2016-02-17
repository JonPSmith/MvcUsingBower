#region licence
// ======================================================================================
// Mvc5UsingBower - An example+library to allow an MVC project to use Bower and Grunt
// Filename: Test02ReadBundleFile.cs
// Date Created: 2016/02/17
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// ======================================================================================
#endregion

using System;
using System.Linq;
using B4BCore.Internal;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test02ReadBundleFile
    {
        [Test]
        public void TestCreateReadBundleFileNonCdnOk()
        {
            //SETUP 

            //ATTEMPT
            var reader = new ReadBundleFile(TestFileHelpers.GetTestFileFilePath("BowerBundles01*.json"));

            //VERIFY
            CollectionAssert.AreEqual(new[] {"mainCss", "standardLibsJs", "appLibsJs", "jqueryval"}, reader.BundleNames);
        }

        [TestCase("mainCss", "bootstrap.css", "site.css")]
        [TestCase("standardLibsJs", "jquery.js", "bootstrap.js")]
        [TestCase("jqueryval", "jquery.validate.js", "jquery.validate.unobtrusive.js")]
        public void TestGetBundleDebugFilesNonCndFileBundleListOk(string bundleName, params string [] expectedfiles)
        {
            //SETUP 
            var reader = new ReadBundleFile(TestFileHelpers.GetTestFileFilePath("BowerBundles01*.json"));

            //ATTEMPT
            var files = reader.GetBundleDebugFiles(bundleName, "");

            //VERIFY
            CollectionAssert.AreEqual(expectedfiles, files);
        }


        [TestCase("appLibsJs", "Script*.js")]
        public void TestGetBundleDebugFilesNonCndFileBundleSingleOk(string bundleName, string expectedfile)
        {
            //SETUP 
            var reader = new ReadBundleFile(TestFileHelpers.GetTestFileFilePath("BowerBundles01*.json"));

            //ATTEMPT
            var files = reader.GetBundleDebugFiles(bundleName, "").ToList();

            //VERIFY
            files.Count().ShouldEqual(1);
            files[0].ShouldEqual(expectedfile);
        }

        [Test]
        public void TestGetBundleCdnInfoNonCdnOk()
        {
            //SETUP 
            var reader = new ReadBundleFile(TestFileHelpers.GetTestFileFilePath("BowerBundles01*.json"));

            //ATTEMPT
            var cdns = reader.GetBundleCdnInfo("standardLibsJs");

            //VERIFY
            cdns.Count.ShouldEqual(0);
        }

        //-----------------------------------------------
        //now with cdn

        [Test]
        public void TestCreateReadBundleFileCdnOk()
        {
            //SETUP 

            //ATTEMPT
            var reader = new ReadBundleFile(TestFileHelpers.GetTestFileFilePath("BowerBundles02*.json"));

            //VERIFY
            CollectionAssert.AreEqual(new[] { "mainCss", "standardLibsJs", "appLibsJs", "jqueryval" }, reader.BundleNames);
        }


        [TestCase("mainCss", "bootstrap.css", "site.css")]
        [TestCase("standardLibsJs", "jquery.js", "bootstrap.js")]
        [TestCase("jqueryval", "jquery.validate.js", "jquery.validate.unobtrusive.js")]
        public void TestGetBundleDebugFilesCndFileBundleListOk(string bundleName, params string[] expectedfiles)
        {
            //SETUP 
            var reader = new ReadBundleFile(TestFileHelpers.GetTestFileFilePath("BowerBundles01*.json"));

            //ATTEMPT
            var files = reader.GetBundleDebugFiles(bundleName, "");

            //VERIFY
            CollectionAssert.AreEqual(expectedfiles, files);
        }


        [TestCase("appLibsJs", "Script*.js")]
        public void TestGetBundleDebugFilesCndFileBundleSingleOk(string bundleName, string expectedfile)
        {
            //SETUP 
            var reader = new ReadBundleFile(TestFileHelpers.GetTestFileFilePath("BowerBundles01*.json"));

            //ATTEMPT
            var files = reader.GetBundleDebugFiles(bundleName, "").ToList();

            //VERIFY
            files.Count().ShouldEqual(1);
            files[0].ShouldEqual(expectedfile);
        }

        [Test]
        public void TestGetBundleCdnInfoCdnOk()
        {
            //SETUP 
            var reader = new ReadBundleFile(TestFileHelpers.GetTestFileFilePath("BowerBundles02*.json"));

            //ATTEMPT
            var cdns = reader.GetBundleCdnInfo("standardLibsJs").ToList();

            //VERIFY
            cdns.Count.ShouldEqual(2);
            var i = 0;
            cdns[i++].ToString().ShouldEqual("Development: jquery.js, Production: jquery.min.js, "+
                "cdnUrl: https://ajax.aspnetcdn.com/ajax/jquery/jquery-2.1.4.min.js, cdnSuccessTest: window.jQuery");
            cdns[i++].ToString().ShouldEqual("Development: bootstrap.js, Production: bootstrap.min.js, "+
                "cdnUrl: https://ajax.aspnetcdn.com/ajax/bootstrap/3.3.5/bootstrap.min.js, cdnSuccessTest: window.jQuery && window.jQuery.fn && window.jQuery.fn.modal");
            //foreach (var cdnInfo in cdns)
            //{
            //    Console.WriteLine("cdns[i++].ToString().ShouldEqual(\"{0}\");", cdnInfo);
            //}
        }

        //----------------------------------------------------------
        //cdn errors

        [Test]
        public void TestGetBundleDebugFilesBadCndFileBundleSingleOk()
        {
            //SETUP 
            var reader = new ReadBundleFile(TestFileHelpers.GetTestFileFilePath("BowerBundles03*.json"));

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>( () => reader.GetBundleDebugFiles("missingDevelopmentJs"));

            //VERIFY
            ex.Message.ShouldEqual("The CDN bundle missingDevelopmentJs, array element 0, is missing a property called 'development'.");
        }
    }
}