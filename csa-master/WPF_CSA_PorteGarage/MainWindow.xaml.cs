using EngineIO;
using System;
using System.Windows;
using System.Windows.Threading;

namespace WPF_CSA_PorteGarage
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //bouton fermer/ouvrir
        private bool up = false;
        private bool down = false;

        //Etapes precedentes
        public bool Xs0prec { get; private set; }
        public bool Xs1prec { get; private set; }
        public bool Xs2prec { get; private set; }
        public bool Xs3prec { get; private set; }
        public bool Xs4prec { get; private set; }
        public bool Xs5prec { get; private set; }
        //Etapes
        public bool Xs0 { get; private set; }
        public bool Xs1 { get; private set; }
        public bool Xs2 { get; private set; }
        public bool Xs3 { get; private set; }
        public bool Xs4 { get; private set; }
        public bool Xs5 { get; private set; }

        //bouton fermer/ouvrir prec
        private bool upPrec;

        //volet = ouvert ou fermer
        private bool porte;

        private bool frontUp { get; set; }
        

        public bool capteurPorteOuverte { get; private set; }
        public bool capteurPorteFermee { get; private set; }
        public bool capteurPresenceIR { get; private set; }

        public int timerTime { get; private set; }
        public bool finTimer { get; private set; }

        private bool fintimer = false;

        private DispatcherTimer timerOuverture = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            //timer 
            this.Xs0 = true;
            this.Xs1 = false;
            this.Xs2 = false;
            this.Xs3 = false;
            this.Xs4 = false;
            this.Xs5 = false;

            
            timerOuverture.Tick += new EventHandler(this.timeClock);
            timerOuverture.Interval = new TimeSpan(0, 0, 2);

            DispatcherTimer Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(runCycleGarage);
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();

            
        }

        private void timeClock(object sender, EventArgs e)
        {
            Console.WriteLine("Clock :" + this.timerTime);
            this.timerTime += 1;
        }

        private void btnPorteOuvrir(object sender, RoutedEventArgs e)
        {
            this.up = true;
            this.output.Text += "\n >Bouton pressé<";
        }

        private void print(String msg)
        {
            this.output.Text += "\n" + msg;
        }


        private void runCycleGarage(object sender, EventArgs e)
        {
            Console.WriteLine("*********** Cycle ***********");
            this.textBox.Text = System.DateTime.Now.ToString();

            //Sauvegarde des états précédents
            this.Xs0prec = this.Xs0;
            this.Xs1prec = this.Xs1;
            this.Xs2prec = this.Xs2;
            this.Xs3prec = this.Xs3;
            this.Xs4prec = this.Xs4;
            this.Xs5prec = this.Xs5;

            //Sauvegarde des receptivités
            this.capteurPorteOuverte = MemoryMap.Instance.GetBit(100, MemoryType.Input).Value;
            this.capteurPorteFermee  = MemoryMap.Instance.GetBit(101, MemoryType.Input).Value;
            this.capteurPresenceIR = MemoryMap.Instance.GetBit(102, MemoryType.Input).Value;

            this.frontUp = this.up && !this.upPrec;

            this.finTimer = this.timerTime < 5 ? false : true;

            bool ft1s = this.Xs0prec && this.frontUp;
            bool ft2s = this.Xs1prec && this.capteurPorteOuverte;
            bool ft3s = this.Xs2prec;
            bool ft4s = this.Xs3prec && this.finTimer; //tempo
            bool ft5s = this.Xs4prec && this.capteurPorteFermee;
            bool ft6s = this.Xs5prec;
            bool ft7s = this.Xs4prec && !this.capteurPorteFermee && (this.frontUp || this.capteurPresenceIR);

            Console.WriteLine("ft1s-> " + ft1s);
            Console.WriteLine("ft2s-> " + ft2s);
            Console.WriteLine("ft3s-> " + ft3s);
            Console.WriteLine("ft4s-> " + ft4s);
            Console.WriteLine("ft5s-> " + ft5s);
            Console.WriteLine("ft6s-> " + ft6s);
            Console.WriteLine("--------------");
            Console.WriteLine("Xs0-> " + this.Xs0);
            Console.WriteLine("Xs1-> " + this.Xs1);
            Console.WriteLine("Xs2-> " + this.Xs2);
            Console.WriteLine("Xs3> "  + this.Xs3);
            Console.WriteLine("Xs4-> " + this.Xs4);
            Console.WriteLine("Xs5-> " + this.Xs5);
            Console.WriteLine("Xs0prec-> " + this.Xs0prec);
            Console.WriteLine("Xs1prec-> " + this.Xs1prec);
            Console.WriteLine("Xs2prec-> " + this.Xs2prec);
            Console.WriteLine("Xs3prec> " + this.Xs3prec);
            Console.WriteLine("Xs4prec-> " + this.Xs4prec);
            Console.WriteLine("Xs5prec-> " + this.Xs5prec);

            //TODO Continuer
            //Faire les étapes et agencer leurs transitions

            //Xn(T) = FT1(t) + Xn(T-1) . !FT2(t)
            this.Xs0 = ft6s || this.Xs0prec && !ft1s;
            this.Xs1 = (ft1s || ft7s) || this.Xs1prec && !ft2s;
            this.Xs2 = ft2s || this.Xs2prec && !ft3s;
            this.Xs3 = ft3s || this.Xs3prec && !ft4s;
            this.Xs4 = ft4s || this.Xs4prec && !(ft5s || ft7s);
            this.Xs5 = ft5s || this.Xs5prec && !ft6s;


            if(this.Xs1)
            {
                //ouvrir
                MemoryMap.Instance.GetBit(72, MemoryType.Output).Value = true;

            }
            if(this.Xs2 || this.Xs5)
            {
                //stopper
                MemoryMap.Instance.GetBit(72, MemoryType.Output).Value = false;
                MemoryMap.Instance.GetBit(73, MemoryType.Output).Value = false;

            }
            if (this.Xs4)
            {
                //fermer
                MemoryMap.Instance.GetBit(73, MemoryType.Output).Value = true;

            }


            // Etape 3 : Chronomètre
                if (this.Xs3)
            {
                if (!this.timerOuverture.IsEnabled)
                {
                    this.timerTime = 0;
                    this.fintimer = false;
                    this.timerOuverture.Start();
                    Console.WriteLine(">Activation du Timer de fermeture");
                }
                if (this.timerTime >= 5)
                {
                    this.fintimer = true;
                    this.timerTime = 0;
                    this.timerOuverture.Stop();
                }
            }



            //Resets des entrées / Réceptivités
            this.upPrec = this.up;
            this.up = false;

            MemoryMap.Instance.Update();

        }
    }
}
