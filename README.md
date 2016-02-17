# Mvc With Bower

This is an example of an existing [ASP.NET MVC5](http://www.asp.net/mvc/mvc5) project 
that has been converted over to use [Bower](http://bower.io/) for web package management 
and [Grunt](http://gruntjs.com/) for web build automation issues. This means that:

- Bower, not NuGet looks after any web packages, e.g. JQuery, BootStrap.
- Grunt, not BundleConfig.cs looks after the concatination and minification of CSS and JavaScript.
- Grunt, not Web Essentials looks after any converting, compiling of JavaScript, template, images etc.
- The application tries to follow the design approach that 
[ASP.NET 5](https://docs.asp.net/en/latest/conceptual-overview/aspnet.html) is taking 
so that updating to ASP.NET5 should not change the way you handle front-end code that much.

**I have written an article called 
[Converting your ASP.NET MVC5 application to use Bower, Grunt and Gulp](http://www.thereformedprogrammer.net/converting-your-asp-net-mvc5-application-to-use-bower-grunt-and-gulp/)
which goes into much more detail on the whole conversion process. 
Please read this for an overview.**

The rest of this ReadMe file is about how to use this package if you wish to fork it.

It also includes a useful library which I call *BundlerForBower*, or B4B for short.
This provides similar features to MVC's `BundleConfig` class, but are designed specifically
to work with Bower.
*Full information on how to setup and use BundlerForBower is available in the 
[ReadMe file](https://github.com/JonPSmith/MvcUsingBower/blob/master/B4BCore/README.md) 
in the [B4BCore project](https://github.com/JonPSmith/MvcUsingBower/tree/master/B4BCore).*

This sample application has a [article](#) which describes in detail the steps I took
to convert a new MVC5 web application over to using Bower and Grunt.

## Points to note about this application

If you are downloading or just looking at this application there are a few important 
things you need to know.

### 1. I have not included the web packages in source control
Leaving packages out of source control is best practice, but it does mean you won't 
see the directory 'lib'. If you download this sample application then you need to 
right-click the bower.json file and selected **Restore Packages** to load the 
web packages.

### 2. I have not included the Grunt modules in source control
Again, best practice to leave them out. If you download this sample application then 
you need to right-click the packages.json file and selected **Restore Packages**
to load the web packages.

### 3. You need to turn on 'Show All Files' to see everything
The directory holding the web paackages and the Grunt modules should not be
deployed to your web site, so I left them out of the project. Therefore you
won't see them in the Visual Studio **Solution Explorer** unless you turn on the Explorers'
**Show All Files** feature (top menu to the rigth)

## If you want to change/run the application

You can run the MVC application as soon as you have downloaded it as I have run
the Grunt commands to create the CSS and JavaScript files you need. 

If you want to changes things then here are a few pointers:

### 1. App_Data/BowerBundles.json

the 
file holds lists of the CSS and JavaScript files you want to use in your application. 
It is used by [gruntfile.js](https://github.com/JonPSmith/MvcUsingBower/blob/master/WebApplication.Mvc5/gruntfile.js)
when concatenating files and by 
[BowerBundlerHelper.cs](https://github.com/JonPSmith/MvcUsingBower/blob/master/WebApplication.Mvc5/App_Start/BowerBundlerHelper.cs)
when it is delivering individual files in Debug mode.

The current [BowerBundles.json](https://github.com/JonPSmith/MvcUsingBower/blob/master/WebApplication.Mvc5/App_Data/BowerBundles.json)
and gruntfile.js [gruntfile.js](https://github.com/JonPSmith/MvcUsingBower/blob/master/WebApplication.Mvc5/gruntfile.js) 
are fairly simple and just has one CSS and three JavaScript groups of files: 

1. **standardLibsJs**: which holds the standard JavaScript libraries like JQuery that rarely change
2. **appLibsJs**: which holds JavaScript files specifically written for this app that will change a lot.
3. **jqueryval**: which you only include when it is needed, e.g. in views with forms in them.

*WARNING: Its tempting to add comments to the BowerBundles.json, but `grunt.file.readJSON`
does not handle comments in JSON and fails.*

### 2. BundlerForBower (B4B)

In the article I explain that we cannot use some of the new ASP.NET5 tags so
I had to come up with another way of delivering either single files in Development 
mode and concatenated/minified files in Release mode.
[BowerBundlerHelper.cs](https://github.com/JonPSmith/MvcUsingBower/blob/master/WebApplication.Mvc5/App_Start/BowerBundlerHelper.cs)
is the front-end that lives in the MVC app so that it can pick up MVC features and also the debug mode.

`BowerBundlerHelper.cs` is a static extension class on the MVC `HtmlHelper` class. 
There are two calls that can be used in a MVC view:

1. **`@Html.HtmlCssCached("bundleName")`**, which is equivalent to `@Styles.Render("~/Content/bundleName")`
2. **`@Html.HtmlScriptsCached("bundleName")`**, which is equivalent to `@Scripts.Render("~/bundles/bundleName")`

It makes a decision to delivers individual files or the single minfied file that Grunt produced is defined by
whether the code was compiled in DEBUG mode or not. This feature can be overridden on each method by the 
optional 'forceState' parameter.

*Note: If you don't like `BowerBundlerHelper.cs` then the article give you an alternative
using MVC5's own `BundleConfig.cs`*

## Full information on how to setup and use BundlerForBower is available in the [ReadMe file](https://github.com/JonPSmith/MvcUsingBower/blob/master/B4BCore/README.md) in the [B4BCore project](https://github.com/JonPSmith/MvcUsingBower/tree/master/B4BCore)


