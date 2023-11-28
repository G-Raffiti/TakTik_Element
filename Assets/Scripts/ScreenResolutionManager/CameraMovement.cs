using System;
using _EventSystem.CustomEvents;
using UnityEngine;
using Void = _EventSystem.CustomEvents.Void;

namespace ScreenResolutionManager
{
    public class CameraMovement : MonoBehaviour
    {
        private float speed = 0.05f;
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
            onBordLoaded.EventListeners += SetMax;
        }
        private void OnDestroy()
        {
            onBordLoaded.EventListeners -= SetMax;
        }

        public void SetMax(Void _empty)
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
        }
 
        void Update()
        {
            float _xAxisValue = Input.GetAxis("Horizontal") * speed;
            float _yAxisValue = Input.GetAxis("Vertical") * speed;

            float _xPos = Mathf.Clamp(transform.position.x + _xAxisValue, minX, maxX);
            float _yPos = Mathf.Clamp(transform.position.y + _yAxisValue, minY, maxY);

            newbackPos = new Vector3(backGround.position.x + (transform.position.x - _xPos) * 0.5f, backGround.position.y + (transform.position.y - _yPos) * 0.5f,
                backGround.position.z);
            newCameraPos = new Vector3(_xPos,_yPos, transform.position.z);
        }

        private void LateUpdate()
        {
            transform.position = newCameraPos;
            backGround.position = newbackPos;
        }
    }
}