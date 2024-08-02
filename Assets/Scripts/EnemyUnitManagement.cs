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
    //public GameObject GroupDetectionObject;
    //private GameObject _detectionObject;
    //private Collider2D _detection;
    void Start()
    {
        //_detectionObject = Instantiate(GroupDetectionObject);
        //_detection = _detectionObject.GetComponent<Collider2D>();
    }

    // Update is called once per frame
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
    //private void GetAreaScore()
    //{
    //    List<Enemy> enemies = CheckPlayerTroops();
    //    List<float> scores = GetScores();
    //    float x = 0;
    //    float y = 0;
    //    float score = 0;
    //    for (int i = 0; i < enemies.Count; i++)
    //    {
    //        x += enemies[i].transform.position.x;
    //        y += enemies[i].transform.position.y;
    //        score += scores[i];
    //    }
    //    GameObject render = Instantiate(TempRenderObject);
    //    render.transform.position = new Vector2(x / enemies.Count, y / enemies.Count);//average position between
    //    render.transform.localScale = new Vector2(score / scores.Count, score / scores.Count);
    //}
    private void GetUnitGroups()
    {
        List<Enemy> enemies = CheckPlayerTroops();
        List<float> scores = GetScores();
        Dictionary<Enemy, int> enemyDensity = new();
        foreach (Enemy e in enemies)
        {
            int count = 0;
            Collider2D[] hits = Physics2D.OverlapCircleAll(e.transform.position, HexData.InnerRadius * 6f);
            foreach (Collider2D hit in hits)
            {
                GameObject enemy = hit.gameObject;
                if (hit.gameObject.CompareTag("Enemy"))
                {
                    if (enemy.GetComponent<Enemy>().OnPlayerTeam == true)
                    {
                        count++;
                    }
                }
            }
            enemyDensity.Add(e, count);
        }

        //feature for removing duplicate groups

        foreach (KeyValuePair<Enemy, int> pair in enemyDensity)
        {
            Debug.Log(pair.Key);
            Debug.Log(pair.Value);
        }
    }
}
