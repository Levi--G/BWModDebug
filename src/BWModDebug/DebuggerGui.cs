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
        ModLogger logger = new ModLogger("[BWMDUI]", Debugger.DebugLogPath);
        Dictionary<FileInfo, List<Type>> allmods;

        Debugger debugger = new Debugger();

        SelectionGrid screenSelection;
        IUIElement logWindow;
        IUIElement modWindow;
        IUIElement modInspectionWindow;

        void RefreshMods()
        {
            allmods = ModLoader.Instance.GetAllMods();
        }

        void Start()
        {
            try
            {
                logger.ClearLog();
                logger.Log("Starting Debugger UI");
                debugger.OnModUpdate += Debugger_OnModUpdate;
                debugger.Load();
                debugger.StartWatch();
                DialogHandler.Instance = new DialogHandler();
                screenSelection = new SelectionGrid(new string[] { "Log", "Mods", "Debugger" }, GUILayout.Height(25));
                screenSelection.Selected = 1;
                logWindow = LogWindow();
                modWindow = ModWindow();
                modInspectionWindow = ModInspectionWindow();
                RefreshMods();
                logger.Log("Debugger UI startup complete");
            }
            catch (Exception e)
            {
                logger.Log($"While starting UI threw {e.GetType().FullName}: {e.Message}{Environment.NewLine}{e.StackTrace}");
            }
        }

        private void Debugger_OnModUpdate(object sender, EventArgs e)
        {
            modInspectionWindow = ModInspectionWindow();
        }

        void OnGUI()
        {
            try
            {
                debugger.Update();
                if (DialogHandler.Instance.DialogPending)
                {
                    DialogHandler.Instance.Draw();
                }
                else
                {
                    GUI.Window(0, new Rect(0, 0, 800, 600), Window, "[BWMD]Debugger");
                }
            }
            catch (Exception e)
            {
                logger.Log($"While drawing UI threw {e.GetType().FullName}: {e.Message}{Environment.NewLine}{e.StackTrace}");
            }
        }

        private void Window(int id)
        {
            try
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
                        modInspectionWindow.Draw();
                        break;
                }
            }
            catch (Exception e)
            {
                logger.Log($"While drawing window UI threw {e.GetType().FullName}: {e.Message}{Environment.NewLine}{e.StackTrace}");
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
                    GUILayout.BeginHorizontal();
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
                    GUILayout.EndHorizontal();
                }
            };
            scroll.Children.Add(list);
            return padding;
        }

        private IUIElement ModInspectionWindow()
        {
            var padding = new PaddingPanel() { Left = 5, Right = 5 };
            var scroll = new ScrollView();
            padding.Child = scroll;
            foreach (var mod in debugger.GetMods().SelectMany(m => m.Types.Where(t => t.Instance != null)))
            {
                scroll.Children.Add(new ObjectInspector(mod.Type.FullName, () => mod.Instance));
            }
            return padding;
        }
    }
}
