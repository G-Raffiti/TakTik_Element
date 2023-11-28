using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScreenResolutionManager
{
    public class ResolutionManager : MonoBehaviour
    {
        static public ResolutionManager Instance;

        // Fixed aspect ratio parameters
        static public bool FixedAspectRatio = true;
        static public float TargetAspectRatio = 4 / 3f;

        // Windowed aspect ratio when FixedAspectRatio is false
        static public float WindowedAspectRatio = 4f / 3f;

        // List of horizontal resolutions to include
        int[] resolutions = new int[] { 600, 800, 1024, 1280, 1400, 1600, 1920 };

        public Resolution DisplayResolution;
        [FormerlySerializedAs("WindowedResolutions")]
        public List<Vector2> windowedResolutions;
        [FormerlySerializedAs("FullscreenResolutions")]
        public List<Vector2> fullscreenResolutions;

        int currWindowedRes, currFullscreenRes;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            StartCoroutine(StartRoutine());
        }

        private void PrintResolution()
        {
            Debug.Log("Current res: " + Screen.currentResolution.width + "x" + Screen.currentResolution.height);
        }

        private IEnumerator StartRoutine()
        {
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                DisplayResolution = Screen.currentResolution;
            }
            else
            {
                if (Screen.fullScreen)
                {
                    Resolution _r = Screen.currentResolution;
                    Screen.fullScreen = false;

                    yield return null;
                    yield return null;

                    DisplayResolution = Screen.currentResolution;

                    Screen.SetResolution(_r.width, _r.height, true);

                    yield return null;
                }
                else
                {
                    DisplayResolution = Screen.currentResolution;
                }
            }

            InitResolutions();
        }

        private void InitResolutions()
        {
            float _screenAspect = (float)DisplayResolution.width / DisplayResolution.height;

            windowedResolutions = new List<Vector2>();
            fullscreenResolutions = new List<Vector2>();

            foreach (int _w in resolutions)
            {
                if (_w < DisplayResolution.width)
                {
                    // Adding resolution only if it's 20% smaller than the screen
                    if (_w < DisplayResolution.width * 0.8f)
                    {
                        Vector2 _windowedResolution = new Vector2(_w, Mathf.Round(_w / (FixedAspectRatio ? TargetAspectRatio : WindowedAspectRatio)));
                        if (_windowedResolution.y < DisplayResolution.height * 0.8f)
                            windowedResolutions.Add(_windowedResolution);

                        fullscreenResolutions.Add(new Vector2(_w, Mathf.Round(_w / _screenAspect)));
                    }
                }
            }

            // Adding fullscreen native resolution
            fullscreenResolutions.Add(new Vector2(DisplayResolution.width, DisplayResolution.height));

            // Adding half fullscreen native resolution
            Vector2 _halfNative = new Vector2(DisplayResolution.width * 0.5f, DisplayResolution.height * 0.5f);
            if (_halfNative.x > resolutions[0] && fullscreenResolutions.IndexOf(_halfNative) == -1)
                fullscreenResolutions.Add(_halfNative);

            fullscreenResolutions = fullscreenResolutions.OrderBy(_resolution => _resolution.x).ToList();

            bool _found = false;

            if (Screen.fullScreen)
            {
                currWindowedRes = windowedResolutions.Count - 1;

                for (int _i = 0; _i < fullscreenResolutions.Count; _i++)
                {
                    if (fullscreenResolutions[_i].x == Screen.width && fullscreenResolutions[_i].y == Screen.height)
                    {
                        currFullscreenRes = _i;
                        _found = true;
                        break;
                    }
                }

                if (!_found)
                    SetResolution(fullscreenResolutions.Count - 1, true);
            }
            else
            {
                currFullscreenRes = fullscreenResolutions.Count - 1;

                for (int _i = 0; _i < windowedResolutions.Count; _i++)
                {
                    if (windowedResolutions[_i].x == Screen.width && windowedResolutions[_i].y == Screen.height)
                    {
                        _found = true;
                        currWindowedRes = _i;
                        break;
                    }
                }

                if (!_found)
                    SetResolution(windowedResolutions.Count - 1, false);
            }
        }

        public void SetResolution(int _index, bool _fullscreen)
        {
            Vector2 _r = new Vector2();
            if (_fullscreen)
            {
                currFullscreenRes = _index;
                _r = fullscreenResolutions[currFullscreenRes];
            }
            else
            {
                currWindowedRes = _index;
                _r = windowedResolutions[currWindowedRes];
            }

            bool _fullscreen2Windowed = Screen.fullScreen & !_fullscreen;

            Debug.Log("Setting resolution to " + (int)_r.x + "x" + (int)_r.y);
            Screen.SetResolution((int)_r.x, (int)_r.y, _fullscreen);

            // On OSX the application will pass from fullscreen to windowed with an animated transition of a couple of seconds.
            // After this transition, the first time you exit fullscreen you have to call SetResolution again to ensure that the window is resized correctly.
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                // Ensure that there is no SetResolutionAfterResize coroutine running and waiting for screen size changes
                StopAllCoroutines();

                // Resize the window again after the end of the resize transition
                if (_fullscreen2Windowed) StartCoroutine(SetResolutionAfterResize(_r));
            }
        }

        private IEnumerator SetResolutionAfterResize(Vector2 _r)
        {
            int _maxTime = 5; // Max wait for the end of the resize transition
            float _time = Time.time;

            // Skipping a couple of frames during which the screen size will change
            yield return null;
            yield return null;

            int _lastW = Screen.width;
            int _lastH = Screen.height;

            // Waiting for another screen size change at the end of the transition animation
            while (Time.time - _time < _maxTime)
            {
                if (_lastW != Screen.width || _lastH != Screen.height)
                {
                    Debug.Log("Resize! " + Screen.width + "x" + Screen.height);

                    Screen.SetResolution((int)_r.x, (int)_r.y, Screen.fullScreen);
                    yield break;
                }

                yield return null;
            }

            Debug.Log("End waiting");
        }

        public void ToggleFullscreen()
        {
            SetResolution(
                Screen.fullScreen ? currWindowedRes : currFullscreenRes,
                !Screen.fullScreen);
        }
    }
}