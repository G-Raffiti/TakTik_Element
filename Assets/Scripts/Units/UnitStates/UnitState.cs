namespace Units.UnitStates
{
    /// <summary>
    /// The purpose of this class is to prevent units from changing appearance from "finished" to any other than "normal".
    /// This was the cleanest way to do it that I could come up with. I'm thinking how it affects customizability and may change it in the future.
    /// </summary>
    public abstract class UnitState
    {
        protected Unit Unit;

        public UnitState(Unit _unit)
        {
            Unit = _unit;
        }

        public abstract void Apply();
        public abstract void MakeTransition(UnitState state);
    }
}

