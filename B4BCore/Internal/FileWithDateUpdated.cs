#region licence
// ======================================================================================
// Mvc5UsingBower - An example+library to allow an MVC project to use Bower and Grunt
// Filename: FileWithDateUpdated.cs
// Date Created: 2016/02/17
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// ======================================================================================
#endregion

using System;
using System.IO;

namespace B4BCore.Internal
{
    internal class FileWithDateUpdated
    {
        public FileWithDateUpdated(string mvcAppPath, string relativePath)
        {
            FileRelPath = relativePath;
            var filePath = Path.Combine(mvcAppPath, relativePath);
            Exists = File.Exists(filePath);
            if (Exists)
                LastWrittenUtc = File.GetLastWriteTimeUtc(filePath);
        }

        public string FileRelPath { get; private set; }

        public bool Exists { get; private set; }

        public DateTime LastWrittenUtc { get; private set; }
    }
}