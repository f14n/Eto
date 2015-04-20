using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using System;

namespace Eto.WinForms.Forms.Controls
{
    public class PanelHandler : WindowsPanel<PanelHandler.EtoPanel, Panel, Panel.ICallback>, Panel.IHandler
    {
        public class EtoPanel : swf.Panel
        {
            public PanelHandler Handler { get; set; }

            public override sd.Size GetPreferredSize(sd.Size proposedSize)
            {
                // WinForms have problems with autosizing vs. docking
                // this will solve some of its problems and speed things up.
                // Not perfect, would be better to write it all new,
                // especially if all inner controls are docked,
                // but this is common scenairo when panels are emitted
                // from Eto layout engine.
                if (Controls.Count == 1 && Controls[0].Dock == swf.DockStyle.Fill)
                    return Controls[0].GetPreferredSize(new sd.Size(
                        Math.Max(0, proposedSize.Width - Padding.Horizontal),
                        Math.Max(0, proposedSize.Height - Padding.Vertical)))
                        + Padding.Size;

                // fallback to default engine
                return base.GetPreferredSize(proposedSize);
            }

            // Need to override IsInputKey to capture 
            // the arrow keys.
            protected override bool IsInputKey (swf.Keys keyData)
            {
                switch (keyData & swf.Keys.KeyCode) {
                case swf.Keys.Up:
                case swf.Keys.Down:
                case swf.Keys.Left:
                case swf.Keys.Right:
                case swf.Keys.Back:
                    return true;
                default:
                    return base.IsInputKey (keyData);
                }
            }

            const int WM_NCHITTEST = 0x0084;
            const int HTTRANSPARENT = -1;

            protected override void WndProc(ref swf.Message m)
            {
                if (!Handler.IsHitTestVisible && m.Msg == WM_NCHITTEST)
                {
                    m.Result = (IntPtr)HTTRANSPARENT;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        public PanelHandler ()
        {
            Control = new EtoPanel
            {
                Size = sd.Size.Empty,
                MinimumSize = sd.Size.Empty,
                AutoSize = true,
                AutoSizeMode = swf.AutoSizeMode.GrowAndShrink,
                Handler = this
            };
        }

        public bool IsHitTestVisible { get; set; } = true;
    }
}
