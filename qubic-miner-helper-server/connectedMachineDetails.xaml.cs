using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace qubic_miner_helper_server
{
    /// <summary>
    /// Interaktionslogik für connectedMachineDetails.xaml
    /// </summary>
    public partial class connectedMachineDetails : UserControl
    {
        public string MachineName;
        public float machineCurrentIterationsPerSecond = 0;
        public float machineSessionErrorsFound = 0;
        public int machineWorkerCount = 0;
        public string machineCommandLine = String.Empty;
        public string machineQinerVersion = String.Empty;
        public MachineState machineState;

        public connectedMachineDetails()
        {
            InitializeComponent();
            this.Background = new SolidColorBrush(Colors.LightGray);
        }

        public void setData(MachineState receivedMachineState)
        {
            MachineName = receivedMachineState.machineName;
            updateData(receivedMachineState);
        }

        public void updateData(MachineState receivedMachineState)
        {
            machineCurrentIterationsPerSecond = receivedMachineState.overallCurrentIterationsPerSecond;
            machineSessionErrorsFound = receivedMachineState.overallSessionErrorsFound;
            machineWorkerCount = receivedMachineState.overallWorkerCount;
            machineCommandLine = receivedMachineState.currentCommandLine;
            machineQinerVersion = receivedMachineState.currentMinerVersion;
            machineState = receivedMachineState;

            MachineNameTextBox.Text = receivedMachineState.machineName;
            if (receivedMachineState.currentHelperVersion != String.Empty && receivedMachineState.currentHelperVersion != null)
            {
                HelperVersionTextBox.Text = receivedMachineState.currentHelperVersion;
                HelperVersionTextBox.Background = Brushes.White;
                HelperVersionTextBox.Foreground = Brushes.Black;
            }
            else
            {
                HelperVersionTextBox.Text = "not found - please update helper";
                HelperVersionTextBox.Background = Brushes.DarkRed;
                HelperVersionTextBox.Foreground = Brushes.White;
            }


            // Enthält der Helper bereits den Wert LastTimeErrorFound?
            if (!string.IsNullOrEmpty(receivedMachineState.currentHelperVersion))
            {
                var currMachineVersion = Version.Parse(receivedMachineState.currentHelperVersion);
                if (currMachineVersion < new Version(1, 1, 2, 0))
                {
                    setLastTimeErrorFoundNotAvailable();
                }
                else
                {
                    setLastTimeErrorFoundAvailable();
                    LastTimeErrorFoundTextBox.Text = receivedMachineState.lastErrorReductionByMachineDateTime != default ? receivedMachineState.lastErrorReductionByMachineDateTime.ToString() : "Waiting for next solution...";
                }
            }
            else
            {
                setLastTimeErrorFoundNotAvailable();
            }

            
            IterationsTextBox.Text = receivedMachineState.overallCurrentIterationsPerSecond.ToString();
            ErrorsFoundTextBox.Text = receivedMachineState.overallSessionErrorsFound.ToString();
            WorkerCountTextBox.Text = receivedMachineState.overallWorkerCount.ToString();
            CommandLineTextBox.Text = receivedMachineState.currentCommandLine;
            QinerVersionTextBox.Text = receivedMachineState.currentMinerVersion;
            LastUpdateTextBox.Text = receivedMachineState.currentMachineDateTime.ToString();
        }

        private void setLastTimeErrorFoundNotAvailable()
        {
            LastTimeErrorFoundTextBox.Text = "not found - please update helper";
            LastTimeErrorFoundTextBox.Background = Brushes.DarkRed;
            LastTimeErrorFoundTextBox.Foreground = Brushes.White;
        }

        private void setLastTimeErrorFoundAvailable()
        {
            LastTimeErrorFoundTextBox.Background = Brushes.White;
            LastTimeErrorFoundTextBox.Foreground = Brushes.Black;
        }

        private void RestartWorkerButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
