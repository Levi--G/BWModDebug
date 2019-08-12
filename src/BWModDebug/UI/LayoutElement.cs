using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BWModDebug.UI
{
    public abstract class LayoutElement : IUIElement
    {
        public GUILayoutOption[] Options { get; set; }

        public LayoutElement(GUILayoutOption[] Options)
        {
            this.Options = Options;
        }

        public void Draw()
        {

        }

        protected abstract void OnDraw();
    }
}
