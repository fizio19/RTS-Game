using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TownHallUIController : MonoBehaviour
{
    [Header("Panel ratusza")]
    [SerializeField] private GameObject townHallPanel;
    [SerializeField] private CanvasGroup townHallCanvasGroup;

    [Header("Statystyki")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("Rekrutacja")]
    [SerializeField] private Button recruitWorkerButton;
    [SerializeField] private Button cancelQueueButton;
    [SerializeField] private TextMeshProUGUI queueStatusText;
    [SerializeField] private Image[] queueSlotIcons;
    [SerializeField] private Color queueActiveColor = Color.white;
    [SerializeField] private Color queueInactiveColor = new Color(1f, 1f, 1f, 0.2f);

    private TownHallProduction selectedTownHall;
    private Building selectedBuilding;

    private void Awake()
    {
        if (townHallCanvasGroup == null && townHallPanel != null)
            townHallCanvasGroup = townHallPanel.GetComponent<CanvasGroup>();

        if (townHallCanvasGroup == null)
            townHallCanvasGroup = GetComponent<CanvasGroup>();

        // UI kontrolera powinno być stale aktywne, ukrywamy tylko widoczność i interakcję.
        SetPanelVisible(false);
    }

    private void Update()
    {
        selectedTownHall = GetSelectedTownHall(out selectedBuilding);
        bool show = selectedTownHall != null;

        SetPanelVisible(show);

        if (!show)
            return;

        UpdateStats();
        UpdateRecruitmentUI();
    }

    public void OnRecruitWorkerClicked()
    {
        if (selectedTownHall == null)
            return;

        selectedTownHall.TryQueueWorker();
    }

    public void OnCancelQueueClicked()
    {
        if (selectedTownHall == null)
            return;

        selectedTownHall.CancelLastQueuedWorker();
    }

    private void UpdateStats()
    {
        if (selectedBuilding == null)
            return;

        if (nameText != null)
            nameText.text = "Name: " + selectedBuilding.BuildingName;

        if (hpText != null)
            hpText.text = "HP: " + Mathf.RoundToInt(selectedBuilding.CurrentHealth) + "/" + Mathf.RoundToInt(selectedBuilding.MaxHealth);
    }

    private void UpdateRecruitmentUI()
    {
        if (recruitWorkerButton != null)
        {
            bool canPay = ResourceManager.Instance != null && ResourceManager.Instance.food >= selectedTownHall.WorkerFoodCost;
            bool hasQueueSpace = selectedTownHall.QueueCount < selectedTownHall.MaxQueueSize;
            recruitWorkerButton.interactable = canPay && hasQueueSpace;
        }

        if (cancelQueueButton != null)
            cancelQueueButton.interactable = selectedTownHall.QueueCount > 0;

        if (queueStatusText != null)
        {
            if (selectedTownHall.IsTraining)
            {
                int percent = Mathf.RoundToInt(selectedTownHall.Progress01 * 100f);
                queueStatusText.text = "Kolejka: " + selectedTownHall.QueueCount + " | Trwa szkolenie: " + percent + "%";
            }
            else
            {
                queueStatusText.text = "Kolejka: 0 | Trwa szkolenie: 0%";
            }
        }

        if (queueSlotIcons != null)
        {
            for (int i = 0; i < queueSlotIcons.Length; i++)
            {
                if (queueSlotIcons[i] == null)
                    continue;

                bool occupied = i < selectedTownHall.QueueCount;
                queueSlotIcons[i].color = occupied ? queueActiveColor : queueInactiveColor;
            }
        }
    }

    private TownHallProduction GetSelectedTownHall(out Building building)
    {
        building = null;

        if (SelectionManager.Instance == null)
            return null;

        building = SelectionManager.Instance.selectedBuilding;
        if (building == null || !building.IsConstructed || building.data == null || !building.data.isMainBuilding)
            return null;

        return building.GetComponent<TownHallProduction>();
    }

    private void SetPanelVisible(bool visible)
    {
        if (townHallCanvasGroup != null)
        {
            townHallCanvasGroup.alpha = visible ? 1f : 0f;
            townHallCanvasGroup.interactable = visible;
            townHallCanvasGroup.blocksRaycasts = visible;
        }

        // Nie wyłączamy GameObjecta panelu, aby nie wyłączyć samego kontrolera UI.
        if (townHallPanel != null)
        {
            foreach (Transform child in townHallPanel.transform)
                child.gameObject.SetActive(visible);

            Image background = townHallPanel.GetComponent<Image>();
            if (background != null)
                background.enabled = visible;
        }
    }
}
