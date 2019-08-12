using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BWModDebug.UI
{
    public class ListBox<T> : LayoutElement
    {
        public IEnumerable<T> ItemsSource { get; set; }
        public Func<IEnumerable<T>> DynamicSource { get; set; }

        public Action<T> ItemTemplate { get; set; }

        public ListBox(params GUILayoutOption[] Options) : base(Options)
        {
            ItemTemplate = DefaultTemplate;
        }

        protected void DefaultTemplate(T o)
        {
            GUILayout.Label(o.ToString(), Options);
        }

        public override void Draw()
        {
            var source = ItemsSource ?? DynamicSource?.Invoke();
            if (source == null) { return; }
            foreach (var o in source)
            {
                ItemTemplate(o);
            }
        }
    }

    public class ListBox : ListBox<object>
    {
        public ListBox(params GUILayoutOption[] Options) : base(Options)
        {
        }
    }
}
