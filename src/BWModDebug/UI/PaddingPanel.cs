using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BWModDebug.UI
{
    public class PaddingPanel : LayoutElement
    {
        public IUIElement Child { get; set; }

        public int? Left { get; set; }

        public int? Right { get; set; }

        public int? Top { get; set; }

        public int? Bottom { get; set; }

        public PaddingPanel(params GUILayoutOption[] Options) : base(Options)
        {

        }

        void Space(int? space)
        {
            if (space.HasValue)
            {
                GUILayout.Space(space.Value);
            }
        }

        protected override void OnDraw()
        {
            if (Top.HasValue || Bottom.HasValue)
            {
                GUILayout.BeginVertical(Options);
                Space(Top);
            }
            if (Left.HasValue || Right.HasValue)
            {
                GUILayout.BeginHorizontal(Options);
                Space(Left);
            }
            Child.Draw();
            if (Left.HasValue || Right.HasValue)
            {
                Space(Right);
                GUILayout.EndHorizontal();
            }
            if (Top.HasValue || Bottom.HasValue)
            {
                Space(Bottom);
                GUILayout.EndVertical();
            }
        }
    }
}
