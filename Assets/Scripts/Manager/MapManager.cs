using NsfwDelivery.Map;
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

        public bool CalculatePath(List<Node> path)
        {
            var last = path.Last();
            foreach (var node in last.GetNodes().Where(x => !path.Contains(x)))
            {
                var newPath = new List<Node>(path);
                newPath.Add(node);

                if (node == _target) return true;

                if (CalculatePath(newPath)) return true;
            }

            return false;
        }
    }
}
