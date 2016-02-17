#region licence
// ======================================================================================
// Mvc5WithBowerAndGrunt - An example of how to change a MVC5 project to Bower and Grunt
// Filename: RelPathSearcher.cs
// Date Created: 2016/02/04
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// ======================================================================================
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace B4BCore.Internal
{
    /// <summary>
    /// This class helps with relative paths that contain a search string
    /// </summary>
    internal class RelPathSearcher
    {
        private readonly Func<string, string> _getActualFilePathFromVirtualPath;

        public RelPathSearcher(Func<string, string> getActualFilePathFromVirtualPath)
        {
            _getActualFilePathFromVirtualPath = getActualFilePathFromVirtualPath;
        }

        public static bool ContainsSearchChars(string relFilePath)
        {
            return relFilePath.IndexOfAny(new[] { '*', '?' }) >= 0;
        }

        public static bool ContainsInvalidSearchChars(string relFilePath)
        {
            return relFilePath.IndexOf("**", StringComparison.Ordinal) >= 0 ;
        }

        public IEnumerable<string> UnpackBundle(IEnumerable<string> relFiles, Action<string> errorHandler = null)
        {
            var result = new List<string>();
            foreach (var relFilePath in relFiles)
            {
                if (ContainsSearchChars(relFilePath))
                    result.AddRange(FindAllFiles(relFilePath, errorHandler));
                else
                {
                    result.Add(relFilePath);
                }
            }

            return result;
        }

        public IEnumerable<string> FindAllFiles(string relFilePath, Action<string> errorHandler = null, string originalRelPath = null)
        {
            if (errorHandler == null) errorHandler = s => { throw new InvalidOperationException(s); };
            if (originalRelPath == null) originalRelPath = relFilePath;

            var filename = Path.GetFileName(relFilePath);
            if (filename == null)
                throw new NullReferenceException("filename not found");
            var relDirPath = Path.GetDirectoryName(relFilePath);
            if (relDirPath == null)
                throw new NullReferenceException("directory not found");

            var fileNameSearchNeeded = ContainsSearchChars(filename);
            var dirSearchNeeded = ContainsSearchChars(relDirPath);

            if (fileNameSearchNeeded && !dirSearchNeeded)
            {
                //primary use case, so catch early
                try
                {
                    return Directory.GetFiles(
                        _getActualFilePathFromVirtualPath(Path.GetDirectoryName(relFilePath)), Path.GetFileName(relFilePath))
                        .Select(x => Path.Combine(relDirPath, Path.GetFileName(x)).Replace('\\','/'));
                }
                catch (DirectoryNotFoundException e)
                {
                    errorHandler($"The fileRef '{originalRelPath}' contains a directory that does not exist.");
                    return new string[] {};
                }
            }

            if (dirSearchNeeded)
            {
                //This is the complex case where we need to search by dir

                if (ContainsInvalidSearchChars(relDirPath))
                {
                    //Otherwise we throw an exception
                    errorHandler($"'{relFilePath} contains a search string that BundlerForBower does not currently support.");
                    return new string[] { };
                    //todo: implement depth search. see http://stackoverflow.com/a/2700080/1434764 
                }


                var firstSearchCharIndex = relDirPath.IndexOfAny(new[] { '*', '?' });

                //we need to search all directories at this level, so split into parts
                var dirParts = SplitRelDirPathAroundSearchDir(relDirPath, firstSearchCharIndex);

                var absSearchDir = _getActualFilePathFromVirtualPath(dirParts[0]);
                if (!Directory.Exists(absSearchDir))
                {
                    errorHandler($"The directory '{dirParts[0]}' in {originalRelPath} does not exist.");
                    return new string[] { };
                }
                return (from dirToSearch in Directory.GetDirectories(absSearchDir, dirParts[1])
                       let searchDir = dirToSearch.Substring(absSearchDir.Length+1)
                       let relPath = Path.Combine(dirParts[0], searchDir, dirParts[2], filename)
                       select FindAllFiles(relPath, errorHandler, originalRelPath)).SelectMany(x => x);
            }

            //else we were called with a non-search request
            return new[] {relFilePath};
        }


        //------------------------------------------------------------------
        //private helpers

        /// <summary>
        /// This returns a array of three parts
        /// 0 - directories before the search dir - can be empty
        /// 1 - the directory search string
        /// 2 - the directories after the search dir - can be empty
        /// </summary>
        /// <param name="relDirPath"></param>
        /// <param name="firstSearchCharIndex"></param>
        /// <returns></returns>
        private static string[] SplitRelDirPathAroundSearchDir(string relDirPath, int firstSearchCharIndex)
        {
            //we first isolate the directory part of the search
            var startOfDir = relDirPath.LastIndexOfAny(new[] { '/', '\\' }, firstSearchCharIndex);
            if (startOfDir < 0) startOfDir = 0;
            var endOfDir = relDirPath.IndexOfAny(new[] { '/', '\\' }, firstSearchCharIndex);
            if (endOfDir < 0) endOfDir = relDirPath.Length - 1;

            var result = new string[3];
            result[0] = startOfDir == 0 ? "" : relDirPath.Substring(0, startOfDir);  //-1 to miss the separator
            result[1] = relDirPath.Substring(startOfDir + 1, endOfDir - startOfDir);
            if (result[1] == "*\\" || result[1] == "*/") result[1] = "*";       //Directory search doesn't like "*\\"
            result[2] = endOfDir == relDirPath.Length - 1 ? "" : relDirPath.Substring(endOfDir + 1);

            return result;
        }
    }
}