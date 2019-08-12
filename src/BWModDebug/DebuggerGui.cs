using BWModDebug.UI;
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
        Dictionary<FileInfo, List<Type>> allmods;

        Debugger debugger = new Debugger();

        SelectionGrid screenSelection;
        IUIElement logWindow;
        IUIElement modWindow;
        IUIElement debugWindow;

        void RefreshMods()
        {
            allmods = ModLoader.Instance.GetAllMods();
        }

        void Start()
        {
            debugger.StartWatch();

            screenSelection = new SelectionGrid(new string[] { "Log", "Mods", "Debugger" }, GUILayout.Height(25));
            logWindow = LogWindow();
            modWindow = ModWindow();
            debugWindow = DebugWindow();
        }

        void OnGUI()
        {
            GUI.ModalWindow(0, new Rect(0, 0, 600, 800), Window, "[BWMD]Debugger");
        }

        private void Window(int id)
        {
            screenSelection.Draw();
            switch (screenSelection.Selected)
            {
                case 0:
                    logWindow.Draw();
                    break;
                case 1:
                    modWindow.Draw();
                    break;
                case 2:
                    debugWindow.Draw();
                    break;
            }
        }

        private IUIElement LogWindow()
        {
            var padding = new PaddingPanel() { Left = 5, Right = 5 };
            var scroll = new ScrollView();
            padding.Child = scroll;
            var clearbtn = new Button("Clear");
            clearbtn.OnClick += (s, e) => { ModLogger.Logs.Clear(); };
            scroll.Children.Add(clearbtn);
            var list = new ListBox<string>(GUILayout.Height(25)) { ItemsSource = ModLogger.Logs };
            scroll.Children.Add(list);
            return padding;
        }

        private IUIElement ModWindow()
        {
            var padding = new PaddingPanel() { Left = 5, Right = 5 };
            var scroll = new ScrollView();
            padding.Child = scroll;
            var refreshbtn = new Button("Refresh all mods");
            refreshbtn.OnClick += (s, e) =>
            {
                ModLoader.Instance.RefreshModFiles();
                RefreshMods();
            };
            scroll.Children.Add(refreshbtn);
            var list = new ListBox<FileInfo>(GUILayout.Height(25))
            {
                DynamicSource = () => allmods.Keys,
                ItemTemplate = (file) =>
                {
                    bool loaded = ModLoader.Instance.IsLoaded(file);
                    bool newCheckboxStatus = GUILayout.Toggle(loaded, file.Name, GUILayout.Width(150));
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
                    if (GUILayout.Button("Reload", GUILayout.Width(100)))
                    {
                        ModLoader.Instance.Unload(file);
                        ModLoader.Instance.Load(file);
                        RefreshMods();
                    }

                    if (GUILayout.Button("Debug", GUILayout.Width(100)))
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
            };
            scroll.Children.Add(list);
            return padding;
        }

        private IUIElement DebugWindow()
        {
            throw new NotImplementedException();
        }
    }
}
