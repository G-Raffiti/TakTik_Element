using System;
using System.Collections;
using System.Collections.Generic;
using _Instances;
using SceneManagement;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UserInterface;

namespace Shops
{
    public class ShopResurrection : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI pointsTxt;
        private List<Hero> heroes;
        private Hero actualHero;
        [SerializeField] private GameObject sprite;
        [FormerlySerializedAs("HeroSelector")]
        [SerializeField] private List<Image> heroSelector;

        private void UpdateDisplay()
        {
            pointsTxt.text = $"You have {PlayerData.GetInstance().ResurrectionPoints}";
            for (int _i = 0; _i < heroSelector.Count; _i++)
            {
                heroSelector[_i].GetComponentInChildren<PersonalInventory>().Initialize(heroes[_i]);
            }
        }

        private void Start()
        {
            heroes = PlayerData.GetInstance().Heroes;
            UpdateDisplay();
            ChangeActualHero(0);
        }

        public void Resurrection()
        {
            if (!actualHero.isDead) return;
            if (PlayerData.GetInstance().ResurrectionPoints < 1) return;
            PlayerData.GetInstance().ResurrectionPoints--;
            actualHero.isDead = false;
            actualHero.HealHp(50);
            sprite.GetComponent<Animation>().Play("Resurrection");
            UpdateDisplay();
        }

        public void ChangeActualHero(int _index)
        {
            actualHero = heroes[_index];
            foreach (Image _image in heroSelector)
            {
                _image.color = new Color(0.1568628f, 0.1568628f, 0.1568628f);
            }
            heroSelector[_index].color = Color.yellow;
            StartCoroutine(FadeOff());
        }

        private IEnumerator FadeOff()
        {
            if (sprite.GetComponent<SpriteRenderer>().color.a != 0)
                sprite.GetComponent<Animation>().Play("FadeOff");
            while (sprite.GetComponent<Animation>().isPlaying)
            {
                yield return null;}
            sprite.GetComponent<SpriteRenderer>().sprite = actualHero.UnitSprite;
        }
    }
}