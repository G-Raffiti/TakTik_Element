using System;
using System.Collections.Generic;
using _Extension;
using _SaveSystem;
using Players;
using Units;

namespace Score
{
    [Serializable]
    public class PersistentData : ISaveable
    {
        private static PersistentData instance;
        public static PersistentData Instance => instance ??= new PersistentData();
        public int SaveNumber;
        public string Id => "this is the Only Save you have" + SaveNumber;
        public int WinStrike { get; private set; } = 0;
        public int BestWinStrike { get; private set; } = 0;
        public int DamageDealtTotal { get; private set; } = 0;
        public int DamageDealtBiggest { get; private set; } = 0;
        public int DamageDealtInOneGameTotal { get; private set; } = 0;
        public int DamageTakenTotal { get; private set; } = 0;
        public int DamageTakenBiggest { get; private set; } = 0;
        public int DamageTakenInOneGameTotal { get; private set; } = 0;
        public int CellWalkedMax { get; private set; } = 0;
        public int CellWalkedTotal { get; private set; } = 0;
        public int GearSalvagedTotal { get; private set; } = 0;
        public int GearSalvagedMax { get; private set; } = 0;
        public int CraftingMaterialCollectedTotal { get; private set; } = 0;
        public int CraftingMaterialCollectedMax { get; private set; } = 0;
        public int MaxScore { get; private set; } = 0;
        public PersistentData(){}
        public PersistentData(PersistentData _data)
        {
            WinStrike = _data.WinStrike;
            BestWinStrike = _data.BestWinStrike;
            DamageDealtTotal = _data.DamageDealtTotal;
            DamageDealtBiggest = _data.DamageDealtBiggest;
            DamageDealtInOneGameTotal = _data.DamageDealtInOneGameTotal;
            DamageTakenTotal = _data.DamageTakenTotal;
            DamageTakenBiggest = _data.DamageTakenBiggest;
            DamageTakenInOneGameTotal = _data.DamageTakenInOneGameTotal;
            CellWalkedMax = _data.CellWalkedMax;
            CellWalkedTotal = _data.CellWalkedTotal;
            GearSalvagedTotal = _data.GearSalvagedTotal;
            GearSalvagedMax = _data.GearSalvagedMax;
            CraftingMaterialCollectedTotal = _data.CraftingMaterialCollectedTotal;
            CraftingMaterialCollectedMax = _data.CraftingMaterialCollectedMax;
            MaxScore = _data.MaxScore;
        }

        public void SetScore(bool isWin)
        {
            if (isWin)
                WinStrike++;
            BestWinStrike = BestWinStrike.Max(WinStrike);
            if (!isWin)
                WinStrike = 0;
            
            SetDamage();
            SetCrafting();
            SetCellWalked();
            SetMaxScore();
        }

        private void SetDamage()
        {
            DamageDealtTotal += ScoreHolder.DamageDealtTotal;
            DamageDealtBiggest = DamageDealtBiggest.Max(ScoreHolder.DamageDealtBiggest);
            DamageDealtInOneGameTotal = DamageDealtInOneGameTotal.Max(ScoreHolder.DamageDealtTotal);
            DamageTakenTotal += ScoreHolder.DamageTakenTotal;
            DamageTakenBiggest = DamageTakenBiggest.Max(ScoreHolder.DamageTakenBiggest);
            DamageTakenInOneGameTotal = DamageTakenInOneGameTotal.Max(ScoreHolder.DamageTakenTotal);
        }

        private void SetCellWalked()
        {
            CellWalkedMax = CellWalkedMax.Max(ScoreHolder.CellWalked);
            CellWalkedTotal += ScoreHolder.CellWalked;
        }

        private void SetCrafting()
        {
            GearSalvagedTotal += ScoreHolder.GearSalvaged;
            GearSalvagedMax = GearSalvagedMax.Max(ScoreHolder.GearSalvaged);
            CraftingMaterialCollectedTotal += ScoreHolder.CraftingMaterialCollected;
            CraftingMaterialCollectedMax = CraftingMaterialCollectedMax.Max(ScoreHolder.CraftingMaterialCollected);
        }

        private void SetMaxScore()
        {
            MaxScore = MaxScore.Max(ScoreHolder.GameScore());
        }

        public object CaptureState()
        {
            return instance;
        }

        public void RestoreState(object _state)
        {
            PersistentData _data = (PersistentData) _state;
            instance = new PersistentData(_data);
        }
    }
}