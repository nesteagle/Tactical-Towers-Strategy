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

    private void AnalyzeMovement(Vector3 deltaPos)
    {
        string state = GetMovementDirection(deltaPos.x, deltaPos.z);
        switch (state)
        {
            case "NE":
                Debug.Log("Moving NE");
                break;
            case "NW":
                Debug.Log("Moving NW");
                break;
            case "SE":
                Debug.Log("Moving SE");
                break;
            case "SW":
                Debug.Log("Moving SW");
                break;
            case "EN":
                Debug.Log("Moving EN");
                break;
            case "ES":
                Debug.Log("Moving ES");
                break;
            case "WN":
                Debug.Log("Moving WN");
                break;
            case "WS":
                Debug.Log("Moving WS");
                break;
        }
    }
    public void AnalyzeScoutMovement(Unit unit, Vector2 destination)
    {
        if (UnitsAtPoint(destination, 4, "Player").Count >= 1)
        {
            // player massing at point
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
                }
            }
            else
            {
                // scouting an arbitrary point
            }
        }

    }

    public void AnalyzeKnightMovement(Unit unit, Vector2 destination)
    {
        if (UnitsAtPoint(destination, 4, "Player").Count >= 1)
        {
            // player massing at point
        }
        else
        {
            HashSet<Unit> units = UnitsAtPoint(destination, 2, "Enemy");
            if (units.Count > 0)
            {
                // attacking enemy units.
                // evaluate threat level with UnitsAtPoint(radius 4).Count to view reinforcements

                foreach (Unit u in units)
                {
                    if (u.Type == "Archer")
                    {
                        // archer under threat
                    }
                }
            }
            else
            {
                // evaluate if flanking manuever or build-up
            }
        }
    }
    public void AnalyzeArcherMovement(Unit unit, Vector2 destination)
    {
        HashSet<Unit> units = UnitsAtPoint(destination, 9f, "Enemy");

        if (UnitsAtPoint(destination, 5, "Player").Count >= 1)
        {
            if (units.Count > 0)
            {
                // setting up to bombard, though WITH REINFORCEMENTS
                // consider priority way to counter.
            } else
            {
                // player massing at point
            }
        }
        else
        {
            if (units.Count > 0)
            {
                // send scout quickly
            }
            else
            {
                // evaluate if flanking manuever or build-up
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
}
