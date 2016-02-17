#region licence
// ======================================================================================
// Mvc5UsingBower - An example+library to allow an MVC project to use Bower and Grunt
// Filename: FileTypeConfigInfo.cs
// Date Created: 2016/02/17
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// ======================================================================================
#endregion
namespace B4BCore.Internal
{
    internal class FileTypeConfigInfo
    {
        public const string FileUrlParam = "{fileUrl}";
        public const string CachebusterParam = "{cachebuster}";

        public FileTypeConfigInfo(string directory, string debugHtmlFormatString, string nonDebugHtmlFormatString, string cdnHtmlFormatString)
        {
            Directory = directory;
            DebugHtmlFormatString = debugHtmlFormatString;
            NonDebugHtmlFormatString = nonDebugHtmlFormatString;
            CdnHtmlFormatString = cdnHtmlFormatString;
        }

        /// <summary>
        /// This holds the Directory path of where the concatenated and minified files are
        /// </summary>
        public string Directory { get; private set; }

        /// <summary>
        /// This holds the html to include the given file in the page when in debug mode 
        /// It contains one parameter called {fileUrl}, which takes the name of the file to include
        /// </summary>
        public string DebugHtmlFormatString { get; private set; }

        /// <summary>
        /// This holds the html to include the given file in the page when in non-debug mode
        /// It contains two parameters:
        /// {fileUrl} which takes the name of the file to include
        /// an optional {cachebuster} property that, if present, places a cachebuster value in the 
        /// </summary>
        public string NonDebugHtmlFormatString { get; private set; }

        /// <summary>
        /// This holds the html to include the given file in the page when in non-debug mode
        /// It contains four parameters:
        /// {cdnUrl} which is the url to try to get the file from
        /// {cdnSuccessTest} which is a JavaScript test which returns true if cnd loaded successfully
        /// {fileUrl} which takes the name of the file to include if the cdnSuccessTest fails
        /// an optional {cachebuster} property that, if present, places a cachebuster value on the non-cdn file
        /// </summary>
        public string CdnHtmlFormatString { get; set; }
    }
}