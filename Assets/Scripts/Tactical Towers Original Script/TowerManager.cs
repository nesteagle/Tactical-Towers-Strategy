using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TowerManager : MonoBehaviour
{
    public GameObject[] TowerPrefabs;
    public string ObtainedTower;
    public bool Replacing = false;
    private ButtonControl _menuControl;
    private GameObject _rangeIndicator;
    private HexMap _map;
    private HexCell _selected = null;
    private float _radius = 2f * HexData.InnerRadius;
    private HashSet<int> _triggerWaves = new() { 2, 4, 6, 9, 11, 14, 16, 19 };
    private void Awake()
    {
        _menuControl = GetComponent<ButtonControl>();
        _map = GameObject.Find("HexGrid").GetComponent<HexMap>();
        _rangeIndicator = GameObject.Find("RangeIndicator");
        _rangeIndicator.SetActive(false);
        GameControl.StartWave.AddListener(HandleTowerRewards);
    }
    public IEnumerator PreviewTower(int selected, HexCell cell) {
        _menuControl.TowerConfirmationCanvas.enabled = true;
        GameObject tower = Instantiate(TowerPrefabs[selected], cell.transform);
        cell.Tower = tower.GetComponent<Tower>();
        _menuControl.TowerConfirmButtons.GetComponentInChildren<Text>().text = "Placing: " +cell.Tower.TowerType+" Tower";
        if (selected != 0) StartCoroutine(ShowRange(cell));
        yield return new WaitUntil(()=>_menuControl.Previewing == false);
        _menuControl.TowerConfirmationCanvas.enabled = false;
        Destroy(tower);
    }
    public IEnumerator ShowRange(HexCell cell)
    {
        float i = 0f;
        if (cell.Tower == null|| _selected == cell) {
            StartCoroutine(SetRangeInactive());
            yield break;
        }
        _selected = cell;
        _rangeIndicator.SetActive(true);
        _rangeIndicator.transform.position = new Vector3(cell.gameObject.transform.position.x, cell.gameObject.transform.position.y, 1);
        _rangeIndicator.transform.localScale = new Vector2(0, 0);
        while (i < 1f)
        {
            i += Time.deltaTime * 6;
            _rangeIndicator.transform.localScale = new Vector2(i * GetRange(cell.Tower.Range), i * GetRange(cell.Tower.Range));
            yield return new WaitForEndOfFrame();
        }

    }
    private IEnumerator SetRangeInactive()
    {
        float i = 1f;
        while (i > 0.1f)
        {
            if (_selected == null) yield break;
            i -= Time.deltaTime * 6;
            if (_selected.Tower != null)
            {
                _rangeIndicator.transform.localScale = new Vector2(i * GetRange(_selected.Tower.Range), i * GetRange(_selected.Tower.Range));
            }
            else break;
            yield return new WaitForEndOfFrame();
        }
        _selected = null;
        _rangeIndicator.SetActive(false);
    }
    public void PlaceTower(int selected, HexCell cell)
    {
        int index = CheckTowers(TowerPrefabs[selected].name);
        if (index == -1|| Currency.Towers[index].Item2 < 1) return;
        if (Replacing == true)
        {
            RemoveTower(cell);
            Replacing = false;
        }
        if (CheckPlaceable(cell) == true)
        {
            Currency.Towers[CheckTowers(TowerPrefabs[selected].name)].Item2--;
            GameObject tower = Instantiate(TowerPrefabs[selected], cell.transform);
            Tower towerComponent = tower.GetComponent<Tower>();
            cell.Occupied = true;
            cell.Tower = towerComponent;
        }
    }
    public void RemoveTower(HexCell cell)
    {
        _selected = cell;
        StartCoroutine(SetRangeInactive());
        Tower tower = cell.GetComponentInChildren<Tower>();
        Currency.Towers[CheckTowers(tower.TowerType + " Tower")].Item2++;
        Destroy(tower.gameObject);
        cell.Tower = null;
        cell.Occupied = false;
    }
    private float GetRange(float hexes) {
        if (hexes <= 0) return 0;
        return _radius + (hexes* 2f* _radius);//Diameter=2 radius
    }
    private int CheckTowers(string tower)
    {
        switch (tower)
        {
            case "Wall Tower":
                return 0;
            case "Cannon Tower":
                return 1;
            case "Archer Tower":
                return 2;
            case "Fire Tower":
                return 3;
            case "Tesla Tower":
                return 4;
            default:
                return -1;
        }
    }
    bool CheckPlaceable(HexCell cell)
    {
        cell.Occupied = true;
        List<HexCell> hexPath = Pathfinding.FindPath(_map.ReturnHex(-3, 7), _map.ReturnHex(3, -6));
        List<HexCell> hexPathAlternate = Pathfinding.FindPath(_map.ReturnHex(-4, 7), _map.ReturnHex(3, -6));
        cell.Occupied = false;
        if (hexPath.Count > 0&& hexPathAlternate.Count > 0) return true;
        return false;
    }
    private void HandleTowerRewards()
    {
        StartCoroutine(CheckForRewards());
    }
    private IEnumerator CheckForRewards()
    {
        yield return new WaitUntil(() => GameControl.InWave == false);
        yield return new WaitForSeconds(0.1f);
        switch (GameControl.CurrentWave)
        {
            case 2:
                    Currency.Towers[1].Item2++;
                    ObtainedTower = "Cannon Tower";
                    break;
            case 4:
                    Currency.Towers[0].Item2 += 5;
                    Currency.Towers[1].Item2++;
                    break;
            case 6:
                    Currency.Towers[1].Item2++;
                    break;
            case 9:
                    Currency.Towers[0].Item2 += 5;
                    Currency.Towers[2].Item2++;
                    ObtainedTower = "Archer Tower";
                    break;
            case 11:
                    Currency.Towers[2].Item2++;
                    break;
            case 14:
                    Currency.Towers[0].Item2 += 5;
                    Currency.Towers[3].Item2++;
                    ObtainedTower = "Fire Tower";
                    break;
            case 16:
                    Currency.Towers[3].Item2++;
                    break;
            case 19:
                    Currency.Towers[4].Item2++;
                    Currency.Towers[0].Item2 += 5;
                    ObtainedTower = "Tesla Tower";
                    break;
        }
        if (_triggerWaves.Contains(GameControl.CurrentWave)) _menuControl.OpenTowerObtainMenu();
    }
    //ADD METHODS RELATING TO TOWERS HERE. (IE UPGRADING, SELECTING, ETC.)
}
