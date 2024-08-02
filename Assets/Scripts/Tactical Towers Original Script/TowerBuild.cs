using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class TowerBuild : MonoBehaviour
{
    public Canvas TowerOptionsCanvas;
    public Canvas BuildTowerCanvas;
    public RectTransform OptionsCanvasTransform;
    public RectTransform BuildCanvasTransform;
    public HexCell HitCell=null;
    private TowerManager _manager;
    private ButtonControl _buttonControl;
    private RaycastHit2D _hit;
    private void Awake()
    {
        _manager = GetComponent<TowerManager>();
        _buttonControl = GetComponent<ButtonControl>();
        ResetMenu();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)&&GameControl.InWave==false)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            _hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            ResetMenu();
            _manager.Replacing = false;
            if (!_hit.collider|| !_hit.collider.gameObject.GetComponent<HexCell>()) return;
            if (_buttonControl.Previewing == true) _buttonControl.Previewing = false;
            _buttonControl.UpdateText();
            HitCell = _hit.collider.gameObject.GetComponent<HexCell>();
            Vector3 screenPos;
            HitCell.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f);
            if (HitCell.Tower != null)
            {
                TowerOptionsCanvas.enabled = true;
                if (_hit.transform.position.x > 5.5f) screenPos = HitCell.transform.position - new Vector3(2.5f, 0);//Camera.main.WorldToScreenPoint
                else screenPos = HitCell.transform.position + new Vector3(2.5f, 0); //Camera.main.WorldToScreenPoint
                OptionsCanvasTransform.position = screenPos;
            }
            else
            {
                BuildTowerCanvas.enabled = true;
                screenPos = HitCell.transform.position;
                //if (_hit.transform.position.x > 2f) screenPos = HitCell.transform.position - new Vector3(3.33f, 0);//Camera.main.WorldToScreenPoint
                //else screenPos = HitCell.transform.position + new Vector3(3.33f, 0); //Camera.main.WorldToScreenPoint
                if (_hit.transform.position.y < 3f) screenPos += new Vector3(0, 2.5f);
                else screenPos -= new Vector3(0, 2.5f);
                BuildCanvasTransform.position = screenPos;
            }
        }
    }
    public void ResetMenu()
    {
        TowerOptionsCanvas.enabled = false;
        BuildTowerCanvas.enabled = false;
        if (HitCell != null) HitCell.GetComponent<SpriteRenderer>().color = new Color(0.9f,0.9f,0.9f);
    }
    public void OpenBuildMenu(HexCell cell)
    {
        ResetMenu();
        _manager.Replacing = false;
        cell.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f);
        Vector3 screenPos;
        BuildTowerCanvas.enabled = true;
        _manager.Replacing = true;
        if (_hit.transform.position.x > 2f) screenPos = cell.transform.position - new Vector3(3.33f, 0);//Camera.main.WorldToScreenPoint
        else screenPos = HitCell.transform.position + new Vector3(3.33f, 0); //Camera.main.WorldToScreenPoint
        if (_hit.transform.position.y > 5f) screenPos -= new Vector3(0, 1.75f);
        else if (_hit.transform.position.y < -5f) screenPos += new Vector3(0, 2.75f);
        BuildCanvasTransform.position = screenPos;
    }
}