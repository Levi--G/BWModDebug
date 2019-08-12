using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BWModDebug.UI
{
    public class StackPanel : LayoutPanel
    {
        public Orientation Orientation { get; set; } = Orientation.Vertical;

        public StackPanel(Orientation orientation, params GUILayoutOption[] Options) : base(Options)
        {
            Orientation = orientation;
        }

        public override void Draw()
        {
            if (Orientation == Orientation.Vertical)
            {
                GUILayout.BeginVertical(Options);
            }
            else
            {
                GUILayout.BeginHorizontal(Options);
            }
            DrawChildren();
            if (Orientation == Orientation.Vertical)
            {
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.EndHorizontal();
            }
        }
    }

    public enum Orientation
    {
        Vertical, Horizontal
    }
}
