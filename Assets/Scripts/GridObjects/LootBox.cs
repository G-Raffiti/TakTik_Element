using System.Collections;
using System.Collections.Generic;
using _EventSystem.CustomEvents;
using _Extension;
using _Instances;
using Cells;
using Gears;
using Units;
using UnityEngine;

namespace GridObjects
{
    [CreateAssetMenu(fileName = "GridObject_LootBox", menuName = "Scriptable Object/Grid Objects/LootBox")]
    public class LootBox : GridObjectSO
    {
        [SerializeField] private GridObjectEvent OnLootBoxOpen;
        [SerializeField] private List<Sprite> lootBoxesSprites;
        public override Sprite Image => lootBoxesSprites.GetRandom();
        public override void Interact(Unit actor, Cell location)
        {
            if (location.CurrentGridObject.Inventory.gears.Count == 0)
            {
                location.CurrentGridObject.Inventory = new Inventory();
                Gear _gear = new Gear();
                _gear.CreateGear();
                location.CurrentGridObject.Inventory.gears.Add(_gear);
                
                if (location.isCorrupted)
                {
                    Gear gear2 = new Gear();
                    gear2.CreateGear();
                    location.CurrentGridObject.Inventory.gears.Add(gear2);
                }
            }
            
            OnLootBoxOpen.Raise(location.CurrentGridObject);
            
            Utility.RunCoroutine(DestroyAfterUse(location.CurrentGridObject));
        }

        public IEnumerator DestroyAfterUse(GridObject lootBox)
        {
            yield return new WaitForSeconds(0.1f);
            GameObject inventory = GameObject.Find("InventoryUI");
            while (inventory.activeSelf)
                yield return null;
            if (lootBox.Inventory.gears.Count == 0)
            {
                Utility.RunCoroutine(lootBox.OnDestroyed());
            }
        }
    }
    
}