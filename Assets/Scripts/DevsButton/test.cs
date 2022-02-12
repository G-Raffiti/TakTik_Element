using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DevsButton
{
    public class test :MonoBehaviour
    {

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    Debug.Log("hit");
                PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
                eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
                foreach (RaycastResult _result in results)
                {
                    Debug.Log(_result.gameObject + _result.gameObject.transform.parent.name);
                }
            }
        }
    }
}