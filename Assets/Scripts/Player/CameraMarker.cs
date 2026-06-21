using UnityEngine;

namespace NsfwDelivery.Player
{
    public class CameraMarker : MonoBehaviour
    {
        [SerializeField]
        private Transform _cameraSpot;
        public Vector3 CameraPos => _cameraSpot.position;
    }
}
