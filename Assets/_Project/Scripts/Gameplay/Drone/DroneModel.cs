using _Project.Scripts.Gameplay.Base;
using _Project.Scripts.Gameplay.Drone.Config;
using _Project.Scripts.Gameplay.GameResource;

namespace _Project.Scripts.Gameplay.Drone
{
    public class DroneModel
    {
        public DroneModel(BaseComponent homeBase) =>  HomeBase = homeBase;
        
        public DroneState CurrentDroneState { get; private set; } = DroneState.Idle;
        public ResourceModel TargetResource { get; private set; }
        public BaseComponent HomeBase { get; }

        public void SetTarget(ResourceModel resource)
        {
            TargetResource = resource;
            CurrentDroneState = DroneState.ToResource;
        }

        public void StartGathering() => CurrentDroneState = DroneState.Gathering;

        public void WaitUnload() => CurrentDroneState = DroneState.WaitingUnload;
        
        public void ReturnHome() => CurrentDroneState = DroneState.ToBase;

        public void StartUnloading() => CurrentDroneState = DroneState.Unloading;

        public void FinishUnloading() => CurrentDroneState = DroneState.Idle;
    }
}