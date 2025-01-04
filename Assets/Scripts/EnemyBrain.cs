using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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

    private Dictionary<string, List<Unit>> _playerZoneUnits = new();
    private Dictionary<string, List<Unit>> _enemyZoneUnits = new();

    private Dictionary<Unit, Unit> _unitAndTargets = new();
    // Key is unit, Value is target unit.

    private string _defensePriority = null;
    private Queue<string> _unitQueue = new();


    private IEnumerator Think()
    {
        yield return new WaitForFixedUpdate();
        Expand();

        _playerZoneUnits = GetUnitDistribution("Player");
        _enemyZoneUnits = GetUnitDistribution("Enemy");

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
            //Expand();
            BuildArmy();
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
        _playerZoneUnits = GetUnitDistribution("Player");
        _enemyZoneUnits = GetUnitDistribution("Enemy");

        if (Manager.PlayerUnits.Count == 0) // Attack threshold
        {
            Debug.Log("CASE WEAK");
            if (Manager.EnemyCoins > 0) // and rateofgrowth > ? !!!
            {
                Unit newAttacker = Game.EnemySpawn.PlaceTroop("Knight"); // !!!
                // do by zone
            }
        }
        if (Manager.PlayerUnits.Count > Manager.EnemyUnits.Count-ResourceGroup.Count)
        {
            int midCount = _enemyZoneUnits["Middle"].Count;
            int plrMidCount = _playerZoneUnits["Middle"].Count;
            int midDiff = plrMidCount - midCount;

            int leftCount = _enemyZoneUnits["Left"].Count;
            int plrLeftCount = _playerZoneUnits["Left"].Count;
            int leftDiff = plrLeftCount - leftCount;

            int rightCount = _enemyZoneUnits["Right"].Count;
            int plrRightCount = _playerZoneUnits["Right"].Count;
            int rightDiff = plrRightCount - rightCount;

            switch (_defensePriority)
            {
                case null:
                    Debug.Log("null, initializing def_pri");
                    _defensePriority = midDiff > leftDiff ? midDiff > rightDiff ? "Middle" : "Right" : leftDiff > rightDiff ? "Left" : "Right";
                    break;
                case "Left":
                    Debug.Log(leftDiff);
                    if (leftDiff <= 0)
                    {
                        Debug.Log("left neutralized.");
                        _defensePriority = rightDiff > midDiff ? "Right" : "Middle";
                    }
                    else if (2 * plrLeftCount > 3 * leftCount)
                    {
                        Composition missing = UnitManagement.GetMissingComposition(_enemyZoneUnits["Left"], _playerZoneUnits["Left"]);
                        _unitQueue = UnitManagement.CompositionToUnitQueue(missing);
                    }
                    break;
                case "Right":
                    Debug.Log(rightDiff);
                    if (rightDiff <= 0)
                    {
                        Debug.Log("right neutralized.");
                        _defensePriority = leftDiff > midDiff ? "Left" : "Middle";
                    }
                    else if (2 * plrRightCount > 3 * rightCount)
                    {
                        Composition missing = UnitManagement.GetMissingComposition(_enemyZoneUnits["Right"], _playerZoneUnits["Right"]);
                        _unitQueue = UnitManagement.CompositionToUnitQueue(missing);
                    }
                    break;
                case "Middle":
                    Debug.Log(midDiff);
                    if (midDiff <= 0) // maybe -1 for outnumbering? consider when testing !!!
                    {
                        Debug.Log("mid neutralized.");
                        _defensePriority = leftDiff > rightDiff ? "Left" : "Right";
                    }
                    else if (2 * plrMidCount > 3 * midCount)
                    {
                        Composition missing = UnitManagement.GetMissingComposition(_enemyZoneUnits["Middle"], _playerZoneUnits["Middle"]);
                        _unitQueue = UnitManagement.CompositionToUnitQueue(missing);
                    }
                    break;
            }
            Debug.Log(_defensePriority);

            if (_unitQueue.Count > 0)
            {
                Debug.Log(_unitQueue.Peek());
                Unit unit = Game.EnemySpawn.PlaceTroop(_unitQueue.Peek());
                StartCoroutine(MoveWhenSpawned(unit));

                // assign target?

                _unitQueue.Dequeue();
            }
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

        foreach (Unit unit in team == "Player" ? Manager.PlayerUnits : Manager.EnemyUnits)
        {
            if (ResourceGroup.Contains(unit)) continue;

            string zoneKey = GetZone(unit.transform.position.x);
            zoneDistribution[zoneKey].Add(unit);
        }

        return zoneDistribution;
    }

    private string GetZone(float x)
    {
        float leftBound = -7f * HexData.InnerRadius;
        float rightBound = 7f * HexData.InnerRadius;

        // Horizontal Zones (Left, Middle, Right)
        string zone = x < leftBound ? "Left" :
                        x > rightBound ? "Right" : "Middle";
        return zone;
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
    public string CalculatePlayerMovement(Unit unit, HexCell to) 
    {
        Vector3 deltaPos = to.Position - unit.transform.position;
        string resultingZone = GetZone(to.Position.x);

        // get closest unit in _enemyZoneUnits[resultingZone], and to.Position

        string playerAction = "";
        if (Math.Abs(deltaPos.y) > Math.Abs(deltaPos.x))
        {
            if (deltaPos.y > 0)
            {
                playerAction = "Advance";
            }
            else playerAction = "Retreat";
        }
        else playerAction = "Flank";
        return playerAction;
    }

    private Unit GetClosestUnit(List<Unit> units, Vector3 position)
    {
        float maxDistance = int.MaxValue;
        Unit closestUnit = null;
        foreach (Unit u in units)
        {
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
