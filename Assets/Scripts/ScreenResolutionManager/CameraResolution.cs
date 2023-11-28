using UnityEngine;

namespace ScreenResolutionManager
{
    /// <summary>
    /// Skrypt odpowiada za usatwienie rozdzielczosci kemerze
    /// </summary>
    public class CameraResolution : MonoBehaviour
    {
 
 
        #region Pola
        private int screenSizeX = 0;
        private int screenSizeY = 0;
        #endregion
 
        #region metody
 
        #region rescale camera
        private void RescaleCamera()
        {
 
            if (Screen.width == screenSizeX && Screen.height == screenSizeY) return;
 
            float _targetaspect = 16.0f / 9.0f;
            float _windowaspect = (float)Screen.width / (float)Screen.height;
            float _scaleheight = _windowaspect / _targetaspect;
            Camera _camera = GetComponent<Camera>();
 
            if (_scaleheight < 1.0f)
            {
                Rect _rect = _camera.rect;
 
                _rect.width = 1.0f;
                _rect.height = _scaleheight;
                _rect.x = 0;
                _rect.y = (1.0f - _scaleheight) / 2.0f;
 
                _camera.rect = _rect;
            }
            else // add pillarbox
            {
                float _scalewidth = 1.0f / _scaleheight;
 
                Rect _rect = _camera.rect;
 
                _rect.width = _scalewidth;
                _rect.height = 1.0f;
                _rect.x = (1.0f - _scalewidth) / 2.0f;
                _rect.y = 0;
 
                _camera.rect = _rect;
            }
 
            screenSizeX = Screen.width;
            screenSizeY = Screen.height;
        }
        #endregion
 
        #endregion
 
        #region metody unity
 
        void OnPreCull()
        {
            if (Application.isEditor) return;
            Rect _wp = Camera.main.rect;
            Rect _nr = new Rect(0, 0, 1, 1);
 
            Camera.main.rect = _nr;
            GL.Clear(true, true, Color.black);
       
            Camera.main.rect = _wp;
 
        }
 
        // Use this for initialization
        void Start () {
            RescaleCamera();
        }
   
        // Update is called once per frame
        void Update () {
            RescaleCamera();
        }
        #endregion
    }
}