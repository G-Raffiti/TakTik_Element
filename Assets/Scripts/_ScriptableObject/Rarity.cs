using System;
using UnityEngine;

namespace _ScriptableObject
{
    [Serializable]
    [CreateAssetMenu(fileName = "Rarity_", menuName = "Scriptable Object/New Rarity")]
    public class Rarity : ScriptableObject
    {
        [SerializeField] private string rarityName;
        [SerializeField] private Color textColour;
        [SerializeField] private int affixes;

        public string Name => $"<color=#{ColorUtility.ToHtmlStringRGB(textColour)}>{rarityName}</color>";
        public Color TextColour => textColour;
        public int Affixes => affixes;
    }
}
