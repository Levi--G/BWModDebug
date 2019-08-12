using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace BWModDebug
{
    class Mod
    {
        public string Name => Path.GetFileNameWithoutExtension(FullPath);

        public string FullPath { get; set; }

        public string UniqueFolderPath => MD5 != null ? Path.Combine(Debugger.DebugPath, MD5) : null;

        public string UniqueFilePath => UniqueFolderPath != null ? Path.Combine(UniqueFolderPath, Path.GetFileName(FullPath)) : null;

        public string MD5 { get; set; }

        public List<ModType> Types { get; } = new List<ModType>();

        public class ModType
        {
            public Type Type { get; set; }
            public MonoBehaviour Instance { get; set; }
        }
    }
}
