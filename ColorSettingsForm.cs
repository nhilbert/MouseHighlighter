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
        private readonly Label opacityLabel;

        public event EventHandler<ColorSettings> SettingsChanged;

        public ColorSettingsForm(Color currentCursorColor, Color currentClickColor, float currentOpacity)
        {
            this.Text = "Mouse Highlighter Settings";
            this.Size = new Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Padding = new Padding(20);

            // Color section
            var colorGroupBox = new GroupBox
            {
                Text = "Colors",
                Dock = DockStyle.Top,
                Height = 120,
                Padding = new Padding(10)
            };

            cursorColorButton = new Button
            {
                Text = "Cursor Color",
                Width = 150,
                Height = 30,
                Location = new Point(10, 30),
                BackColor = currentCursorColor,
                FlatStyle = FlatStyle.Flat
            };
            cursorColorButton.Click += (s, e) => ChooseColor(cursorColorButton);

            clickColorButton = new Button
            {
                Text = "Click Color",
                Width = 150,
                Height = 30,
                Location = new Point(180, 30),
                BackColor = currentClickColor,
                FlatStyle = FlatStyle.Flat
            };
            clickColorButton.Click += (s, e) => ChooseColor(clickColorButton);

            colorGroupBox.Controls.AddRange(new Control[] { cursorColorButton, clickColorButton });

            // Opacity section
            var opacityGroupBox = new GroupBox
            {
                Text = "Transparency",
                Dock = DockStyle.Top,
                Height = 100,
                Padding = new Padding(10),
                Top = 130
            };

            opacityLabel = new Label
            {
                Text = "Opacity: " + (currentOpacity * 100).ToString("0") + "%",
                AutoSize = true,
                Location = new Point(10, 30)
            };

            opacityTrackBar = new TrackBar
            {
                Location = new Point(10, 50),
                Width = 340,
                Minimum = 1,
                Maximum = 100,
                Value = (int)(currentOpacity * 100),
                TickFrequency = 10,
                TickStyle = TickStyle.BottomRight
            };
            opacityTrackBar.ValueChanged += (s, e) =>
            {
                opacityLabel.Text = "Opacity: " + opacityTrackBar.Value.ToString() + "%";
                NotifySettingsChanged();
            };

            opacityGroupBox.Controls.AddRange(new Control[] { opacityLabel, opacityTrackBar });

            // Button section
            var buttonPanel = new Panel
            {
                Height = 40,
                Dock = DockStyle.Bottom
            };

            var applyButton = new Button
            {
                Text = "Apply and Close",
                Width = 120,
                Height = 30,
                DialogResult = DialogResult.OK,
                Anchor = AnchorStyles.Right
            };
            applyButton.Location = new Point(buttonPanel.Width - applyButton.Width - 10, 5);
            applyButton.Click += (s, e) => this.Close();

            buttonPanel.Controls.Add(applyButton);

            this.Controls.AddRange(new Control[] 
            { 
                colorGroupBox,
                opacityGroupBox,
                buttonPanel
            });
        }

        private void ChooseColor(Button button)
        {
            using (var colorDialog = new ColorDialog
            {
                Color = button.BackColor,
                FullOpen = true
            })
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    button.BackColor = colorDialog.Color;
                    NotifySettingsChanged();
                }
            }
        }

        private void NotifySettingsChanged()
        {
            SettingsChanged?.Invoke(this, new ColorSettings(
                cursorColorButton.BackColor,
                clickColorButton.BackColor,
                opacityTrackBar.Value / 100f
            ));
        }
    }

    public class ColorSettings : EventArgs
    {
        public Color CursorColor { get; }
        public Color ClickColor { get; }
        public float Opacity { get; }

        public ColorSettings(Color cursorColor, Color clickColor, float opacity)
        {
            CursorColor = cursorColor;
            ClickColor = clickColor;
            Opacity = opacity;
        }
    }
}
