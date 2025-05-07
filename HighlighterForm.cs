using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;

namespace MouseHighlighter
{
    public class HighlighterForm : Form
    {
        private readonly System.Windows.Forms.Timer cursorTimer = new System.Windows.Forms.Timer();
        private readonly int circleSize = 30;
        private Point lastPosition = Cursor.Position;
        private bool isClicking = false;
        private Color highlightColor = Color.Yellow;
        private Color clickColor = Color.Red;
        private float opacity = 0.5f;

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        public HighlighterForm()
        {
            // Form settings
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.TransparencyKey = BackColor;

            // Set up form to cover all screens
            Rectangle virtualScreen = SystemInformation.VirtualScreen;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(virtualScreen.X, virtualScreen.Y);
            this.Size = new Size(virtualScreen.Width, virtualScreen.Height);

            // Handle DPI changes and monitor configuration changes
            this.DpiChanged += (s, e) => UpdateFormBounds();
            SystemEvents.DisplaySettingsChanged += (s, e) => UpdateFormBounds();

            // Make the form click-through
            int initialStyle = GetWindowLong(this.Handle, -20);
            SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20);

            // Setup timer for cursor tracking
            cursorTimer.Interval = 16; // approximately 60 FPS
            cursorTimer.Tick += CursorTimer_Tick;
            cursorTimer.Start();

            // Create system tray icon
            CreateTrayIcon();
        }

        private void CreateTrayIcon()
        {
            // Generate and save the icon
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
            IconGenerator.SaveIconToFile(iconPath);

            var trayIcon = new NotifyIcon
            {
                Icon = new Icon(iconPath),
                Text = "Mouse Highlighter",
                Visible = true
            };

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Settings", null, (s, e) => ShowSettings());
            contextMenu.Items.Add("Exit", null, (s, e) => Application.Exit());
            trayIcon.ContextMenuStrip = contextMenu;
        }

        private void CursorTimer_Tick(object sender, EventArgs e)
        {
            var currentPosition = Cursor.Position;
            bool leftClick = (GetAsyncKeyState(0x01) & 0x8000) != 0;

            if (currentPosition != lastPosition || leftClick != isClicking)
            {
                lastPosition = currentPosition;
                isClicking = leftClick;
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using (var brush = new SolidBrush(Color.FromArgb(
                (int)(255 * opacity),
                isClicking ? clickColor : highlightColor)))
            {
                e.Graphics.FillEllipse(
                    brush,
                    lastPosition.X - circleSize / 2,
                    lastPosition.Y - circleSize / 2,
                    circleSize,
                    circleSize);
            }
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80000; // WS_EX_LAYERED
                cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }
        private void ShowSettings()
        {
            var settingsForm = new ColorSettingsForm(highlightColor, clickColor, opacity);
            settingsForm.SettingsChanged += (s, settings) =>
            {
                highlightColor = settings.CursorColor;
                clickColor = settings.ClickColor;
                opacity = settings.Opacity;
                this.Invalidate();
            };
            settingsForm.ShowDialog();
        }
        private void UpdateFormBounds()
        {
            // Update form bounds to cover all screens
            Rectangle virtualScreen = SystemInformation.VirtualScreen;
            this.Location = new Point(virtualScreen.X, virtualScreen.Y);
            this.Size = new Size(virtualScreen.Width, virtualScreen.Height);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SystemEvents.DisplaySettingsChanged -= (s, e) => UpdateFormBounds();
            }
            base.Dispose(disposing);
        }
    }
}
