using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BWModDebug.UI
{
    public class TextDialog : DialogBase
    {
        public event EventHandler OnSubmit;
        public event EventHandler OnClose;

        public string Text { get => field.Text; set => field.Text = value; }

        Label label = new Label("");
        TextField field = new TextField();
        Button ok = new Button("ok");
        Button cancel = new Button("cancel");
        string title;

        public TextDialog(string title, string text, string placeholder = "")
        {
            this.title = title;
            label.Text = text;
            field.Text = placeholder;
            ok.OnClick += Ok_OnClick;
            cancel.OnClick += Cancel_OnClick;
        }

        private void Cancel_OnClick(object sender, EventArgs e)
        {
            Close();
            OnClose?.Invoke(this, EventArgs.Empty);
        }

        private void Ok_OnClick(object sender, EventArgs e)
        {
            OnSubmit?.Invoke(this, EventArgs.Empty);
            Close();
            OnClose?.Invoke(this, EventArgs.Empty);
        }

        void DrawWindow(int id)
        {
            GUILayout.BeginVertical();
            label.Draw();
            field.Draw();
            GUILayout.BeginHorizontal();
            ok.Draw();
            cancel.Draw();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        public override void Draw()
        {
            GUI.ModalWindow(0, new Rect(Screen.width / 3, Screen.height / 3, Screen.width / 3, Screen.height / 3), DrawWindow, title);
        }
    }
}
