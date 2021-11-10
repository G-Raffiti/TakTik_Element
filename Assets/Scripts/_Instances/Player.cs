using System.Collections.Generic;
using Units;
using UnityEngine;

namespace _Instances
{
    public class PlayerData
    {
        private static PlayerData instance;
        private List<Hero> heroes;
        public List<Hero> Heroes => Heroes;

        public static PlayerData getInstance()
        {
            if (instance == null)
                instance = new PlayerData();
            return instance;
        }

        private PlayerData()
        {
            if (GameObject.Find("Player") != null)
            {
                foreach (Transform _child in GameObject.Find("Player").transform)
                {
                    heroes.Add(_child.GetComponent<Hero>());
                }
            } 
        }
    }
}