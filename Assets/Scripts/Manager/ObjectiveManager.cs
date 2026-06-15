using NsfwDelivery.Map;
using UnityEngine;

namespace NsfwDelivery.Manager
{
    public class ObjectiveManager : MonoBehaviour
    {
        public static ObjectiveManager Instance { private set; get; }

        [SerializeField]
        private Node _officeNode;
        public Node OfficeNode => _officeNode;

        private GameState _gameState;

        private void Awake()
        {
            Instance = this;
        }
    }

    public enum GameState
    {
        GoToGarage
    }
}
