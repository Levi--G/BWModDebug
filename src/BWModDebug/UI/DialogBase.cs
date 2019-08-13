using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BWModDebug.UI
{
    public abstract class DialogBase : IDialog
    {
        public bool Visible { get; private set; } = true;

        protected void Close()
        {
            Visible = false;
        }

        public abstract void Draw();
    }
}
