using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BWModDebug.UI
{
    public class TextField : LayoutElement
    {
        public event EventHandler<TextChangedEventArgs> OnTextChanged;

        public string Text { get; set; }

        public TextField(params GUILayoutOption[] Options) : base(Options)
        {
            this.Text = Text;
        }

        public override void Draw()
        {
            string newText;
            if (Text != (newText = GUILayout.TextField(Text, Options)) && OnTextChanged != null)
            {
                var args = new TextChangedEventArgs(Text, newText);
                Text = newText;
                OnTextChanged.Invoke(this, args);
            }
        }

        public class TextChangedEventArgs : EventArgs
        {
            public TextChangedEventArgs(string old, string @new)
            {
                Old = old;
                New = @new;
            }

            public string Old { get; }

            public string New { get; }
        }
    }
}
