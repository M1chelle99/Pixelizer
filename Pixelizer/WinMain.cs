namespace Pixelizer;

public partial class WinMain : Form
{
    private readonly MemoryStream _pictureStream;
    private readonly Bitmap _picture;
    private bool _drawing;
    private readonly List<Point> _drawnPixels = new();

    public WinMain(IReadOnlyList<string> args)
    {
        InitializeComponent();

        var imageBuffer = File.ReadAllBytes(args[0]);
        _pictureStream = new MemoryStream(imageBuffer);
        _picture = new Bitmap(_pictureStream);
        ClientSize = _picture.Size;

        Disposed += OnDispose;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        _drawnPixels.Clear();
        Task.Factory.StartNew(RenderSafe);
    }

    private void RenderSafe()
    {
        try
        {
            Render();
        }
        finally
        {
            _drawing = false;
        }
    }

    private void Render()
    {
        if (_drawing) return;
        _drawing = true;

        using var graphics = GetGraphics_ThreadSafe();
        var random = new Random();
        while (true)
        {
            var x = random.Next(0, _picture.Width - 1);
            var y = random.Next(0, _picture.Height - 1);
            if (_drawnPixels.Contains(new Point(x, y)))
                continue;

            _drawnPixels.Add(new Point(x, y));
            var color = _picture.GetPixel(x, y);
            using var brush = new SolidBrush(color);
            using var pen = new Pen(brush);
            graphics.DrawRectangle(pen, new Rectangle(x, y, 1, 1));
        }
    }
 
    private delegate Graphics GetGraphicsSafe();

    private Graphics GetGraphics_ThreadSafe()
    {
        if (InvokeRequired)
            return (Graphics)Invoke(new GetGraphicsSafe(GetGraphics_ThreadSafe));

        return Graphics.FromHwnd(Handle);
    }

    private void OnDispose(object sender, EventArgs e)
    {
        _picture.Dispose();
        _pictureStream.Dispose();
    }
}