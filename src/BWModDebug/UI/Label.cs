using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BWModDebug.UI
{
    public class Label : LayoutElement
    {
        public string Text { get; set; }

        public Label(string Text, params GUILayoutOption[] Options) : base(Options)
        {
            this.Text = Text;
        }

        protected override void OnDraw()
        {
            GUILayout.Label(Text, Options);
        }
    }
}
