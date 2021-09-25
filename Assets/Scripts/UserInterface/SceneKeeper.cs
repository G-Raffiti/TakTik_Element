﻿using UnityEngine;

namespace UserInterface
{
    public class SceneKeeper : MonoBehaviour
    {
        private static GameObject instance;
        private void Start() 
        {
            DontDestroyOnLoad(gameObject.transform);
            if (instance == null)
                instance = gameObject;
            else
                Destroy(gameObject);
        }
    }
}