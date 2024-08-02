using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class Currency : MonoBehaviour
{
    public static (string,int)[] Towers=new (string, int)[5] {
        ("Wall", 0),
        ("Cannon", 5),
        ("Archer", 5),
        ("Fire", 5),
        ("Tesla", 5)
    };

    //private void Awake()
    //{
    //    towers[0] = ("Wall", 100);
    //    towers[1] = ("Cannon", 5);
    //    towers[2] = ("Archer", 5);
    //    towers[3] = ("Fire", 5);
    //    towers[4] = ("Tesla", 5);
    //    //need to balance later
    //}


    //public static Text textToUpdate;
    //private static int _currency = 10000;
    //public static int currency
    //{
    //    get
    //    {
    //        return _currency;
    //    }
    //    set
    //    {
    //        if (value > 0)
    //        {
    //            _currency = value;
    //        }
    //        else
    //        {
    //            _currency = 0;
    //        }
    //        ChangeDisplay();
    //    }
    //}
    //private void Awake()
    //{
    //    textToUpdate = GameObject.Find("CurrencyDisplay").GetComponentInChildren<Text>();
    //}

    //static void ChangeDisplay()
    //{
    //    textToUpdate.text = currency.ToString();
    //}HAVE BETTER ALTERNATIVE ALREADY. WILL CONTINUE ON THIS IF NEEDED.

}
