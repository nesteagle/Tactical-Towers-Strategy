//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//public class EnemyHealthDisplay : MonoBehaviour
//{
//    public Text EnemyTypeText;
//    public Text HealthNumberText;
//    public RectTransform HealthBarPosition;
//    private RaycastHit2D _hit;
//    private Canvas _healthBarIndicator;
//    private RectTransform _healthBar;
//    private Image _healthBarRenderer;
//    public Sprite[] Sprites;
//    private string _type;
//    private void Awake()
//    {
//        _healthBar = GetComponent<RectTransform>();
//        _healthBarIndicator = GetComponentInParent<Canvas>();
//        _healthBarRenderer = GetComponent<Image>();
//    }
//    private void FixedUpdate()
//    {
//        _hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
//        if (!_hit.collider)
//        {
//            StopAllCoroutines();
//            _healthBarIndicator.enabled = false;
//            return;
//        };
//        if (!_hit.collider.GetComponent<Enemy>())
//        {
//            StopAllCoroutines();
//            _healthBarIndicator.enabled = false;
//            return;
//        };
//        Enemy enemy = _hit.collider.GetComponent<Enemy>();
//        //_type = enemy.Type;
//        EnemyTypeText.text = enemy.Type;
//        StartCoroutine(UpdateHealthBar(enemy));
//    }
//    private string GetRomanNum(string input)
//    {
//        switch (input)
//        {
//            case "1":
//                return "I";
//            case "2":
//                return "II";
//            case "3":
//                return "III";
//            case "4":
//                return "IV";
//            case "5":
//                return "V";
//            default:
//                return null;
//        }
//    }
//    private float GetHealthPercent(string enemyType, float currentHealth)
//    {
//        float maxHealth = 0;
//        string[] enemyTypeSplit = enemyType.Split(' ');
//        if (enemyTypeSplit[0] == "Boss")
//        {
//            maxHealth = 5 * Mathf.Pow(int.Parse(enemyTypeSplit[1]) + 2, 2) + 5;
//        }
//        else switch (enemyType)
//            {
//                case "Knight":
//                    maxHealth = 2;
//                    break;
//                case "Archer":
//                    maxHealth = 2;
//                    break;
//                case "Scout":
//                    maxHealth = 1;
//                    break;
//                case "Supply":
//                    maxHealth = 2;
//                    break;
//            }
//        return currentHealth / maxHealth;
//    }
//    private IEnumerator UpdateHealthBar(Enemy enemy)
//    {
//        _healthBarIndicator.enabled = true;
//        while (enemy)
//        {
//            float healthPercent = GetHealthPercent(enemy.Type, enemy.Health);
//            HealthNumberText.text = enemy.Health.ToString() + " HP";
//            _healthBarRenderer.color = Color.HSVToRGB(healthPercent*.28f, 1, 1);
//            _healthBar.localScale = new Vector3(healthPercent, 1, 1);
//            _healthBar.localPosition = new Vector2(-39.33f + healthPercent * 39.33f, -15);
//            HealthBarPosition.position = enemy.transform.position - new Vector3(0, 1.25f);
//            yield return new WaitForEndOfFrame();
//        }
//        _healthBarIndicator.enabled = false;
//    }
//    private IEnumerator UpdateHealthImage(Enemy enemy)
//    {
//        _healthBarIndicator.enabled = true;
//        while (enemy)
//        {
//            switch (enemy.Health)
//            {
//                case 2:
//                    _healthBarRenderer.sprite= Sprites[0];
//                    break;
//                case 1:
//                    _healthBarRenderer.sprite = Sprites[2];
//                    break;
//                case 1.5f:
//                    _healthBarRenderer.sprite = Sprites[1];
//                    break;
//                case 0.5f:
//                    _healthBarRenderer.sprite = Sprites[3];
//                    break;
//            }
//            HealthBarPosition.position = enemy.transform.position - new Vector3(0, 1.25f);
//            yield return new WaitForEndOfFrame();
//        }
//        _healthBarIndicator.enabled = false;
//    }
//}

////WHOLE SCRIPT IS OUTDATED SYSTEM, REPLACE WITH UPDATED GRAPHICS EVENTUALLY.
