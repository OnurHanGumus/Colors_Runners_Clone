using System;
using System.Numerics;

namespace Data.ValueObject
{
    [Serializable]
    public class PlayerData
    {
        public PlayerMovementData MovementData;
    }

    [Serializable]
    public class PlayerMovementData
    {
        public float ForwardSpeed = 10;
        public float SidewaysSpeed = 2;
        public float JumpDistance = 10f;
        public float JumpDuration = 1f;
        public float IdleRotateSpeed = 300f;
        public float RotateBorder = 15f;
        public float CrouchSpeed = 8f;
        public float RunSpeed = 10f;
    }
}