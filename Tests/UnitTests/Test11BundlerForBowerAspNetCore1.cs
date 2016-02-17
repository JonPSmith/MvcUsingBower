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
    public class Test11BundlerForBowerAspNetCore1
    {

        //-------------------------------------------------------------


        [Test]
        public void TestBundlerForBowerCssDebugOk()
        {
            //SETUP 
            var b4b = new BundlerForBower( s => "url:" + s.Substring(2), B4BSetupHelper.GetActualFilePathFromVirtualPath(), B4BSetupHelper.GetDirRelToTestDirectory("ASPNET Core 1 Config\\"));

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
            var b4b = new BundlerForBower(s => "url:" + s.Substring(2), B4BSetupHelper.GetActualFilePathFromVirtualPath(), B4BSetupHelper.GetDirRelToTestDirectory("ASPNET Core 1 Config\\"));

            //ATTEMPT
            var output = b4b.CalculateHtmlIncludes("mainCss", CssOrJs.Css, false);

            //VERIFY
            output.ShouldEqual("<link href=\"url:wwwroot/css/mainCss.min.css\" asp-append-version=\"true\" rel=\"stylesheet\"/>\r\n");
        }

        [Test]
        public void TestBundlerForBowerJsDebugOk()
        {
            //SETUP 

            var b4b = new BundlerForBower(s => "url:" + s.Substring(2), B4BSetupHelper.GetActualFilePathFromVirtualPath(), B4BSetupHelper.GetDirRelToTestDirectory("ASPNET Core 1 Config\\"));

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
            var b4b = new BundlerForBower(s => "url:" + s.Substring(2), B4BSetupHelper.GetActualFilePathFromVirtualPath(), B4BSetupHelper.GetDirRelToTestDirectory("ASPNET Core 1 Config\\"));

            //ATTEMPT
            var output = b4b.CalculateHtmlIncludes("appLibsJs", CssOrJs.Js, false);

            //VERIFY
            output.ShouldEqual("<script src=\"url:wwwroot/js/appLibsJs.min.js\" asp-append-version=\"true\"></script>\r\n");
        }
    }
}