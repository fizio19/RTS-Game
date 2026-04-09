using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour
{
    public BuildingData buildingData;

    [Header("UI")]
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI requirementText;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    private void Update()
    {
        if (button == null || buildingData == null)
            return;

        bool requiresTownHall = buildingData.requiresMainBuilding && !buildingData.isMainBuilding;
        bool hasTownHall = BuildingRegistry.HasConstructedMainBuilding();
        bool unlocked = !requiresTownHall || hasTownHall;

        button.interactable = unlocked;

        if (requirementText != null)
            requirementText.gameObject.SetActive(!unlocked);
    }

    public void OnClick()
    {
        if (buildingData == null)
            return;

        bool requiresTownHall = buildingData.requiresMainBuilding && !buildingData.isMainBuilding;
        if (requiresTownHall && !BuildingRegistry.HasConstructedMainBuilding())
            return;

        BuildingPlacer.Instance.selectedBuilding = buildingData;
    }
}
