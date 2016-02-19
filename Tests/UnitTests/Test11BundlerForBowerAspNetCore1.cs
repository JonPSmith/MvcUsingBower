#region licence
// ======================================================================================
// Mvc5UsingBower - An example+library to allow an MVC project to use Bower and Grunt
// Filename: Test11BundlerForBowerAspNetCore1.cs
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
    public class Test11BundlerForBowerAspNetCore1
    {
        [Test]
        public void TestBundlerForBowerCssDebugOk()
        {
            //SETUP 
            var b4b = new BundlerForBower(B4BSetupHelper.GetDirRelToTestDirectory("ASPNET Core 1 Config\\"), s => "url:" + s.Substring(2), B4BSetupHelper.GetActualFilePathFromVirtualPath());

            //ATTEMPT
            var output = b4b.CalculateHtmlIncludes("mainCss", CssOrJs.Css, true);

            //VERIFY
            var lines = output.Split(new [] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
            lines.Length.ShouldEqual(2);
            lines[0].ShouldEqual("<link href='url:bootstrap.css' rel='stylesheet'/>\r");
            lines[1].ShouldEqual("<link href='url:site.css' rel='stylesheet'/>\r");
        }

        [Test]
        public void TestBundlerForBowerCssNonDebugOk()
        {
            //SETUP 
            var b4b = new BundlerForBower(B4BSetupHelper.GetDirRelToTestDirectory("ASPNET Core 1 Config\\"), s => "url:" + s.Substring(2), B4BSetupHelper.GetActualFilePathFromVirtualPath());

            //ATTEMPT
            var output = b4b.CalculateHtmlIncludes("mainCss", CssOrJs.Css, false);

            //VERIFY
            output.ShouldEqual("<link href=\"url:wwwroot/css/mainCss.min.css\" asp-append-version=\"true\" rel=\"stylesheet\"/>");
        }

        [Test]
        public void TestBundlerForBowerJsDebugOk()
        {
            //SETUP 

            var b4b = new BundlerForBower(B4BSetupHelper.GetDirRelToTestDirectory("ASPNET Core 1 Config\\"), s => "url:" + s.Substring(2), B4BSetupHelper.GetActualFilePathFromVirtualPath());

            //ATTEMPT
            var output = b4b.CalculateHtmlIncludes("appLibsJs", CssOrJs.Js, true);

            //VERIFY
            var lines = output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            lines.Length.ShouldEqual(3);
            lines[0].ShouldEqual("<script src='url:Script1.js'></script>\r");
            lines[1].ShouldEqual("<script src='url:Script2.js'></script>\r");
            lines[2].ShouldEqual("<script src='url:Script3.js'></script>\r");
        }

        [Test]
        public void TestBundlerForBowerJsNonDebugOk()
        {
            //SETUP 
            var b4b = new BundlerForBower(B4BSetupHelper.GetDirRelToTestDirectory("ASPNET Core 1 Config\\"), s => "url:" + s.Substring(2), B4BSetupHelper.GetActualFilePathFromVirtualPath());

            //ATTEMPT
            var output = b4b.CalculateHtmlIncludes("appLibsJs", CssOrJs.Js, false);

            //VERIFY
            output.ShouldEqual("<script src=\"url:wwwroot/js/appLibsJs.min.js\" asp-append-version=\"true\"></script>");
        }

        //--------------------------------------------------------------
        //Now check with CDN

        [Test]
        public void TestBundlerForBowerWithCdnJsDebugOk()
        {
            //SETUP 
            var b4b = new BundlerForBower(B4BSetupHelper.GetDirRelToTestDirectory("ASPNET Core 1 Config\\"), s => "url:" + s.Substring(2), B4BSetupHelper.GetActualFilePathFromVirtualPath());

            //ATTEMPT
            var output = b4b.CalculateHtmlIncludes("standardLibsCndJs", CssOrJs.Js, true);

            //VERIFY
            var lines = output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            lines.Length.ShouldEqual(2);
            lines[0].ShouldEqual("<script src='url:lib/jquery/dist/jquery.js'></script>\r");
            lines[1].ShouldEqual("<script src='url:lib/bootstrap/js/bootstrap.js'></script>\r");
        }

        [Test]
        public void TestBundlerForBowerWithCdnJsNonDebugOk()
        {
            //SETUP 
            var b4b = new BundlerForBower(B4BSetupHelper.GetDirRelToTestDirectory("ASPNET Core 1 Config\\"), s => "url:" + s.Substring(2), B4BSetupHelper.GetActualFilePathFromVirtualPath());

            //ATTEMPT
            var output = b4b.CalculateHtmlIncludes("standardLibsCndJs", CssOrJs.Js, false);

            //VERIFY
            var lines = output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            lines.Length.ShouldEqual(2);
            lines[0].ShouldEqual("<script src='https://ajax.aspnetcdn.com/ajax/jquery/jquery-2.1.4.min.js' asp-fallback-src='url:wwwroot/js/jquery.min.js' asp-fallback-test='window.jQuery'></script>\r");
            lines[1].ShouldEqual("<script src='https://ajax.aspnetcdn.com/ajax/bootstrap/3.3.5/bootstrap.min.js' asp-fallback-src='url:wwwroot/js/bootstrap.min.js' asp-fallback-test='window.jQuery && window.jQuery.fn && window.jQuery.fn.modal'></script>\r");
        }

        [Test]
        public void TestBundlerForBowerWithCdnCssDebugOk()
        {
            //SETUP 
            var b4b = new BundlerForBower(B4BSetupHelper.GetDirRelToTestDirectory("ASPNET Core 1 Config\\"), s => "url:" + s.Substring(2), B4BSetupHelper.GetActualFilePathFromVirtualPath());

            //ATTEMPT
            var output = b4b.CalculateHtmlIncludes("bootstrapCdnCss", CssOrJs.Css, true);

            //VERIFY
            var lines = output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            lines.Length.ShouldEqual(1);
            lines[0].ShouldEqual("<link href='url:lib/bootstrap/dist/css/bootstrap.min.css' rel='stylesheet'/>\r");
        }

        [Test]
        public void TestBundlerForBowerWithCdnCssNonDebugOk()
        {
            //SETUP 
            var b4b = new BundlerForBower(B4BSetupHelper.GetDirRelToTestDirectory("ASPNET Core 1 Config\\"), s => "url:" + s.Substring(2), B4BSetupHelper.GetActualFilePathFromVirtualPath());

            //ATTEMPT
            var output = b4b.CalculateHtmlIncludes("bootstrapCdnCss", CssOrJs.Css, false);

            //VERIFY
            var lines = output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            lines.Length.ShouldEqual(1);
            lines[0].ShouldEqual("<link rel=\"stylesheet\" href='https://ajax.aspnetcdn.com/ajax/bootstrap/3.3.5/css/bootstrap.min.css' asp-fallback-href='url:wwwroot/css/css/bootstrap.min.css' asp-fallback-test-class=\"sr-only\" asp-fallback-test-property=\"position\" asp-fallback-test-value=\"absolute\" />\r");
        }
    }
}