using System.Collections;
using _EventSystem.CustomEvents;
using Gears;
using Relics;
using Skills;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _DragAndDropSystem
{
	/// <summary>
	/// Every item's cell must contain this script
	/// </summary>
	[RequireComponent(typeof(Image))]
	public class SlotDragAndDrop : MonoBehaviour, IDropHandler
	{
		[SerializeField] private VoidEvent onItemMoved;
		[SerializeField] private Image frame;
		[SerializeField] private Image rarity;
		public enum CellType                                                    // Cell types
		{
			Swap,                                                               // Items will be swapped between any cells
			DropOnly,                                                           // Item will be dropped into cell
			DragOnly                                                            // Item will be dragged from this cell
		}

		public enum TriggerType                                                 // Types of drag and drop events
		{
			DropRequest,                                                        // Request for item drop from one cell to another
			DropEventEnd,                                                       // Drop event completed
			ItemAdded,                                                          // Item manualy added into cell
			ItemWillBeDestroyed                                                 // Called just before item will be destroyed
		}

		public enum ContainType
		{
			Skill,
			Gear,
			Relic,
		}

		public class DropEventDescriptor                                        // Info about item's drop event
		{
			public TriggerType triggerType;                                     // Type of drag and drop trigger
			public SlotDragAndDrop SourceSlot;                                  // From this cell item was dragged
			public SlotDragAndDrop DestinationSlot;                             // Into this cell item was dropped
			public ItemDragAndDrop item;                                        // Dropped item
			public bool permission;                                             // Decision need to be made on request
		}

		[Tooltip("Functional type of this cell")]
		public CellType cellType = CellType.Swap;                               // Special type of this cell
		[Tooltip("Sprite color for empty cell")]
		public Color empty = new Color();                                       // Sprite color for empty cell
		[Tooltip("Sprite color for filled cell")]
		public Color full = new Color();                                        // Sprite color for filled cell
		[Tooltip("This cell has unlimited amount of items")]
		public bool unlimitedSource = false;                                    // Item from this cell will be cloned on drag start
		[Tooltip("What the Cell can contain")] 
		public ContainType containType = ContainType.Gear;

		private ItemDragAndDrop currentItem;										// Item of this DaD cell

		void OnEnable()
		{
			ItemDragAndDrop.OnItemDragStartEvent += OnAnyItemDragStart;         // Handle any item drag start
			ItemDragAndDrop.OnItemDragEndEvent += OnAnyItemDragEnd;             // Handle any item drag end
			UpdateMyItem();
			UpdateBackgroundState();
		}

		void OnDisable()
		{
			ItemDragAndDrop.OnItemDragStartEvent -= OnAnyItemDragStart;
			ItemDragAndDrop.OnItemDragEndEvent -= OnAnyItemDragEnd;
			StopAllCoroutines();                                                // Stop all coroutines if there is any
		}

		/// <summary>
		/// On any item drag start need to disable all items raycast for correct drop operation
		/// </summary>
		/// <param name="item"> dragged item </param>
		private void OnAnyItemDragStart(ItemDragAndDrop _item)
		{
			UpdateMyItem();
			if (currentItem != null)
			{
				currentItem.MakeRaycast(false);                                  	// Disable item's raycast for correct drop handling
				if (currentItem == _item)                                         	// If item dragged from this cell
				{
					// Check cell's type
					switch (cellType)
					{
						case CellType.DropOnly:
							ItemDragAndDrop.icon.SetActive(false);              // Item can not be dragged. Hide icon
							break;
					}
				}
			}
		}

		/// <summary>
		/// On any item drag end enable all items raycast
		/// </summary>
		/// <param name="item"> dragged item </param>
		private void OnAnyItemDragEnd(ItemDragAndDrop _item)
		{
			UpdateMyItem();
			if (currentItem != null)
			{
				currentItem.MakeRaycast(true);                                  	// Enable item's raycast
			}
			UpdateBackgroundState();
			onItemMoved?.Raise();
		}

		/// <summary>
		/// Item is dropped in this cell
		/// </summary>
		/// <param name="data"></param>
		public void OnDrop(PointerEventData _data)
		{
			
			if (ItemDragAndDrop.icon != null)
			{
				ItemDragAndDrop _item = ItemDragAndDrop.DraggedItem;
				if (containType != _item.Type) return;
				SlotDragAndDrop _sourceSlot = ItemDragAndDrop.SourceSlot;
				if (ItemDragAndDrop.icon.activeSelf == true)                    // If icon inactive do not need to drop item into cell
				{
					if ((_item != null) && (_sourceSlot != this))
					{
						DropEventDescriptor _desc = new DropEventDescriptor();
						switch (cellType)                                       // Check this cell's type
						{
							case CellType.Swap:                                 // Item in destination cell can be swapped
								UpdateMyItem();
								switch (_sourceSlot.cellType)
								{
									case CellType.Swap:                         // Item in source cell can be swapped
										// Fill event descriptor
										_desc.item = _item;
										_desc.SourceSlot = _sourceSlot;
										_desc.DestinationSlot = this;
										SendRequest(_desc);                      // Send drop request
										StartCoroutine(NotifyOnDragEnd(_desc));  // Send notification after drop will be finished
										if (_desc.permission == true)            // If drop permitted by application
										{
											if (currentItem != null)            // If destination cell has item
											{
												// Fill event descriptor
												DropEventDescriptor _descAutoswap = new DropEventDescriptor();
												_descAutoswap.item = currentItem;
												_descAutoswap.SourceSlot = this;
												_descAutoswap.DestinationSlot = _sourceSlot;
												SendRequest(_descAutoswap);                      // Send drop request
												StartCoroutine(NotifyOnDragEnd(_descAutoswap));  // Send notification after drop will be finished
												if (_descAutoswap.permission == true)            // If drop permitted by application
												{
													SwapItems(_sourceSlot, this);                // Swap items between cells
												}
												else
												{
													PlaceItem(_item, _sourceSlot);            // Delete old item and place dropped item into this cell
												}
											}
											else
											{
												PlaceItem(_item, _sourceSlot);                // Place dropped item into this empty cell
											}
										}
										break;
									default:                                    // Item in source cell can not be swapped
										// Fill event descriptor
										_desc.item = _item;
										_desc.SourceSlot = _sourceSlot;
										_desc.DestinationSlot = this;
										SendRequest(_desc);                      // Send drop request
										StartCoroutine(NotifyOnDragEnd(_desc));  // Send notification after drop will be finished
										if (_desc.permission == true)            // If drop permitted by application
										{
											PlaceItem(_item, _sourceSlot);                    // Place dropped item into this cell
										}
										break;
								}
								break;
							case CellType.DropOnly:                             // Item only can be dropped into destination cell
								// Fill event descriptor
								_desc.item = _item;
								_desc.SourceSlot = _sourceSlot;
								_desc.DestinationSlot = this;
								SendRequest(_desc);                              // Send drop request
								StartCoroutine(NotifyOnDragEnd(_desc));          // Send notification after drop will be finished
								if (_desc.permission == true)                    // If drop permitted by application
								{
									PlaceItem(_item, _sourceSlot);                            // Place dropped item in this cell
								}
								break;
							default:
								break;
						}
					}
				}
				if (_item != null)
				{
					if (_item.GetComponentInParent<SlotDragAndDrop>() == null)   // If item have no cell after drop
					{
						Destroy(_item.gameObject);                               // Destroy it
					}
				}
				UpdateMyItem();
				UpdateBackgroundState();
				_sourceSlot.UpdateMyItem();
				_sourceSlot.UpdateBackgroundState();
			}
		}

		/// <summary>
		/// Put item into this cell.
		/// </summary>
		/// <param name="item">Item.</param>
		private void PlaceItem(ItemDragAndDrop _item, SlotDragAndDrop _sourceSlot)
		{
			if (_item != null)
			{
				DestroyItem();                                            	// Remove current item from this cell
				currentItem = null;
				SlotDragAndDrop _cell = _item.GetComponentInParent<SlotDragAndDrop>();
				if (_cell != null)
				{
					if (_cell.unlimitedSource == true)
					{
						string _itemName = _item.name;
						_item = Instantiate(_item);                               // Clone item from source cell
						_item.name = _itemName;
						CloneItem(_item, _sourceSlot);
					}
				}
				_item.transform.SetParent(transform, false);
				_item.transform.localPosition = Vector3.zero;
				_item.MakeRaycast(true);
				currentItem = _item;
			}
			UpdateBackgroundState();
		}

		/// <summary>
		/// If the source Cell is Unlimited Copy the value in the item
		/// </summary>
		/// <param name="item"></param>
		/// <param name="_sourceSlot"></param>
		private void CloneItem(ItemDragAndDrop _item, SlotDragAndDrop _sourceSlot)
		{
			
			switch (containType)
			{
				case ContainType.Skill:
				{
					_item.GetComponent<SkillInfo>().skill = _sourceSlot.GetInfoSkill().skill;
					_item.GetComponent<SkillInfo>().unit = _sourceSlot.GetInfoSkill().unit;
				}
					break;
				case ContainType.Gear:
				{
					_item.GetComponent<GearInfo>().Gear = _sourceSlot.GetInfoGear().Gear;
				}
					break;
				case ContainType.Relic:
				{
					_item.GetComponent<RelicInfo>().CreateRelic(_sourceSlot.GetInfoRelic().Relic);
				}
					break;
			}
		}
		/// <summary>
		/// Destroy item in this cell
		/// </summary>
		private void DestroyItem()
		{
			UpdateMyItem();
			if (currentItem != null)
			{
				DropEventDescriptor _desc = new DropEventDescriptor();
				// Fill event descriptor
				_desc.triggerType = TriggerType.ItemWillBeDestroyed;
				_desc.item = currentItem;
				_desc.SourceSlot = this;
				_desc.DestinationSlot = this;
				SendNotification(_desc);                                         // Notify application about item destruction
				if (currentItem != null)
				{
					Destroy(currentItem.gameObject);
				}
			}
			currentItem = null;
			UpdateBackgroundState();
		}

		/// <summary>
		/// Send drag and drop information to application
		/// </summary>
		/// <param name="desc"> drag and drop event descriptor </param>
		private void SendNotification(DropEventDescriptor _desc)
		{
			if (_desc != null)
			{
				// Send message with DragAndDrop info to parents GameObjects
				gameObject.SendMessageUpwards("OnSimpleDragAndDropEvent", _desc, SendMessageOptions.DontRequireReceiver);
			}
		}

		/// <summary>
		/// Send drag and drop request to application
		/// </summary>
		/// <param name="desc"> drag and drop event descriptor </param>
		/// <returns> result from desc.permission </returns>
		private bool SendRequest(DropEventDescriptor _desc)
		{
			bool _result = false;
			if (_desc != null)
			{
				_desc.triggerType = TriggerType.DropRequest;
				_desc.permission = true;
				SendNotification(_desc);
				_result = _desc.permission;
			}
			return _result;
		}

		/// <summary>
		/// Wait for event end and send notification to application
		/// </summary>
		/// <param name="desc"> drag and drop event descriptor </param>
		/// <returns></returns>
		private IEnumerator NotifyOnDragEnd(DropEventDescriptor _desc)
		{
			// Wait end of drag operation
			while (ItemDragAndDrop.DraggedItem != null)
			{
				yield return new WaitForEndOfFrame();
			}
			_desc.triggerType = TriggerType.DropEventEnd;
			SendNotification(_desc);
		}

		/// <summary>
		/// Change cell's sprite color on item put/remove.
		/// </summary>
		/// <param name="condition"> true - filled, false - empty </param>
		public void UpdateBackgroundState()
		{
			if (frame != null)
			{
				frame.color = currentItem != null ? full : empty;
			}

			if (rarity != null)
			{
				rarity.color = Color.clear;
				if (containType == ContainType.Gear)
				{
					rarity.color = currentItem != null ? currentItem.GetComponent<GearInfo>().Gear.GearSo.Rarity.TextColour : Color.black;
				}
			}
		}

		/// <summary>
		/// Updates my item
		/// </summary>
		public void UpdateMyItem()
		{
			currentItem = GetComponentInChildren<ItemDragAndDrop>();
		}

		/// <summary>
		/// Get item from this cell
		/// </summary>
		/// <returns> Item </returns>
		public ItemDragAndDrop GetItem()
		{
			return currentItem;
		}
		
		/// <summary>
		/// Get InfoGear from Cell
		/// </summary>
		public GearInfo GetInfoGear()
		{
			return GetComponentInChildren<GearInfo>();
		}
		
		/// <summary>
		/// Get Skill from Cell
		/// </summary>
		public SkillInfo GetInfoSkill()
		{
			return GetComponentInChildren<SkillInfo>();
		}

		/// <summary>
		/// Get InfoRelic from Cell
		/// </summary>
		public RelicInfo GetInfoRelic()
		{
			return GetComponentInChildren<RelicInfo>();
		}
		
		/// <summary>
		/// Manualy add item into this cell
		/// </summary>
		/// <param name="_newItem"> New item </param>
		public void AddItem(ItemDragAndDrop _newItem)
		{
			if (_newItem != null)
			{
				PlaceItem(_newItem, this);
				DropEventDescriptor _desc = new DropEventDescriptor();
				// Fill event descriptor
				_desc.triggerType = TriggerType.ItemAdded;
				_desc.item = _newItem;
				_desc.SourceSlot = this;
				_desc.DestinationSlot = this;
				SendNotification(_desc);
			}
		}

		/// <summary>
		/// Manualy delete item from this cell
		/// </summary>
		public void RemoveItem()
		{
			DestroyItem();
		}

		/// <summary>
		/// Swap items between two cells
		/// </summary>
		/// <param name="_firstSlot"> Cell </param>
		/// <param name="_secondSlot"> Cell </param>
		public void SwapItems(SlotDragAndDrop _firstSlot, SlotDragAndDrop _secondSlot)
		{
			if ((_firstSlot != null) && (_secondSlot != null))
			{
				ItemDragAndDrop _firstItem = _firstSlot.GetItem();                // Get item from first cell
				ItemDragAndDrop _secondItem = _secondSlot.GetItem();              // Get item from second cell
				// Swap items
				if (_firstItem != null)
				{
					_firstItem.transform.SetParent(_secondSlot.transform, false);
					_firstItem.transform.localPosition = Vector3.zero;
					_firstItem.MakeRaycast(true);
				}
				if (_secondItem != null)
				{
					_secondItem.transform.SetParent(_firstSlot.transform, false);
					_secondItem.transform.localPosition = Vector3.zero;
					_secondItem.MakeRaycast(true);
				}
				// Update states
				_firstSlot.UpdateMyItem();
				_secondSlot.UpdateMyItem();
				_firstSlot.UpdateBackgroundState();
				_secondSlot.UpdateBackgroundState();
			}
		}
	}
}
