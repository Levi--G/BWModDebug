using BWModLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace BWModDebug
{
    class Debugger
    {
        public static string DebugPath => ModLoader.FolderPath + "\\ModDebug";

        public static string DebugLogPath => ModLoader.LogPath + "\\ModDebugger.log";
        
        FileSystemWatcher watcher;

        object queueKey = new object();
        Queue<string> fileQueue = new Queue<string>();

        ModLogger logger = new ModLogger("[BWMD]", DebugLogPath);

        List<Mod> Mods = new List<Mod>();

        GameObject GameObject = new GameObject();

        public void Load()
        {
            logger.Log("Starting Debugger");
            UnityEngine.Object.DontDestroyOnLoad(GameObject);
        }

        public void StartWatch()
        {
            logger.Log("Starting watch on Debug folder");
            Directory.CreateDirectory(Debugger.DebugPath);

            foreach (var dir in Directory.GetDirectories(Debugger.DebugPath))
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch { }
            }

            if (watcher != null)
            {
                watcher.Dispose();
                watcher = null;
            }

            watcher = new FileSystemWatcher(DebugPath, "*.dll");
            watcher.NotifyFilter = NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName 
                                 | NotifyFilters.Size;
            watcher.Changed += Watcher_Triggered;
            watcher.Deleted += Watcher_Triggered;
            watcher.Created += Watcher_Triggered;
            watcher.Renamed += Watcher_Triggered;
            watcher.Error += Watcher_Error;
            watcher.EnableRaisingEvents = true;
            logger.Log("Watching started");
            logger.Log("Loading mods");
            foreach (var file in Directory.GetFiles(DebugPath))
            {
                HandleChange(file);
            }
            logger.Log("Mods loaded");
        }

        private void Watcher_Error(object sender, ErrorEventArgs e)
        {
            var ex = e.GetException();
            logger.Log($"Filewatcher Error: {ex.GetType().FullName}{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }

        private void Watcher_Triggered(object sender, FileSystemEventArgs e)
        {
            if (Path.GetDirectoryName(e.FullPath) != DebugPath || !e.FullPath.EndsWith(".dll")) { return; }//Don't care, not in main dir
            lock (queueKey)
            {
                if (!fileQueue.Contains(e.FullPath))
                {
                    fileQueue.Enqueue(e.FullPath);
                }
            }
        }

        public void Update()
        {
            string s = null;
            lock (queueKey)
            {
                if (fileQueue.Count > 0)
                {
                    s = fileQueue.Dequeue();
                }
            }
            if (s != null)
            {
                HandleChange(s);
            }
        }

        void HandleChange(string file)
        {
            if (Path.GetDirectoryName(file) != DebugPath || !file.EndsWith(".dll")) { return; }//Don't care, not in main dir
            logger.Log($"INFO: File {file} triggered an update");
            var mod = Mods.FirstOrDefault(m => m.FullPath == file);
            if (File.Exists(file))
            {
                if (mod != null)
                {
                    UpdateMod(mod);
                }
                else
                {
                    AddMod(file);
                }
            }
            else
            {
                if (mod != null)
                {
                    RemoveMod(mod);
                }
                else
                {
                    //should not happen?
                    logger.Log($"WARN: File {file} was removed without being added!");
                }
            }
        }

        void AddMod(string File)
        {
            Mod mod = new Mod() { FullPath = File };
            logger.Log($"Adding {mod.Name}");
            Mods.Add(mod);
            UpdateMod(mod);
        }

        void UpdateMod(Mod mod)
        {
            logger.Log($"Updating {mod.Name}");
            var md5 = CalculateMD5(mod.FullPath);
            if (mod.MD5 != null)
            {
                if (mod.MD5 == md5)
                {
                    logger.Log($"{mod.Name} no update needed, files are identical");
                    return;
                }
                foreach (var type in mod.Types.Where(t => t.Instance != null))
                {
                    UnityEngine.Object.Destroy(type.Instance);
                }
            }
            mod.Types.Clear();
            mod.MD5 = md5;
            if (Directory.Exists(mod.UniqueFolderPath) && File.Exists(mod.UniqueFilePath))
            {
                logger.Log($"This version of mod {mod.Name} was already added once, using old copy!");
            }
            else
            {
                Directory.CreateDirectory(mod.UniqueFolderPath);
                File.Copy(mod.FullPath, mod.UniqueFilePath);
            }
            FileInfo file = new FileInfo(mod.UniqueFilePath);
            ModLoader.Instance.AddModFile(file);
            var found = ModLoader.Instance.GetAllMods().TryGetValue(file, out var types);
            if (found)
            {
                mod.Types.AddRange(types.Select(t => new Mod.ModType() { Type = t }));
            }
            else
            {
                logger.Log($"Mod {mod.Name} not correctly loaded!");
            }
            foreach (var type in mod.Types)
            {
                //type.Instance = (MonoBehaviour)Activator.CreateInstance(type.Type);
                type.Instance = (MonoBehaviour)GameObject.AddComponent(type.Type);
                logger.Log($"Mod {mod.Name} loaded {type.Type.FullName}!");
            }
        }

        void RemoveMod(Mod mod)
        {
            logger.Log($"Removing {mod.Name}");
            if (mod.MD5 != null)
            {
                foreach (var type in mod.Types.Where(t => t.Instance != null))
                {
                    UnityEngine.Object.Destroy(type.Instance);
                }
            }
            mod.Types.Clear();
            mod.MD5 = null;
        }

        string CalculateMD5(string file)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(file))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
