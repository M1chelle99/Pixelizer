using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FunWithPictures
{
    public partial class WinMain : Form, IDisposable
    {
        private byte[] _imageBuffer;
        private MemoryStream _pictureStream;
        private Bitmap _picture;
        private bool _drawingBegun;
        private List<Point> _drawnPixels = new();

        public WinMain(string[] args)
        {
            InitializeComponent();

            _imageBuffer = File.ReadAllBytes(args[0]);
            _pictureStream = new MemoryStream(_imageBuffer);
            _picture = new Bitmap(_pictureStream);
            ClientSize = _picture.Size;

            Disposed += OnDispose;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            _drawnPixels.Clear();

            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (_drawingBegun)
                        return;

                    _drawingBegun = true;

                    using var graphics = GetGraphics_ThreadSafe();
                    var random = new Random();
                    while (true)
                    {
                        var x = random.Next(0, _picture.Width - 1);
                        var y = random.Next(0, _picture.Height - 1);
                        if (_drawnPixels.Contains(new(x, y)))
                            continue;

                        _drawnPixels.Add(new(x, y));
                        var color = _picture.GetPixel(x, y);
                        using var brush = new SolidBrush(color);
                        using var pen = new Pen(brush);
                        graphics.DrawRectangle(pen, new Rectangle(x, y, 1, 1));
                    }
                }
                finally
                {
                    _drawingBegun = false;
                }
            });
        }

        private delegate Graphics GetGraphics_SafeCallDelegate();
        private Graphics GetGraphics_ThreadSafe()
        {
            if (InvokeRequired)
                return (Graphics)Invoke(new GetGraphics_SafeCallDelegate(GetGraphics_ThreadSafe));
            else
                return Graphics.FromHwnd(Handle);
        }

        private void OnDispose(object sender, EventArgs e)
        {
            _picture.Dispose();
            _pictureStream.Dispose();
        }
    }
}
