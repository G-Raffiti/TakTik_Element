using UnityEngine;

namespace ScreenResolutionManager.Example
{
    public class ResolutionSelector : MonoBehaviour {
        void OnGUI()
        {
            if (ResolutionManager.Instance == null) return;

            ResolutionManager _resolutionManager = ResolutionManager.Instance;

            GUILayout.BeginArea(new Rect(20, 10, 200, Screen.height - 10));

            GUILayout.Label("Select Resolution");

            if (GUILayout.Button(Screen.fullScreen ? "Windowed" : "Fullscreen"))
                _resolutionManager.ToggleFullscreen();

            int _i = 0;
            foreach (Vector2 _r in Screen.fullScreen ? _resolutionManager.fullscreenResolutions : _resolutionManager.windowedResolutions)
            {
                string _label = _r.x + "x" + _r.y;
                if (_r.x == Screen.width && _r.y == Screen.height) _label += "*";
                if (_r.x == _resolutionManager.DisplayResolution.width && _r.y == _resolutionManager.DisplayResolution.height) _label += " (native)";

                if (GUILayout.Button(_label))
                    _resolutionManager.SetResolution(_i, Screen.fullScreen);

                _i++;
            }

            if (GUILayout.Button("Get Current Resolution"))
            {
                Resolution _r = Screen.currentResolution;
                Debug.Log("Display resolution is " + _r.width + "x" + _r.height);
            }

            GUILayout.EndArea();
        }
    }
}
