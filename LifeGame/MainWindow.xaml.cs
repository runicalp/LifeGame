using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LifeGame
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = (ViewModel)DataContext;
            var wb = new WriteableBitmap(viewModel.CellBoard.Width, viewModel.CellBoard.Height, 96, 96, PixelFormats.Bgr32, null);
            canvas.Source = wb;
            // RenderOptions.SetBitmapScalingMode(canvas, BitmapScalingMode.NearestNeighbor);

            CompositionTarget.Rendering += (sender, args) =>
            {
                wb.Lock();
                unsafe
                {
                    CellBoard.Render((uint*)wb.BackBuffer, wb.BackBufferStride, viewModel.CellBoard.Width, viewModel.CellBoard.Height, viewModel.CellBoard.Cells);
                }
                wb.AddDirtyRect(new Int32Rect(0, 0, wb.PixelWidth, wb.PixelHeight));
                wb.Unlock();
            };
        }
    }
}
