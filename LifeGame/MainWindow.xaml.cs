using System;
using System.Windows;
using System.Windows.Input;
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
            AntiAlias = false;
            var wb = new WriteableBitmap(ViewModel.CellBoard.Width, ViewModel.CellBoard.Height, 96, 96, PixelFormats.Bgr32, null);
            Canvas.Source = wb;

            CompositionTarget.Rendering += (sender, args) =>
            {
                wb.Lock();
                unsafe
                {
                    CellBoard.Render((uint*)wb.BackBuffer, wb.BackBufferStride, ViewModel.CellBoard.Width, ViewModel.CellBoard.Height, ViewModel.CellBoard.Cells);
                }
                wb.AddDirtyRect(new Int32Rect(0, 0, wb.PixelWidth, wb.PixelHeight));
                wb.Unlock();
            };
        }

        ViewModel ViewModel { get { return (ViewModel)DataContext; } }

        private void PlayCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.Play();
        }

        private void PauseCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.Pause();
        }

        private void StopCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.Stop();
        }

        public bool AntiAlias
        {
            set
            {
                RenderOptions.SetBitmapScalingMode(Canvas,
                    value ? BitmapScalingMode.Linear : BitmapScalingMode.NearestNeighbor);
            }
            get { return RenderOptions.GetBitmapScalingMode(Canvas) != BitmapScalingMode.NearestNeighbor; }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                var p = e.GetPosition(Canvas);
                ViewModel.CellBoard[(int)Math.Floor(p.X), (int)Math.Floor(p.Y)] = !ViewModel.CellBoard[(int)Math.Floor(p.X), (int)Math.Floor(p.Y)];
                ViewModel.CellBoard.CalcuratePopulation();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Play();
        }
    }
}
