using System;
using UnityEngine;

namespace _ScriptableObject
{
    [Serializable]
    [CreateAssetMenu(fileName = "Rarity_", menuName = "Scriptable Object/New Rarity")]
    public class Rarity : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private Color textColour;
        [SerializeField] private int affixes;

        public string Name => name;
        public Color TextColour => textColour;
        public int Affixes => affixes;
    }
}
