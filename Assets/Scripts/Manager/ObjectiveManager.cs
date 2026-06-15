using NsfwDelivery.Map;
using TMPro;
using UnityEngine;

namespace NsfwDelivery.Manager
{
    public class ObjectiveManager : MonoBehaviour
    {
        public static ObjectiveManager Instance { private set; get; }

        [SerializeField]
        private TMP_Text _objectiveText;

        [SerializeField]
        private Node _officeNode;
        public Node OfficeNode => _officeNode;

        public GameState GameState
        {
            set
            {
                _gameState = value;
                _objectiveText.text = value switch
                {
                    GameState.GoToGarage => "Go to the post office",
                    GameState.DeliverPackage => "Deliver your packages",
                    _ => string.Empty
                };
            }
            get => _gameState;
        }
        private GameState _gameState;

        private void Awake()
        {
            Instance = this;
            GameState = GameState.GoToGarage;
        }
    }

    public enum GameState
    {
        GoToGarage,
        DeliverPackage
    }
}
