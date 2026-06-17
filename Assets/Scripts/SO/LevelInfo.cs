using UnityEngine;

namespace NsfwDelivery.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/LevelInfo", fileName = "LevelInfo")]
    public class LevelInfo : ScriptableObject
    {
        public TextAsset Story;
        public int PackageCount;
    }
}