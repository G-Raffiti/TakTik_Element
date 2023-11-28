using Resources.ToolTip.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _DragAndDropSystem
{
	/// <summary>
	/// Drag and Drop item.
	/// </summary>
	[RequireComponent(typeof(InfoBehaviour))]
	public class ItemDragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		[FormerlySerializedAs("DraggedImage")]
		[Header("If this is not filled, it will drag the Background of the Object")]
		[SerializeField] private Image draggedImage;
		public static bool dragDisabled = false;										// Drag start global disable

		public static ItemDragAndDrop DraggedItem;                                      // Item that is dragged now
		public static GameObject icon;                                                  // Icon of dragged item
		public static SlotDragAndDrop SourceSlot;                                       // From this cell dragged item is

		public delegate void DragEvent(ItemDragAndDrop _item);
		public static event DragEvent OnItemDragStartEvent;                             // Drag start event
		public static event DragEvent OnItemDragEndEvent;                               // Drag end event

		private static Canvas _canvas;                                                   // Canvas for item drag operation
		private static string _canvasName = "DragAndDropCanvas";                   		// Name of canvas
		private static int _canvasSortOrder = 5000;										// Sort order for canvas

		[Header("Type to change in the Prefabs")]
		[SerializeField] private SlotDragAndDrop.ContainType type;
		public SlotDragAndDrop.ContainType Type => type;

		/// <summary>
		/// Awake this instance.
		/// </summary>
		void Awake()
		{
			if (_canvas == null)
			{
				GameObject _canvasObj = new GameObject(_canvasName);
				_canvas = _canvasObj.AddComponent<Canvas>();
				_canvas.renderMode = RenderMode.ScreenSpaceCamera;
				_canvas.worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
				_canvas.sortingOrder = _canvasSortOrder;
				CanvasScaler _canvasScaler = _canvasObj.AddComponent<CanvasScaler>();
				_canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
				_canvasScaler.referenceResolution = new Vector2(1920, 1080);
				_canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
				_canvasScaler.matchWidthOrHeight = 0;
				_canvasScaler.referencePixelsPerUnit = 100;
			}
		}

		/// <summary>
		/// This item started to drag.
		/// </summary>
		/// <param name="eventData"></param>
		public void OnBeginDrag(PointerEventData _eventData)
		{
			if (dragDisabled == false)
			{
				SourceSlot = GetCell();                       							// Remember source cell
				DraggedItem = this;                                             		// Set as dragged item
				// Create item's icon
				icon = new GameObject();
				icon.transform.SetParent(_canvas.transform);
				icon.name = "Icon";
				Image _myImage = GetComponent<Image>();
				_myImage.raycastTarget = false;                                        	// Disable icon's raycast for correct drop handling
				Image _iconImage = icon.AddComponent<Image>();
				_iconImage.raycastTarget = false;
				_iconImage.sprite = draggedImage == null ? _myImage.sprite : draggedImage.sprite;
				
				RectTransform _iconRect = icon.GetComponent<RectTransform>();
				// Set icon's dimensions
				RectTransform _myRect = GetComponent<RectTransform>();
				_iconRect.pivot = new Vector2(0.5f, 0.5f);
				_iconRect.anchorMin = new Vector2(0.5f, 0.5f);
				_iconRect.anchorMax = new Vector2(0.5f, 0.5f);
				_iconRect.sizeDelta = new Vector2(_myRect.rect.width, _myRect.rect.height);
				_iconRect.localScale = Vector3.one;

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
		public void OnDrag(PointerEventData _data)
		{
			if (icon != null)
			{
				Vector3 _screenPoint = Input.mousePosition;
				_screenPoint.z = _canvas.planeDistance;                                          //distance of the plane from the camera
				icon.transform.position = _canvas.worldCamera.ScreenToWorldPoint(_screenPoint);  // Item's icon follows to cursor in screen pixels
			}
		}

		/// <summary>
		/// This item is dropped.
		/// </summary>
		/// <param name="eventData"></param>
		public void OnEndDrag(PointerEventData _eventData)
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
			DraggedItem = null;
			icon = null;
			SourceSlot = null;
		}

		/// <summary>
		/// Enable item's raycast.
		/// </summary>
		/// <param name="condition"> true - enable, false - disable </param>
		public void MakeRaycast(bool _condition)
		{
			Image _image = GetComponent<Image>();
			if (_image != null)
			{
				_image.raycastTarget = _condition;
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
