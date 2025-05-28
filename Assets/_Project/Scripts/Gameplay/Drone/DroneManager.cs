using System.Collections.Generic;
using _Project.Scripts.Gameplay.Base;
using _Project.Scripts.Gameplay.Drone.Controllers;

namespace _Project.Scripts.Gameplay.Drone
{
    public class DroneManager
    {
        private readonly List<DroneController> _drones = new();

        public void Add(DroneController drone) => _drones.Add(drone);
        
        public void Remove(DroneController drone) => _drones.Remove(drone);

        public void UpdateAll()
        {
            foreach (DroneController drone in _drones)
                drone.UpdateController();
        }

        public int CountForHome(BaseComponent home) => _drones.FindAll(drone => drone.Model.HomeBase == home).Count;
        
        public IEnumerable<DroneController> GetAll() => _drones;
    }
}