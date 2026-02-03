using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Trayscout
{
    public partial class GlucoseDiagram : Form
    {
        private bool _isDragging;
        private Point _dragOffset;

        public GlucoseDiagram(Configuration config, IList<Entry> entries)
        {
            InitializeComponent();

            Width = config.Width;
            Height = config.Height;
            PictureBox.Width = config.Width;
            PictureBox.Height = config.Height;

            Size screenSize = Screen.PrimaryScreen.WorkingArea.Size;
            Location = new Point(screenSize.Width - Width - 16, screenSize.Height - Height - 16);

            PictureBox.Image = config.Style.DrawDiagram(config, entries);

            MouseDown += HandleMouseDown;
            MouseMove += HandleMouseMove;
            MouseUp += HandleMouseUp;
            PictureBox.MouseDown += HandleMouseDown;
            PictureBox.MouseMove += HandleMouseMove;
            PictureBox.MouseUp += HandleMouseUp;
        }

        private void HandleMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            _isDragging = true;
            Control control = sender as Control;
            Point screenPoint = control?.PointToScreen(e.Location) ?? PointToScreen(e.Location);
            _dragOffset = new Point(screenPoint.X - Location.X, screenPoint.Y - Location.Y);
        }

        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging)
            {
                return;
            }

            Control control = sender as Control;
            Point screenPoint = control?.PointToScreen(e.Location) ?? PointToScreen(e.Location);
            Location = new Point(screenPoint.X - _dragOffset.X, screenPoint.Y - _dragOffset.Y);
        }

        private void HandleMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = false;
            }
        }
    }
}
