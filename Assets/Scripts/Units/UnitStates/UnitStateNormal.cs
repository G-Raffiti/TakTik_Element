namespace Units.UnitStates
{
    public class UnitStateNormal : UnitState
    {
        public UnitStateNormal(Unit _unit) : base(_unit)
        {
        }

        public override void Apply()
        {
            Unit.UnMark();
        }

        public override void MakeTransition(UnitState state)
        {
            state.Apply();
            Unit.UnitState = state;
        }
    }
}

