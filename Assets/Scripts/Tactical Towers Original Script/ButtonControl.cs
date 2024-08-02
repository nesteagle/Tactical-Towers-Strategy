using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class ButtonControl : MonoBehaviour
{
    public Text[] TowerRemainingText;
    public Text HealthText;
    public Text WaveText;
    public Text ObtainText;
    public Text ObtainTitle;
    public Canvas WaveConfirmationCanvas;
    public Canvas ReadyButton;
    public Canvas HealthIndicator;
    public Canvas TowerObtainMenu;
    public Canvas TowerConfirmationCanvas;
    public Canvas RewardIndicator;
    public RectTransform TowerConfirmButtons;
    public bool Previewing=false;
    private TowerManager _manager;
    private TowerBuild _towerBuildScript;
    private bool _menuOpen = false;
    private int selectedTower;
    public Animation ShakeAnimation;
    private Coroutine _animRoutine;
    private void Awake()
    {
        _manager = GetComponent<TowerManager>();
        _towerBuildScript = GetComponent<TowerBuild>();
        //if (_animRoutine == null) _animRoutine = StartCoroutine(PlayAnimation(ShakeAnimation));
    }
    public void RemoveTower()
    {
        UpdateText();
        _manager.RemoveTower(_towerBuildScript.HitCell);
        _towerBuildScript.ResetMenu();
    }
    public void ConfirmTowerBuild()
    {
        Previewing = false;
        _manager.PlaceTower(selectedTower, _towerBuildScript.HitCell);
    }
    public void CancelTowerBuild()
    {
        Previewing = false;
    }
    public void ReplaceTower()
    {
        _towerBuildScript.OpenBuildMenu(_towerBuildScript.HitCell);
        UpdateText();
    }
    public void PlaceTower(int selectedTower)
    {
        OpenTowerConfirmation(selectedTower);//in order of TowerPrefabs.
        UpdateText();
    }
    public void OpenWaveConfirmation()
    {
        Previewing = false;
        _towerBuildScript.ResetMenu();
        if (_menuOpen == false)
        {
            WaveConfirmationCanvas.enabled = true;
            _menuOpen = true;
        } else CloseWaveConfirmation();
    }
    public void ConfirmReady()
    {
        CloseWaveConfirmation();
        GameControl.BeginWave();
        StartCoroutine(ToggleDuringWave(ReadyButton,true));
        StartCoroutine(ToggleDuringWave(HealthIndicator, false));
        StartCoroutine(ToggleDuringWave(RewardIndicator, true));
        StartCoroutine(UpdateHealth());
        ReadyButton.GetComponentInChildren<Text>().text = "Start Wave " + (GameControl.CurrentWave + 2);
    }
    public void UpdateText()
    {
        TowerRemainingText[0].text = "(" + Currency.Towers[0].Item2.ToString() + " left )";
        TowerRemainingText[1].text = "(" + Currency.Towers[1].Item2.ToString() + " left)";
        if (GameControl.CurrentWave < 0) return;
        else Destroy(GameObject.Find("ArcherTowerNotUnlocked"));
        TowerRemainingText[2].text = "(" + Currency.Towers[2].Item2.ToString() + " left)";
        if (GameControl.CurrentWave < 0) return;
        else Destroy(GameObject.Find("FireTowerNotUnlocked"));
        TowerRemainingText[3].text = "(" + Currency.Towers[3].Item2.ToString() + " left)";
        if (GameControl.CurrentWave < 0) return;
        else Destroy(GameObject.Find("TeslaTowerNotUnlocked"));
        TowerRemainingText[4].text = "(" + Currency.Towers[4].Item2.ToString() + " left)";
    }
    public void CloseWaveConfirmation()
    {
        WaveConfirmationCanvas.enabled = false;
        _menuOpen = false;
    }
    private IEnumerator ToggleDuringWave(Canvas canvasToHide, bool hiding)
    {
        canvasToHide.enabled = !hiding;
        yield return new WaitUntil(() => GameControl.InWave == false);
        canvasToHide.enabled = hiding;
        if (_animRoutine == null)
        {
            _animRoutine = StartCoroutine(PlayAnimation(ShakeAnimation));
        }
    }
    private IEnumerator UpdateHealth()
    {
        WaveText.text = "Wave " + (GameControl.CurrentWave+1).ToString();
        while (GameControl.InWave == true)
        {
            int health = GameControl.health;
            yield return new WaitUntil(() => GameControl.health != health);
            HealthText.text = GameControl.health.ToString();
        }
    }
    public void OpenTowerObtainMenu()
    {
        TowerObtainMenu.enabled = true;
        if ((GameControl.CurrentWave - 4) % 5 == 0 && GameControl.CurrentWave != 4) ObtainTitle.text = "You Unlocked:";
        else ObtainTitle.text = "You obtained:";
        ObtainText.text = _manager.ObtainedTower;
        if ((GameControl.CurrentWave - 4) % 5 == 0)ObtainText.text+= " + 5x Wall Tower";
    }
    private void OpenTowerConfirmation(int currentlySelected)
    {
        if (Previewing == true||Currency.Towers[currentlySelected].Item2<1) return;
        Previewing = true;
        _towerBuildScript.ResetMenu();
        StartCoroutine(_manager.PreviewTower(currentlySelected, _towerBuildScript.HitCell));
        TowerConfirmButtons.position = _towerBuildScript.HitCell.transform.position - new Vector3(0, 1.15f);
        selectedTower = currentlySelected;
    }
    public void CloseCanvas(Canvas canvasToClose)
    {
        canvasToClose.enabled = false;
        _menuOpen = false;
    }
    public void OpenCanvas (Canvas canvasToOpen)
    {
        canvasToOpen.enabled = true;
    }
    private IEnumerator PlayAnimation(Animation anim)
    {
        while (GameControl.InWave == false)
        {
            Debug.Log("Playing Animation!");
            ShakeAnimation.Play("Shake");
            yield return new WaitForSeconds(Random.Range(2, 5));
        }
        _animRoutine = null;
    }

}
