using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SceneManagement
{
    public class ShopChoice_UI : MonoBehaviour 
    {
        [SerializeField] private Button Btn1;
        [SerializeField] private Button Btn2;
        [SerializeField] private List<string> Shops;
        [SerializeField] private List<Sprite> ShopIcons;
        [SerializeField] private int indexBtn1;
        [SerializeField] private int indexBtn2;

        private void OnEnable()
        {
            RandomizeShops();
            SetButtons();
        }

        /// <summary>
        /// Set the 2 index to different Random numbers between 0 and the total of Shops
        /// </summary>
        private void RandomizeShops()
        {
            indexBtn1 = Random.Range(0, Shops.Count);
            indexBtn2 = Random.Range(0, Shops.Count);
            if (indexBtn1 == indexBtn2)
            {
                if (indexBtn1 > 0)
                    indexBtn1 -= Random.Range(1, indexBtn1 + 1);
                else
                    indexBtn2 += Random.Range(1, Shops.Count);
            }
        }

        /// <summary>
        /// Update Icon Text and OnClick Method form the Buttons
        /// </summary>
        private void SetButtons()
        {
            BattleFade fade = FindObjectOfType<BattleFade>();
            Btn1.onClick.AddListener(() => fade.GoToShop(Shops[indexBtn1]));
            Btn1.onClick.AddListener(() => gameObject.SetActive(false));
            Btn2.onClick.AddListener(() => fade.GoToShop(Shops[indexBtn2]));
            Btn2.onClick.AddListener(() => gameObject.SetActive(false));
            Btn1.transform.Find("text").GetComponent<TextMeshProUGUI>().text = $"Go to {Shops[indexBtn1]}";
            Btn2.transform.Find("text").GetComponent<TextMeshProUGUI>().text = $"Go to {Shops[indexBtn2]}";
            Btn1.transform.Find("icon").GetComponent<Image>().sprite = ShopIcons[indexBtn1];
            Btn2.transform.Find("icon").GetComponent<Image>().sprite = ShopIcons[indexBtn2];
        }

        private void OnDisable()
        {
            Btn1.onClick.RemoveAllListeners();
            Btn2.onClick.RemoveAllListeners();
        }
    }
}