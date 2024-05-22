// Lewis Cottrill
// 20/5/2024
// C# AT3

using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel; // Import standard imports
using Xceed.Wpf.Toolkit; // Import custom toolkit
using System.Windows.Controls.Primitives; 

namespace DroneService // Namespace
{
    public partial class MainWindow : Window // Partial class, inherits from Window class
    {
        List<Drone> FinishedList = new List<Drone>(); // Create list of Drone class 
        Queue<Drone> RegularService = new Queue<Drone>(); // Create queue of Drone class
        Queue<Drone> ExpressService = new Queue<Drone>(); // Create queue of Drone class

        public MainWindow() // Define the constructor for MainWindow class
        {
            InitializeComponent(); // GUI stuff
            serviceTagUpDownControl.Value = 100; // Set serviceTag to 100
            serviceTagUpDownControl.ValueChanged += serviceTagUpDownControl_ValueChanged;
            this.finishedServiceListBox.MouseDoubleClick += finishedServiceListBox_MouseDoubleClick;
            this.MouseLeftButtonDown += mainGridMouseClick;
            GridView regularGridView = new GridView(); // Create grid for list columns
            regularGridView.Columns.Add(new GridViewColumn { Header = "Service Tag Number", DisplayMemberBinding = new Binding("ServiceTagNumber") });
            regularGridView.Columns.Add(new GridViewColumn { Header = "Client Name", DisplayMemberBinding = new Binding("ClientName") });
            regularGridView.Columns.Add(new GridViewColumn { Header = "Drone Model", DisplayMemberBinding = new Binding("DroneModel") });
            regularGridView.Columns.Add(new GridViewColumn { Header = "Service Problem", DisplayMemberBinding = new Binding("ServiceProblem") });
            regularGridView.Columns.Add(new GridViewColumn { Header = "Service Cost", DisplayMemberBinding = new Binding("ServiceCost") });
            regularServiceListView.View = regularGridView;
            GridView expressGridView = new GridView();
            expressGridView.Columns.Add(new GridViewColumn { Header = "Service Tag Number", DisplayMemberBinding = new Binding("ServiceTagNumber") });
            expressGridView.Columns.Add(new GridViewColumn { Header = "Client Name", DisplayMemberBinding = new Binding("ClientName") });
            expressGridView.Columns.Add(new GridViewColumn { Header = "Drone Model", DisplayMemberBinding = new Binding("DroneModel") });
            expressGridView.Columns.Add(new GridViewColumn { Header = "Service Problem", DisplayMemberBinding = new Binding("ServiceProblem") });
            expressGridView.Columns.Add(new GridViewColumn { Header = "Service Cost", DisplayMemberBinding = new Binding("ServiceCost") });
            expressServiceListView.View = expressGridView;
        }
        private string GetPriority() // Radio Button value getter
        {
            if (lowPriorityRadioButton.IsChecked == true)
            {
                return "Low";
            }
            else if (highPriorityRadioButton.IsChecked == true)
            {
                return "High";
            }
            else
            {
                return null;
            }
        }
        private void serviceTagUpDownControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e) // Service tags numerical rocker switch 
        {
            if (serviceTagUpDownControl.Value != null) // If not null
            {
                int currentValue = int.Parse(serviceTagTextBox.Text); // Parse current value from service tag text box
                int newValue = (int)serviceTagUpDownControl.Value; // Get the new value from service tag numerical rocker switch

                if (newValue > currentValue && currentValue < 900) // Checking numerical boundaries
                {
                    currentValue += 10; // Increase by 10
                    serviceTagTextBox.Text = currentValue.ToString(); // Change text box value to service tag numerical rocker switch value
                    serviceTagUpDownControl.Value = currentValue;
                }
                else if (newValue < currentValue && currentValue > 100) // Checking numerical boundaries
                {
                    currentValue -= 10; // Decrease by 10
                    serviceTagTextBox.Text = currentValue.ToString(); // Change text box value to service tag numerical rocker switch value
                    serviceTagUpDownControl.Value = currentValue;
                }
                else if (newValue < 100 || newValue > 900) // If out of numerical bounds
                {
                    System.Windows.MessageBox.Show("Service tag number must be between 100 and 900.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    serviceTagUpDownControl.Value = 100;  // Reset to 100
                }
            }
        }
        private void UpdateListView() // Method to update the ListViews
        {
            regularServiceListView.ItemsSource = null;
            regularServiceListView.ItemsSource = RegularService.Select(d => new { Drone = d, d.ServiceTagNumber, d.ClientName, d.DroneModel, d.ServiceProblem, d.ServiceCost }); // Instert data
            expressServiceListView.ItemsSource = null;
            expressServiceListView.ItemsSource = ExpressService.Select(d => new { Drone = d, d.ServiceTagNumber, d.ClientName, d.DroneModel, d.ServiceProblem, d.ServiceCost });
            finishedServiceListBox.ItemsSource = null;
            finishedServiceListBox.ItemsSource = FinishedList.Select(d => new { ClientName = d.ClientName, ServiceCost = d.ServiceCost });
        }
        private void regularServiceListView_SelectionChanged(object sender, SelectionChangedEventArgs e) 
        {
            expressServiceListView.SelectedItem = null; // Clear selected item
            finishedServiceListBox.SelectedItem = null; // Clear selected item
            if (regularServiceListView.SelectedItem != null) // If item selected, get data and update the text boxes and radio buttons
            {
                var selectedItem = (dynamic)regularServiceListView.SelectedItem;
                Drone selectedDrone = selectedItem.Drone;
                UpdateTextBoxesAndRadioButtons(selectedDrone);
            }
        }
        private void expressServiceListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            regularServiceListView.SelectedItem = null; // Clear selected item
            finishedServiceListBox.SelectedItem = null; // Clear selected item
            if (expressServiceListView.SelectedItem != null) // If item selected, get data and update the text boxes and radio buttons
            {
                var selectedItem = (dynamic)expressServiceListView.SelectedItem;
                Drone selectedDrone = selectedItem.Drone;
                UpdateTextBoxesAndRadioButtons(selectedDrone);
            }
        }
        private bool IsServiceNumberInUse(int serviceTagNumber) // Checks if service number is in use
        {
            return RegularService.Any(d => d.ServiceTagNumber == serviceTagNumber) || // Checks listview data for matching service numbers in the drone data
                   ExpressService.Any(d => d.ServiceTagNumber == serviceTagNumber) ||
                   FinishedList.Any(d => d.ServiceTagNumber == serviceTagNumber);
        }
        private void addNewItemButton_Click(object sender, RoutedEventArgs e) // Add new item button method 
        {
            if (string.IsNullOrWhiteSpace(clientNameTextBox.Text) || // Checks if user filled everything 
                string.IsNullOrWhiteSpace(droneModelTextBox.Text) ||
                string.IsNullOrWhiteSpace(serviceProblemTextBox.Text) ||
                string.IsNullOrWhiteSpace(serviceCostTextBox.Text))
            {
                System.Windows.MessageBox.Show("Please fill in all fields.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // If they did not fill everything
            }
            if (!double.TryParse(serviceCostTextBox.Text, out double serviceCost))
            {
                System.Windows.MessageBox.Show("Service cost must be a number.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // If they made service cost not a number
            }
            if (!int.TryParse(serviceTagTextBox.Text, out int serviceTagNumber))
            {
                System.Windows.MessageBox.Show("Service tag number must be an integer.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // If they made service tag not an int number
            }
            string priority = GetPriority(); // Get service priority 
            if (priority == null) // Checks that radio buttons not null (something is selected) and warns user
            {
                System.Windows.MessageBox.Show("Please select a priority.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (IsServiceNumberInUse(serviceTagNumber)) // Alerts user if service tag is already in use
            {
                System.Windows.MessageBox.Show("Service tag number is already in use.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Drone newDrone = new Drone() // Create new instance of drone class
            {
                ClientName = clientNameTextBox.Text, // Assignments
                DroneModel = droneModelTextBox.Text,
                ServiceProblem = serviceProblemTextBox.Text,
                ServiceCost = serviceCost,
                ServiceTagNumber = serviceTagNumber
            };
            if (priority == "Low") // If service priority low
            {
                statusBarItem.Text = "Added new item to Regular Service";
                RegularService.Enqueue(newDrone); // Enqueue
            }
            else if (priority == "High") // If service priority high
            {
                newDrone.ServiceCost *= 1.15; // Increase price by 15%
                statusBarItem.Text = "Added new item to Express Service";
                ExpressService.Enqueue(newDrone); // Enqueue
            }
            clientNameTextBox.Text = ""; // Clear text boxes
            droneModelTextBox.Text = "";
            serviceProblemTextBox.Text = "";
            serviceCostTextBox.Text = "";
            lowPriorityRadioButton.IsChecked = false; // Uncheck radio buttons
            highPriorityRadioButton.IsChecked = false;
            UpdateListView(); // Call UpdateListView to update list views
        }
        private void serviceTagTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        private void finishedServiceListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        private void UpdateTextBoxesAndRadioButtons(Drone drone) // Update text boxes and radio buttons method
        { 
            clientNameTextBox.Text = drone.ClientName; // This method returns the data from the drone queue into the GUI text boxes and radio buttons-
            droneModelTextBox.Text = drone.DroneModel; // called when an index is selected from the listviews
            serviceProblemTextBox.Text = drone.ServiceProblem;
            serviceCostTextBox.Text = drone.ServiceCost.ToString();
            serviceTagTextBox.Text = drone.ServiceTagNumber.ToString();

            if (RegularService.Contains(drone))
            {
                lowPriorityRadioButton.IsChecked = true; // Checks what service prioity the job is and selects corresponding radio button
            }
            else if (ExpressService.Contains(drone))
            {
                highPriorityRadioButton.IsChecked = true;
            }
        }
        private void declareFinishedButton_Click(object sender, RoutedEventArgs e) // Declare job finished method
        {
            Drone finishedDrone = null; // Initialize a null drone object
            if (regularServiceListView.SelectedItem != null) // If drone selected in regular service listview
            {
                var selectedItem = (dynamic)regularServiceListView.SelectedItem; // Get data of selected drone
                finishedDrone = selectedItem.Drone; // finishedDrone becomes selected item
                RegularService = new Queue<Drone>(RegularService.Where(d => d.ServiceTagNumber != finishedDrone.ServiceTagNumber)); // Remove from queue
            }
            else if (expressServiceListView.SelectedItem != null) // If drone selected in express service listview
            {
                var selectedItem = (dynamic)expressServiceListView.SelectedItem;
                finishedDrone = selectedItem.Drone; // finishedDrone becomes selected item
                ExpressService = new Queue<Drone>(ExpressService.Where(d => d.ServiceTagNumber != finishedDrone.ServiceTagNumber)); // Remove from queue
            }
            if (finishedDrone != null) // If finishedDrone not null
            {
                FinishedList.Add(finishedDrone); // Add to finished list
                statusBarItem.Text = "Added new item to Finished list";
                UpdateListView(); // Call UpdateListView
            }
        }
        private void serviceCostTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        private void serviceProblemTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        private void droneModelTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        private void clientNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        private void clearAllFieldsButton_Click(object sender, RoutedEventArgs e) // Method to clear all entry fields
        {
            clientNameTextBox.Text = "";
            droneModelTextBox.Text = "";
            serviceProblemTextBox.Text = "";
            serviceCostTextBox.Text = "";
            lowPriorityRadioButton.IsChecked = false;
            highPriorityRadioButton.IsChecked = false;
            UpdateListView();
        }
        private void mainGridMouseClick(object sender, MouseButtonEventArgs e) // If click on main grid (to clear selected index)
        {
            regularServiceListView.SelectedItem = null;
            expressServiceListView.SelectedItem = null;
            finishedServiceListBox.SelectedItem = null;
        }
        private void finishedServiceListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e) // When double click the items in finishedServiceListBox
        {
            if (finishedServiceListBox.SelectedItem != null) // If selected item not null
            {
                var selectedItem = (dynamic)finishedServiceListBox.SelectedItem; // Get the selected item
                string clientName = selectedItem.ClientName;
                double serviceCost = selectedItem.ServiceCost;
                Drone selectedDrone = FinishedList.FirstOrDefault(d => d.ClientName == clientName && d.ServiceCost == serviceCost); // Find the drone in the FinishedList with the same clientName and serviceCost
                if (selectedDrone != null)
                {
                    FinishedList.Remove(selectedDrone); // Remove drone from finished list and service queues
                    statusBarItem.Text = "Removed item from Finished list";
                    RegularService = new Queue<Drone>(RegularService.Where(d => d.ServiceTagNumber != selectedDrone.ServiceTagNumber));
                    ExpressService = new Queue<Drone>(ExpressService.Where(d => d.ServiceTagNumber != selectedDrone.ServiceTagNumber));
                    UpdateListView(); // Call UpdateListView
                }
            }
        }
        private void finishedServiceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) // Clear index of the serviceListViews
        {
            regularServiceListView.SelectedItem = null;
            expressServiceListView.SelectedItem = null;
        }
    }
}