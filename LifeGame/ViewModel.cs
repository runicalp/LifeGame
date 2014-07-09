using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using LifeGame.Annotations;

namespace LifeGame
{
    sealed class ViewModel : INotifyPropertyChanged
    {
        private readonly DispatcherTimer dispatcherTimer;
        private bool isPlay;
        public CellBoard CellBoard { get; private set; }

        public ViewModel()
        {
            Initialize();

            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal) { Interval = new TimeSpan(0, 0, 0, 0, 10) };
            dispatcherTimer.Tick += (sender1, e) => Update();
            Play();
        }

        private void Initialize()
        {
            CellBoard = new CellBoard(640, 640);
            const string patern = @"
Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†
Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†
Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†
Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å†Å†Å†Å†Å†Å†Å†
Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å†Å°Å†Å†Å†Å†Å†
Å†Å†Å†Å†Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å†Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å°Å†Å†Å†Å†Å†Å†
Å†Å†Å†Å†Å°Å†Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†
Å†Å†Å†Å†Å°Å°Å†Å†Å†Å†Å†Å†Å°Å†Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å°Å°Å†Å†Å†
Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å†Å†Å†Å†Å†
Å†Å†Å†Å†Å†Å†Å°Å°Å°Å†Å†Å†Å†Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å†Å†Å†Å†
Å†Å†Å†Å†Å†Å†Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å†Å°Å†Å†Å†Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†
Å†Å†Å†Å†Å†Å†Å†Å°Å†Å†Å†Å†Å†Å°Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å†Å†Å†Å†Å°Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†
Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å†Å°Å†Å†Å†Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å†Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†
Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å†Å†Å†Å†Å°Å°Å†Å†Å†Å†Å°Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†
Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å†Å°Å†Å†Å†Å°Å†Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†
Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†
Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å°Å†
Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å†Å°
Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å†Å†
Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†
Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†
Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å°Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†
Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†
Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å°Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†Å†";

            foreach (var c in patern
                .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .SelectMany((s, y) => s.Select((c, x) => new { x, y, visible = c == 'Å°' })))
            {
                CellBoard[c.x, c.y] = c.visible;
            }
        }

        private void Update()
        {
            CellBoard.NextGeneration();
        }

        public bool IsPlay
        {
            get { return isPlay; }
            private set
            {
                if (value.Equals(isPlay)) return;
                isPlay = value;
                OnPropertyChanged();
            }
        }

        public void Play()
        {
            dispatcherTimer.Start();
            IsPlay = true;
        }

        public void Pause()
        {
            dispatcherTimer.Stop();
            IsPlay = false;
        }

        public void Stop()
        {
            Pause();
            Initialize();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}