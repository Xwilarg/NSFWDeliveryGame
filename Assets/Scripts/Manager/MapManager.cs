using NsfwDelivery.Map;
using UnityEngine;

namespace NsfwDelivery.Manager
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { private set; get; }

        [SerializeField]
        private Node _target;

        public void CalculatePath()
        {

        }
    }
}
