using UnityEngine;

namespace NsfwDelivery.Manager
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { private set; get; }

        [SerializeField]
        private GameObject[] _bridgeWalls, _bridgeFloors;

        private void Awake()
        {
            Instance = this;

            foreach (var f in _bridgeFloors) f.SetActive(false);
        }

        public void ToggleBridges(bool value)
        {
            foreach (var w in _bridgeWalls) w.SetActive(!value);
            foreach (var f in _bridgeFloors) f.SetActive(value);
        }
    }
}
