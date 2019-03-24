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

namespace ScenarioCustom
{

    public partial class MainWindow : Window
    {

        private DispatcherTimer scheduler = new DispatcherTimer();





        /** Réceptivités **/
        private float brightnessSensor { get; set; }

        private bool IsActive { get { return this.switchAllumage.IsChecked.Value; } }

        private bool X0 {get; set;}
        private bool X1 { get; set; }
        private bool X0prec { get; set; }
        private bool X1prec { get; set; }

        private bool ft1 { get { return this.X0prec && ( this.IsActive && this.brightnessSensor <= 8); } }
        private bool ft2 { get { return this.X1prec && this.IsActive; } }



        /** Actionneurs **/
        private bool lightsA1 { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            this.initialize();

            this.scheduler.Tick += new EventHandler(Run);
            this.scheduler.Interval = new TimeSpan(0, 0, 1);
            this.scheduler.Start();

        }

        private void initialize()
        {
            this.switchAllumage.IsChecked = false;
            this.X0prec = true;
            this.X0 = true;

            this.updateValues();
        }


        private void updateValues()
        {
            MemoryMap.Instance.Update();

            this.X0prec = this.X0;
            this.X1prec = this.X1;


            this.X0 = this.ft2 || X0prec && !ft1;
            this.X1 = this.ft1 || X1prec && !ft2;

            this.brightnessSensor = MemoryMap.Instance.GetFloat(0, MemoryType.Input).Value;
           
        }

        private void updateUI()
        {
            //Ecriture dans l'interface
            this.labelLuminosite.Content = this.brightnessSensor;

            this.labelSwitchAllumage.Content = this.IsActive ? "Allumé" : "Eteint";
        }


        private void Run(object sender, EventArgs args)
        {
            Console.WriteLine("---------------------");
            Console.WriteLine("         Cycle       ");
            Console.WriteLine("---------------------");

            this.updateValues();


            MemoryMap.Instance.GetBit(0, MemoryType.Output).Value = this.X1;


            Console.WriteLine("X0       :"+ this.X0);
            Console.WriteLine("X1       :"+ this.X1);
            Console.WriteLine("-------------------");
            Console.WriteLine("X0prec   :" + this.X0prec);
            Console.WriteLine("X1prec   :" + this.X1prec);
            Console.WriteLine("-------------------");
            Console.WriteLine("ft1      :" + this.ft1);
            Console.WriteLine("ft2      :" + this.ft2);


            this.updateUI();
        }

    }
}
