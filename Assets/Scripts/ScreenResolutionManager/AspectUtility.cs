using UnityEngine;

namespace ScreenResolutionManager
{
    public class AspectUtility : MonoBehaviour
    {
        static Camera _backgroundCam;
        static Camera _staticCam; // This is the last camera where Awake is called. It is used for the static getter methods.
        Camera cam;

        void Awake()
        {
            cam = GetComponent<Camera>();

            if (!cam)
            {
                cam = Camera.main;
            }
            if (!cam)
            {
                Debug.LogError("No camera available");
                return;
            }

            _staticCam = cam;

            UpdateCamera ();
        }

        private void UpdateCamera()
        {
            if (!ResolutionManager.FixedAspectRatio || !cam) return;

            float _currentAspectRatio = (float)Screen.width / Screen.height;

            // If the current aspect ratio is already approximately equal to the desired aspect ratio,
            // use a full-screen Rect (in case it was set to something else previously)
            if ((int)(_currentAspectRatio * 100) / 100.0f == (int)(ResolutionManager.TargetAspectRatio * 100) / 100.0f)
            {
                cam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                if (_backgroundCam)
                {
                    Destroy(_backgroundCam.gameObject);
                }
                return;
            }

            // Pillarbox
            if (_currentAspectRatio > ResolutionManager.TargetAspectRatio)
            {
                float _inset = 1.0f - ResolutionManager.TargetAspectRatio / _currentAspectRatio;
                cam.rect = new Rect(_inset / 2, 0.0f, 1.0f - _inset, 1.0f);
            }
            // Letterbox
            else
            {
                float _inset = 1.0f - _currentAspectRatio / ResolutionManager.TargetAspectRatio;
                cam.rect = new Rect(0.0f, _inset / 2, 1.0f, 1.0f - _inset);
            }

            if (!_backgroundCam)
            {
                // Make a new camera behind the normal camera which displays black; otherwise the unused space is undefined
                _backgroundCam = new GameObject("BackgroundCam", typeof(Camera)).GetComponent<Camera>();
                _backgroundCam.depth = int.MinValue;
                _backgroundCam.clearFlags = CameraClearFlags.SolidColor;
                _backgroundCam.backgroundColor = Color.black;
                _backgroundCam.cullingMask = 0;
            }
        }

        public static int ScreenHeight
        {
            get
            {
                return (int)(Screen.height * _staticCam.rect.height);
            }
        }

        public static int ScreenWidth
        {
            get
            {
                return (int)(Screen.width * _staticCam.rect.width);
            }
        }

        public static int XOffset
        {
            get
            {
                return (int)(Screen.width * _staticCam.rect.x);
            }
        }

        public static int YOffset
        {
            get
            {
                return (int)(Screen.height * _staticCam.rect.y);
            }
        }

        public static Rect ScreenRect
        {
            get
            {
                return new Rect(_staticCam.rect.x * Screen.width, _staticCam.rect.y * Screen.height, _staticCam.rect.width * Screen.width, _staticCam.rect.height * Screen.height);
            }
        }

        public static Vector3 MousePosition
        {
            get
            {
                Vector3 _mousePos = Input.mousePosition;
                _mousePos.y -= (int)(_staticCam.rect.y * Screen.height);
                _mousePos.x -= (int)(_staticCam.rect.x * Screen.width);
                return _mousePos;
            }
        }

        public static Vector2 GUIMousePosition
        {
            get
            {
                Vector2 _mousePos = Event.current.mousePosition;
                _mousePos.y = Mathf.Clamp(_mousePos.y, _staticCam.rect.y * Screen.height, _staticCam.rect.y * Screen.height + _staticCam.rect.height * Screen.height);
                _mousePos.x = Mathf.Clamp(_mousePos.x, _staticCam.rect.x * Screen.width, _staticCam.rect.x * Screen.width + _staticCam.rect.width * Screen.width);
                return _mousePos;
            }
        }

        private int lastWidth = -1, lastHeight = -1;
        public void Update()
        {
            if (Screen.width != lastWidth || Screen.height != lastHeight) {
                lastWidth = Screen.width;
                lastHeight = Screen.height;

                UpdateCamera();
            }
        }
    }
}