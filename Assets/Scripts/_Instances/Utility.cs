using System;
using System.Collections;
using UnityEngine;

namespace _Instances
{
    public class Utility : MonoBehaviour
    {
        private static Utility _instance;
        [SerializeField] private DataBase dataBase;

        private void Awake()
        {
            _instance = this;
            dataBase.InstantiateDataBases();
        }

        private void Start()
        {
            dataBase.InstantiateDataBases();
        }

        public static void RunCoroutine(IEnumerator _coroutine)
        {
            _instance.StartCoroutine(_coroutine);
        }
    }
}