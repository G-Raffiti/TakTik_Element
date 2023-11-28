using _DragAndDropSystem;
using _Extension;
using _Instances;
using Units;
using UnityEngine;

namespace UserInterface.BattleScene.InfoUI
{
    public class BattleInfoUIInventory : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryPrefab;

        private void OnEnable()
        {
            foreach (Hero _hero in PlayerData.GetInstance().Heroes)
            {
                GameObject _inventoryObj = Instantiate(inventoryPrefab, transform);
                _inventoryObj.GetComponentInChildren<PersonalInventory>().Initialize(_hero);
                _inventoryObj.GetComponentInChildren<PersonalInventory>().FillInventory();
            }

            foreach (SlotDragAndDrop _slot in inventoryPrefab.GetComponentsInChildren<SlotDragAndDrop>())
            {
                _slot.cellType = SlotDragAndDrop.CellType.DropOnly;
            }
        }

        private void OnDisable()
        {
            int _i = 0;
            //Array to hold all child obj
            GameObject[] _allChildren = new GameObject[transform.childCount - 1];

            //Find all child obj and store to that array
            foreach (Transform _child in transform)
            {
                if (_child.GetComponentInChildren<PersonalInventory>() == null) continue;
                _allChildren[_i] = _child.gameObject;
                _i += 1;
            }

            //Now destroy them
            foreach (GameObject _child in _allChildren)
            {
                DestroyImmediate(_child.gameObject);
            }
        }
    }
}