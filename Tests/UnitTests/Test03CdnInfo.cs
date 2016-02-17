#region licence
// ======================================================================================
// Mvc5WithBowerAndGrunt - An example of how to change a MVC5 project to Bower and Grunt
// Filename: Test03CdnInfo.cs
// Date Created: 2016/02/16
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// ======================================================================================
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using B4BCore.Internal;
using NUnit.Framework;
using Tests.Helpers;
using Newtonsoft.Json.Linq;

namespace Tests.UnitTests
{
    public class Test03CdnInfo
    {
        private const string JsCdnHtmlInclude = 
            "<script src='{cdnUrl}'></script><script>{cdnSuccessTest}||document.write(\"\\x3Cscript src='{fileUrl}?v={cachebuster}'>\\x3C/script>\")</script>";

        [Test]
        public void TestFormCdnInfoOk()
        {
            //SETUP 
            var jObject = JObject.Parse(@"{
      ""development"": ""lib/jquery/dist/jquery.js"",
      ""production"": ""jquery.min.js"",
      ""cdnUrl"": ""https://ajax.aspnetcdn.com/ajax/jquery/jquery-2.1.4.min.js"",
      ""cdnSuccessTest"": ""window.jQuery""
    }");

            //ATTEMPT
            var cdn = new CdnInfo("Unit Test", jObject);

            //VERIFY
            cdn.Development.ShouldEqual("lib/jquery/dist/jquery.js");
            cdn.Production.ShouldEqual("jquery.min.js");
        }

        [Test]
        public void TestBuildCdnIncludeOk()
        {
            //SETUP 
            var jObject = JObject.Parse(@"{
      ""development"": ""lib/jquery/dist/jquery.js"",
      ""production"": ""jquery.min.js"",
      ""cdnUrl"": ""https://ajax.aspnetcdn.com/ajax/jquery/jquery-2.1.4.min.js"",
      ""cdnSuccessTest"": ""window.jQuery""
    }");
            var cdn = new CdnInfo("Unit Test", jObject);

            //ATTEMPT
            var html =
                cdn.BuildCdnIncludeString( JsCdnHtmlInclude, "http:localhost:1234/js/jquery.min.js", () => "123");

            //VERIFY
            html.ShouldEqual("<script src='https://ajax.aspnetcdn.com/ajax/jquery/jquery-2.1.4.min.js'></script><script>window.jQuery||document.write(\"\\x3Cscript src='http:localhost:1234/js/jquery.min.js?v=123'>\\x3C/script>\")</script>");
        }

        [Test]
        public void TestValidateCdnInfoOk()
        {
            //SETUP 
            var jObject = JObject.Parse(@"{
      ""development"": ""lib/jquery/dist/jquery.js"",
      ""production"": ""jquery.min.js"",
      ""cdnUrl"": ""https://ajax.aspnetcdn.com/ajax/jquery/jquery-2.1.4.min.js"",
      ""cdnSuccessTest"": ""window.jQuery""
    }");
            var cdn = new CdnInfo("Unit Test", jObject);

            //ATTEMPT
            var missingParams = cdn.FindMissingPropertiesNeededByHtmlInclude(JsCdnHtmlInclude);

            //VERIFY
            missingParams.Any().ShouldEqual(false);
        }

        //---------------------------------------------------------------------
        //errors

        [Test]
        public void TestValidateCdnInfoMissingCdnSuccessTest()
        {
            //SETUP 
            var jObject = JObject.Parse(@"{
      ""development"": ""lib/jquery/dist/jquery.js"",
      ""production"": ""jquery.min.js"",
      ""cdnUrl"": ""https://ajax.aspnetcdn.com/ajax/jquery/jquery-2.1.4.min.js""
    }");
            var cdn = new CdnInfo("Unit Test", jObject);

            //ATTEMPT
            var missingParams = cdn.FindMissingPropertiesNeededByHtmlInclude(JsCdnHtmlInclude);

            //VERIFY
            CollectionAssert.AreEquivalent(new string[] { "cdnSuccessTest" }, missingParams);
        }


    }
}