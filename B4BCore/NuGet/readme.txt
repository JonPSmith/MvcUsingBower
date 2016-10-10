Bundler for Bower
================

You have installed Bundler For Bower, B4B for short. This will have installed a .dll called B4BCore which does all the work.
You can use it in two places: a) an ASP.NET MVC5 application and/or b) your Unit Tests.

ASP.NET MVC5 application - MORE WORK TO DO
------------------------

If you want to use it with an ASP.NET MVC5 application you need to download the BowerBundlerHelper.cs into your MVC4 application.
I normally put it in App_Start becasue that is were BundleConfig.cs, which B4B replaces, lives. The file can be downloaded from

https://raw.githubusercontent.com/JonPSmith/MvcUsingBower/master/WebApplication.Mvc5/App_Start/BowerBundlerHelper.cs

You also need to create a `BowerBudles.json` file. Here is a link to an example 
https://raw.githubusercontent.com/JonPSmith/MvcUsingBower/master/WebApplication.Mvc5/App_Data/BowerBundles.json

Then you need to set up your Bower libraries and the Grunt/Glup commands. I have written an article on
swapping over to Bower and Grunt/Gulp. You can find it here:
http://www.thereformedprogrammer.net/converting-your-asp-net-mvc5-application-to-use-bower-grunt-and-gulp/

Have a look at the example MVC5 application which is parts of the project:
https://github.com/JonPSmith/MvcUsingBower/tree/master/WebApplication.Mvc5

Files to look at are:

- App_Data BowerBundles.json        -- NEW: this will hold the files you want to bundle for production
- .bowerrc                          -- NEW: this has the name of the file to put the bower file in. Needs to be short
- bower.json                        -- NEW: this contains the libraries you want to download
- package.json                      -- NEW: contains the grunt/gulp files for automation
- gruntfile.js                      -- NEW: my version of automation, which uses Grunt
- Views/Shared/_Layout.cshtml       -- UPDATE: this is changed to use the B4B bundle features.


UNIT TESTS
----------

B4B allows you to check that the minified js/css files are up to date. - useful before you deploy sometthing.
To use `CheckBundles` you don't need to add any more files to your Unit Test. You should read the Unit Tests section of the B4BCore README file. See:
https://github.com/JonPSmith/MvcUsingBower/blob/master/B4BCore/README.md#using-checkbundles-to-check-your-bundles-are-up-to-date



