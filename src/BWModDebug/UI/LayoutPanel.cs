using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BWModDebug.UI
{
    public abstract class LayoutPanel : LayoutElement
    {
        public List<IUIElement> Children { get; set; } = new List<IUIElement>();

        public LayoutPanel(params GUILayoutOption[] Options) : base(Options)
        {
        }

        protected void DrawChildren()
        {
            foreach (var child in Children)
            {
                child.Draw();
            }
        }
    }
}
