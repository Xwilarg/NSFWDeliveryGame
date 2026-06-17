using NsfwDelivery.Map;
using NsfwDelivery.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NsfwDelivery.Manager
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { private set; get; }

        private Node _target;

        [SerializeField]
        private LineRenderer _gps;

        private IEnumerable<Node> _allNodes;

        private List<Node> _currentPath = null;

        private void Awake()
        {
            Instance = this;

            _allNodes = Object.FindObjectsByType<Node>(FindObjectsSortMode.None);
        }
        public void SetTarget(CarController car, Node node)
        {
            _target = node;

            ShowGPSPath(car);
        }

        private void SetRandomTarget(CarController car)
        {
            var possibleTargets = _allNodes.Where(x => x.IsObjective && x != _target).ToArray();
            SetTarget(car, possibleTargets[Random.Range(0, possibleTargets.Length)]);
        }

        public void StartGPS(CarController car)
        {
            StartCoroutine(UpdateGPS(car));
        }

        private IEnumerator UpdateGPS(CarController car)
        {
            while (true)
            {
                yield return new WaitForSeconds(.2f);

                /*var closestPoint = GetClosestNode(car.transform.position);
                var amICloseToTarget = _currentPath[0] == closestPoint;

                if (amICloseToTarget) // We passed a previous point
                {
                    var targetPoint = _currentPath[1];

                    var meVector = car.transform.position - targetPoint.transform.position;
                    var pathVector = targetPoint.transform.position - closestPoint.transform.position;

                    if (Vector3.Dot(meVector, pathVector) < 0f) // We are between the 2 dots
                    {
                        _currentPath.RemoveAt(0);
                        ShowPath(car.transform.position);
                    }
                }
                else
                {
                    /*var targetPoint = _currentPath[0];

                    var meVector = car.transform.position - targetPoint.transform.position;
                    var pathVector = targetPoint.transform.position - closestPoint.transform.position;

                    if (Vector3.Dot(meVector, pathVector) > 0f) // We are NOT between the 2 dots
                    {
                        ShowGPSPath(car.transform.position);
                    }*/
                //}

                if (_currentPath == null)
                { }
                else if (_currentPath.Count == 1)
                {
                    if (Vector2.Distance(car.transform.position, _currentPath.First().transform.position) < 1f)
                    {
                        SetRandomTarget(car);
                        if (ObjectiveManager.Instance.GameState == GameState.GoToGarage)
                        {
                            ObjectiveManager.Instance.GameState = GameState.DeliverPackage;
                        }
                        else
                        {
                            ObjectiveManager.Instance.DeliverPackage(car);
                        }
                    }
                    else
                    {
                        ShowGPSPath(car);
                    }
                }
                else
                {
                    ShowGPSPath(car);
                }
            }
        }

        private Node GetClosestNode(Vector3 position)
            => _allNodes
            .OrderBy(x => Vector2.Distance(position, x.transform.position))
            .FirstOrDefault(x => Physics2D.Linecast(position, x.transform.position, LayerMask.GetMask("Wall")).collider == null);

        public void ShowGPSPath(CarController car)
        {
            var closest = GetClosestNode(car.transform.position);
            if (closest == null) return;

            _currentPath = CalculatePath(closest, car);
            ShowPath(car.transform.position);
        }

        private void ShowPath(Vector3 startPos)
        {
            if (_currentPath == null) return;

            _gps.positionCount = _currentPath.Count + 1;
            List<Vector3> positions = new() { startPos };
            positions.AddRange(_currentPath.Select(x => x.transform.position));
            _gps.SetPositions(positions.ToArray());
        }

        public void UpdatePlayerPath(Vector3 playerPos)
        {
            if (_gps.positionCount > 0) _gps.SetPosition(0, playerPos);
        }

        private List<Node> CalculatePath(Node start, CarController car)
        {
            if (start == _target) return new List<Node>() { start };

            var queue = new Queue<List<Node>>();
            queue.Enqueue(new List<Node> { start });

            while (queue.Count > 0)
            {
                var path = queue.Dequeue();
                var last = path.Last();

                foreach (var node in last.GetNodes().Where(x => !path.Contains(x)))
                {
                    var newPath = new List<Node>(path) { node };

                    if (node == _target)
                    {
                        if (newPath.Count > 1)
                        {
                            var meVector = car.transform.position - newPath[0].transform.position;
                            var pathVector = newPath[1].transform.position - newPath[0].transform.position;
                            var dot = Vector3.Dot(meVector, pathVector);

                            if (dot > 0f && dot < pathVector.sqrMagnitude && Physics2D.Linecast(car.transform.position, newPath[1].transform.position, LayerMask.GetMask("Wall")).collider == null)
                            {
                                newPath.RemoveAt(0);
                                ShowPath(car.transform.position);
                            }
                        }

                        return newPath;
                    }

                    queue.Enqueue(newPath);
                }
            }

            return null;
        }
    }
}
