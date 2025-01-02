using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Composition
{
    // Composition is army composition, consists of 3 integers representing the number of each unit type.
    public int Scouts;
    public int Knights;
    public int Archers;
    public Composition(int scouts, int knights, int archers)
    {
        Scouts = scouts; Knights = knights; Archers = archers;
    }
}

public class EnemyUnitManagement : MonoBehaviour
{
    // Start is called before the first frame update
    public Game Manager;
    private List<Unit> _playerTroops = new();
    private List<Vector2> _playerTroopPositions = new();
    public GameObject TempRenderObject;

    public Composition GroupToComposition(List<Unit> group)
    {
        int scoutCount = 0;
        int knightCount = 0;
        int archerCount = 0;

        foreach (Unit unit in group)
        {
            switch (unit.Type)
            {
                case "Archer":
                    archerCount++;
                    break;
                case "Knight":
                    knightCount++;
                    break;
                case "Scout":
                    scoutCount++;
                    break;
            }
        }
        return new Composition(scoutCount, knightCount, archerCount);
    }
    public Composition GetNewComposition(List<Unit> group)
    {
        Composition composition = GroupToComposition(group);

        const float archerToKnightRatio = 2.5f;
        const float knightMultiplier = 1.5f;
        const float scoutMultiplier = 1.5f;

        int archerCount = Mathf.FloorToInt(composition.Knights / archerToKnightRatio);
        int knightCount = Mathf.FloorToInt(composition.Knights * knightMultiplier);
        int scoutCount = Mathf.FloorToInt(composition.Archers * scoutMultiplier);

        if (composition.Scouts >= 2)
        {
            knightCount += Mathf.FloorToInt(composition.Scouts * knightMultiplier);
        }
        else
        {
            scoutCount += composition.Scouts;
        }
        return new Composition(scoutCount, knightCount, archerCount);
    }
    public Queue<string> CompositionToUnitQueue(Composition comp)
    {
        Queue<string> unitQueue = new();
        for (int i = 0; i < comp.Knights; i++)
        {
            unitQueue.Enqueue("Knight");
        };
        for (int i = 0; i < comp.Scouts; i++)
        {
            unitQueue.Enqueue("Scout");
        };
        for (int i = 0; i < comp.Archers; i++)
        {
            unitQueue.Enqueue("Archer");
        };
        return unitQueue;
    }
    public Composition GetMissingComposition(List<Unit> currentComposition, List<Unit> totalComposition)
    {
        Composition total = GroupToComposition(totalComposition);
        Composition curr = GroupToComposition(currentComposition);
        return new Composition(total.Scouts - curr.Scouts, total.Knights - curr.Knights, total.Archers - curr.Archers);
    }
}
