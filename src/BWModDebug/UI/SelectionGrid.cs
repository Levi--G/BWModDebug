using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BWModDebug.UI
{
    public class SelectionGrid : LayoutElement
    {
        public event EventHandler OnSelectionChanged;

        public int Selected { get; set; }

        public string[] Buttons { get; set; }

        public SelectionGrid(string[] buttons, params GUILayoutOption[] Options) : base(Options)
        {
            Buttons = buttons;
        }

        public override void Draw()
        {
            var newval = GUILayout.SelectionGrid(Selected, Buttons, Buttons.Length, Options);
            if (newval != Selected)
            {
                Selected = newval;
                OnSelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
