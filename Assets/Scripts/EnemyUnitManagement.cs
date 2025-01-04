using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyUnitManagement : MonoBehaviour
{
    public EnemyBrain Brain;

    private Dictionary<Unit, Unit> _targetAndDefenders = new();
    // Key is target unit, value is the unit that is defending it.
    public void PlayerUnitMoved(Unit unit, HexCell to)
    {
        float deltaX = to.transform.position.x - unit.transform.position.x;
        float deltaY = to.transform.position.y - unit.transform.position.y;

        // get closest unit in _enemyZoneUnits[resultingZone], and to.Position

        if (deltaY >= 4.05)
        {
            // player advance
            Debug.Log("Advance");
            MoveDefender(unit, to.transform.position);
        }
        else if (deltaY <= -4.05)
        {
            // player retreat
            Debug.Log("Retreat");
            MoveDefender(unit, to.transform.position);
        }
        else if (Math.Abs(deltaX) >= 6f * HexData.InnerRadius)
        {
            // player flanking manuever
            Debug.Log("Flank");
            MoveDefender(unit, to.transform.position);
        }
    }
    private HexCell GetDestinationHex(Vector2 position)
    {
        (int x, int y) pos = Game.Map.TransformToTilePosition(position.x, position.y + 4.05f);
        return Game.Map.ReturnHex(pos.x, pos.y);
    }
    private void MoveDefender(Unit target, Vector2 position)
    {
        if (_targetAndDefenders.ContainsKey(target))
        {
            _targetAndDefenders[target].MoveTo(GetDestinationHex(position));
        }
        else
        {
            Unit unit = MoveClosestTo(position);
            if (unit != null)
            {
                _targetAndDefenders.Add(target, unit);
            }
        }
    }
    private Unit MoveClosestTo(Vector2 position)
    {
        string resultingZone = Brain.GetZone(position.x);
        Unit unit = null;
        if (Brain.EnemyZoneUnits[resultingZone].Count > 0)
        {
            unit = GetClosestUnit(Brain.EnemyZoneUnits[resultingZone], position, 6);
            unit.MoveTo(GetDestinationHex(position));
            Debug.Log("moving!");
        }
        else if (Brain.EnemyZoneUnits["Middle"].Count > 0)
        {
            unit = GetClosestUnit(Brain.EnemyZoneUnits["Middle"], position, 6);
            unit.MoveTo(GetDestinationHex(position));
            Debug.Log("moving middle unit");
        }
        return unit;
    }
    private Unit GetClosestUnit(List<Unit> units, Vector3 position, int range = 32)
    {
        float maxDistance = range;
        Unit closestUnit = null;
        foreach (Unit u in units)
        {
            if (_targetAndDefenders.ContainsValue(u)) continue;
            Vector3 dPos = u.transform.position - position;
            if (dPos.magnitude < maxDistance)
            {
                maxDistance = dPos.magnitude;
                closestUnit = u;
            }
        }
        return closestUnit;
    }
}
