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
            }
        }
    }

    public class MouseScrollElementBehavior : Behavior<FrameworkElement>
    {
        private bool isDrag;
        private Point mouseStartPosition;
        private Point elementStartPosition;

        #region ScrollViewer
        public ScrollViewer ScrollViewer
        {
            get { return (ScrollViewer)GetValue(ScrollViewerProperty); }
            set { SetValue(ScrollViewerProperty, value); }
        }

        public static readonly DependencyProperty ScrollViewerProperty =
            DependencyProperty.Register("ScrollViewer", typeof(ScrollViewer), typeof(MouseScrollElementBehavior), new PropertyMetadata(null));
        #endregion
        
        #region LockX
        public bool LockX
        {
            get { return (bool)GetValue(LockXProperty); }
            set { SetValue(LockXProperty, value); }
        }

        public static readonly DependencyProperty LockXProperty =
            DependencyProperty.Register("LockX", typeof(bool), typeof(MouseScrollElementBehavior), new PropertyMetadata(false));
        #endregion

        #region LockY
        public bool LockY
        {
            get { return (bool)GetValue(LockYProperty); }
            set { SetValue(LockYProperty, value); }
        }

        public static readonly DependencyProperty LockYProperty =
            DependencyProperty.Register("LockY", typeof(bool), typeof(MouseScrollElementBehavior), new PropertyMetadata(false));
        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonDown += MouseLeftButtonDown;
            AssociatedObject.MouseMove += MouseMove;
            AssociatedObject.MouseLeftButtonUp += MouseLeftButtonUp;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseLeftButtonDown -= MouseLeftButtonDown;
            AssociatedObject.MouseMove -= MouseMove;
            AssociatedObject.MouseRightButtonUp -= MouseLeftButtonUp;
            base.OnDetaching();
        }

        void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ScrollViewer == null) return;
            if ((Keyboard.GetKeyStates(Key.Space) & KeyStates.Down) == 0) return;

            isDrag = true;
            AssociatedObject.CaptureMouse();

            mouseStartPosition = e.GetPosition(ScrollViewer);

            elementStartPosition = new Point(
                ScrollViewer.HorizontalOffset,
                ScrollViewer.VerticalOffset);

        }

        void MouseMove(object sender, MouseEventArgs e)
        {
            if (ScrollViewer == null) return;
            if (!isDrag) return;

            var mouseCurrentPosition = e.GetPosition(ScrollViewer);
            var scrollOffset = elementStartPosition - (mouseCurrentPosition - mouseStartPosition);

            if (!LockX) ScrollViewer.ScrollToHorizontalOffset(scrollOffset.X);
            if (!LockY) ScrollViewer.ScrollToVerticalOffset(scrollOffset.Y);
        }

        void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDrag = false;
            if (AssociatedObject.IsMouseCaptured)
                AssociatedObject.ReleaseMouseCapture();
        }
    }
}
