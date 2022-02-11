using System;
using _EventSystem.CustomEvents;
using UnityEngine;
using Void = _EventSystem.CustomEvents.Void;

namespace ScreenResolutionManager
{
    public class CameraMovement : MonoBehaviour
    {
        private float Speed = 0.05f;
        private float maxX;
        private float maxY;
        private float minX;
        private float minY;

        private Vector3 newbackPos;
        private Vector3 newCameraPos;

        [SerializeField] private VoidEvent onBordLoaded;
        [SerializeField] private RectTransform backGround;

        private void Awake()
        {
            onBordLoaded.EventListeners += setMax;
        }
        private void OnDestroy()
        {
            onBordLoaded.EventListeners -= setMax;
        }

        public void setMax(Void empty)
        {
            foreach (Transform _cell in GameObject.Find("CellGrid").transform)
            {
                if (_cell.transform.localPosition.x > maxX)
                    maxX = _cell.transform.localPosition.x;
                if (_cell.transform.localPosition.y > maxY)
                    maxY = _cell.transform.localPosition.y;
                if (_cell.transform.localPosition.x < minX)
                    minX = _cell.transform.localPosition.x;
                if (_cell.transform.localPosition.y < minY)
                    minY = _cell.transform.localPosition.y;
            }


            transform.position = new Vector3(maxX / 2f, maxY / 2f, -15);
            maxX -= 4;
            maxY -= 2;
            minX += 4;
            minY += 2;
            
            Debug.Log($"max: {maxX} - {maxY}, min: {minX} - {minY}");
        }
 
        void Update()
        {
            float xAxisValue = Input.GetAxis("Horizontal") * Speed;
            float yAxisValue = Input.GetAxis("Vertical") * Speed;

            float xPos = Mathf.Clamp(transform.position.x + xAxisValue, minX, maxX);
            float yPos = Mathf.Clamp(transform.position.y + yAxisValue, minY, maxY);

            newbackPos = new Vector3(backGround.position.x + (transform.position.x - xPos) * 0.5f, backGround.position.y + (transform.position.y - yPos) * 0.5f,
                backGround.position.z);
            newCameraPos = new Vector3(xPos,yPos, transform.position.z);
        }

        private void LateUpdate()
        {
            transform.position = newCameraPos;
            backGround.position = newbackPos;
        }
    }
}