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

using rtaNetworking.Streaming;
using System.Timers;
using System.Threading;

using System.Net; //Include this namespace

using System.Windows.Threading;

using System.Net.NetworkInformation;
using rtaNetworking.Streaming.nafis;


 

namespace WPF_RtaStreamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ImageStreamingServer _Server;
        private static int buttonClicked = 0;
        
        public void initializeTimer()
        {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private DateTime time = DateTime.MinValue;



        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if ( buttonClicked ==0 )
            {
                buttonClicked = 1;

                _Server = new ImageStreamingServer();

                _Server.MaxClients = 10;
                _Server.StartWithRandomPort();                            

                IpBox.Text = _Server.getServerURL();

                initializeTimer();
            }
            else
            {
                MessageBoxResult result = 
                    MessageBox.Show("Server Already Running!!", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        
        }


        /// <summary>
        ///  for client count
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // code goes here
            if (_Server != null)
            {
                int count = (_Server.Clients != null) ? _Server.Clients.Count() : 0;

                ClientCount.Text = "Clients: " + count.ToString();
            }
        
        }
           
    }
}
