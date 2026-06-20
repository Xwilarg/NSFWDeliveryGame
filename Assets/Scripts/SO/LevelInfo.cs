using UnityEngine;

namespace NsfwDelivery.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/LevelInfo", fileName = "LevelInfo")]
    public class LevelInfo : ScriptableObject
    {
        public TextAsset StoryIntro, StoryOutro;
        public int PackageCount;

        public bool CanUseBoost, CanUseBridges;
    }
}