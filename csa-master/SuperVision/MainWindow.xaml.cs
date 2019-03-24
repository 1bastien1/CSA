using EngineIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Threading.Tasks;

namespace SuperVision
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //Chambre J
        private string météo;
        private double température;
        private double humidité;
        private double luminosité;


        public MainWindow()
        {
            InitializeComponent();

            this.mainWindow.Show();

            DispatcherTimer Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(update);
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();

           
        }

        public void update(object sender, EventArgs e)
        {
           
                this.SetMétéo();
                this.temp.Content = this.GetTemp() + "C°";
                this.humidity.Content = MemoryMap.Instance.GetFloat(133, MemoryType.Memory).Value + "%";
                this.lum.Content = this.GetLum() ? "Jour" : "Nuit";
                this.ventlbl.Content = this.GetWInd() + "Km.h-1";
                this.check1.IsChecked = this.PresencePorte();
                this.check2.IsChecked = this.presencePièce();



    
                //Supervision console
                Console.WriteLine("--------------------");
                Console.WriteLine("Météo : " + this.météo);
                Console.WriteLine("Présence à la porte : " + this.PresencePorte());
                Console.WriteLine("Présence dans la pièce : " + this.presencePièce());
                Console.WriteLine("température : " + this.température);
                Console.WriteLine("Humidité : " + this.humidité);
                Console.WriteLine("Luminosité : " + this.lum);
                Console.WriteLine("--------------------");
                Console.WriteLine("Sleep for 2 seconds.");

                MemoryMap.Instance.Update();
            
        }

        public void SetMétéo()
        {
            if (this.GetMeteo() == "pluie")
                this.img.Source = new BitmapImage(new Uri("nuage.jpg", UriKind.RelativeOrAbsolute));
            if (this.GetMeteo() == "soleil")
                this.img.Source = new BitmapImage(new Uri("soleil.jpg", UriKind.RelativeOrAbsolute));
            if (this.GetMeteo() == "vent")
                this.img.Source = new BitmapImage(new Uri("vent.jpg", UriKind.RelativeOrAbsolute));
        }

        public string GetMeteo()
        {
            //retourne "pluie", "vent", "soleil"
            if(GetWInd() > 35.0)
            {
                return "vent";
            }
            if(humidité > 80.0)
            {
                return "pluie";
            }
            return "soleil";

        }
        public double GetTemp()
        {
            //retourne des degrés C°
            return Math.Round(MemoryMap.Instance.GetFloat(132, MemoryType.Memory).Value - 273.15, 2);
        }
        public double GetWInd()
        {
            //retourne des Km.h-1
            return Math.Round(MemoryMap.Instance.GetFloat(137, MemoryType.Memory).Value * 3.6, 2);
        }

        public bool PresencePorte()
        {
            return MemoryMap.Instance.GetBit(169, MemoryType.Input).Value && MemoryMap.Instance.GetBit(170, MemoryType.Input).Value;
        }
        public bool presencePièce()
        {
            return MemoryMap.Instance.GetBit(171, MemoryType.Input).Value;
        }

        public bool GetLum()
        {
            return MemoryMap.Instance.GetBit(122, MemoryType.Output).Value;
        }



    }
}
