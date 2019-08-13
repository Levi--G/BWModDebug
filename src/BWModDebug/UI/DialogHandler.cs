using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BWModDebug.UI
{
    public class DialogHandler : IUIElement
    {
        public bool DialogPending => Dialogs.Count > 0;

        public Queue<IDialog> Dialogs { get; private set; } = new Queue<IDialog>();

        public void Draw()
        {
            if (DialogPending)
            {
                var dialog = Dialogs.Peek();
                dialog.Draw();
                if (!dialog.Visible) { Dialogs.Dequeue(); }
            }
        }

        public static DialogHandler Instance { get; set; }

        public static void QueueDialog(IDialog dialog)
        {
            Instance.Dialogs.Enqueue(dialog);
        }
    }

    public interface IDialog : IUIElement
    {
        bool Visible { get; }
    }
}
