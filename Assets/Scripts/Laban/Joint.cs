using MyBox;
using UnityEngine;

namespace EI_MiniProject
{
    [System.Serializable]
    public class Joint
    {
        [field: SerializeField, ReadOnly]
        public Vector3 Position { get; private set; }

        [field: SerializeField, ReadOnly]
        public Vector3 Velocity { get; private set; }

        [field: SerializeField, ReadOnly]
        public Vector3 Acceleration { get; private set; }

        [field: SerializeField, ReadOnly]
        public Vector3 Jerk { get; private set; }

        [field: Header("Laban Efforts"), SerializeField, ReadOnly]
        public float WeightEffort { get; set; }

        [field: SerializeField, ReadOnly]
        public float TimeEffort { get; set; }

        [field: SerializeField, ReadOnly]
        public float FlowEffort { get; set; }

        private Vector3 previousPosition;
        private Vector3 previousVelocity;
        private Vector3 prevAcceleration;

        public void CalculatePositionDerivatives(Vector3 currentPosition)
        {
            Position = currentPosition;
            Velocity = (Position - previousPosition) / Time.fixedDeltaTime;
            Acceleration = (Velocity - previousVelocity) / Time.fixedDeltaTime;
            Jerk = (Acceleration - prevAcceleration) / Time.fixedDeltaTime;

            previousPosition = Position;
            previousVelocity = Velocity;
            prevAcceleration = Acceleration;
        }

        //public void CalculatePositionDerivatives(Vector3 currentPosition)
        //{
        //    Position = currentPosition;

        //    Velocity = (Position - previousPosition) / (2 * Time.fixedDeltaTime);
        //    Acceleration = (Position + Velocity - (2 * Position) + previousPosition) / Mathf.Pow(Time.fixedDeltaTime, 2);
        //    Jerk = (Position + 2 * Velocity) - 2 * (Position + Velocity) + 2 * (previousPosition) - (previousPosition - previousVelocity) / (2 * Mathf.Pow(Time.fixedDeltaTime, 3));

        //    previousPosition = Position;
        //    previousVelocity = Velocity;
        //}
    }
}

// Simplified calculations:
//Velocity = (Position - previousPosition) / 2 * Time.fixedDeltaTime;
//Acceleration = (Velocity - previousVelocity) / Time.fixedDeltaTime;
//Jerk = (Acceleration - prevAcceleration) / Time.fixedDeltaTime;
//prevAcceleration = Acceleration;
