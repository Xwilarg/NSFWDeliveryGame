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
        private TMP_Text _timerText;

        [SerializeField]
        private LevelInfo[] _levels;

        [SerializeField]
        private TextAsset _intro;

        private int _packagesLeft;

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
                    InitDay();
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
            _timerText.gameObject.SetActive(false);
        }

        private void Start()
        {
            VNManager.Instance.ShowStory(new InkStory(_intro));
        }

        private void InitDay()
        {
            VNManager.Instance.ShowStory(new InkStory(CurrentLevel.Story));
            _packagesLeft = CurrentLevel.PackageCount;
            _objectiveText.text = $"Deliver your packages... {_packagesLeft}";
            _timerText.gameObject.SetActive(true);
            ShowTimer();
        }

        public void DeliverPackage()
        {
            _packagesLeft--;
            _objectiveText.text = $"Deliver your packages... {_packagesLeft}";
            if (_packagesLeft == 0)
            {
                Win();
            }
        }

        private void ShowTimer()
        {
            var time = 120 - Mathf.FloorToInt(_missionTimer.TimerClamped01 * 120f);
            _timerText.text = $"{time / 60:00}:{time % 60:00}";
        }

        private void Update()
        {
            if (GameState == GameState.DeliverPackage && !VNManager.Instance.IsStoryOngoing)
            {
                if (_missionTimer.IsActive)
                {
                    _missionTimer.Update(Time.deltaTime);
                    ShowTimer();

                    if (!_missionTimer.IsActive)
                    {
                        Loose();
                    }
                }
            }
        }

        private void Loose()
        {

        }

        private void Win()
        {

        }
    }

    public enum GameState
    {
        GoToGarage,
        DeliverPackage,
        FinishDelivery
    }
}
