using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BWModDebug.UI
{
    public class Button : LayoutElement
    {
        public event EventHandler OnClick;

        public string Text { get; set; }

        public Button(string Text, params GUILayoutOption[] Options) : base(Options)
        {
            this.Text = Text;
        }

        public override void Draw()
        {
            if (GUILayout.Button(Text, Options))
            {
                OnClick?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
