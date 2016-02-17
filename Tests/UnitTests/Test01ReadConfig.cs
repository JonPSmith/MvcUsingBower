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
using B4BCore.Internal;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test01ReadConfig
    {

        [Test]
        public void CheckReadDefaultConfigOk()
        {
            //SETUP 

            //ATTEMPT
            var config = ConfigInfo.ReadConfig(null);

            //VERIFY
            config.ToString().ShouldEqual(
                "BundlesFileName: BowerBundles.json, JsDirectory: js/, " +
                "JsDebugHtmlFormatString: <script src='{fileUrl}'></script>, JsNonDebugHtmlFormatString: <script src='{fileUrl}?v={cachebuster}'></script>, "+
                "CssDirectory: css/, " +
                "CssDebugHtmlFormatString: <link href='{fileUrl}' rel='stylesheet'/>, CssNonDebugHtmlFormatString: <link href='{fileUrl}?v={cachebuster}' rel='stylesheet'/>"
                );
        }

        [Test]
        public void CheckReadConfigAspNetCore1ConfigOk()
        {
            //SETUP 

            //ATTEMPT
            var config = ConfigInfo.ReadConfig(TestFileHelpers.GetTestFileFilePath("ASPNET Core 1 Config/bundlerForBower.json"));

            //VERIFY
            config.ToString().ShouldEqual(
                "BundlesFileName: AspNetCore1BowerBundles.json, JsDirectory: wwwroot/js/, " +
                "JsDebugHtmlFormatString: <script src='{fileUrl}'></script>, JsNonDebugHtmlFormatString: <script src=\"{fileUrl}\" asp-append-version=\"true\"></script>, " +
                "CssDirectory: wwwroot/css/, " +
                "CssDebugHtmlFormatString: <link href='{fileUrl}' rel='stylesheet'/>, " +
                "CssNonDebugHtmlFormatString: <link href=\"{fileUrl}\" asp-append-version=\"true\" rel=\"stylesheet\"/>"
                );
        }

        [Test]
        public void CheckReadConfigOverrideJustBundlesFileNameOk()
        {
            //SETUP 

            //ATTEMPT
            var config = ConfigInfo.ReadConfig(TestFileHelpers.GetTestFileFilePath("bundlerForBower01*.json"));

            //VERIFY
            config.ToString().ShouldEqual(
                "BundlesFileName: newName.json, JsDirectory: js/, " +
                "JsDebugHtmlFormatString: <script src='{fileUrl}'></script>, JsNonDebugHtmlFormatString: <script src='{fileUrl}?v={cachebuster}'></script>, " +
                "CssDirectory: css/, " +
                "CssDebugHtmlFormatString: <link href='{fileUrl}' rel='stylesheet'/>, CssNonDebugHtmlFormatString: <link href='{fileUrl}?v={cachebuster}' rel='stylesheet'/>"
                );
        }

    }
}