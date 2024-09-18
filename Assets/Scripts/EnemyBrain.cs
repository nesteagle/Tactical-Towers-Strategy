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
    private int _explorationWeight = 15; //based on positioning of units
    

    //maybe predefine at start and then start Think();
    // Update is called once per frame
    private IEnumerator Think()
    {
        yield return new WaitForEndOfFrame();

        CalculateEconomyWeight();
        CalculateMilitaryWeight();

        Debug.Log("ECONOMYWEIGHT"+_economyWeight);
        Debug.Log("MILITARYWEIGHT"+_militaryWeight);
        Debug.Log("EXPLORATIONWEIGHT"+_explorationWeight);

        int randomValue = Random.Range(1,_economyWeight+_militaryWeight+_explorationWeight+1);
        Debug.Log(randomValue);
        if (randomValue <= _economyWeight)
        {
           //defend current villages
           //or attack with military escort to opponent's village
           //Economy
        }
        else if (randomValue <= _economyWeight + _militaryWeight)
        {
            //Military
            //train troops and set assign target groups using GetUnitGroups and GetNewComposition
        }
        else
        {
            //Exploration
            Debug.Log("Exploration");
            if (Manager.EnemyEnemies.Count > 0)
            {
                ScoutVillage(Manager.EnemyEnemies[0]);
                //will be sorted based on enemy "squad", if they are assigned to a group they will not be called.
            } else
            Manager.EnemySpawn.PlaceTroop("Scout");
            //add feature for troop destination.
            //spawn scout and go to villages
        }
    }
    private void ScoutVillage(Enemy scout) {
        float minDistance = Mathf.Infinity;
        float d = 0;
        HexCell cell=null;
        foreach(HexCell c in Manager.TotalVillages)
        {
            d = Mathf.Abs(Vector2.Distance(c.transform.position, scout.transform.position));
            if (d < minDistance
                && Mathf.Abs(c.GetComponentInChildren<Village>().Control) <= 0.9f)
            {
                minDistance = d;
                cell = c;
            }
        }
        if (cell == null) return;
        scout.FollowPath(cell.Position.x, cell.Position.y);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Think());
        }
    }
    private void CalculateEconomyWeight() {
        _economyWeight = Mathf.Abs((Manager.PlayerVillages.Count - Manager.EnemyVillages.Count) * 15);
    }
    private void CalculateMilitaryWeight() {
        float score=0;
        foreach (float f in UnitManagement.GetScores()){
            score += f;
        }
        _militaryWeight = Mathf.RoundToInt(score*1.3f);
    }
}
