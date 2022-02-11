using System;
using System.Collections;
using UnityEngine;

namespace _Instances
{
    public class Utility : MonoBehaviour
    {
        private static Utility Instance;
        [SerializeField] private DataBase dataBase;

        private void Awake()
        {
            Instance = this;
            dataBase.InstantiateDataBases();
        }

        private void Start()
        {
            dataBase.InstantiateDataBases();
        }

        public static void RunCoroutine(IEnumerator coroutine)
        {
            Instance.StartCoroutine(coroutine);
        }
    }
}