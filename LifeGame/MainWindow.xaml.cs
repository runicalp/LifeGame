using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
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
            Canvas.Source = wb;
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

    public class MyMouseDragElementBehavior : Behavior<FrameworkElement>
    {
        private bool _IsDrag = false;
        private Point _MouseStartPosition;
        private Point _ElementStartPosition;

        #region Canvas
        public Canvas Canvas
        {
            get { return (Canvas)GetValue(CanvasProperty); }
            set { SetValue(CanvasProperty, value); }
        }

        public static readonly DependencyProperty CanvasProperty =
            DependencyProperty.Register("Canvas", typeof(Canvas), typeof(MyMouseDragElementBehavior), new PropertyMetadata(null));
        #endregion
        
        #region LockX
        public bool LockX
        {
            get { return (bool)GetValue(LockXProperty); }
            set { SetValue(LockXProperty, value); }
        }

        public static readonly DependencyProperty LockXProperty =
            DependencyProperty.Register("LockX", typeof(bool), typeof(MyMouseDragElementBehavior), new PropertyMetadata(false));
        #endregion

        #region LockY
        public bool LockY
        {
            get { return (bool)GetValue(LockYProperty); }
            set { SetValue(LockYProperty, value); }
        }

        public static readonly DependencyProperty LockYProperty =
            DependencyProperty.Register("LockY", typeof(bool), typeof(MyMouseDragElementBehavior), new PropertyMetadata(false));
        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            AssociatedObject.MouseLeftButtonUp += AssociatedObject_MouseLeftButtonUp;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseLeftButtonDown;
            AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            AssociatedObject.MouseRightButtonUp -= AssociatedObject_MouseLeftButtonUp;
            base.OnDetaching();
        }

        void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Canvas == null) return;

            _IsDrag = true;
            // ドラッグ開始時のマウスと要素の位置を取得する。
            _MouseStartPosition = e.GetPosition(Canvas);

            _ElementStartPosition = new Point(
                Canvas.GetLeft(AssociatedObject),
                Canvas.GetTop(AssociatedObject));
            if (double.IsNaN(_ElementStartPosition.X)) _ElementStartPosition.X = 0.0;
            if (double.IsNaN(_ElementStartPosition.Y)) _ElementStartPosition.Y = 0.0;

            AssociatedObject.CaptureMouse();
        }

        void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (Canvas == null) return;

            if (_IsDrag)
            {
                // 移動後の要素の位置を計算する
                var currentMousePos = e.GetPosition(Canvas);
                var currentElementPos = _ElementStartPosition
                                                + (currentMousePos - _MouseStartPosition);

                // Canvas内に収まる様に補正する
//                var maxX = Canvas.ActualWidth - AssociatedObject.ActualWidth;
//                if (currentElementPos.X < 0)
//                {
//                    currentElementPos.X = 0.0;
//                }
//                else if (maxX < currentElementPos.X)
//                {
//                    currentElementPos.X = maxX;
//                }
//
//                var maxY = Canvas.ActualHeight - AssociatedObject.ActualHeight;
//                if (currentElementPos.Y < 0)
//                {
//                    currentElementPos.Y = 0.0;
//                }
//                else if (maxY < currentElementPos.Y)
//                {
//                    currentElementPos.Y = maxY;
//                }

                // 要素を移動させる
                if (!LockX) Canvas.SetLeft(AssociatedObject, currentElementPos.X);
                if (!LockY) Canvas.SetTop(AssociatedObject, currentElementPos.Y);
            }
        }

        void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_IsDrag)
            {
                _IsDrag = false;
                AssociatedObject.ReleaseMouseCapture();
            }
        }
    }
}
