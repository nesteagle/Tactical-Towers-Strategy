//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PathDrawer : MonoBehaviour
//{
//    public bool Drawing = false;
//    public List<HexCell> PathDrawn = new();
//    private List<HexCell> _hexPath = new();
//    public Enemy SelectedTroop = null;
//    private void Start()
//    {
//        StartCoroutine(WaitForSelectedEnemy());
//    }
//    private IEnumerator WaitForSelectedEnemy()
//    {
//        while (Drawing == false)
//        {
//            yield return new WaitForEndOfFrame();
//            if (Input.GetMouseButton(0))
//            {
//                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
//                if (hit.collider == null) continue;
//                if (hit.collider.gameObject.GetComponent<Enemy>())
//                {
//                    SelectedTroop = hit.collider.gameObject.GetComponent<Enemy>();
//                    Drawing = true;
//                    StartCoroutine(DrawPath());
//                }
//            }
//        }
//    }
//    private IEnumerator DrawPath()
//    {
//        Vector2Int cellPos = new();
//        while (Drawing == true)
//        {
//            if (!Input.GetMouseButton(0))
//            {
//                SelectedTroop.FollowPath(cellPos.x, cellPos.y);
//                Drawing = false;
//                yield return new WaitUntil(() => SelectedTroop.Position == new Vector2(cellPos.x, cellPos.y));
//                StartCoroutine(WaitForSelectedEnemy());
//                yield break;
//            }
//            yield return new WaitForEndOfFrame();
//            if (Input.GetMouseButton(0))
//            {
//                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
//                if (hit.collider == null) continue;
//                HexCell cell;
//                if (hit.collider.gameObject.GetComponent<HexCell>())
//                {
//                    cell = hit.collider.gameObject.GetComponent<HexCell>();
//                }
//                else continue;
//                if (_hexPath.Count > 0)
//                {
//                    foreach (HexCell c in _hexPath)
//                    {
//                        if (c.TerrainType=="Forest") c.GetComponent<SpriteRenderer>().color = Color.green;
//                        else if (c.TerrainType=="Mountain") c.GetComponent<SpriteRenderer>().color = Color.cyan;
//                        else if (c.TerrainType=="Spawn") c.GetComponent<SpriteRenderer>().color = Color.yellow;
//                        else c.GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.9f, 0.9f);
//                    }
//                }
//                Pathfind(new Vector2Int(SelectedTroop.Position.x, SelectedTroop.Position.y), cell.Position);
//                cellPos = new Vector2Int(cell.Position.x, cell.Position.y);
//            }
//        }
//    }
//    private void Pathfind(Vector2Int position, Vector3Int clickedPos)
//    {
//        HexMap hexMap = GameObject.Find("HexGrid").GetComponent<HexMap>();
//        _hexPath = Pathfinding.FindPath(hexMap.ReturnHex(position.x, position.y), hexMap.ReturnHex(clickedPos.x, clickedPos.y));
//        if (_hexPath.Count != 0)
//        {
//            List<Vector3> pathToFollow = new();
//            foreach (HexCell c in _hexPath)
//            {
//                pathToFollow.Add(new Vector3(c.gameObject.transform.position.x, c.gameObject.transform.position.y, 1));
//                c.GetComponent<SpriteRenderer>().color = Color.red;
//            }
//        }
//    }
//}
