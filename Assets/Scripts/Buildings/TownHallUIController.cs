using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TownHallUIController : MonoBehaviour
{
    [Header("Panel ratusza")]
    [SerializeField] private GameObject townHallPanel;
    [SerializeField] private CanvasGroup townHallCanvasGroup;
    [SerializeField] private Button recruitWorkerButton;
    [SerializeField] private TextMeshProUGUI queueStatusText;

    private TownHallProduction selectedTownHall;

    private void Awake()
    {
        // UI kontrolera powinno być stale aktywne, ukrywamy tylko widoczność panelu.
        SetPanelVisible(false);
    }

    private void Update()
    {
        selectedTownHall = GetSelectedTownHall();
        bool show = selectedTownHall != null;

        SetPanelVisible(show);

        if (!show)
            return;

        if (recruitWorkerButton != null)
        {
            bool canPay = ResourceManager.Instance != null && ResourceManager.Instance.food >= selectedTownHall.WorkerFoodCost;
            recruitWorkerButton.interactable = !selectedTownHall.IsTraining && canPay;
        }

        if (queueStatusText != null)
        {
            if (selectedTownHall.IsTraining)
            {
                int percent = Mathf.RoundToInt(selectedTownHall.Progress01 * 100f);
                queueStatusText.text = "Rekrutacja Workera: " + percent + "%";
            }
            else
            {
                queueStatusText.text = "Rekrutacja Workera: gotowy";
            }
        }
    }

    public void OnRecruitWorkerClicked()
    {
        if (selectedTownHall == null)
            return;

        selectedTownHall.TryQueueWorker();
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
        if (townHallCanvasGroup != null)
        {
            townHallCanvasGroup.alpha = visible ? 1f : 0f;
            townHallCanvasGroup.interactable = visible;
            townHallCanvasGroup.blocksRaycasts = visible;
        }

        if (townHallPanel != null)
            townHallPanel.SetActive(visible);
    }
}
