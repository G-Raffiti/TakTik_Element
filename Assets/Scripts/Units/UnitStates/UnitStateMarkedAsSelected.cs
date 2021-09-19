namespace Units.UnitStates
{
    public class UnitStateMarkedAsSelected : UnitState
    {
        public UnitStateMarkedAsSelected(Unit _unit) : base(_unit)
        {
        }

        public override void Apply()
        {
            Unit.MarkAsSelected();
        }

        public override void MakeTransition(UnitState state)
        {
            state.Apply();
            Unit.UnitState = state;
        }
    }
}
