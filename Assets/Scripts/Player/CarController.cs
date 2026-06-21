using NsfwDelivery.Manager;
using NsfwDelivery.SO;
using Sketch.VN;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NsfwDelivery.Player
{
    public class CarController : MonoBehaviour
    {
        [SerializeField]
        private CarInfo _info;
        public CarInfo Info => _info;

        [SerializeField]
        private Transform _minimapCamera;

        [SerializeField]
        private Animator _knock;

        private Rigidbody2D _rb;

        private int _grassCount;

        private float _forward;
        private int _horizontal;

        private float _forwardSpeed;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            MapManager.Instance.StartGPS(this);
            MapManager.Instance.SetTarget(this, ObjectiveManager.Instance.OfficeNode);
        }

        private void Update()
        {
            var acceleration = Time.deltaTime * _info.Acceleration;
            if (_forward > 0f) _forwardSpeed = Mathf.Clamp(_forwardSpeed + acceleration, _forwardSpeed, _info.Speed);
            else if (_forward < 0f) _forwardSpeed = Mathf.Clamp(_forwardSpeed - acceleration, -_info.Speed, _forwardSpeed);
            else
            {
                if (_forwardSpeed > 0f) _forwardSpeed = Mathf.Clamp(_forwardSpeed - Time.deltaTime, 0f, _forwardSpeed);
                else if (_forwardSpeed < 0f) _forwardSpeed = Mathf.Clamp(_forwardSpeed + Time.deltaTime, _forwardSpeed, 0f);
            }
        }

        private void FixedUpdate()
        {
            if (VNManager.Instance.IsStoryOngoing)
            {
                _rb.linearVelocity = Vector2.zero;
                return;
            }

            var vel = transform.up * _forwardSpeed * (ObjectiveManager.Instance.IsUsingBoost ? 2f : 1f);
            vel *= _grassCount > 0 ? _info.GrassSpeedMultiplier : 1f;
            _rb.linearVelocity = vel;
            var torqueForce = -_horizontal * _forwardSpeed * Time.fixedDeltaTime * _info.Torque;
            torqueForce *= _info.TorqueCurve.Evaluate(Mathf.Abs(_forwardSpeed) / _info.Speed);
            transform.Rotate(0f, 0f, torqueForce);

            if (vel.magnitude > 0f)
            {
                MapManager.Instance.UpdatePlayerPath(transform.position);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Grass")) _grassCount++;
            if (collision.CompareTag("Camera"))
            {
                _minimapCamera.position = collision.GetComponent<CameraMarker>().CameraPos;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Grass")) _grassCount--;
        }

        public void DeliverPackage()
        {
            _knock.gameObject.SetActive(true);
            _knock.SetTrigger("Knock");
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            var val = value.ReadValue<Vector2>();
            _forward = val.y;
            if (val.x < 0f) _horizontal = -1;
            else if (val.x > 0f) _horizontal = 1;
            else _horizontal = 0;
        }
    }
}
