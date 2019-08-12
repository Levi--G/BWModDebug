﻿using System;
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

        protected override void OnDraw()
        {
            var newval = GUILayout.SelectionGrid(Selected, Buttons, Buttons.Length, Options);
            if (newval != Selected)
            {
                OnSelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
