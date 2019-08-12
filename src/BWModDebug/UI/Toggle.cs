using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BWModDebug.UI
{
    public class Toggle : LayoutElement
    {
        public event EventHandler OnClick;

        public string Text { get; set; }
        public bool Checked { get; set; }

        public Toggle(string Text, params GUILayoutOption[] Options) : base(Options)
        {
            this.Text = Text;
        }

        protected override void OnDraw()
        {
            if (GUILayout.Toggle(Checked, Text, Options) != Checked)
            {
                Checked = !Checked;
                OnClick?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
