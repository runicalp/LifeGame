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
            var wb = new WriteableBitmap(ViewModel.CellBoard.Width, ViewModel.CellBoard.Height, 96, 96, PixelFormats.Bgr32, null);
            canvas.Source = wb;
            // RenderOptions.SetBitmapScalingMode(canvas, BitmapScalingMode.NearestNeighbor);

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
            //PlayPauseButton.IsChecked = ViewModel.IsPlay;
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
    }
}
