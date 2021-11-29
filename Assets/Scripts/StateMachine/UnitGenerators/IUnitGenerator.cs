using System.Collections.Generic;
using Cells;
using Units;

namespace StateMachine.UnitGenerators
{
    public interface IUnitGenerator
    {
        List<Unit> SpawnUnits(List<Cell> cells);
    }
}
