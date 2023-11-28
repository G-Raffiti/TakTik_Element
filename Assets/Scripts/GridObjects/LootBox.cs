using System.Collections;
using System.Collections.Generic;
using _EventSystem.CustomEvents;
using _Extension;
using _Instances;
using Cells;
using Gears;
using Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace GridObjects
{
    [CreateAssetMenu(fileName = "GridObject_LootBox", menuName = "Scriptable Object/Grid Objects/LootBox")]
    public class LootBox : GridObjectSo
    {
        [FormerlySerializedAs("OnLootBoxOpen")]
        [SerializeField] private GridObjectEvent onLootBoxOpen;
        [SerializeField] private List<Sprite> lootBoxesSprites;
        public override Sprite Image => lootBoxesSprites.GetRandom();
        public override void Interact(Unit _actor, Cell _location)
        {
            if (_location.CurrentGridObject.inventory.gears.Count == 0)
            {
                _location.CurrentGridObject.inventory = new Inventory();
                Gear _gear = new Gear();
                _gear.CreateGear();
                _location.CurrentGridObject.inventory.gears.Add(_gear);
                
                if (_location.IsCorrupted)
                {
                    Gear _gear2 = new Gear();
                    _gear2.CreateGear();
                    _location.CurrentGridObject.inventory.gears.Add(_gear2);
                }
            }
            
            onLootBoxOpen.Raise(_location.CurrentGridObject);
            
            Utility.RunCoroutine(DestroyAfterUse(_location.CurrentGridObject));
        }

        public IEnumerator DestroyAfterUse(GridObject _lootBox)
        {
            yield return new WaitForSeconds(0.1f);
            GameObject _inventory = GameObject.Find("InventoryUI");
            while (_inventory.activeSelf)
                yield return null;
            if (_lootBox.inventory.gears.Count == 0)
            {
                Utility.RunCoroutine(_lootBox.OnDestroyed());
            }
        }
    }
    
}