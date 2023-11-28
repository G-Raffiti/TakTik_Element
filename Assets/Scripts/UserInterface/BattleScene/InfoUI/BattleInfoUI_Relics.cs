using _DragAndDropSystem;
using _Instances;
using Relics;
using Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace UserInterface.BattleScene.InfoUI
{
    public class BattleInfoUIRelics : MonoBehaviour
    {
        [FormerlySerializedAs("PortraitPrefab")]
        [SerializeField] private PersonalInventory portraitPrefab;
        [FormerlySerializedAs("HolderPrefab")]
        [SerializeField] private GameObject holderPrefab;
        [FormerlySerializedAs("RelicPrefab")]
        [SerializeField] private RelicInfo relicPrefab;


        private void OnEnable()
        {
            foreach (Hero _hero in PlayerData.GetInstance().Heroes)
            {
                GameObject _holder = Instantiate(holderPrefab, transform);
                GameObject _portrait = Instantiate(portraitPrefab.gameObject, _holder.transform);
                _portrait.GetComponent<PersonalInventory>().Initialize(_hero);
                foreach (RelicSo _relic in _hero.Relics)
                {
                    GameObject _relicObj = Instantiate(relicPrefab.gameObject, _holder.transform);
                    _relicObj.GetComponent<RelicInfo>().CreateRelic(_relic);
                    _relicObj.GetComponent<RelicInfo>().DisplayIcon();
                }
            }
        }

        private void OnDisable()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
    }
}