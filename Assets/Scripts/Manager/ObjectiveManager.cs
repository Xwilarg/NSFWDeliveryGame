using NsfwDelivery.Map;
using NsfwDelivery.Player;
using NsfwDelivery.SO;
using Sketch.Common;
using Sketch.VN;
using Sketch.VN.InkleInk;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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

        [SerializeField]
        private RectTransform _boostJauge;

        private int _packagesLeft;

        private Timer _missionTimer = new();
        private Timer _recoverBoostTimer = new();
        private int _index;

        public LevelInfo CurrentLevel => _levels[_index];

        public int _goodDeliveries = 0;

        private float _boost = 1f;
        private bool _isPressingBoostKey;
        private bool _isRecovering;
        public bool IsUsingBoost { set; get; }

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
            _boostJauge.gameObject.SetActive(false);

            _missionTimer.OnDone.AddListener(() => { Loose(GameObject.FindFirstObjectByType<CarController>()); });

            _recoverBoostTimer.OnDone.AddListener(() =>
            {
                _isRecovering = true;
            });
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
            _boostJauge.gameObject.SetActive(CurrentLevel.CanUseBoost);
        }

        public void DeliverPackage(CarController car)
        {
            _packagesLeft--;
            _objectiveText.text = $"Deliver your packages... {_packagesLeft}";
            if (_packagesLeft == 0)
            {
                Win(car);
            }
        }

        private void ShowTimer()
        {
            var time = 120 - Mathf.FloorToInt(_missionTimer.TimerClamped01 * 120f);
            _timerText.text = $"{time / 60:00}:{time % 60:00}";
        }

        private void Update()
        {
            if (!VNManager.Instance.IsStoryOngoing)
            {
                if (GameState == GameState.DeliverPackage)
                {
                    if (_missionTimer.IsActive)
                    {
                        _missionTimer.Update(Time.deltaTime);
                        ShowTimer();
                    }
                }

                IsUsingBoost = _isPressingBoostKey && _boost > 0f;
                if (IsUsingBoost && _boost > 0f)
                {
                    _boost = Mathf.Clamp01(_boost - Time.deltaTime);
                    _isRecovering = false;
                }
                else if (_isRecovering)
                {
                    _boost = Mathf.Clamp01(_boost + Time.deltaTime / 3f);
                }
                else if (_recoverBoostTimer.IsActive)
                {
                    _recoverBoostTimer.Update(Time.deltaTime);
                }
                _boostJauge.localScale = new Vector3(1f, _boost, 1f);
            }
        }

        private void Loose(CarController car)
        {
            _timerText.gameObject.SetActive(false);

            GoToGarage(car);
        }

        private void Win(CarController car)
        {
            _timerText.gameObject.SetActive(false);
            _goodDeliveries++;

            GoToGarage(car);
        }

        private void GoToGarage(CarController car)
        {
            if (_index < _levels.Length - 1)
            {
                _index++;
                GameState = GameState.GoToGarage;
                MapManager.Instance.SetTarget(car, OfficeNode);
            }
        }

        public void OnUseBoost(InputAction.CallbackContext value)
        {
            if (value.phase == InputActionPhase.Started && CurrentLevel.CanUseBoost)
            {
                _isPressingBoostKey = true;
            }
            else if (value.phase == InputActionPhase.Canceled)
            {
                _isPressingBoostKey = false;
                _recoverBoostTimer.Start(2f);
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
