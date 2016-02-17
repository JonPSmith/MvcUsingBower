#region licence
// ======================================================================================
// Mvc5UsingBower - An example+library to allow an MVC project to use Bower and Grunt
// Filename: Test01ReadConfig.cs
// Date Created: 2016/02/17
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
                "JsCdnHtmlFormatString: <script src='{cdnUrl}'></script><script>({cdnSuccessTest}||document.write(\"\\x3Cscript src='{fileUrl}?v={cachebuster}'\\x3C/script>\"));</script>, " +
                "CssDirectory: css/, " +
                "CssDebugHtmlFormatString: <link href='{fileUrl}' rel='stylesheet'/>, CssNonDebugHtmlFormatString: <link href='{fileUrl}?v={cachebuster}' rel='stylesheet'/>, "+
                "CssCdnHtmlFormatString: "
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
                "JsCdnHtmlFormatString: <script src='{cdnUrl}' asp-fallback-src='{fileUrl}' asp-fallback-test='{cdnSuccessTest}'></script>, " +
                "CssDirectory: wwwroot/css/, " +
                "CssDebugHtmlFormatString: <link href='{fileUrl}' rel='stylesheet'/>, " +
                "CssNonDebugHtmlFormatString: <link href=\"{fileUrl}\" asp-append-version=\"true\" rel=\"stylesheet\"/>, " +
                "CssCdnHtmlFormatString: <link rel=\"stylesheet\" href='{cdnUrl}' asp-fallback-href='{fileUrl}' asp-fallback-test-class=\"{testClass}\" asp-fallback-test-property=\"{testProperty}\" asp-fallback-test-value=\"{testValue}\" />"
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
                "JsCdnHtmlFormatString: <script src='{cdnUrl}'></script><script>({cdnSuccessTest}||document.write(\"\\x3Cscript src='{fileUrl}?v={cachebuster}'\\x3C/script>\"));</script>, " +
                "CssDirectory: css/, " +
                "CssDebugHtmlFormatString: <link href='{fileUrl}' rel='stylesheet'/>, CssNonDebugHtmlFormatString: <link href='{fileUrl}?v={cachebuster}' rel='stylesheet'/>, " +
                "CssCdnHtmlFormatString: "
                );
        }

        [Test]
        public void CheckReadConfigOverrideCssCndToNullOk()
        {
            //SETUP 

            //ATTEMPT
            var config = ConfigInfo.ReadConfig(TestFileHelpers.GetTestFileFilePath("bundlerForBower02*.json"));

            //VERIFY
            config.ToString().ShouldEqual(
                "BundlesFileName: newName.json, JsDirectory: js/, " +
                "JsDebugHtmlFormatString: <script src='{fileUrl}'></script>, JsNonDebugHtmlFormatString: <script src='{fileUrl}?v={cachebuster}'></script>, " +
                "JsCdnHtmlFormatString: <script src='{cdnUrl}'></script><script>({cdnSuccessTest}||document.write(\"\\x3Cscript src='{fileUrl}?v={cachebuster}'\\x3C/script>\"));</script>, " +
                "CssDirectory: css/, " +
                "CssDebugHtmlFormatString: <link href='{fileUrl}' rel='stylesheet'/>, CssNonDebugHtmlFormatString: <link href='{fileUrl}?v={cachebuster}' rel='stylesheet'/>, " +
                "CssCdnHtmlFormatString: "
                );
        }
    }
}