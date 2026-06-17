using NsfwDelivery.Map;
using NsfwDelivery.SO;
using Sketch.Common;
using Sketch.VN;
using Sketch.VN.InkleInk;
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

        [SerializeField]
        private LevelInfo[] _levels;

        private Timer _missionTimer = new();
        private int _index;

        public LevelInfo CurrentLevel => _levels[_index];

        public GameState GameState
        {
            set
            {
                if (value == GameState.DeliverPackage && _gameState == GameState.GoToGarage)
                {
                    _missionTimer.Start(120f);
                }
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

        private void Start()
        {
            VNManager.Instance.ShowStory(new InkStory(CurrentLevel.Story));
        }

        private void Update()
        {
            if (GameState == GameState.DeliverPackage)
            {
                if (_missionTimer.IsActive)
                {
                    _missionTimer.Update(Time.deltaTime);
                    _objectiveText.text = $"Deliver your packages... {120 - Mathf.FloorToInt(_missionTimer.TimerClamped01 * 120f)}";
                }
                else
                {
                    _objectiveText.text = $"Deliver your packages";
                }
            }
        }
    }

    public enum GameState
    {
        GoToGarage,
        DeliverPackage,
        FinishDelivery
    }
}
