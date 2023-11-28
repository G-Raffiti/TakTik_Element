using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DevsButton
{
    public class Test :MonoBehaviour
    {
        void Start()
        {
            Debug.Log(Application.persistentDataPath);
        }
    
        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    Debug.Log("hit");
                PointerEventData _eventDataCurrentPosition = new PointerEventData(EventSystem.current);
                _eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                List<RaycastResult> _results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
                foreach (RaycastResult _result in _results)
                {
                    Debug.Log(_result.gameObject + _result.gameObject.transform.parent.name);
                }
            }
        }
    }
}