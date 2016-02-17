#region licence
// ======================================================================================
// Mvc5WithBowerAndGrunt - An example of how to change a MVC5 project to Bower and Grunt
// Filename: FileTypeConfigInfo.cs
// Date Created: 2016/02/03
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

        public FileTypeConfigInfo(string directory, string debugHtmlFormatString, string nonDebugHtmlFormatString)
        {
            Directory = directory;
            DebugHtmlFormatString = debugHtmlFormatString;
            NonDebugHtmlFormatString = nonDebugHtmlFormatString;
        }
    }
}