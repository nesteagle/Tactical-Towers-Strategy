using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.CanvasScaler;

public class EnemyUnitManagement : MonoBehaviour
{
    public EnemyBrain Brain;
    public Game Manager;

    public Dictionary<Unit, List<Unit>> TargetAndDefenders = new();
    // Key is target unit, value is the unit(s) that are defending it.
    public Dictionary<Unit, Unit> DefenderAndTargets = new();
    // Key is defender unit, value is the target unit.

    private readonly Dictionary<string, string[]> _unitTargetRanking = new()
    {
        { "Scout", new string[] { "Archer", "Scout", "Knight" } },
        { "Knight", new string[] { "Knight", "Archer", "Scout" } },
        { "Archer" , new string[] { "Knight", "Archer", "Scout" } }
    };

    public void PlayerUnitMoved(Unit unit, HexCell to)
    {
        float deltaX = to.transform.position.x - unit.transform.position.x;
        float deltaY = to.transform.position.y - unit.transform.position.y;

        // get closest unit in _enemyZoneUnits[resultingZone], and to.Position
        Vector2 position = to.transform.position;

        //MoveDefender(unit, to.transform.position);

        //if (deltaY >= 4.05)
        //{
        //    // player advance
        //}
        //else if (deltaY <= -4.05)
        //{
        //    // player retreat
        //}
        //else if (Math.Abs(deltaX) >= 6f * HexData.InnerRadius)
        //{
        //    // player flanking manuever
        //}
    }
    private HexCell GetDestinationHex(float x, float y)
    {
        (int x, int y) pos = Game.Map.TransformToTilePosition(x, y);
        return Game.Map.ReturnHex(pos.x, pos.y);
    }

    private Unit GetClosestTarget(List<Unit> targets, Vector3 from, int range = 32)
    {
        float maxDistance = range;
        Unit closestUnit = null;
        bool allAssigned = true;
        foreach (Unit target in targets)
        {
            if (TargetAndDefenders.ContainsKey(target)) continue;
            Vector3 dPos = target.transform.position - from;
            if (dPos.magnitude < maxDistance)
            {
                maxDistance = dPos.magnitude;
                closestUnit = target;
            }
            allAssigned = false;
        }
        if (allAssigned == true)
        {
            foreach (Unit target in targets)
            {
                if (TargetAndDefenders[target].Count > 2) continue;
                Vector3 dPos = target.transform.position - from;
                if (dPos.magnitude < maxDistance)
                {
                    maxDistance = dPos.magnitude;
                    closestUnit = target;
                }
            }
        }
        return closestUnit;
    }

    private Unit GetAvailableEnemyUnit(List<Unit> enemyUnits, Vector3 from, int range = 32)
    {
        float maxDistance = range;
        Unit closestUnit = null;

        foreach (Unit unit in enemyUnits)
        {
            if (DefenderAndTargets.ContainsKey(unit)) continue; // No unit can have more than one target
            Vector3 dPos = unit.transform.position - from;
            if (dPos.magnitude < maxDistance)
            {
                maxDistance = dPos.magnitude;
                closestUnit = unit;
            }
        }

        return closestUnit;
    }

    public void AssignAllTargets(HashSet<Unit> units)
    {
        List<Unit> toReassign = new();
        //foreach (Unit u in units)
        //{
        //    if (Brain.ResourceGroup.Contains(u)) continue;
        //    if (DefenderAndTargets.ContainsKey(u))
        //    {
        //        if (DefenderAndTargets[u].Type == _unitTargetRanking[u.Type][0]) continue;
        //        else
        //        {
        //            DefenderAndTargets.Remove(u);
        //            TargetAndDefenders[DefenderAndTargets[u]].Remove(u);
        //            toReassign.Add(u);
        //        }
        //    }
        //}
        //foreach (Unit u in toReassign)
        //{
        //    AssignTarget(u, Manager.PlayerUnits);
        //}
        TargetAndDefenders.Clear();
        DefenderAndTargets.Clear();
        foreach (Unit u in units)
        {
            if (Brain.ResourceGroup.Contains(u)) continue;
            AssignTarget(u, Manager.PlayerUnits);
        }
    }

    public Unit AssignTarget(Unit toAssign, HashSet<Unit> targets)
    {
        Dictionary<string, List<Unit>> sortedTargets = new()
        {
            {"Scout", new List<Unit>() },
            {"Knight", new List<Unit>() },
            {"Archer", new List<Unit>() }
        };

        foreach (Unit target in targets)
        {
            string targetType = target.Type;
            sortedTargets[targetType].Add(target);
        }
        string[] targetPref = _unitTargetRanking[toAssign.Type];

        for (int i = 0; i < 3; i++)
        {
            Unit tryAssign = GetClosestTarget(sortedTargets[targetPref[i]], toAssign.transform.position, 32);
            // tryAssign is target.
            if (tryAssign != null)
            {
                if (TargetAndDefenders.ContainsKey(tryAssign))
                {
                    TargetAndDefenders[tryAssign].Add(toAssign); ;
                }
                else
                {
                    TargetAndDefenders.Add(tryAssign, new List<Unit> { toAssign });
                    DefenderAndTargets.Add(toAssign, tryAssign);
                }
                return toAssign;
            }
        }
        return null;
    }
}