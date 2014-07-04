using System;
using System.Linq;
using System.Windows.Threading;

namespace LifeGame
{
    class ViewModel
    {
        public CellBoard CellBoard { get; private set; }

        public ViewModel()
        {
            CellBoard = new CellBoard(640, 640);
            const string patern = @"
                                              
                                              
                                              
                                      ¡       
                                      ¡ ¡     
    ¡                     ¡ ¡         ¡¡      
    ¡ ¡                   ¡¡                  
    ¡¡      ¡ ¡            ¡            ¡¡¡   
            ¡¡                          ¡     
      ¡¡¡    ¡             ¡¡            ¡    
      ¡                    ¡ ¡   ¡            
       ¡     ¡¡            ¡    ¡¡            
             ¡ ¡   ¡            ¡ ¡           
             ¡    ¡¡    ¡¡                    
                  ¡ ¡   ¡ ¡                   
                        ¡                     
                                           ¡¡ 
                                           ¡ ¡
                                           ¡  
                                              
                                              
                                ¡¡¡           
                                ¡             
                                 ¡            ";
            
            foreach(var c in patern
                .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .SelectMany((s, y) => s.Select((c, x) => new {x, y, visible = c == '¡'})))
            {
                CellBoard[c.x, c.y] = c.visible;
            }

            var dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal) { Interval = new TimeSpan(0, 0, 0, 0, 10) };
            dispatcherTimer.Tick += (sender1, e) => Update();
            dispatcherTimer.Start();
        }

        private void Update()
        {
            CellBoard.NextGeneration();
        }
    }
}