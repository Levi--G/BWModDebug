using BWModLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BWModDebug
{
    public class DebuggerGui : MonoBehaviour
    {
        int WindowWidth = 600;
        int WindowHeight = 800;

        int currentScreen = 0;

        Vector2 logScrollPosition;
        Vector2 modScrollPosition;

        Dictionary<FileInfo, List<Type>> allmods;

        Debugger debugger = new Debugger();

        void RefreshMods()
        {
            allmods = ModLoader.Instance.GetAllMods();
        }

        void Start()
        {
            debugger.StartWatch();
        }

        void OnGUI()
        {
            GUI.ModalWindow(0, new Rect(0, 0, WindowWidth, WindowHeight), Window, "[BWMD]Debugger");
        }

        private void Window(int id)
        {
            currentScreen = GUI.SelectionGrid(new Rect(25, 0, WindowWidth - 50, 25), currentScreen, new string[] { "Log", "Mods", "Debugger" }, 3);
            switch (currentScreen)
            {
                case 0:
                    LogWindow();
                    break;
                case 1:
                    ModWindow();
                    break;
                case 2:
                    DebuggingWindow();
                    break;
            }
        }

        private void LogWindow()
        {
            int logNum = 0;
            if (ModLogger.Logs.Any())
            {
                logScrollPosition = GUI.BeginScrollView(new Rect(5, 30, WindowWidth - 10, WindowHeight - 30),
                                                     logScrollPosition, new Rect(0, 0, WindowWidth - 10, 25 * ModLogger.Logs.Count + 1));
                if (GUI.Button(new Rect(0, 0, WindowWidth - 10, 25), "Clear"))
                {
                    ModLogger.Logs.Clear();
                }
                foreach (string log in ModLogger.Logs)
                {
                    logNum++;
                    GUI.Label(new Rect(5, 25 * logNum, WindowWidth - 10, 25), log);
                }
                GUI.EndScrollView();
            }
        }

        private void ModWindow()
        {
            modScrollPosition = GUI.BeginScrollView(new Rect(5, 30, WindowWidth - 10, WindowHeight - 30), modScrollPosition, new Rect(0, 0, WindowWidth - 10, 50 + allmods.Count * 25));
            if (GUI.Button(new Rect(0, 0, WindowWidth - 10, 25), "Refresh all mods"))
            {
                ModLoader.Instance.RefreshModFiles();
                RefreshMods();
            }
            int modNum = 1;
            foreach (FileInfo file in allmods.Keys)
            {
                modNum++;
                bool loaded = ModLoader.Instance.IsLoaded(file);
                bool newCheckboxStatus = GUI.Toggle(new Rect(5, modNum * 25, 150, 25), loaded, file.Name);
                if (newCheckboxStatus && !loaded)
                {
                    ModLoader.Instance.Load(file);
                    RefreshMods();
                }
                else if (!newCheckboxStatus && loaded)
                {
                    ModLoader.Instance.Unload(file);
                    RefreshMods();
                }

                // Reload the respective Mod
                if (GUI.Button(new Rect(155, modNum * 25, 100, 25), "Reload"))
                {
                    ModLoader.Instance.Unload(file);
                    ModLoader.Instance.Load(file);
                    RefreshMods();
                }

                if (GUI.Button(new Rect(255, modNum * 25, 100, 25), "Debug"))
                {
                    ModLoader.Instance.RemoveModFile(file);
                    file.Refresh();
                    if (file.Exists)
                    {
                        file.MoveTo(file.FullName.Replace(ModLoader.ModsPath, Debugger.DebugPath));
                    }
                    ModLoader.Instance.RefreshModFiles();
                    RefreshMods();
                }
            }
            GUI.EndScrollView();
        }

        private void DebuggingWindow()
        {
            throw new NotImplementedException();
        }
    }
}
