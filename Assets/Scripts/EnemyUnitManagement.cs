using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnitManagement : MonoBehaviour
{
    // Start is called before the first frame update
    public Game Manager;
    private List<Enemy> _playerTroops=new();
    private List<Vector2> _playerTroopPositions = new();
    private float _scoreLeft;
    private float _scoreCenter;
    public GameObject TempRenderObject;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetUnitGroups();
            Debug.Log("DONE");
        }
    }
    private List<Enemy> CheckPlayerTroops()
    {
        List<Enemy> playerTroops=new();
        foreach(Enemy enemy in Manager.Enemies)
        {
            if (enemy.OnPlayerTeam == true)
            {
                if (!enemy) continue;
                playerTroops.Add(enemy);
            }
        }
        return playerTroops;
    }
    private List<Vector2> CheckPlayerPositions()
    {
        List<Vector2> playerPositions = new();
        List<Enemy> enemies = CheckPlayerTroops();  
        foreach (Enemy enemy in enemies)
        {
            playerPositions.Add(enemy.transform.position);
        }
        return playerPositions;
    }
    private List<float> GetScores()
    {
        List<Enemy> enemies = CheckPlayerTroops();
        List<Vector2> positions = CheckPlayerPositions();
        List<float> scores=new();
        for (int i = 0; i < enemies.Count; i++)
        {
            scores.Add((positions[i].y + 15f) * 0.12f);
        }
        return scores;
    }
    private void RenderScores()
    {
        List<Enemy> enemies = CheckPlayerTroops();
        List<float> scores = GetScores();
        for (int i = 0; i < enemies.Count; i++)
        {
            Debug.Log(scores[i]);
            GameObject render = Instantiate(TempRenderObject);
            render.transform.position = enemies[i].transform.position;
            render.transform.localScale = new Vector2(scores[i], scores[i]);
        }
    }
    private void GetUnitGroups()
    {
        List<Enemy> enemies = CheckPlayerTroops();
        List<float> scores = GetScores();
        List<Vector2> positions = CheckPlayerPositions();
        Dictionary<Enemy, int> enemyDensity = new();
        List<Enemy> collided = new();

        foreach (Enemy e in enemies)
        {
            int count = 0;
            Collider2D[] hits = Physics2D.OverlapCircleAll(e.transform.position, HexData.InnerRadius * 4f);
            foreach (Collider2D hit in hits)
            {
                GameObject enemyObject = hit.gameObject;
                if (hit.gameObject.CompareTag("Enemy"))
                {
                    Enemy enemy = enemyObject.GetComponent<Enemy>();

                    if (enemy.OnPlayerTeam == true)
                    {
                        if (collided.Contains(enemy) == false)
                        {
                            collided.Add(enemy);
                            count++;
                        }
                    }
                }
            }
            if (count == 0) continue;
            enemyDensity.Add(e, count);
        }

        //feature for removing duplicate groups
        //for (int i = 1; i < positions.Count; i++)
        //{
        //    if (Vector2.Distance(positions[i], positions[i - 1]) < HexData.CellDiameter)
        //    {
        //        enemyDensity.Remove(enemies[i-1]);
        //    }
        //}
        
        foreach (KeyValuePair<Enemy, int> pair in enemyDensity)
        {
            Debug.Log(pair.Key);
            Debug.Log(pair.Value);
        }

        //make it so that group score is slightly higher based on how many units in group
    }
}
