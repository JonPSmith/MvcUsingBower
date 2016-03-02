#region licence
// ======================================================================================
// Mvc5UsingBower - An example+library to allow an MVC project to use Bower and Grunt
// Filename: Test10BundlerForBowerMvc5.cs
// Date Created: 2016/02/17
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

        [Test]
        public void CheckGetChecksumFromRelPathOk()
        {
            //SETUP 
            var func = B4BSetupHelper.GetChecksumFromRelPath();
            const string relPath = "Scripts\\Script1.js";

            //ATTEMPT
            var path = func("~/" + relPath);

            //VERIFY
            path.ShouldEqual("fJjHT45pis5EogBORiyWYn5UIa0PBrrnusR2zVSlRL8");
        }
        //-------------------------------------------------------------


        [Test]
        public void TestBundlerForBowerCssDebugOk()
        {
            //SETUP 
            var b4b = new BundlerForBower(B4BSetupHelper.GetDirRelToTestDirectory("NoConfig\\"), s => "url:" + s.Substring(2), 
                B4BSetupHelper.GetActualFilePathFromVirtualPath(), B4BSetupHelper.GetChecksumFromRelPath());

            //ATTEMPT
            var output = b4b.CalculateHtmlIncludes("mainCss", CssOrJs.Css, true);

            //VERIFY
            var lines = output.Split(new [] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
            lines.Length.ShouldEqual(2);
            lines[0].ShouldEqual("<link href='url:Content/bootstrap.css' rel='stylesheet'/>\r");
            lines[1].ShouldEqual("<link href='url:Content/site.css' rel='stylesheet'/>\r");
        }

        [Test]
        public void TestBundlerForBowerCssNonDebugOk()
        {
            //SETUP 
            var b4b = new BundlerForBower(B4BSetupHelper.GetDirRelToTestDirectory("NoConfig\\"), s => "url:" + s.Substring(2), 
                B4BSetupHelper.GetActualFilePathFromVirtualPath(), B4BSetupHelper.GetChecksumFromRelPath());

            //ATTEMPT
            var output = b4b.CalculateHtmlIncludes("mainCss", CssOrJs.Css, false);

            //VERIFY
            output.ShouldEqual("<link href='url:css/mainCss.min.css?v=TdTxYoaXjmCw1qNZ3ECkVr0eMx3rj6OFikZ6GH_a_Hw' rel='stylesheet'/>");
        }

        [Test]
        public void TestBundlerForBowerJsDebugOk()
        {
            //SETUP 
            var b4b = new BundlerForBower(B4BSetupHelper.GetDirRelToTestDirectory("NoConfig\\"), s => "url:" + s.Substring(2), 
                B4BSetupHelper.GetActualFilePathFromVirtualPath(), B4BSetupHelper.GetChecksumFromRelPath());

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
            var b4b = new BundlerForBower(B4BSetupHelper.GetDirRelToTestDirectory("NoConfig\\"), s => "url:" + s.Substring(2), 
                B4BSetupHelper.GetActualFilePathFromVirtualPath(), B4BSetupHelper.GetChecksumFromRelPath());

            //ATTEMPT
            var output = b4b.CalculateHtmlIncludes("appLibsJs", CssOrJs.Js, false);

            //VERIFY
            output.ShouldEqual("<script src='url:js/appLibsJs.min.js?v=SnW8SeyCxQMkwmWggnI6zdSJoIVYPkVYHyM4jpW3jaQ'></script>");
        }

        //--------------------------------------------------------------
        //Now check with CDN

        [Test]
        public void TestBundlerForBowerWithCdnJsDebugOk()
        {
            //SETUP 
            var b4b = new BundlerForBower(B4BSetupHelper.GetDirRelToTestDirectory("WithCdn\\"), s => "url:" + s.Substring(2), 
                B4BSetupHelper.GetActualFilePathFromVirtualPath(), B4BSetupHelper.GetChecksumFromRelPath());

            //ATTEMPT
            var output = b4b.CalculateHtmlIncludes("standardLibsCdnJs", CssOrJs.Js, true);

            //VERIFY
            var lines = output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            lines.Length.ShouldEqual(2);
            lines[0].ShouldEqual("<script src='url:lib/jquery/dist/jquery.js'></script>\r");
            lines[1].ShouldEqual("<script src='url:lib/bootstrap/dist/js/bootstrap.js'></script>\r");
        }

        [Test]
        public void TestBundlerForBowerWithCdnJsNonDebugOk()
        {
            //SETUP 
            var b4b = new BundlerForBower(B4BSetupHelper.GetDirRelToTestDirectory("WithCdn\\"), s => "url:" + s.Substring(2), 
                B4BSetupHelper.GetActualFilePathFromVirtualPath(), B4BSetupHelper.GetChecksumFromRelPath());

            //ATTEMPT
            var output = b4b.CalculateHtmlIncludes("standardLibsCdnJs", CssOrJs.Js, false);

            //VERIFY
            var lines = output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            lines.Length.ShouldEqual(2);
            lines[0].ShouldEqual("<script src='https://ajax.aspnetcdn.com/ajax/jquery/jquery-2.1.4.min.js'></script><script>(window.jQuery||document.write(\"\\x3Cscript src='url:js/jquery.min.js?v=SnW8SeyCxQMkwmWggnI6zdSJoIVYPkVYHyM4jpW3jaQ'\\x3C/script>\"));</script>\r");
            lines[1].ShouldEqual("<script src='https://ajax.aspnetcdn.com/ajax/bootstrap/3.3.5/bootstrap.min.js'></script><script>(window.jQuery && window.jQuery.fn && window.jQuery.fn.modal||document.write(\"\\x3Cscript src='url:js/bootstrap.min.js?v=SnW8SeyCxQMkwmWggnI6zdSJoIVYPkVYHyM4jpW3jaQ'\\x3C/script>\"));</script>\r");
        }
    }
}