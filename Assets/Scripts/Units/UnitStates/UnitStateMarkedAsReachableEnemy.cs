namespace Units.UnitStates
{
    public class UnitStateMarkedAsReachableEnemy : UnitState
    {
        public UnitStateMarkedAsReachableEnemy(Unit _unit) : base(_unit)
        {
        }

        public override void Apply()
        {
            Unit.MarkAsReachableEnemy();
        }

        public override void MakeTransition(UnitState state)
        {
            state.Apply();
            Unit.UnitState = state;
        }
    }
}

