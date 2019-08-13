using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BWModDebug.UI
{
    public class ToggleHide : LayoutElement
    {
        public List<IUIElement> Children => stackPanel.Children;

        public string Text { get => toggle.Text; set => toggle.Text = value; }

        Toggle toggle;
        StackPanel stackPanel;

        public ToggleHide(string text, params GUILayoutOption[] Options) : base(Options)
        {
            toggle = new Toggle(text, Options);
            stackPanel = new StackPanel(Orientation.Vertical, Options);
        }

        public override void Draw()
        {
            toggle.Draw();
            if (toggle.Checked)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                stackPanel.Draw();
                GUILayout.EndHorizontal();
            }
        }
    }
}
