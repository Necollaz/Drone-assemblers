using _Project.Scripts.Gameplay.Base;
using _Project.Scripts.Gameplay.Drone.Config;
using _Project.Scripts.Gameplay.GameResource;

namespace _Project.Scripts.Gameplay.Drone
{
    public class DroneModel
    {
        public DroneModel(BaseComponent homeBase) => HomeBase = homeBase;

        public DroneState CurrentState { get; private set; } = DroneState.Idle;
        public ResourceModel TargetResource { get; private set; }
        public BaseComponent HomeBase { get; }

        public void SetTarget(ResourceModel resource)
        {
            TargetResource = resource;
            CurrentState = DroneState.ToResource;
        }

        public void SetState(DroneState newState, ResourceModel resource = null)
        {
            CurrentState = newState;
            
            if (newState == DroneState.ToResource && resource != null)
            {
                TargetResource = resource;
            }
            else if (newState == DroneState.Idle)
            {
                TargetResource = null;
            }
        }
    }
}