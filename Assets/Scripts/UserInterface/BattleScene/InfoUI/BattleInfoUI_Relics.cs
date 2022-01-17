using _DragAndDropSystem;
using _Instances;
using Relics;
using Units;
using UnityEngine;

namespace UserInterface.BattleScene.InfoUI
{
    public class BattleInfoUI_Relics : MonoBehaviour
    {
        [SerializeField] private PersonalInventory PortraitPrefab;
        [SerializeField] private DragAndDropCell SlotPrefab;
        [SerializeField] private GameObject HolderPrefab;
        [SerializeField] private RelicInfo RelicPrefab;


        private void OnEnable()
        {
            foreach (Hero hero in PlayerData.getInstance().Heroes)
            {
                GameObject holder = Instantiate(HolderPrefab, transform);
                GameObject portrait = Instantiate(PortraitPrefab.gameObject, holder.transform);
                portrait.GetComponent<PersonalInventory>().Initialize(hero);
                foreach (RelicSO _relic in hero.Relics)
                {
                    GameObject slotObj = Instantiate(SlotPrefab.gameObject, holder.transform);
                    slotObj.GetComponent<DragAndDropCell>().cellType = DragAndDropCell.CellType.DropOnly;
                    slotObj.GetComponent<DragAndDropCell>().containType = DragAndDropCell.ContainType.Relic;
                    GameObject relicObj = Instantiate(RelicPrefab.gameObject, slotObj.transform);
                    relicObj.GetComponent<RelicInfo>().CreateRelic(_relic);
                    relicObj.GetComponent<RelicInfo>().DisplayIcon();
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