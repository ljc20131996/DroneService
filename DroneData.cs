// Lewis Cottrill
// 20/5/2024
// C# AT3

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; // Import stuff

namespace DroneService // Namespace
{
    public class Drone // Drone class
    {
        private string clientName;
        private string droneModel;
        private string serviceProblem;
        private double serviceCost;
        private int serviceTagNumber;
        public int ServiceTagNumber // Getters and setters
        {
            get { return serviceTagNumber; }
            set { serviceTagNumber = value; }
        }
        public string ClientName
        {
            get { return clientName; }
            set { clientName = value; }
        }
        public string DroneModel
        {
            get { return droneModel; }
            set { droneModel = value; }
        }
        public string ServiceProblem
        {
            get { return serviceProblem; }
            set { serviceProblem = value; }
        }
        public double ServiceCost
        {
            get { return serviceCost; }
            set { serviceCost = Math.Round(value, 2); } // Limit decimal places to 2
        }
    }
}
