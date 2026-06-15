using NsfwDelivery.Manager;
using NsfwDelivery.SO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NsfwDelivery.Player
{
    public class CarController : MonoBehaviour
    {
        [SerializeField]
        private CarInfo _info;

        private Rigidbody2D _rb;

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
            var vel = transform.up * _forwardSpeed;
            _rb.linearVelocity = vel;
            transform.Rotate(0f, 0f, -_horizontal * _forwardSpeed * Time.fixedDeltaTime * _info.Torque);

            if (vel.magnitude > 0f)
            {
                MapManager.Instance.UpdatePlayerPath(transform.position);
            }
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
