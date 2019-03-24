using EngineIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WPF_CSA_HomeIO
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool up = false;

        public bool Xs0prec { get; private set; }
        public bool Xs1prec { get; private set; }
        public bool Xs2prec { get; private set; }

        private bool upPrec;
        private bool downPrec;
        private bool down = false;
        private float volet;
        private bool haut;
        private bool bas;
        private bool frontUp;
        private bool frontDown;

        public bool Xs0 { get; private set; }
        public bool Xs1 { get; private set; }
        public bool Xs2 { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            //timer 
            this.Xs0 = true;
            DispatcherTimer Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(runCylceStore);
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
        }

        /**
         * Ouvre le store
         */

        private void btnStoreOuvrir(object sender, RoutedEventArgs e)
        {
            this.up = true;
            this.output.Text += "\n Ouvrir store";
        }

        private void btnStoreFermer(object sender, RoutedEventArgs e)
        {
            this.down = true;
            this.output.Text += "\n Fermer store";
        }


        private void runCylceStore(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("*********** Cycle ***********");

            this.textBox.Text = System.DateTime.Now.ToString();
            this.Xs0prec = this.Xs0; this.Xs1prec = this.Xs1; this.Xs2prec = this.Xs2;
            

            Console.WriteLine(this.up + "<- UP\n" + this.down+"<- down");
            Console.WriteLine(this.upPrec + "<- UPPrec\n" + this.downPrec+"<- downPrec");
            this.volet = MemoryMap.Instance.GetFloat(6, MemoryType.Input).Value;

            if (this.volet == 0)
            {
                this.haut = false; this.bas = true;
            }
            else if (this.volet == 10.0)
            {
                this.haut = true; this.bas = false;
            }
            else
            {
                this.haut = false; this.bas = false;
            }

            this.frontUp = !this.upPrec && this.up;
            this.frontDown = !this.downPrec && this.down;
            Console.WriteLine(this.frontUp + "<- UP\n" + this.frontDown + "<- frontDown");

            bool ft1s = this.Xs0 && this.frontUp;
            bool ft2s = this.Xs1 && this.haut;
            bool ft3s = this.Xs1 && this.frontDown;
            bool ft4s = this.Xs0 && this.frontDown;
            bool ft5s = this.Xs2 && this.bas;
            bool ft6s = this.Xs2 && this.frontUp;
            //super Vision
                //fonction de transition
            Console.WriteLine("ft1s->"+ ft1s);
            Console.WriteLine("ft2s->"+ ft2s);
            Console.WriteLine("ft3s->"+ ft3s);
            Console.WriteLine("ft4s->"+ ft4s);
            Console.WriteLine("ft5s->"+ ft5s);
            Console.WriteLine("ft6s->"+ ft6s);
                //étapes
            Console.WriteLine("Xs0->" + Xs0);
            Console.WriteLine("Xs1->" + Xs1);
            Console.WriteLine("Xs2->" + Xs2);

            this.Xs0 = (ft2s || ft5s) || this.Xs0prec && !(ft1s || ft4s);
            this.Xs1 = (ft1s || ft6s) || this.Xs1prec && !(ft2s || ft3s);
            this.Xs2 = (ft3s || ft4s) || this.Xs2prec && !(ft5s || ft6s);

            MemoryBit Monter = MemoryMap.Instance.GetBit(7, MemoryType.Output);
            MemoryBit Descendre = MemoryMap.Instance.GetBit(8, MemoryType.Output);
            Monter.Value = this.Xs1;
            Descendre.Value = this.Xs2;

            MemoryMap.Instance.Update();
            this.upPrec = this.up; this.downPrec = this.down;
            this.up = false;
            this.down = false;
        }
    }
}
