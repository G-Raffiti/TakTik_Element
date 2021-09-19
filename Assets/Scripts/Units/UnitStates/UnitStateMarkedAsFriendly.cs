namespace Units.UnitStates
{
    public class UnitStateMarkedAsFriendly : UnitState
    {
        public UnitStateMarkedAsFriendly(Unit _unit) : base(_unit)
        {

        }

        public override void Apply()
        {
            Unit.MarkAsFriendly();
        }

        public override void MakeTransition(UnitState state)
        {
            state.Apply();
            Unit.UnitState = state;
        }
    }
}

