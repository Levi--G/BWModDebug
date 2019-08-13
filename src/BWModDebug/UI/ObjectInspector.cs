using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BWModDebug.UI
{
    class ObjectInspector : LayoutElement
    {
        public bool Changed { get; private set; }
        public bool SelfUpdate { get; set; }

        Func<object> getter;
        Action<object> setter;
        Action<string> stringsetter;

        object current;

        Toggle self;
        Label type;
        Label value;
        ToggleHide properties;
        ToggleHide fields;
        ToggleHide methods;

        IEnumerable<ObjectInspector> Children => properties.Children.Concat(fields.Children).Concat(methods.Children).Cast<ObjectInspector>();

        public bool ChildrenVisible => self.Checked && current != null;

        public ObjectInspector(string title, Func<object> getter, Action<object> setter = null, params GUILayoutOption[] Options) : base(Options)
        {
            this.getter = getter;
            this.setter = setter;
            self = new Toggle(title, Options);
            self.OnClick += (s, e) => { ContructChildren(); };
            type = new Label("Null", Options);
            value = new Label("Null", Options);
            if (this.getter != null)
            {
                properties = new ToggleHide("[Properties]", Options);
                fields = new ToggleHide("[Fields]", Options);
                methods = new ToggleHide("[Methods]", Options);
            }
        }

        public void Update()
        {
            if (getter == null) { return; }
            Changed = false;
            var old = current;
            current = getter();
            if (!object.Equals(old, current))
            {
                Changed = true;
                bool typechanged = old?.GetType().FullName != current?.GetType().FullName;
                if (typechanged)
                {
                    stringsetter = null;
                    if (setter != null && current != null)
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(current.GetType());
                        BWModLoader.ModLoader.Instance.Logger.Log(converter?.ToString());
                        if (converter != null && converter.CanConvertFrom(typeof(string)))
                        {
                            stringsetter = (s) => setter(converter.ConvertFromString(s));
                        }
                    }
                    ContructChildren();
                }
            }
            if (ChildrenVisible)
            {
                foreach (var child in Children)
                {
                    child.Update();
                    Changed = Changed | child.Changed;
                }
            }
            if (Changed)
            {
                type.Text = current?.GetType().Name ?? "Null";
                value.Text = current?.ToString() ?? "Null";
            }
        }

        void ContructChildren()
        {
            if (getter == null) { return; }
            properties.Children.Clear();
            fields.Children.Clear();
            methods.Children.Clear();
            if (ChildrenVisible)
            {
                var type = current.GetType();
                foreach (var Property in type.GetProperties().Where(p => p.GetIndexParameters().Length == 0))
                {
                    Func<object> g = null;
                    Action<object> s = null;
                    if (Property.CanRead)
                    {
                        g = () => Property.GetValue(current, null);
                    }
                    if (Property.CanWrite)
                    {
                        s = (o) => Property.SetValue(current, o, null);
                    }
                    properties.Children.Add(new ObjectInspector(Property.Name, g, s, Options));
                }
                foreach (var Field in type.GetFields().Where(f => f.IsPublic))
                {
                    fields.Children.Add(new ObjectInspector(Field.Name, () => Field.GetValue(current), (o) => Field.SetValue(current, o), Options));
                }
                //todo
            }
        }

        public override void Draw()
        {
            if (SelfUpdate)
            {
                Update();
            }
            GUILayout.BeginHorizontal();
            self.Draw();
            type.Draw();
            value.Draw();
            if (stringsetter != null)
            {
                if (GUILayout.Button("Set", Options))
                {
                    var d = new TextDialog(self.Text, "Enter new value:", value.Text);
                    d.OnSubmit += (s, e) => { stringsetter(d.Text); };
                    DialogHandler.QueueDialog(d);
                }
            }
            GUILayout.EndHorizontal();
            if (ChildrenVisible)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.BeginVertical();
                properties.Draw();
                fields.Draw();
                methods.Draw();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }
    }
}
