using System.Linq;
using UnityEngine;

namespace NsfwDelivery.Map
{
    public class Node : MonoBehaviour
    {
        [SerializeField]
        private Node[] _nextNodes;

        public bool HasNode(Node node)
            => _nextNodes.Contains(node);

        private void OnDrawGizmos()
        {
            foreach (var n in _nextNodes)
            {
                if (!n.HasNode(this))
                {
                    Gizmos.color = Color.red;
                }
                else if (GetInstanceID() > n.GetInstanceID())
                {
                    Gizmos.color = Color.blue;
                }
                else continue;

                Gizmos.DrawLine(transform.position, n.transform.position);
            }
        }
    }
}
