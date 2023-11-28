using System;
using System.Collections.Generic;
using _EventSystem.CustomEvents;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SceneManagement
{
    public class ShopChoiceUI : MonoBehaviour 
    {
        [FormerlySerializedAs("Btn1")]
        [SerializeField] private Button btn1;
        [FormerlySerializedAs("Btn2")]
        [SerializeField] private Button btn2;
        [FormerlySerializedAs("Shops")]
        [SerializeField] private List<string> shops;
        [FormerlySerializedAs("ShopIcons")]
        [SerializeField] private List<Sprite> shopIcons;
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
            indexBtn1 = Random.Range(0, shops.Count);
            indexBtn2 = Random.Range(0, shops.Count);
            if (indexBtn1 == indexBtn2)
            {
                if (indexBtn1 > 0)
                    indexBtn1 -= Random.Range(1, indexBtn1 + 1);
                else
                    indexBtn2 += Random.Range(1, shops.Count);
            }
        }

        /// <summary>
        /// Update Icon Text and OnClick Method form the Buttons
        /// </summary>
        private void SetButtons()
        {
            BattleFade _fade = FindObjectOfType<BattleFade>();
            btn1.onClick.AddListener(() => _fade.GoToShop(shops[indexBtn1]));
            btn1.onClick.AddListener(() => gameObject.SetActive(false));
            btn2.onClick.AddListener(() => _fade.GoToShop(shops[indexBtn2]));
            btn2.onClick.AddListener(() => gameObject.SetActive(false));
            btn1.transform.Find("text").GetComponent<TextMeshProUGUI>().text = $"Go to {shops[indexBtn1]}";
            btn2.transform.Find("text").GetComponent<TextMeshProUGUI>().text = $"Go to {shops[indexBtn2]}";
            btn1.transform.Find("icon").GetComponent<Image>().sprite = shopIcons[indexBtn1];
            btn2.transform.Find("icon").GetComponent<Image>().sprite = shopIcons[indexBtn2];
        }

        private void OnDisable()
        {
            btn1.onClick.RemoveAllListeners();
            btn2.onClick.RemoveAllListeners();
        }
    }
}