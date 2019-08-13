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
        
        Func<object> getter;
        Action<object> setter;
        Action<string> stringsetter;

        object current;

        Toggle self;
        Label value;
        ToggleHide properties;
        ToggleHide fields;
        ToggleHide methods;

        IEnumerable<ObjectInspector> Children => properties.Children.Concat(fields.Children).Concat(methods.Children).Cast<ObjectInspector>();

        public bool ChildrenVisible { get; set; } = false;

        public ObjectInspector(string title, Func<object> getter, Action<object> setter = null, params GUILayoutOption[] Options) : base(Options)
        {
            this.getter = getter;
            this.setter = setter;
            self = new Toggle(title, Options);
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
            var newobj = getter();
            if (!object.Equals(current, newobj))
            {
                Changed = true;
                if (ChildrenVisible && current?.GetType().FullName != newobj?.GetType().FullName)
                {
                    if (setter != null)
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(newobj);
                        if (converter != null && converter.CanConvertFrom(typeof(string)))
                        {
                            stringsetter = (s) => converter.ConvertFromString(s);
                        }
                    }
                    ContructChildren();
                }
                current = newobj;
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
                value.Text = current?.ToString() ?? "Null";
            }
        }

        void ContructChildren()
        {
            if (getter == null) { return; }
            var type = current.GetType();

            properties.Children.Clear();
            foreach (var Property in type.GetProperties())
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
            fields.Children.Clear();
            foreach (var Field in type.GetFields().Where(f => f.IsPublic))
            {
                fields.Children.Add(new ObjectInspector(Field.Name, () => Field.GetValue(current), (o) => Field.SetValue(current, o), Options));
            }
            methods.Children.Clear();
            //todo
        }

        public override void Draw()
        {
            GUILayout.BeginHorizontal();
            self.Draw();
            value.Draw();
            if (stringsetter != null)
            {
                if (GUILayout.Button("Set", Options))
                {
                    var d = new TextDialog(self.Text, "Enter new value:", value.Text);
                    DialogHandler.QueueDialog(d);
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
