using System;
using System.Collections;
using Cells;
using Gears;
using Skills;
using Stats;
using Units;
using UnityEngine;

namespace _Instances
{
    public class DataBase : MonoBehaviour
    {
        public static DataBase Instance;
        public static DataBaseMonster Monster;
        public static DataBaseRelic Relic;
        public static DataBaseCell Cell;
        public static DataBaseAffix Affix;
        public static DataBaseGear Gear;
        public static DataBaseBoard Board;
        public static DataBaseSkill Skill;

        [SerializeField] private DataBaseMonster dataMonster;
        [SerializeField] private DataBaseRelic dataRelic;
        [SerializeField] private DataBaseCell dataCell;
        [SerializeField] private DataBaseAffix dataAffix;
        [SerializeField] private DataBaseGear dataGear;
        [SerializeField] private DataBaseBoard dataBoard;
        [SerializeField] private DataBaseSkill dataSkill;

        public void Start()
        {
            DontDestroyOnLoad(gameObject.transform);
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            InstantiateDataBases();
        }

        public void InstantiateDataBases()
        {
            Monster = dataMonster;
            Relic = dataRelic;
            Cell = dataCell;
            Affix = dataAffix;
            Gear = dataGear;
            Board = dataBoard;
            Skill = dataSkill;
        }

        public static void RunCoroutine(IEnumerator coroutine)
        {
            Instance.StartCoroutine(coroutine);
        }
    }
}