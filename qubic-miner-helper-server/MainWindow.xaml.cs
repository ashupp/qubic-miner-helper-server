using SimpleTcp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace qubic_miner_helper_server
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SimpleTcpServer qhServer;
        private Dictionary<String, MachineState> currentConnectedMachines = new Dictionary<string, MachineState>();
        private Dictionary<String, String> currentConnectedMachinesIp = new Dictionary<string, string>();
        private float errorsFoundOverall;
        private DateTime errorsLastReset = DateTime.Now;
        private Timer collectDataTimer;


        //private QhServer qhServer;

        public MainWindow()
        {
            InitializeComponent();
            Init(true);
        }

        private void Init(bool startup)
        {
            this.ServerAddressTextBox.Text = Properties.Settings.Default.serverAddress;
            this.AutoStartServerCheckBox.IsChecked = Properties.Settings.Default.serverAutoStartOnOpen;


            if (Properties.Settings.Default.serverAutoStartOnOpen && startup)
            {
                StartServer();
            }

            this.errorsFoundOverall = 0;
            this.OverallErrorsFoundTextBox.Text = errorsFoundOverall.ToString();
            //this.ErrorsFoundLastResetTextBox.Text = errorsLastReset.ToString();
            this.currentConnectedMachines.Clear();
            this.currentConnectedMachinesIp.Clear();
            this.errorsLastReset = DateTime.Now;
            this.connectedMachinesStackPanel.Children.Clear();
        }


        private void CollectDataTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
           CollectOverallData();

           if (qhServer != null && qhServer.IsListening)
           {
               if (qhServer.IsListening && qhServer.GetClients().Count() > 0)
               {
                   Dispatcher.Invoke(() =>
                   {
                       ServerStatusTextBox.Text = "client/s connected";
                   });
               }
               else
               {
                   Dispatcher.Invoke(() =>
                   {
                       ServerStatusTextBox.Text = "listening";
                   });
               }
           }

        }



        private void CollectOverallData()
        {
            float iterationsCount = 0;
            float errorsCount = 0;

            Dispatcher.Invoke(() =>
            {
                // Durch alle Verbundenen Maschinen iterieren und Daten addieren
                foreach (connectedMachineDetails connectedMachine in connectedMachinesStackPanel.Children)
                {
                    iterationsCount += connectedMachine.machineCurrentIterationsPerSecond;
                    errorsCount += connectedMachine.machineSessionErrorsFound;
                }

                OverallIterationsPerSecondTextBox.Text = iterationsCount.ToString();
                OverallErrorsFoundTextBox.Text = errorsCount.ToString();
                if (qhServer != null)
                {
                    OverallMachinesConnectedCountTextBox.Text = qhServer.GetClients().Count().ToString();
                }
                else
                {
                    OverallMachinesConnectedCountTextBox.Text = "0";
                }
                    
            });



        }


        private void AutoStartServerCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.serverAutoStartOnOpen = (bool)AutoStartServerCheckBox.IsChecked;
            Properties.Settings.Default.Save();
        }



        private void StartTimer()
        {
            // Timer starten

            if (collectDataTimer == null)
            {
                collectDataTimer = new Timer(1000);
                collectDataTimer.Elapsed += CollectDataTimer_Elapsed;
            }

            if (!collectDataTimer.Enabled)
            {
                collectDataTimer.Start();
            }
        }

        private void StopTimer()
        {
            if (collectDataTimer != null)
            {
                collectDataTimer.Stop();
            }
        }

        private void StartServer()
        {
            ServerStatusTextBox.Text = "starting";
            try
            {
                if (qhServer == null)
                {
                    qhServer = new SimpleTcpServer(ServerAddressTextBox.Text);
                    qhServer.Events.ClientConnected += ClientConnected;
                    qhServer.Events.ClientDisconnected += ClientDisconnected;
                    qhServer.Events.DataReceived += DataReceived;
                }
                qhServer.Start();
                StartTimer();
                Console.WriteLine("Server started");
            }
            catch (Exception exception)
            {
                ServerStatusTextBox.Text = exception.Message;
                Console.WriteLine("Error starting server: " + exception.Message);
            }
        }

        private void StopServer()
        {
            ServerStatusTextBox.Text = "stopping";
            try
            {
                if (qhServer != null && qhServer.IsListening)
                {
                    Console.WriteLine("Disconnecting clients...");
                    StopTimer();
               

                    foreach (var qhServerClient in qhServer.GetClients())
                    {
                        qhServer.DisconnectClient(qhServerClient);
                    }
                    qhServer.Events.ClientConnected -= ClientConnected;
                    qhServer.Events.ClientDisconnected -= ClientDisconnected;
                    qhServer.Events.DataReceived -= DataReceived;

                    

                    qhServer.Stop();
                    qhServer.Dispose();
                    qhServer = null;
                    Init(false);
                }
                Console.WriteLine("Server stopped");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error during stopping of server: " + e);
                
            }
            ServerStatusTextBox.Text = "stopped";
        }


        void ClientConnected(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("[" + e.IpPort + "] client connected");
        }

        void ClientDisconnected(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("[" + e.IpPort + "] client disconnected: " + e.Reason.ToString());

            try
            {
                RemoveMachineByIpPort(e.IpPort);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error removing machine: " + exception);
            }

        }

        void RemoveMachineByIpPort(string ipPort)
        {
            if (currentConnectedMachinesIp.ContainsKey(ipPort))
            {
                var machineName = currentConnectedMachinesIp[ipPort];
                currentConnectedMachines.Remove(currentConnectedMachinesIp[ipPort]);
                currentConnectedMachinesIp.Remove(ipPort);

                Dispatcher.Invoke(() =>
                {
                    // Durch alle Verbundenen Maschinen iterieren und entsprechende entfernen
                    foreach (connectedMachineDetails connectedMachine in connectedMachinesStackPanel.Children)
                    {
                        if (connectedMachine.MachineName == machineName)
                        {
                            connectedMachinesStackPanel.Children.Remove(connectedMachine);
                            break;
                        }
                    }

                    /*if (connectedMachinesStackPanel.Children.Count <= 0)
                    {
                        
                    }*/
                });

            }
        }

        void DataReceived(object sender, DataReceivedEventArgs e)
        {
            //Console.WriteLine("[" + e.IpPort + "]: " + Encoding.UTF8.GetString(e.Data));
            var rawString = Encoding.UTF8.GetString(e.Data);

            var clearedData = rawString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if (clearedData.Length == 0)
                return;

            MachineState receivedMachineState;
            try
            {
                receivedMachineState = JsonConvert.DeserializeObject<MachineState>(clearedData[0]);

                if (receivedMachineState.machineName != string.Empty)
                {
                    if (currentConnectedMachines != null && currentConnectedMachines.ContainsKey(receivedMachineState.machineName))
                    {
                        currentConnectedMachines[receivedMachineState.machineName] = receivedMachineState;

                        Dispatcher.Invoke(() =>
                        {
                            // Durch alle Verbundenen Maschinen iterieren und vorhandene aktualisieren
                            foreach (connectedMachineDetails connectedMachine in connectedMachinesStackPanel.Children)
                            {
                                if (connectedMachine.MachineName == receivedMachineState.machineName)
                                {
                                    connectedMachine.updateData(receivedMachineState);
                                    CollectOverallData();
                                    break;
                                }
                            }
                        });

                        
                    }
                    else
                    {
                        // Neu hinzufügen
                        
                        // Erst zur Liste der aktuell verbundenen
                        currentConnectedMachines.Add(receivedMachineState.machineName, receivedMachineState);
                        currentConnectedMachinesIp.Add(e.IpPort, receivedMachineState.machineName);

                        // Dann das Control hinzufügen

                        Dispatcher.Invoke(() =>
                        {
                            connectedMachineDetails connectedMachineDetails = new connectedMachineDetails();
                            connectedMachineDetails.setData(receivedMachineState);
                            connectedMachinesStackPanel.Children.Add(connectedMachineDetails);
                            CollectOverallData();
                        });

                        
                    }

                }

                
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error receiving machine state: " + exception);
            }

        }

        private void StartServerButton_Click(object sender, RoutedEventArgs e)
        {
            StartServer();
        }

        private void StopServerButton_Click(object sender, RoutedEventArgs e)
        {
            StopServer();
        }

        private void ResetErrorsFoundNowButton_Click(object sender, RoutedEventArgs e)
        {
            errorsLastReset = DateTime.Now;
            //ErrorsFoundLastResetTextBox.Text = errorsLastReset.ToString();
        }
    }
}
