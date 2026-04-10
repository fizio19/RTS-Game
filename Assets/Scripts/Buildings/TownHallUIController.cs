using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TownHallUIController : MonoBehaviour
{
    [Header("Panel ratusza")]
    [SerializeField] private GameObject townHallPanel;
    [SerializeField] private Button recruitWorkerButton;
    [SerializeField] private Button cancelQueueButton;
    [SerializeField] private TextMeshProUGUI queueStatusText;
    [SerializeField] private TextMeshProUGUI queueIconsText;

    [Header("Stats ratusza")]
    [SerializeField] private TextMeshProUGUI townHallNameText;
    [SerializeField] private TextMeshProUGUI townHallHpText;

    private TownHallProduction selectedTownHall;
    private Building selectedBuilding;
    private CanvasGroup panelCanvasGroup;

    private void Awake()
    {
        if (townHallPanel != null)
            panelCanvasGroup = townHallPanel.GetComponent<CanvasGroup>();

        SetPanelVisible(false);
    }

    private void Update()
    {
        selectedTownHall = GetSelectedTownHall();
        selectedBuilding = SelectionManager.Instance != null ? SelectionManager.Instance.selectedBuilding : null;
        bool show = selectedTownHall != null;

        SetPanelVisible(show);

        if (!show)
            return;

        if (recruitWorkerButton != null)
        {
            bool canPay = ResourceManager.Instance != null && ResourceManager.Instance.food >= selectedTownHall.WorkerFoodCost;
            bool hasQueueSpace = selectedTownHall.QueuedWorkers < selectedTownHall.MaxQueueSize;
            recruitWorkerButton.interactable = canPay && hasQueueSpace;
        }

        if (cancelQueueButton != null)
            cancelQueueButton.interactable = selectedTownHall.QueuedWorkers > 0;

        if (queueStatusText != null)
        {
            if (selectedTownHall.IsTraining)
            {
                int percent = Mathf.RoundToInt(selectedTownHall.Progress01 * 100f);
                queueStatusText.text = "Rekrutacja Workera: " + percent + "% | Kolejka: " + selectedTownHall.QueuedWorkers;
            }
            else
            {
                queueStatusText.text = "Rekrutacja Workera: gotowy";
            }
        }

        if (queueIconsText != null)
            queueIconsText.text = BuildQueueIcons(selectedTownHall.QueuedWorkers);

        if (townHallNameText != null && selectedBuilding != null)
            townHallNameText.text = "Name: " + selectedBuilding.BuildingName;

        if (townHallHpText != null && selectedBuilding != null)
            townHallHpText.text = "HP: " + Mathf.RoundToInt(selectedBuilding.CurrentHealth) + "/" + Mathf.RoundToInt(selectedBuilding.MaxHealth);
    }

    public void OnRecruitWorkerClicked()
    {
        if (selectedTownHall == null)
            return;

        selectedTownHall.TryQueueWorker();
    }

    public void OnCancelLastQueueClicked()
    {
        if (selectedTownHall == null)
            return;

        selectedTownHall.CancelLastQueuedWorker();
    }

    private TownHallProduction GetSelectedTownHall()
    {
        if (SelectionManager.Instance == null)
            return null;

        Building building = SelectionManager.Instance.selectedBuilding;
        if (building == null || !building.IsConstructed || building.data == null || !building.data.isMainBuilding)
            return null;

        return building.GetComponent<TownHallProduction>();
    }

    private void SetPanelVisible(bool visible)
    {
        if (townHallPanel == null)
            return;

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = visible ? 1f : 0f;
            panelCanvasGroup.interactable = visible;
            panelCanvasGroup.blocksRaycasts = visible;
            return;
        }

        townHallPanel.SetActive(visible);
    }

    private string BuildQueueIcons(int amount)
    {
        if (amount <= 0)
            return "Kolejka: pusta";

        const int maxVisualIcons = 10;
        int visualCount = Mathf.Min(amount, maxVisualIcons);
        string icons = new string('■', visualCount);
        return amount > maxVisualIcons
            ? "Kolejka: " + icons + " +" + (amount - maxVisualIcons)
            : "Kolejka: " + icons;
    }
}
