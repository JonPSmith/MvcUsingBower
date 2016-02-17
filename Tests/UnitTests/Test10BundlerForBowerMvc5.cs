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
using B4BCore;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test10BundlerForBowerMvc5
    {

        [Test]
        public void CheckGetActualFilePathFromVirtualPathOk()
        {
            //SETUP 
            var func = B4BSetupHelper.GetActualFilePathFromVirtualPath();
            const string relPath = "Scripts\\Script1.js";

            //ATTEMPT
            var path = func("~/" + relPath);

            //VERIFY
            path.ShouldEqual(TestFileHelpers.GetTestFileFilePath(relPath));
        }

        //-------------------------------------------------------------


        [Test]
        public void TestBundlerForBowerCssDebugOk()
        {
            //SETUP 
            var b4b = new BundlerForBower(s => "url:" + s.Substring(2), B4BSetupHelper.GetActualFilePathFromVirtualPath(), B4BSetupHelper.GetDirRelToTestDirectory("NoConfig\\"));

            //ATTEMPT
            var output = b4b.CalculateHtmlIncludes("mainCss", CssOrJs.Css, true);

            //VERIFY
            var lines = output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            lines.Length.ShouldEqual(2);
            lines[0].ShouldEqual("<link href='url:Content/bootstrap.css' rel='stylesheet'/>\r");
            lines[1].ShouldEqual("<link href='url:Content/site.css' rel='stylesheet'/>\r");
        }

        [Test]
        public void TestBundlerForBowerCssNonDebugOk()
        {
            //SETUP 
            var b4b = new BundlerForBower(s => "url:" + s.Substring(2), B4BSetupHelper.GetActualFilePathFromVirtualPath(), B4BSetupHelper.GetDirRelToTestDirectory("NoConfig\\"));

            //ATTEMPT
            var output = b4b.CalculateHtmlIncludes("mainCss", CssOrJs.Css, false);

            //VERIFY
            output.ShouldEqual("<link href='url:css/mainCss.min.css?v=TdTxYoaXjmCw1qNZ3ECkVr0eMx3rj6OFikZ6GH_a_Hw' rel='stylesheet'/>\r\n");
        }

        [Test]
        public void TestBundlerForBowerJsDebugOk()
        {
            //SETUP 
            var b4b = new BundlerForBower(s => "url:" + s.Substring(2), B4BSetupHelper.GetActualFilePathFromVirtualPath(), B4BSetupHelper.GetDirRelToTestDirectory("NoConfig\\"));

            //ATTEMPT
            var output = b4b.CalculateHtmlIncludes("appLibsJs", CssOrJs.Js, true);

            //VERIFY
            var lines = output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            lines.Length.ShouldEqual(3);
            lines[0].ShouldEqual("<script src='url:Scripts/Script1.js'></script>\r");
            lines[1].ShouldEqual("<script src='url:Scripts/Script2.js'></script>\r");
            lines[2].ShouldEqual("<script src='url:Scripts/Script3.js'></script>\r");
        }

        [Test]
        public void TestBundlerForBowerJsNonDebugOk()
        {
            //SETUP 
            var b4b = new BundlerForBower(s => "url:" + s.Substring(2), B4BSetupHelper.GetActualFilePathFromVirtualPath(), B4BSetupHelper.GetDirRelToTestDirectory("NoConfig\\"));

            //ATTEMPT
            var output = b4b.CalculateHtmlIncludes("appLibsJs", CssOrJs.Js, false);

            //VERIFY
            output.ShouldEqual("<script src='url:js/appLibsJs.min.js?v=SnW8SeyCxQMkwmWggnI6zdSJoIVYPkVYHyM4jpW3jaQ'></script>\r\n");
        }
    }
}