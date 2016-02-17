#region licence
// ======================================================================================
// Mvc5WithBowerAndGrunt - An example of how to change a MVC5 project to Bower and Grunt
// Filename: FileWithDateUpdated.cs
// Date Created: 2016/02/05
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

        public string FileRelPath { get; private set; }

        public bool Exists { get; private set; }

        public DateTime LastWrittenUtc { get; private set; }

        public FileWithDateUpdated(string mvcAppPath, string relativePath)
        {
            FileRelPath = relativePath;
            var filePath = Path.Combine(mvcAppPath, relativePath);
            Exists = File.Exists(filePath);
            if (Exists)
                LastWrittenUtc = File.GetLastWriteTimeUtc(filePath);
        }

    }
}