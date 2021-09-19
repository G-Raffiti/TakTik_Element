﻿using System;
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

        [SerializeField] private DataBaseMonster dataMonster;
        [SerializeField] private DataBaseRelic dataRelic;
        [SerializeField] private DataBaseCell dataCell;
        [SerializeField] private DataBaseAffix dataAffix;
        [SerializeField] private DataBaseGear dataGear;
        [SerializeField] private DataBaseBoard dataBoard;

        public void Start()
        {
            Monster = dataMonster;
            Relic = dataRelic;
            Cell = dataCell;
            Affix = dataAffix;
            Gear = dataGear;
            Board = dataBoard;
            Instance = this;
        }

        public static void RunCoroutine(IEnumerator coroutine)
        {
            Instance.StartCoroutine(coroutine);
        }
    }
}