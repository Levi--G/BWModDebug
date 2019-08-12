using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BWModDebug.UI
{
    class ScrollView : LayoutPanel
    {
        public Vector2 ScrollPosition { get; set; }

        public bool HorizontalAlwaysVisible { get; set; }

        public bool VerticalAlwaysVisible { get; set; }

        public ScrollView(params GUILayoutOption[] Options) : base(Options)
        {
        }

        protected override void OnDraw()
        {
            GUILayout.BeginScrollView(ScrollPosition, HorizontalAlwaysVisible, VerticalAlwaysVisible, Options);
            DrawChildren();
            GUILayout.EndScrollView();
        }
    }
}
