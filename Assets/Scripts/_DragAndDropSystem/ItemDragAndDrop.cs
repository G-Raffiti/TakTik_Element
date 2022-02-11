using Resources.ToolTip.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _DragAndDropSystem
{
	/// <summary>
	/// Drag and Drop item.
	/// </summary>
	[RequireComponent(typeof(InfoBehaviour))]
	public class ItemDragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public static bool dragDisabled = false;										// Drag start global disable

		public static ItemDragAndDrop _draggedItem;                                      // Item that is dragged now
		public static GameObject icon;                                                  // Icon of dragged item
		public static SlotDragAndDrop _sourceSlot;                                       // From this cell dragged item is

		public delegate void DragEvent(ItemDragAndDrop item);
		public static event DragEvent OnItemDragStartEvent;                             // Drag start event
		public static event DragEvent OnItemDragEndEvent;                               // Drag end event

		private static Canvas canvas;                                                   // Canvas for item drag operation
		private static string canvasName = "DragAndDropCanvas";                   		// Name of canvas
		private static int canvasSortOrder = 5000;										// Sort order for canvas

		[Header("Type to change in the Prefabs")]
		[SerializeField] private SlotDragAndDrop.ContainType type;
		public SlotDragAndDrop.ContainType Type => type;

		/// <summary>
		/// Awake this instance.
		/// </summary>
		void Awake()
		{
			if (canvas == null)
			{
				GameObject canvasObj = new GameObject(canvasName);
				canvas = canvasObj.AddComponent<Canvas>();
				canvas.renderMode = RenderMode.ScreenSpaceCamera;
				canvas.worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
				canvas.sortingOrder = canvasSortOrder;
				CanvasScaler canvasScaler = canvasObj.AddComponent<CanvasScaler>();
				canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
				canvasScaler.referenceResolution = new Vector2(1920, 1080);
				canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
				canvasScaler.matchWidthOrHeight = 0;
				canvasScaler.referencePixelsPerUnit = 100;
			}
		}

		/// <summary>
		/// This item started to drag.
		/// </summary>
		/// <param name="eventData"></param>
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (dragDisabled == false)
			{
				_sourceSlot = GetCell();                       							// Remember source cell
				_draggedItem = this;                                             		// Set as dragged item
				// Create item's icon
				icon = new GameObject();
				icon.transform.SetParent(canvas.transform);
				icon.name = "Icon";
				Image myImage = GetComponent<Image>();
				myImage.raycastTarget = false;                                        	// Disable icon's raycast for correct drop handling
				Image iconImage = icon.AddComponent<Image>();
				iconImage.raycastTarget = false;
				iconImage.sprite = myImage.sprite;
				
				RectTransform iconRect = icon.GetComponent<RectTransform>();
				// Set icon's dimensions
				RectTransform myRect = GetComponent<RectTransform>();
				iconRect.pivot = new Vector2(0.5f, 0.5f);
				iconRect.anchorMin = new Vector2(0.5f, 0.5f);
				iconRect.anchorMax = new Vector2(0.5f, 0.5f);
				iconRect.sizeDelta = new Vector2(myRect.rect.width, myRect.rect.height);
				iconRect.localScale = Vector3.one;

				if (OnItemDragStartEvent != null)
				{
					OnItemDragStartEvent(this);                                			// Notify all items about drag start for raycast disabling
				}
			}
		}

		/// <summary>
		/// Every frame on this item drag.
		/// </summary>
		/// <param name="data"></param>
		public void OnDrag(PointerEventData data)
		{
			if (icon != null)
			{
				Vector3 screenPoint = Input.mousePosition;
				screenPoint.z = canvas.planeDistance;                                          //distance of the plane from the camera
				icon.transform.position = canvas.worldCamera.ScreenToWorldPoint(screenPoint);  // Item's icon follows to cursor in screen pixels
			}
		}

		/// <summary>
		/// This item is dropped.
		/// </summary>
		/// <param name="eventData"></param>
		public void OnEndDrag(PointerEventData eventData)
		{
			ResetConditions();
		}

		/// <summary>
		/// Resets all temporary conditions.
		/// </summary>
		private void ResetConditions()
		{
			if (icon != null)
			{
				Destroy(icon);                                                          // Destroy icon on item drop
			}
			if (OnItemDragEndEvent != null)
			{
				OnItemDragEndEvent(this);                                       		// Notify all cells about item drag end
			}
			_draggedItem = null;
			icon = null;
			_sourceSlot = null;
		}

		/// <summary>
		/// Enable item's raycast.
		/// </summary>
		/// <param name="condition"> true - enable, false - disable </param>
		public void MakeRaycast(bool condition)
		{
			Image image = GetComponent<Image>();
			if (image != null)
			{
				image.raycastTarget = condition;
			}
		}

		/// <summary>
		/// Gets DaD cell which contains this item.
		/// </summary>
		/// <returns>The cell.</returns>
		public SlotDragAndDrop GetCell()
		{
			return GetComponentInParent<SlotDragAndDrop>();
		}

		/// <summary>
		/// Raises the disable event.
		/// </summary>
		void OnDisable()
		{
			ResetConditions();
		}
	}
}
