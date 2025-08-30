using System.Collections.Generic;
using UnityEngine;

namespace GASSample.Gameplay
{
    [System.Serializable]
    public struct LevelData
    {
        public int XpToNextLevel;
        public float HealthGain;
        public float ManaGain;
        public float AttackGain;
        public float DefenseGain;
    }

    [CreateAssetMenu(fileName = "LevelUpData", menuName = "GASSample/Gameplay/GAS/Level Up Data")]
    public class LevelUpDataSO : ScriptableObject
    {
        public List<LevelData> Levels;
    }
}