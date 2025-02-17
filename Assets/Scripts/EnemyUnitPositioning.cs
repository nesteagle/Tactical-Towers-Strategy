using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    public Game Manager;
    public EnemyBrain Brain;

    private string GetMovementDirection(float deltaX, float deltaY)
    {
        string state;
        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
        {
            if (deltaY > 0)
                state = deltaX > 0 ? "EN" : "WN";
            else state = deltaX > 0 ? "ES" : "WS";
        }
        else
        {
            if (deltaY > 0)
                state = deltaX > 0 ? "NE" : "NW";
            else state = deltaX > 0 ? "SE" : "SW";
        }
        return state;
    }

    private void AnalyzeMovement(Unit unit, Vector2 destination)
    {
        if (unit.Type == "Archer")
        {
            AnalyzeRangedMovement(unit, destination);
        }
        else
        {
            AnalyzeMeleeMovement(unit, destination);
        }
    }

    public void AnalyzeMeleeMovement(Unit unit, Vector2 destination)
    {
        HashSet<Unit> enemyUnits = UnitsAtPoint(destination, 2, "Enemy");
        if (DistanceFromEnemySpawn(unit) < 2f)
        {
            // defend base
        }
        else if (enemyUnits.Count > 0)
        {
            // attacking enemy units.
            // evaluate threat level with UnitsAtPoint(radius 4).Count to view reinforcements

            IsAttackingArcher(unit, enemyUnits);
        }
        else
        {
            (Village village, float distance) dist = DistanceFromControl(unit);
            if (dist.distance < 3.1f)
            {
                // going to village
                if (Brain.VillageAndScout.ContainsKey(dist.village))
                {
                    // going to currently scouting village - defend!
                }
                else
                {
                    // evaluate importance of village
                    // Evaluate village distance from enemy spawn and amount of troops to back.
                }
            }
            else
            {
                if (UnitsAtPoint(destination, 4, "Player").Count >= 1)
                {
                    // player massing at point
                }
                else
                {
                    // Check for weakness or mistake move (view reinforcements and evaluate)
                    // ELSE
                    // scouting an arbitrary point or flanking - send unit in x-direction of player unit movement.
                }
            }
        }
    }

    public void AnalyzeRangedMovement(Unit unit, Vector2 destination)
    {
        HashSet<Unit> units = UnitsAtPoint(destination, 9f, "Enemy");

        if (UnitsAtPoint(destination, 5, "Player").Count >= 1)
        {
            if (units.Count > 0)
            {
                // setting up to bombard, though player HAS REINFORCEMENTS
                // consider priority way to counter.
            }
            else
            {
                // player massing at point
            }
        }
        else
        {
            if (DistanceFromEnemySpawn(unit) < 9f)
            {
                // defend the base!
            }
            else
            {
                if (units.Count > 0)
                {
                    // send scout quickly
                }
                else
                {
                    // check for weakness.
                    // evaluate if flanking manuever or build-up
                }
            }
        }
    }
    private (Village Village, float Distance) DistanceFromControl(Unit unit)
    {
        float distance = int.MaxValue;
        Village v = null;
        foreach (Village village in Manager.TotalVillages)
        {
            Vector3 position = village.transform.position;
            float dist = (unit.transform.position - position).magnitude;
            if ((unit.transform.position - position).magnitude < distance)
            {
                distance = dist;
                v = village;
            }
        }
        return (v, distance);
    }
    private float DistanceFromEnemySpawn(Unit unit)
    {
        return Vector2.Distance(unit.transform.position, Manager.EnemySpawnCell.transform.position);
    }
    private HashSet<Unit> UnitsAtPoint(Vector2 position, float radius, string team)
    {
        HashSet<Unit> units = team == "Player" ? Manager.PlayerUnits : Manager.EnemyUnits;
        HashSet<Unit> unitsInRange = new();
        foreach (Unit u in units)
        {
            if (u == null) continue;
            if (Vector2.Distance(new Vector2(u.transform.position.x, u.transform.position.z), position) < radius)
            {
                unitsInRange.Add(u);
            }
        }
        return unitsInRange;
    }
    private void IsAttackingArcher(Unit unit, HashSet<Unit> units)
    {
        foreach (Unit u in units)
        {
            if (u.Type == "Archer")
            {
                // begin retreating archer

                HashSet<Unit> reinforcements = UnitsAtPoint(u.transform.position, 5, "Enemy");
                foreach (Unit reinforcement in reinforcements)
                {
                    // assign target to attacking player unit and move immediately!
                }
            }
        }
    }
}
