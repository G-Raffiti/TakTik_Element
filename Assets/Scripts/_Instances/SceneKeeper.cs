using UnityEngine;

namespace _Instances
{
    public class SceneKeeper : MonoBehaviour
    {
        private static GameObject _instance;
        private void Start() 
        {
            DontDestroyOnLoad(gameObject.transform);
            if (_instance == null)
                _instance = gameObject;
            else
                Destroy(gameObject);
        }
    }
}