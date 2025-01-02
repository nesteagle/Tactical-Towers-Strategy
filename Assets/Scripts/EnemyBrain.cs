using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    // Start is called before the first frame update
    public EnemyUnitManagement UnitManagement;
    public Game Manager;
    private int _economyWeight;
    private int _militaryWeight;
    public List<HashSet<Unit>> Groups;
    // Groups is a list of lists of enemies.
    private List<HexCell> _currentlyScouting;
    // tandem with ResourceGroup.
    public HashSet<Unit> ResourceGroup = new();
    // ResourceGroup is list of scouts.

    private Dictionary<Village, Unit> _villages = new();

    //maybe predefine at start and then start Think();

    private Dictionary<string, List<Unit>> playerZoneUnits = new();
    private Dictionary<string, List<Unit>> enemyZoneUnits = new();

    private string _defensePriority = null;
    private Queue<string> _unitQueue = new();


    private IEnumerator Think()
    {
        yield return new WaitForFixedUpdate();
        Expand();

        playerZoneUnits = GetUnitDistribution("Player");
        enemyZoneUnits = GetUnitDistribution("Enemy");

        //CalculateEconomyWeight();
        //CalculateMilitaryWeight();

        //Debug.Log("ECONOMYWEIGHT" + _economyWeight);
        //Debug.Log("MILITARYWEIGHT" + _militaryWeight);

        //int randomValue = Random.Range(1, _economyWeight + _militaryWeight + 1);
        //Debug.Log(randomValue);
        //if (randomValue <= _economyWeight)
        //{
        //    Expand();
        //}
        //else
        //{
        //    //Military
        //    //train troops and set assign target groups using GetUnitGroups and GetNewComposition

        //}
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Expand();
            //BuildArmy();
            //StartCoroutine(Think());
        }
    }

    public void HandleDeath(Unit unit)
    {
        if (ResourceGroup.Contains(unit))
        {
            ResourceGroup.Remove(unit);
            // do something - maybe train another up.
        }
        else
        {
            foreach (HashSet<Unit> subgroup in Groups)
            {
                if (subgroup.Contains(unit))
                {
                    subgroup.Remove(unit);
                    // do something - maybe train another up.
                    // also reevaluate weights and composition
                }
            }
        }
    }

    private void ScoutVillage(Unit unit)
    {
        List<Village> villages = Manager.TotalVillages.OrderBy(v => Mathf.Abs(Vector2.Distance(v.transform.position, unit.transform.position))).ToList();
        foreach (Village v in villages)
        {
            if (v.Control <= -1f || v.Control > 0.9f) continue;
            if (_villages.ContainsKey(v)) continue;
            _villages.Add(v, unit);
            unit.MoveTo(v.Cell);
            return;
        }
    }
    private void Expand()
    {
        // If # of scouts < max, create one. Otherwise, send them to explore.
        if (!Manager.EnemySpawnCell.Occupied && ResourceGroup.Count < 5)
        {
            Unit newScout = Game.EnemySpawn.PlaceTroop("Scout");
            ResourceGroup.Add(newScout);
        }
        foreach (Village v in Manager.TotalVillages)
        {
            if (v.Control <= -1f) _villages.Remove(v);
        }
        foreach (Unit scout in ResourceGroup)
        {
            if (_villages.ContainsValue(scout)) continue;
            Debug.Log(scout.State);
            if (scout.State == "Rest") ScoutVillage(scout);
        }
    }

    private void BuildArmy()
    {
        if (Manager.PlayerUnits.Count == 0) // Attack threshold
        {
            if (Manager.EnemyCoins > 0) // and rateofgrowth > ? !!!
            {
                Unit newAttacker = Game.EnemySpawn.PlaceTroop("Knight"); // !!!
                // do by zone
            }
        }

        if (playerZoneUnits.Count > enemyZoneUnits.Count)
        {
            int midCount = enemyZoneUnits["Middle"].Count;
            int plrMidCount = playerZoneUnits["Middle"].Count;
            int midDiff = plrMidCount - midCount;

            int leftCount = enemyZoneUnits["Left"].Count;
            int plrLeftCount = playerZoneUnits["Left"].Count;
            int leftDiff = plrLeftCount - leftCount;

            int rightCount = enemyZoneUnits["Right"].Count;
            int plrRightCount = playerZoneUnits["Right"].Count;
            int rightDiff = plrRightCount - rightCount;

            int urgency = Mathf.Max(midDiff, rightDiff, leftDiff);

            switch (_defensePriority)
            {
                case null:
                    _defensePriority = midDiff > leftDiff ? midDiff > rightDiff ? "Middle" : "Right" : leftDiff > rightDiff ? "Left" : "Right";
                    break;
                case "Left":
                    if (leftDiff <= 0)
                    {
                        _defensePriority = rightDiff > midDiff ? "Right" : "Middle";
                    }
                    else if (leftCount == 0 || 2 * plrLeftCount > 3 * leftCount)
                    {
                        Composition missing = UnitManagement.GetMissingComposition(enemyZoneUnits["Left"], playerZoneUnits["Left"]);
                        _unitQueue = UnitManagement.CompositionToUnitQueue(missing);
                    }
                    break;
                case "Right":
                    if (rightDiff <= 0)
                    {
                        _defensePriority = leftDiff > midDiff ? "Left" : "Middle";
                    }
                    else if (midCount == 0 || 2 * plrMidCount > 3 * midCount)
                    {
                        Composition missing = UnitManagement.GetMissingComposition(enemyZoneUnits["Middle"], playerZoneUnits["Middle"]);
                        _unitQueue = UnitManagement.CompositionToUnitQueue(missing);
                    }
                    break;
                case "Middle":
                    if (midDiff <= 0) // maybe -1 for outnumbering? consider when testing !!!
                    {
                        _defensePriority = leftDiff > rightDiff ? "Left" : "Right";
                    }
                    else if (rightCount == 0 || 2 * plrRightCount > 3 * rightCount)
                    {
                        Composition missing = UnitManagement.GetMissingComposition(enemyZoneUnits["Right"], playerZoneUnits["Right"]);
                        _unitQueue = UnitManagement.CompositionToUnitQueue(missing);
                    }
                    break;
            }
            Unit unit = Game.EnemySpawn.PlaceTroop(_unitQueue.Peek());
            StartCoroutine(MoveWhenSpawned(unit)); // MoveUnitOffZone
            _unitQueue.Dequeue();
        }
        // WEIGH MOST IMPORTANT SECTIONS and defend 2/3 of them.


        // other use of resources - build army to attack player or defend.
        // PURPOSE: create group to counter player group or attack.

        // check player strength and if existing groups
        // THEN: If group, make counter group
        //       if no group, train group of (knights?) to attack
    }
    private IEnumerator MoveWhenSpawned(Unit unit)
    {
        yield return new WaitUntil(() => unit.State == "Rest");
        if (Random.Range(0, 2) == 0)
        {
            int targetX = Manager.EnemySpawnPos.x;
            int targetY = Manager.EnemySpawnPos.y - 2;
            unit.MoveTo(Game.Map.ReturnHex(targetX, targetY));
        }
        else
        {
            int targetX = Manager.EnemySpawnPos.x + 2;
            int targetY = Manager.EnemySpawnPos.y - 2;
            unit.MoveTo(Game.Map.ReturnHex(targetX, targetY));
        }
    }
    private void UpdateGroupPurpose()
    {
        // several cases:
        // evaluate player strength and decide to attack or defend.
        // evaluate player position and decide next movement.
        // special cases: 
        // player near control point
        // player massing troops in back

        // Group has State "Attack" "Defend"
        // Group has HexCell Target - target position
    }
    public Dictionary<string, List<Unit>> GetUnitDistribution(string team)
    {
        Dictionary<string, List<Unit>> zoneDistribution = new()
    {
        { "Left", new List<Unit>() },
        { "Middle", new List<Unit>() },
        { "Right", new List<Unit>() }
    };
        // Directions are oriented to player view.

        float leftBound = -7f * HexData.InnerRadius;
        float rightBound = 7f * HexData.InnerRadius;

        foreach (Unit unit in team == "Player" ? Manager.PlayerUnits : Manager.EnemyUnits)
        {
            if (ResourceGroup.Contains(unit)) continue;

            // Horizontal Zones (Left, Middle, Right)
            string zone = unit.transform.position.x < leftBound ? "Left" :
                                    unit.transform.position.x > rightBound ? "Right" : "Middle";
            string zoneKey = zone;
            zoneDistribution[zoneKey].Add(unit);
        }

        return zoneDistribution;
    }

    public (string State, float Score) EvaluateZone(string zone)
    {
        // Zone must be one of: "Left", "Middle", "Right"
        List<Unit> playerZoneUnits = GetUnitDistribution("Player")[zone];
        List<Unit> enemyZoneUnits = GetUnitDistribution("Enemy")[zone];

        float averageY = 0f;
        if (playerZoneUnits.Count == 0 || enemyZoneUnits.Count / playerZoneUnits.Count > 3) return ("ATTACK", 5f); // adjust weight system?
        foreach (Unit u in playerZoneUnits)
        {
            float y = u.transform.position.y;
            if (y > 6 * HexData.InnerRadius)
            {
                return ("DEFEND", 100f); // change to more creative name - currently representing enemy incursion
            }
            averageY += y;
        }
        if (averageY / playerZoneUnits.Count <= -12 * HexData.InnerRadius) // CONSTANT !!!
        {
            return ("BUILDUP", playerZoneUnits.Count); // Player is building up troops in the back.
        }
        return ("NORMAL", playerZoneUnits.Count);
    }
}
