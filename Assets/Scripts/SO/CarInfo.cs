using UnityEngine;

namespace NsfwDelivery.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/CarInfo", fileName = "CarInfo")]
    public class CarInfo : ScriptableObject
    {
        public float Speed;
        public float Acceleration;
        public float Torque;
        public AnimationCurve TorqueCurve;
    }
}