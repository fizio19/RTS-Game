using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUIController : MonoBehaviour
{
    public GameObject buildingPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;

    private Image backgroundImage;

    private void Awake()
    {
        if (buildingPanel != null)
            backgroundImage = buildingPanel.GetComponent<Image>();
    }

    private void Update()
    {
        if (SelectionManager.Instance == null || buildingPanel == null)
            return;

        Building selectedBuilding = SelectionManager.Instance.selectedBuilding;
        bool showPanel = selectedBuilding != null;

        if (backgroundImage != null)
            backgroundImage.enabled = showPanel;

        foreach (Transform child in buildingPanel.transform)
            child.gameObject.SetActive(showPanel);

        if (!showPanel)
            return;

        nameText.text = "Name: " + selectedBuilding.BuildingName;
        hpText.text = "HP: " + Mathf.RoundToInt(selectedBuilding.CurrentHealth) + "/" + Mathf.RoundToInt(selectedBuilding.MaxHealth);
    }
}
