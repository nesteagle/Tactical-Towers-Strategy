using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    // Start is called before the first frame update
    public EnemyUnitManagement UnitManagement;
    public Game Manager;
    private int _economyWeight;
    private int _militaryWeight;
    public List<List<Unit>> Groups;
    // Groups is a list of lists of enemies.
    private List<HexCell> _currentlyScouting;
    // tandem with ResourceGroup.
    public List<Unit> ResourceGroup;
    // ResourceGroup is list of scouts.

    //maybe predefine at start and then start Think();
    // Update is called once per frame
    private IEnumerator Think()
    {
        yield return new WaitForEndOfFrame();

        CalculateEconomyWeight();
        CalculateMilitaryWeight();

        Debug.Log("ECONOMYWEIGHT" + _economyWeight);
        Debug.Log("MILITARYWEIGHT" + _militaryWeight);

        int randomValue = Random.Range(1, _economyWeight + _militaryWeight + 1);
        Debug.Log(randomValue);
        if (randomValue <= _economyWeight)
        {
            ManageScouts();
        }
        else
        {
            //Military
            //train troops and set assign target groups using GetUnitGroups and GetNewComposition

        }
        //else
        //{
        //    //Exploration
        //    Debug.Log("Exploration");
        //    if (Manager.EnemyEnemies.Count > 0)
        //    {
        //        ScoutVillage(Manager.EnemyEnemies[0]);
        //        //will be sorted based on enemy "squad", if they are assigned to a group they will not be called.
        //    }
        //    else
        //        Manager.EnemySpawn.PlaceTroop("Scout");
        //    //add feature for troop destination.
        //    //spawn scout and go to villages
        //}
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Think());
        }
    }
    private void CalculateEconomyWeight()
    {
        _economyWeight = Mathf.Abs((Manager.PlayerVillages.Count - Manager.EnemyVillages.Count) * 15);
    }
    private void CalculateMilitaryWeight()
    {
        float score = 0;
        foreach (float f in UnitManagement.GetScores())
        {
            score += f;
        }
        _militaryWeight = Mathf.RoundToInt(score * 1.3f);
    }
    private void ManageScouts()
    {
        // If # of scouts < max, create one. Otherwise, send them to explore.
        if (ResourceGroup.Count > 0)
        {
            //foreach (Unit e in ResourceGroup)
            //{
            //    if (!e.Moving)
            //    { }
            //}
        }
        else
        {
            if (ResourceGroup.Count < 3) // where 3 is max amt.
            {
                // Game.EnemySpawn.PlaceTroop("Scout");

            }
        }
    }
    private void ScoutVillage(Unit scout)
    {
        float minDistance = Mathf.Infinity;
        float d = 0;
        HexCell cell = null;
        foreach (HexCell c in Manager.TotalVillages)
        {
            if (_currentlyScouting.Contains(c)) continue;
            d = Mathf.Abs(Vector2.Distance(c.transform.position, scout.transform.position));
            if (d < minDistance
                && Mathf.Abs(c.GetComponentInChildren<Village>().Control) <= 0.9f)
            {
                minDistance = d;
                cell = c;
            }
        }
        if (cell == null) return;
        _currentlyScouting.Clear();
        _currentlyScouting.Add(cell);
        scout.MoveTo(cell);
    }
}
