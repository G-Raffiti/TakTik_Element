using System;
using System.Collections;
using System.Collections.Generic;
using _Instances;
using SceneManagement;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.UI;
using UserInterface;

namespace Shops
{
    public class ShopResurrection : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI pointsTxt;
        private List<Hero> heroes;
        private Hero ActualHero;
        [SerializeField] private GameObject sprite;
        [SerializeField] private List<Image> HeroSelector;

        private void UpdateDisplay()
        {
            pointsTxt.text = $"You have {PlayerData.getInstance().ResurrectionPoints}";
            for (int i = 0; i < HeroSelector.Count; i++)
            {
                HeroSelector[i].GetComponentInChildren<PersonalInventory>().Initialize(heroes[i]);
            }
        }

        private void Start()
        {
            heroes = PlayerData.getInstance().Heroes;
            UpdateDisplay();
            ChangeActualHero(0);
        }

        public void Resurrection()
        {
            if (!ActualHero.isDead) return;
            if (PlayerData.getInstance().ResurrectionPoints < 1) return;
            PlayerData.getInstance().ResurrectionPoints--;
            ActualHero.isDead = false;
            ActualHero.HealHP(50);
            sprite.GetComponent<Animation>().Play("Resurrection");
            UpdateDisplay();
        }

        public void ChangeActualHero(int index)
        {
            ActualHero = heroes[index];
            foreach (Image _image in HeroSelector)
            {
                _image.color = new Color(0.1568628f, 0.1568628f, 0.1568628f);
            }
            HeroSelector[index].color = Color.yellow;
            StartCoroutine(FadeOff());
        }

        private IEnumerator FadeOff()
        {
            if (sprite.GetComponent<SpriteRenderer>().color.a != 0)
                sprite.GetComponent<Animation>().Play("FadeOff");
            while (sprite.GetComponent<Animation>().isPlaying)
            {
                yield return null;}
            sprite.GetComponent<SpriteRenderer>().sprite = ActualHero.UnitSprite;
        }
    }
}