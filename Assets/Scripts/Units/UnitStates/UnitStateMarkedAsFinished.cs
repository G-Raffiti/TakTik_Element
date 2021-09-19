namespace Units.UnitStates
{
    public class UnitStateMarkedAsFinished : UnitState
    {
        public UnitStateMarkedAsFinished(Unit _unit) : base(_unit)
        {
        }

        public override void Apply()
        {
            Unit.MarkAsFinished();
        }

        public override void MakeTransition(UnitState state)
        {
            if (state is UnitStateNormal)
            {
                state.Apply();
                Unit.UnitState = state;
            }
        }
    }
}

