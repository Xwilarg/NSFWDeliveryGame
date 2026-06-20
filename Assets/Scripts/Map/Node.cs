using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NsfwDelivery.Map
{
    public class Node : MonoBehaviour
    {
        [SerializeField]
        private Node[] _nextNodes;

        [SerializeField]
        private bool _isObjective;
        public bool IsObjective => _isObjective;

        [SerializeField]
        public bool _isBridgeNode;
        public bool IsBridgeNode => _isBridgeNode;

        public bool HasNode(Node node)
            => _nextNodes.Contains(node);

        public IEnumerable<Node> GetNodes()
            => _nextNodes;

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
                    if (IsBridgeNode || n.IsBridgeNode) Gizmos.color = Color.gray;
                    else Gizmos.color = Color.blue;
                }
                else continue;

                Gizmos.DrawLine(transform.position, n.transform.position);
            }

            if (IsObjective)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, 1f);
            }
        }
    }
}
