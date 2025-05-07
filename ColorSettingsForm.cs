using System;
using System.Drawing;
using System.Windows.Forms;

namespace MouseHighlighter
{
    public class ColorSettingsForm : Form
    {
        private readonly Button cursorColorButton;
        private readonly Button clickColorButton;
        private readonly TrackBar opacityTrackBar;
        private readonly TrackBar sizeTrackBar;
        private readonly Label opacityLabel;
        private readonly Label sizeLabel;

        public event EventHandler<ColorSettings> SettingsChanged;

        public ColorSettingsForm(Color currentCursorColor, Color currentClickColor, int currentThickness, int currentSize)
        {
            this.Text = "Mouse Highlighter Settings";
            this.Size = new Size(800, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Padding = new Padding(10);

            // Color section
            var colorGroupBox = new GroupBox
            {
                Text = "Colors",
                Dock = DockStyle.None,
                Height = 80,
                Padding = new Padding(10),
                Location = new Point(10, 10),
                Width = 740
            };

            var cursorLabel = new Label
            {
                Text = "Cursor Color:",
                AutoSize = true,
                Location = new Point(10, 30)
            };

            cursorColorButton = new Button
            {
                Width = 80,
                Height = 30,
                Location = new Point(250, 25),
                BackColor = currentCursorColor,
                FlatStyle = FlatStyle.Flat
            };
            cursorColorButton.Click += (s, e) => ChooseColor(cursorColorButton);

            var clickLabel = new Label
            {
                Text = "Click Color:",
                AutoSize = true,
                Location = new Point(520, 25)
            };

            clickColorButton = new Button
            {
                Width = 80,
                Height = 30,
                Location = new Point(670, 25),
                BackColor = currentClickColor,
                FlatStyle = FlatStyle.Flat
            };
            clickColorButton.Click += (s, e) => ChooseColor(clickColorButton);

            colorGroupBox.Controls.AddRange(new Control[] { cursorLabel, cursorColorButton, clickLabel, clickColorButton });

            // Thickness section
            var thicknessGroupBox = new GroupBox
            {
                Text = "Ring Thickness",
                Dock = DockStyle.None,
                Height = 80,
                Padding = new Padding(10),
                Location = new Point(10, 100),
                Width = 740
            };

            opacityLabel = new Label
            {
                Text = "Thickness: " + currentThickness + "px",
                AutoSize = true,
                Location = new Point(10, 30)
            };

            opacityTrackBar = new TrackBar
            {
                Location = new Point(10, 50),
                Width = 650,
                Minimum = 1,
                Maximum = 10,
                Value = currentThickness,
                TickFrequency = 1,
                TickStyle = TickStyle.BottomRight
            };
            opacityTrackBar.ValueChanged += (s, e) =>
            {
                opacityLabel.Text = "Thickness: " + opacityTrackBar.Value.ToString() + "px";
                NotifySettingsChanged();
            };

            thicknessGroupBox.Controls.AddRange(new Control[] { opacityLabel, opacityTrackBar });

            // Size controls
            var sizeGroupBox = new GroupBox
            {
                Text = "Circle Size",
                Dock = DockStyle.None,
                Height = 80,
                Padding = new Padding(10),
                Location = new Point(10, 190),
                Width = 740
            };

            sizeLabel = new Label
            {
                Text = "Size: " + currentSize.ToString() + "px",
                AutoSize = true,
                Location = new Point(10, 30)
            };

            sizeTrackBar = new TrackBar
            {
                Location = new Point(10, 50),
                Width = 650,
                Minimum = 10,
                Maximum = 100,
                Value = currentSize,
                TickFrequency = 10,
                TickStyle = TickStyle.BottomRight
            };
            sizeTrackBar.ValueChanged += (s, e) =>
            {
                sizeLabel.Text = "Size: " + sizeTrackBar.Value.ToString() + "px";
                NotifySettingsChanged();
            };

            sizeGroupBox.Controls.AddRange(new Control[] { sizeLabel, sizeTrackBar });

            // Button section
            var buttonPanel = new Panel
            {
                Height = 40,
                Dock = DockStyle.Bottom,
                Width = 950
            };

            var cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(400, 10),
                Width = 100,
                Height = 30
            };

            var applyButton = new Button
            {
                Text = "Apply and Close",
                DialogResult = DialogResult.OK,
                Location = new Point(550, 10),
                Width = 180,
                Height = 30
            };
            applyButton.Click += (s, e) => this.Close();

            buttonPanel.Controls.AddRange(new Control[] { cancelButton, applyButton });

            this.Controls.AddRange(new Control[] 
            { 
                colorGroupBox,
                thicknessGroupBox,
                sizeGroupBox,
                buttonPanel
            });
        }

        private void ChooseColor(Button button)
        {
            using var dialog = new ColorDialog
            {
                Color = button.BackColor,
                FullOpen = true,
                AnyColor = true,
                SolidColorOnly = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                button.BackColor = dialog.Color;
                NotifySettingsChanged();
            }
        }

        private void NotifySettingsChanged()
        {
            SettingsChanged?.Invoke(this, new ColorSettings(
                cursorColorButton.BackColor,
                clickColorButton.BackColor,
                opacityTrackBar.Value,
                sizeTrackBar.Value
            ));
        }
    }

    public class ColorSettings : EventArgs
    {
        public Color CursorColor { get; }
        public Color ClickColor { get; }
        public int Thickness { get; }
        public int Size { get; }

        public ColorSettings(Color cursorColor, Color clickColor, int thickness, int size)
        {
            CursorColor = cursorColor;
            ClickColor = clickColor;
            Thickness = thickness;
            Size = size;
        }
    }
}
