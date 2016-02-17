# BundlerForBower (B4B)

**BundlerForBower**, or B4B for short, is a library that provides similar features to MVC's 
`BundleConfig` class, but are designed specifically to work with Bower. 

I recommend you look at the other 
[ReadMe file](https://github.com/JonPSmith/Mvc5WithBowerAndGrunt/blob/master/README.md)
for how you use Bower and Grunt/Gulp to create your production-ready CSS and JavaScript files.
The rest of this ReadMe assumes you know about that.

*NOTE: I plan to create a NuGet package containing all the files your need to use `BowerBundlerHelper.cs`
but I haven't done that yet. I learnt that I need to use a library myself for a while to check I have covered everything.*

## What does B4B consist of?

B4B consists of three parts:

1. A extension class called 
[BowerBundlerHelper](https://github.com/JonPSmith/Mvc5WithBowerAndGrunt/blob/master/Mvc5WithBowerAndGrunt/App_Start/BowerBundlerHelper.cs)
which needs to be placed in you MVC application so that it has access to various MVC features.
2. A [BowerBundles.json](https://github.com/JonPSmith/Mvc5WithBowerAndGrunt/tree/master/Mvc5WithBowerAndGrunt/App_Data)
file that contains the list of bundles and their files. This is used both by Grunt/Gulp 
to prepare the files and by B4B to deliver the correct files at run time.
*Note: This file should be placed in the App_Data directory of a MVC5 project
(not sure where to put it in ASP.NET Core 1).*
3. A class library called 
[B4BCore](https://github.com/JonPSmith/Mvc5WithBowerAndGrunt/tree/master/B4BCore) which the
`BowerBundlerHelper` class uses to handling bundling.
This class library also contains a useful class called 
[`CheckBundles`](https://github.com/JonPSmith/Mvc5WithBowerAndGrunt/blob/master/B4BCore/CheckBundles.cs)
that is useful for checking all your bundles are up to date before your release anything to production.


## Using BowerBundlerHelper to deliver bundles

The `BowerBundlerHelper` has two, very similar Html helper methods to MVC5's `Styles` 
and `Scripts` classes, but applied as extension methods on the HtmlHelper class.
They are:

1. **`@Html.HtmlCssCached("bundleName")`**, which is equivalent to `@Styles.Render("~/Content/bundleName")`
2. **`@Html.HtmlScriptsCached("bundleName")`**, which is equivalent to `@Scripts.Render("~/bundles/bundleName")`

It makes a decision to delivers individual files or the single minfied file that Grunt produced is defined by
whether the code was compiled in DEBUG mode or not. This feature can be overridden on each method by the 
optional 'forceState' parameter.

## BowerBundles.json file format

The BowerBundles.json file holds the data on what files are in reach bundle.
This file is used both by Grunt/Gulp to concatenate and minify the files 
and by B4B to deliver the right files at run time.

The file format is a json object, with each bundle being a property that contains an array of
relative file references to each file you want in a bundle. Here is a simple example:

```json
{
  "mainCss": [
    "lib/bootstrap/dist/css/bootstrap.css",
    "Content/site.css"
  ],
  "standardLibsJs": [
    "lib/jquery/dist/jquery.js",
    "lib/bootstrap/dist/js/bootstrap.js"
  ],
  "appLibsJs": [
    "Scripts/MyScripts/*.js"
  ]
}
```

The name of the property, e.g. `mainCss` is the name of the bundle and the
array is the list of files in order that should be included.

As you can see you can specify an exact file, or add a search string like `"Scripts/MyScripts/*.js"`,
but the order is then dependant on the name and some (many) files need to be loaded in a specific order.
Directory searches can included, e.g `"Scripts/*/*.js"`,
but at the moment I have not implemented the Grunt/Gulp's /**/
search all directories and subdirectories feature.

## Optional bundlerForBower.json file format

B4B uses a json file to set up various features and settings, and you can override these settings,
say the names/locations of the directories where the minified files are found, then do this 
by placing a file called `BundlerForBower.json` in the MVC App_Data directory.

The default values can be found in the file 
[defaultConfig.json](https://github.com/JonPSmith/Mvc5WithBowerAndGrunt/blob/master/B4BCore/Internal/defaultConfig.json).

You can override just the properties you want to change and the rest. See the following examples:

1. Override just the name of the bundle file [see this file](https://github.com/JonPSmith/Mvc5WithBowerAndGrunt/blob/master/B4BCore/Internal/defaultConfig.json)
2. Override all the properties - see 
[ASPNET Core 1 Config/bundlerForBower.json](https://github.com/JonPSmith/Mvc5WithBowerAndGrunt/blob/master/Tests/TestData/ASPNET%20Core%201%20Config/bundlerForBower.json)

This last example shows how you would change the setting to match what ASP.NET Core 1 would need.

## Using `CheckBundles` to check your bundles are up to date

When using a build tool you can forget you have made a change and forget to update the production code
(well, I have done that!). Therefore I have provided a fairly full checking library to check that the
a file that makes up a bundle is not newer that the minified file (plus other check like not missing a file).

If you want to use this in your Unit Tests then you need to create the `CheckBundles` in such
a way that it knows where you MVC project is. If you are using the standard setup then the 
ctor can work this out by giving a type that is in your MVC application, e.g.

```csharp
var checker = new CheckBundles(typeof(HomeController));
```

*Note: there are other version of the `CheckBundles` ctor if you have an unusual setup. 
Please consult the code.*

Once you have created the checker there are three methods you can call:

1. `checker.CheckSingleBundleIsValid(string bundleName)`. 
This checks a given bundle and returns an empty list if everything is OK or a 
list of error strings if it found something wrong. The tests are:
  - Does the bundle contain any file references?
  - Do any of those file references use a search string that B4B does not support, e.g /**/ 
  - Do all of those files and their directories exist?
  - Does the concat file exist? (can be turned off via ctor param if not using concat files).
  - Was the concat file updated more recently than all the files referenced in the bundle?
  - Does the minified file exist?
  - Was the minified file updated more recently than the concat file (or all the files referenced if no concat)?
2. `checker.CheckAllBundlesAreValid()`. This checks all the bundles found in the 
BowerBundles.json file using `checker.CheckSingleBundleIsUpToDate(string bundleName)`.
It returns an empty list if everything is OK or a list of error strings if it found something wrong.
3. `checker.CheckBundleFileIsNotNewerThanMinifiedFiles()`. This return null if all the minified files
are older than the file that defines what is in the bundle. Otherwise it returns a list of all
the minified files that need updating.


In addition the [Tests Project](https://github.com/JonPSmith/Mvc5WithBowerAndGrunt/tree/master/Tests)
has some tests for the B4BCore project, including examples of 
[`CheckBundles`](https://github.com/JonPSmith/Mvc5WithBowerAndGrunt/blob/master/B4BCore/CheckBundles.cs)
.






