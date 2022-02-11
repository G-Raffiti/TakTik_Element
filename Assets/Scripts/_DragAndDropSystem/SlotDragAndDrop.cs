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
			public SlotDragAndDrop _sourceSlot;                                  // From this cell item was dragged
			public SlotDragAndDrop _destinationSlot;                             // Into this cell item was dropped
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

		private ItemDragAndDrop _currentItem;										// Item of this DaD cell

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
		private void OnAnyItemDragStart(ItemDragAndDrop item)
		{
			UpdateMyItem();
			if (_currentItem != null)
			{
				_currentItem.MakeRaycast(false);                                  	// Disable item's raycast for correct drop handling
				if (_currentItem == item)                                         	// If item dragged from this cell
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
		private void OnAnyItemDragEnd(ItemDragAndDrop item)
		{
			UpdateMyItem();
			if (_currentItem != null)
			{
				_currentItem.MakeRaycast(true);                                  	// Enable item's raycast
			}
			UpdateBackgroundState();
			onItemMoved?.Raise();
		}

		/// <summary>
		/// Item is dropped in this cell
		/// </summary>
		/// <param name="data"></param>
		public void OnDrop(PointerEventData data)
		{
			
			if (ItemDragAndDrop.icon != null)
			{
				ItemDragAndDrop item = ItemDragAndDrop._draggedItem;
				if (containType != item.Type) return;
				SlotDragAndDrop _sourceSlot = ItemDragAndDrop._sourceSlot;
				if (ItemDragAndDrop.icon.activeSelf == true)                    // If icon inactive do not need to drop item into cell
				{
					if ((item != null) && (_sourceSlot != this))
					{
						DropEventDescriptor desc = new DropEventDescriptor();
						switch (cellType)                                       // Check this cell's type
						{
							case CellType.Swap:                                 // Item in destination cell can be swapped
								UpdateMyItem();
								switch (_sourceSlot.cellType)
								{
									case CellType.Swap:                         // Item in source cell can be swapped
										// Fill event descriptor
										desc.item = item;
										desc._sourceSlot = _sourceSlot;
										desc._destinationSlot = this;
										SendRequest(desc);                      // Send drop request
										StartCoroutine(NotifyOnDragEnd(desc));  // Send notification after drop will be finished
										if (desc.permission == true)            // If drop permitted by application
										{
											if (_currentItem != null)            // If destination cell has item
											{
												// Fill event descriptor
												DropEventDescriptor descAutoswap = new DropEventDescriptor();
												descAutoswap.item = _currentItem;
												descAutoswap._sourceSlot = this;
												descAutoswap._destinationSlot = _sourceSlot;
												SendRequest(descAutoswap);                      // Send drop request
												StartCoroutine(NotifyOnDragEnd(descAutoswap));  // Send notification after drop will be finished
												if (descAutoswap.permission == true)            // If drop permitted by application
												{
													SwapItems(_sourceSlot, this);                // Swap items between cells
												}
												else
												{
													PlaceItem(item, _sourceSlot);            // Delete old item and place dropped item into this cell
												}
											}
											else
											{
												PlaceItem(item, _sourceSlot);                // Place dropped item into this empty cell
											}
										}
										break;
									default:                                    // Item in source cell can not be swapped
										// Fill event descriptor
										desc.item = item;
										desc._sourceSlot = _sourceSlot;
										desc._destinationSlot = this;
										SendRequest(desc);                      // Send drop request
										StartCoroutine(NotifyOnDragEnd(desc));  // Send notification after drop will be finished
										if (desc.permission == true)            // If drop permitted by application
										{
											PlaceItem(item, _sourceSlot);                    // Place dropped item into this cell
										}
										break;
								}
								break;
							case CellType.DropOnly:                             // Item only can be dropped into destination cell
								// Fill event descriptor
								desc.item = item;
								desc._sourceSlot = _sourceSlot;
								desc._destinationSlot = this;
								SendRequest(desc);                              // Send drop request
								StartCoroutine(NotifyOnDragEnd(desc));          // Send notification after drop will be finished
								if (desc.permission == true)                    // If drop permitted by application
								{
									PlaceItem(item, _sourceSlot);                            // Place dropped item in this cell
								}
								break;
							default:
								break;
						}
					}
				}
				if (item != null)
				{
					if (item.GetComponentInParent<SlotDragAndDrop>() == null)   // If item have no cell after drop
					{
						Destroy(item.gameObject);                               // Destroy it
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
		private void PlaceItem(ItemDragAndDrop item, SlotDragAndDrop _sourceSlot)
		{
			if (item != null)
			{
				DestroyItem();                                            	// Remove current item from this cell
				_currentItem = null;
				SlotDragAndDrop cell = item.GetComponentInParent<SlotDragAndDrop>();
				if (cell != null)
				{
					if (cell.unlimitedSource == true)
					{
						string itemName = item.name;
						item = Instantiate(item);                               // Clone item from source cell
						item.name = itemName;
						CloneItem(item, _sourceSlot);
					}
				}
				item.transform.SetParent(transform, false);
				item.transform.localPosition = Vector3.zero;
				item.MakeRaycast(true);
				_currentItem = item;
			}
			UpdateBackgroundState();
		}

		/// <summary>
		/// If the source Cell is Unlimited Copy the value in the item
		/// </summary>
		/// <param name="item"></param>
		/// <param name="_sourceSlot"></param>
		private void CloneItem(ItemDragAndDrop item, SlotDragAndDrop _sourceSlot)
		{
			
			switch (containType)
			{
				case ContainType.Skill:
				{
					item.GetComponent<SkillInfo>().skill = _sourceSlot.GetInfoSkill().skill;
					item.GetComponent<SkillInfo>().Unit = _sourceSlot.GetInfoSkill().Unit;
				}
					break;
				case ContainType.Gear:
				{
					item.GetComponent<GearInfo>().Gear = _sourceSlot.GetInfoGear().Gear;
				}
					break;
				case ContainType.Relic:
				{
					item.GetComponent<RelicInfo>().CreateRelic(_sourceSlot.GetInfoRelic().Relic);
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
			if (_currentItem != null)
			{
				DropEventDescriptor desc = new DropEventDescriptor();
				// Fill event descriptor
				desc.triggerType = TriggerType.ItemWillBeDestroyed;
				desc.item = _currentItem;
				desc._sourceSlot = this;
				desc._destinationSlot = this;
				SendNotification(desc);                                         // Notify application about item destruction
				if (_currentItem != null)
				{
					Destroy(_currentItem.gameObject);
				}
			}
			_currentItem = null;
			UpdateBackgroundState();
		}

		/// <summary>
		/// Send drag and drop information to application
		/// </summary>
		/// <param name="desc"> drag and drop event descriptor </param>
		private void SendNotification(DropEventDescriptor desc)
		{
			if (desc != null)
			{
				// Send message with DragAndDrop info to parents GameObjects
				gameObject.SendMessageUpwards("OnSimpleDragAndDropEvent", desc, SendMessageOptions.DontRequireReceiver);
			}
		}

		/// <summary>
		/// Send drag and drop request to application
		/// </summary>
		/// <param name="desc"> drag and drop event descriptor </param>
		/// <returns> result from desc.permission </returns>
		private bool SendRequest(DropEventDescriptor desc)
		{
			bool result = false;
			if (desc != null)
			{
				desc.triggerType = TriggerType.DropRequest;
				desc.permission = true;
				SendNotification(desc);
				result = desc.permission;
			}
			return result;
		}

		/// <summary>
		/// Wait for event end and send notification to application
		/// </summary>
		/// <param name="desc"> drag and drop event descriptor </param>
		/// <returns></returns>
		private IEnumerator NotifyOnDragEnd(DropEventDescriptor desc)
		{
			// Wait end of drag operation
			while (ItemDragAndDrop._draggedItem != null)
			{
				yield return new WaitForEndOfFrame();
			}
			desc.triggerType = TriggerType.DropEventEnd;
			SendNotification(desc);
		}

		/// <summary>
		/// Change cell's sprite color on item put/remove.
		/// </summary>
		/// <param name="condition"> true - filled, false - empty </param>
		public void UpdateBackgroundState()
		{
			if (frame != null)
			{
				frame.color = _currentItem != null ? full : empty;
			}

			if (rarity != null)
			{
				rarity.color = Color.clear;
				if (containType == ContainType.Gear)
				{
					rarity.color = _currentItem != null ? _currentItem.GetComponent<GearInfo>().Gear.GearSO.Rarity.TextColour : Color.black;
				}
			}
		}

		/// <summary>
		/// Updates my item
		/// </summary>
		public void UpdateMyItem()
		{
			_currentItem = GetComponentInChildren<ItemDragAndDrop>();
		}

		/// <summary>
		/// Get item from this cell
		/// </summary>
		/// <returns> Item </returns>
		public ItemDragAndDrop GetItem()
		{
			return _currentItem;
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
				DropEventDescriptor desc = new DropEventDescriptor();
				// Fill event descriptor
				desc.triggerType = TriggerType.ItemAdded;
				desc.item = _newItem;
				desc._sourceSlot = this;
				desc._destinationSlot = this;
				SendNotification(desc);
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
