using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    public int wood;
    public int food;
    public int stone;
    public int gold;

    [Header("UI")]
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI stoneText;
    public TextMeshProUGUI goldText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
        AddWood(100);
        AddFood(50);
        AddStone(25);
        AddGold(10);
    }

    public void AddWood(int amount)
    {
        wood += amount;
        UpdateUI();
    }

    public void AddFood(int amount)
    {
        food += amount;
        UpdateUI();
    }

    public void AddStone(int amount)
    {
        stone += amount;
        UpdateUI();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateUI();
    }

    public bool SpendResources(int woodCost, int foodCost, int stoneCost, int goldCost)
    {
        if (wood < woodCost || food < foodCost || stone < stoneCost || gold < goldCost)
            return false;

        wood -= woodCost;
        food -= foodCost;
        stone -= stoneCost;
        gold -= goldCost;

        UpdateUI();
        return true;
    }

    private void UpdateUI()
    {
        woodText.text = "Wood: " + wood;
        foodText.text = "Food: " + food;
        stoneText.text = "Stone: " + stone;
        goldText.text = "Gold: " + gold;
    }
}