using UnityEngine;

namespace _Instances
{
    public class KeepBetweenScene : MonoBehaviour
    {
        [SerializeField] private int stage;
        private static GameObject instance;
        public static int Stage { get; private set; }
        public const int BattlePerStage = 3;
        public static int BattleNumber = 0;

        public static void NextStage()
        {
            Stage += 1;
            BattleNumber = 0;
        }

        public static void EndBattle()
        {
            BattleNumber += 1;
            Debug.Log($"Stage = {Stage}, Battle number = {BattleNumber}");
            
            if (BattleNumber >= BattlePerStage)
                NextStage();
        }
        
        private void Start() 
        {
            DontDestroyOnLoad(gameObject.transform);
            if (instance == null)
                instance = gameObject;
            else
                Destroy(gameObject);
        }
        
        [ContextMenu("set stage")]
        public void setStage()
        {
            Stage = stage;
        }
    }

    
}