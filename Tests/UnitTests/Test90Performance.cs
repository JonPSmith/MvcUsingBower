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

using B4BCore;
using B4BCore.Internal;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test90Performance
    {
        [Test]
        public void InitialCallOk()
        {
            //SETUP 

            //ATTEMPT
            using (new TimerToConsole("Read default config"))
            {
                ConfigInfo.ReadConfig(null);
            }
            //VERIFY           
        }

        [Test]
        public void CheckReadDefaultConfigOk()
        {
            //SETUP 

            //ATTEMPT
            using (new TimerToConsole("Read default config"))
            {
                ConfigInfo.ReadConfig(null);
            }
            //VERIFY           
        }

        [Test]
        public void CheckReadDefaultConfigAndUserConfigOk()
        {
            //SETUP 

            //ATTEMPT
            using (new TimerToConsole("Read default config and user config"))
            {
                ConfigInfo.ReadConfig(TestFileHelpers.GetTestFileFilePath("bundlerForBower02*.json"));
            }
            //VERIFY           
        }

        [Test]
        public void TestReadBundleFileOk()
        {
            //SETUP 

            //ATTEMPT
            using (new TimerToConsole("Read bundleFile"))
            {
                new ReadBundleFile(TestFileHelpers.GetTestFileFilePath("BowerBundles01*.json"));
            }
            //VERIFY
        }

        [Test]
        public void TestBundlerForBowerCreateOk()
        {
            //SETUP 

            //ATTEMPT
            using (new TimerToConsole("create BundlerForBower"))
            {
                var b4b = new BundlerForBower(s => "url:" + s.Substring(2), B4BSetupHelper.GetActualFilePathFromVirtualPath(), B4BSetupHelper.GetDirRelToTestDirectory("NoConfig\\"));
            }

            //VERIFY
        }

        [Test]
        public void TestBundlerForBowerCssDebugOk()
        {
            //SETUP 
            var b4b = new BundlerForBower(s => "url:" + s.Substring(2), B4BSetupHelper.GetActualFilePathFromVirtualPath(), B4BSetupHelper.GetDirRelToTestDirectory("NoConfig\\"));

            //ATTEMPT
            using (new TimerToConsole("CalculateHtmlIncludes Debug"))
            {
                var output = b4b.CalculateHtmlIncludes("mainCss", CssOrJs.Css, true);
            }

            //VERIFY
        }

        [Test]
        public void TestBundlerForBowerCssNonDebugOk()
        {
            //SETUP 
            var b4b = new BundlerForBower(s => "url:" + s.Substring(2), B4BSetupHelper.GetActualFilePathFromVirtualPath(), B4BSetupHelper.GetDirRelToTestDirectory("NoConfig\\"));

            //ATTEMPT
            using (new TimerToConsole("CalculateHtmlIncludes NonDebug"))
            {
                var output = b4b.CalculateHtmlIncludes("mainCss", CssOrJs.Css, false);
            }

            //VERIFY
        }

    }
}