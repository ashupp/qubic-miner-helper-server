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

            MachineNameTextBox.Text = receivedMachineState.machineName;
            IterationsTextBox.Text = receivedMachineState.overallCurrentIterationsPerSecond.ToString();
            ErrorsFoundTextBox.Text = receivedMachineState.overallSessionErrorsFound.ToString();
            WorkerCountTextBox.Text = receivedMachineState.overallWorkerCount.ToString();
            CommandLineTextBox.Text = receivedMachineState.currentCommandLine;
            QinerVersionTextBox.Text = receivedMachineState.currentMinerVersion;

            LastUpdateTextBox.Text = DateTime.Now.ToString();
            //receivedMachineState.currentMachineDateTime.ToString();
        }

        private void RestartWorkerButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
