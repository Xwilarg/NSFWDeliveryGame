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

        [SerializeField]
        private Node _target;

        [SerializeField]
        private LineRenderer _gps;

        private IEnumerable<Node> _allNodes;

        private List<Node> _currentPath = new List<Node>();

        private void Awake()
        {
            Instance = this;

            _allNodes = Object.FindObjectsByType<Node>(FindObjectsSortMode.None);
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

                var closestPoint = GetClosestNode(car.transform.position);
                var amICloseToTarget = _currentPath[0] == closestPoint;
                var targetPoint = amICloseToTarget ? _currentPath[1] : _currentPath[0];

                var meVector = car.transform.position - targetPoint.transform.position;
                var pathVector = targetPoint.transform.position - closestPoint.transform.position;

                if (Vector3.Dot(meVector, pathVector) < 0f) // We are between the 2 dots
                {
                    if (amICloseToTarget) // We passed previous point
                    {
                        _currentPath.RemoveAt(0);
                        ShowPath(car.transform.position);
                    }
                }
                /*else
                {
                    if (!amICloseToTarget)
                    {
                        _currentPath.Insert(0, closestPoint);
                        ShowGPSPath(car.transform.position);
                    }
                }*/
            }
        }

        private Node GetClosestNode(Vector3 position) => _allNodes.OrderBy(x => Vector2.Distance(position, x.transform.position)).First();

        public void ShowGPSPath(Vector3 position)
        {
            _currentPath = CalculatePath(GetClosestNode(position));
            ShowPath(position);
        }

        private void ShowPath(Vector3 startPos)
        {
            _gps.positionCount = _currentPath.Count + 1;
            List<Vector3> positions = new() { startPos };
            positions.AddRange(_currentPath.Select(x => x.transform.position));
            _gps.SetPositions(positions.ToArray());
        }

        public void UpdatePlayerPath(Vector3 playerPos)
        {
            _gps.SetPosition(0, playerPos);
        }

        private List<Node> CalculatePath(Node start)
        {
            var queue = new Queue<List<Node>>();
            queue.Enqueue(new List<Node> { start });

            while (queue.Count > 0)
            {
                var path = queue.Dequeue();
                var last = path.Last();

                foreach (var node in last.GetNodes().Where(x => !path.Contains(x)))
                {
                    var newPath = new List<Node>(path) { node };

                    if (node == _target) return newPath;

                    queue.Enqueue(newPath);
                }
            }

            return null;
        }
    }
}
