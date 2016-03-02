# BundlerForBower (B4B)

**BundlerForBower**, or B4B for short, is a library that provides similar features to MVC's 
`BundleConfig` class, but are designed specifically to work with Bower. 

I recommend you look at the other 
[ReadMe file](https://github.com/JonPSmith/MvcUsingBower/blob/master/README.md)
for how you use Bower and Grunt/Gulp to create your production-ready CSS and JavaScript files.
The rest of this ReadMe assumes you know about that.

*NOTE: I plan to create a NuGet package containing all the files your need to use `BowerBundlerHelper.cs`
but I haven't done that yet. I learnt that I need to use a library myself for a while to check I have covered everything.*

## What does B4B consist of?

B4B consists of three parts:

1. A extension class called 
[BowerBundlerHelper](https://github.com/JonPSmith/MvcUsingBower/blob/master/WebApplication.Mvc5/App_Start/BowerBundlerHelper.cs)
which needs to be placed in you MVC application so that it has access to various MVC features.
I have placed this in the MVC's  directory as it replaces `BundlesConfig.cs`.  
*Note: this isn't really the right directory to put this class as it doesn't need any actions at startup.
However putting it in the `App_Start` directory in place of `BundlesConfig.cs` makes the change pretty obvious.
You can place this file anywhere inside your MVC application.*
2. A [BowerBundles.json](https://github.com/JonPSmith/MvcUsingBower/tree/master/WebApplication.Mvc5/App_Data)
file that contains the list of bundles and their files. This is used both by Grunt/Gulp 
to prepare the files and by B4B to deliver the correct files at run time.  
*Note: This file should be placed in the App_Data directory of a MVC5 project
(not sure where to put it in ASP.NET Core 1).*
3. A class library called 
[B4BCore](https://github.com/JonPSmith/MvcUsingBower/tree/master/B4BCore) which the
`BowerBundlerHelper` class uses to handling bundling.
This class library also contains a useful class called 
[`CheckBundles`](https://github.com/JonPSmith/MvcUsingBower/blob/master/B4BCore/CheckBundles.cs)
that is useful for checking all your bundles are up to date before your release anything to production.  
*Note: see section at the end of this ReadMe file for more on testing*


## Using BowerBundlerHelper to deliver bundles

The `BowerBundlerHelper` has two, very similar Html helper methods to MVC5's `Styles` 
and `Scripts` classes, but applied as extension methods on the HtmlHelper class.
They are:

1. **`@Html.HtmlCssCached("bundleName")`**, which is equivalent to `@Styles.Render("~/Content/bundleName")`
2. **`@Html.HtmlScriptsCached("bundleName")`**, which is equivalent to `@Scripts.Render("~/bundles/bundleName")`

It makes a decision to delivers individual files or the single minfied file that Grunt produced is defined by
whether the code was compiled in DEBUG mode or not. This feature can be overridden on each method by the 
optional 'forceState' parameter.

## Using BowerBundleHelper to deliver static files with cachebuster added

When delivering static files, e.g. images, you need to think about what happens if you change the
image. The problem is if you change the image content but not its name then is caching is turned on
the user's browser will use the old image, not the new image. 

The `BowerBundlerHelper` has a command to turn a normal file reference into one containing a cahce busting
value. For instance for an image you would use something like this in your razor view:

`<img src='@Html.AddCacheBusterCached("~/images/annoyed-cat.jpg")' />`

*See more in [Adding a cachebuster to other static files](#Adding-a-cachebuster-to-other-static-files) 
section.*

## BowerBundles.json file format

The BowerBundles.json file holds the data on what files are in reach bundle.
This file is used both by Grunt/Gulp to concatenate and minify the files 
and by B4B to deliver the right files at run time.

### 1. Delivery of files from your application

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

#### Steps to add a new files bundle

Here are the steps you would need to do to add a new bundle, which we will call **MyNewBundle** 
that contains two files: one called "Scripts/MyScript1.js" and the other called "Scripts/MyScript2.js".

  a. Add a bundle called **MyNewBundle** to the BowerBundles.json file, e.g.

```json
{
  "MyNewBundle": [
    "Scripts/MyScript1.js",
    "Scripts/MyScript2.js"
  ],
  //... rest of BowerBundles.json
}
```

  b. Update the **gruntfile.js** build script to include an extra concat and uglify task 
(or cssmin if your bundle was of CSS files), e.g.

```javascript
//...
concat: {
    MyNewBundle: {
        src: new Array('<%= properties.MyNewBundle %>'),
        dest: 'js/MyNewBundle.js'
    },
    //... other concat tasks
},

//...

uglify: {
    MyNewBundle: {
        src: 'js/MyNewBundle.js',
        dest: 'js/MyNewBundle.min.js'
    },
    //... other uglify tasks
},
```

c. Update the Update the **gruntfile.js** build task to include the **MyNewBundle** tasks

```javascript
    grunt.registerTask('build:js', [
        'concat:MyNewBundle', 'concat:standardLibsJs', 'concat:appLibsJs', 'concat:jqueryval',
        'uglify:MyNewBundle', 'uglify:standardLibsJs', 'uglify:appLibsJs', 'uglify:jqueryval']);

```
d. Run **build:js** to create the concatenated and minified files.  
e. Include the command `@Html.HtmlScriptsCached("MyNewBundle")` in one of your
MVC .cshtml files, say _Layout.cshtml if you want the library available in every
page, or in the specific page(s) that need it.

### 2. Delivery of files from Content Delivery Network (CDN), with fallback

B4B can also handle the supply of JavaScript via a Content Delivery Network (CDN).
You can define a CDN url, with fallback in the BowerBundles.json file using the following syntax 

```json
{
  "standardLibsCdnJs": [
    {
      "development": "lib/jquery/dist/jquery.js",
      "production": "jquery.min.js",
      "cdnUrl": "https://ajax.aspnetcdn.com/ajax/jquery/jquery-2.1.4.min.js",
      "cdnSuccessTest": "window.jQuery"
    },
    {
      "development": "lib/bootstrap/dist/js/bootstrap.js",
      "production": "bootstrap.min.js",
      "cdnUrl": "https://ajax.aspnetcdn.com/ajax/bootstrap/3.3.5/bootstrap.min.js",
      "cdnSuccessTest": "window.jQuery && window.jQuery.fn && window.jQuery.fn.modal"
    }
  ]
}
```

The properties are:

1. **development**: This is where the non-minified file comes from. 
This *can* be inside the library is you only run debug mode localling (see the first example), 
but be warned: the lib directory is not deployed so would fail if debug mode. 
If this is a problem then simply copy the files to a production directory.
2. **production**: This is the name of the backup file to send if the cdnSuccessTest 
shows that the CDN failed to load the file. It assumes it is the the default js/ or css/
file, depending on the file type.
3. **cdnUrl**: This is the CDN url where it will try to load the file from.
4. **cdnSuccessTest**: This is a JavaScript test that should return true if the CND load was successful.
The two examples above give you an idea of what to use.

In Debug mode it simply outputs a link with the 'development' file in. In non-Debug/Production mode
it outputs the string as defined by the 

An example of the output of the Jquery example about would be:

```html
<script src='https://ajax.aspnetcdn.com/ajax/jquery/jquery-2.1.4.min.js'></script>
<script>(window.jQuery||document.write(\"<script 'src=~/js/jquery.min.js?v=SnW8SeyCxQMkwmWggnI6zdSJoIVYPkVYHyM4jpW3jaQ'));</script>
```

*NOTE: At the moment I have not implemented CSS CDN support. The testing code is
quite complex and I left it out for now. If someone wants to add then let me know.
Also note that the ASP.NET Core 1 version does support CSS CDNs.*

#### <a name="NotesOnCdn"></a>Some notes CDN and copying production files and cachebusting
You will see in the sample application 
[GruntFile.js](https://github.com/JonPSmith/MvcUsingBower/blob/master/WebApplication.Mvc5/gruntfile.js)
file that there is a section for copying files. 
This is needed as if the CDN fails and you have to deliver a file locally 
(If you want to test that this works then set the `cdnUrl` ro a bad URL and you should 
see it loading the file from the `production` source).

The other point about copying these files is that you can give them a name that includes the
version number. If you do this then you can turn off cachebusting for CDN fallback files
by overriding the `JsCdnHtmlFormatString` property using your own bundlerForBower.json
(see next section) and removing the string `?v={cachebuster}` from the normal string.

#### Steps to add a CDN files bundle

Here are the steps you would need to do to add a new bundle, which we will call **MyCDNBundle** 
that contains two files: one called "Scripts/MyScript1.js" and the other called "Scripts/MyScript2.js".

a. Add a bundle called **MyCDNBundle** to the BowerBundles.json file. 
The format is as shown in point 2 above.

b. Update the **gruntfile.js** copy the minified file out of the library, which is not published to
the live site, into a suitable directory that is published. In this case I use js/.  

*Notes:* 

 1. You need separate copy for each file in the CDN bundle. 
 2. You only need to do the copy if the library changes.
 3. See [some notes on CND](#NotesOnCdn) 
about naming the files with the version instead of using cachebusting.

The build script to do this looks like this:

```javascript

copy: {
    jQueryCdn: {
        src: 'lib/jquery/dist/jquery.min.js',
        dest: 'js/jquery.min.js'
    },
    bootstrapCdn: {
        src: 'lib/bootstrap/dist/js/bootstrap.min.js',
        dest: 'js/bootstrap.min.js'
    },
    //... other copy tasks, like coping the fonts
```

c. Run each of the Grunt **copy:???** tasks you created in step b.  
e. Include the command `@Html.HtmlScriptsCached("MyCDNBundle")` in one of your
MVC .cshtml files - most likely _Layout.cshtml.

## Adding a cachebuster to other static files

As well as bundles B4B can help with individual static files, e.g. images.
These is a command called `@Html.AddCacheBusterCached("~/images/my-cat-image.jpg")`.
In this case a checksum of the file will be calculated based on its content and
added as a cachebuster value. 

In the case where you can pre-calculate the cachebsuter value then there is a second version
which looks like this `@Html.AddCacheBusterCached("~/js/jquery.js", "2.1.4")`.

The way that the cachebuster is applied is set by the `StaticFileCaching` property in the B4B config.
This means you can use different ways of applying caching busting by adding your own `BundlerForBower.json` file
with a different cache busting scheme.

By default B4B uses the standard ASP.NET approach of adding a suffix, e.g. 

`http://localhost:61427/images/annoyed-cat.jpg?v=xKyBfWHW-GTt8h8i8iy9p5h4Gx9EszkidtaUrkwVwvY`

## Optional bundlerForBower.json file format

B4B uses a json file to set up various features and settings, and you can override these settings,
say the names/locations of the directories where the minified files are found, then do this 
by placing a file called `BundlerForBower.json` in the MVC App_Data directory.

The default values can be found in the file 
[defaultConfig.json](https://github.com/JonPSmith/MvcUsingBower/blob/master/B4BCore/Internal/defaultConfig.json).

You can override just the properties you want to change and the rest. See the following examples:

1. Override just the name of the bundle file [see this file](https://github.com/JonPSmith/MvcUsingBower/blob/master/B4BCore/Internal/defaultConfig.json)
2. Override all the properties - see 
[ASPNET Core 1 Config/bundlerForBower.json](https://github.com/JonPSmith/MvcUsingBower/blob/master/Tests/TestData/ASPNET%20Core%201%20Config/bundlerForBower.json)

This last example shows how you would change the setting to match what ASP.NET Core 1 would need.

## Using `CheckBundles` to check your bundles are up to date

When using a build tool you can forget you have made a change and forget to update the production code
(well, I have done that!). Therefore I have provided a 
[`CheckBundles`](https://github.com/JonPSmith/MvcUsingBower/blob/master/B4BCore/CheckBundles.cs)
that provides a comprehensive set of tests to check the bundles are correct and up to date.

If you want to use this in your Unit Tests then you need to create the `CheckBundles` in such
a way that it knows where you MVC project is. If you are using the standard setup then the 
ctor can work this out by giving a type that is in your MVC application, e.g.

```csharp
var checker = new CheckBundles(typeof(BowerBundlerHelper));
```

*Note: there are other version of the `CheckBundles` ctor if you have an unusual setup. 
Please consult the code.*

Once you have created the checker there are three methods you can call:

1. `checker.CheckSingleBundleIsUpToDate(string bundleName)`. 
This checks an individual bundle by name and returns an empty list if everything is OK or a 
list of error strings if it found something wrong. The tests for a normal bundle is:
  - Does the bundle contain any file references?
  - Do any of those file references use a search string that B4B does not support, e.g /**/ 
  - Do all of those files and their directories exist?
  - Does the concat file exist? (can be turned off via ctor param if not using concat files).
  - Was the concat file updated more recently than all the files referenced in the bundle?
  - Does the minified file exist?
  - Was the minified file updated more recently than the concat file (or all the files referenced if no concat)?  
  CDN: The tests for a CND bundle are:
  - Does the configuration support CDN for this file type?
  - Does your CDN bundle contain all the properties that the CDN format string needs?
  - Does any of the file definitions contain a search pattern? That is not allowed.
  - Does the 'Development' file and the 'Production' file exist?
2. `checker.CheckAllBundlesAreUpToDate()` is more useful as it checks ALL the bundles 
found in the BowerBundles.json file in one go. It uses `checker.CheckSingleBundleIsUpToDate(string bundleName)`
and returns an empty list if everything is OK or a list of error strings if it found something wrong.
3. `checker.CheckBundleFileIsNotNewerThanMinifiedFiles()`. This return null if all the minified files
are older than the file that defines what is in the bundle. Otherwise it returns a list of all
the minified files that need updating.


You can see an example of using these commands in an NUnit based Unit Test
in [this Unit Test class](https://github.com/JonPSmith/MvcUsingBower/blob/master/Tests/UnitTests/Test30CheckBundlesMvc.cs)
.






