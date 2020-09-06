using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShopController : MonoBehaviour
{
    public Text ArrowDMG, PickAxeDMG, AttackRate;
    public Text ArrowDMGCost, PickAxeDMGCost, AttackRateCost;
    public Text GoldCount;
    public static ShopController Instance;
    public int ArrowCost,PickAxeCost,RateCost;
    Player Playercontroller;
   private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Err there are 2 instances of gameControllers");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        Playercontroller = Player.Instance;
        CalculateCost();
    }
    void Update()
    {
        UpdateShop();
        GoldCount.text = "Gold:" + gameController.Instance.Gold.ToString();
    }
    void UpdateShop()
    {
        ArrowDMG.text = Playercontroller.ArrowDamage.ToString();
        PickAxeDMG.text = Playercontroller.PickAxeStrength.ToString();
        AttackRate.text = Playercontroller.AttackRate.ToString();

        ArrowDMGCost.text = ArrowCost.ToString();
        PickAxeDMGCost.text = PickAxeCost.ToString();
        AttackRateCost.text = RateCost.ToString();
    }
    public void CalculateCost()
    {
        ArrowCost = Playercontroller.ArrowLevel * 250;
        PickAxeCost = Playercontroller.PickaxeLevel * 500;
        RateCost = Playercontroller.AttackRateLevel * 1000;
    }
}
