using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkerUIController : MonoBehaviour
{
    public GameObject workerPanel;

    [Header("Texts")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI carryText;

    private Image backgroundImage;

    void Awake()
    {
        if (workerPanel != null)
            backgroundImage = workerPanel.GetComponent<Image>();
    }

    void Update()
    {
        if (SelectionManager.Instance == null || workerPanel == null)
            return;

        Worker selectedWorker = GetSelectedWorker();
        bool showPanel = selectedWorker != null;

        if (backgroundImage != null)
            backgroundImage.enabled = showPanel;

        foreach (Transform child in workerPanel.transform)
            child.gameObject.SetActive(showPanel);

        if (!showPanel)
            return;

        nameText.text = "Name: " + selectedWorker.UnitName;
        hpText.text = "HP: " + Mathf.RoundToInt(selectedWorker.CurrentHealth) + "/" + Mathf.RoundToInt(selectedWorker.MaxHealth);
        attackText.text = "Atak: " + selectedWorker.AttackDamage;
        carryText.text = "Surowce: " + selectedWorker.CarriedAmount + "/" + selectedWorker.CarryCapacity;
    }

    private Worker GetSelectedWorker()
    {
        if (SelectionManager.Instance.selectedUnits.Count == 0)
            return null;

        UnitMovement firstUnit = SelectionManager.Instance.selectedUnits[0];
        if (firstUnit == null)
            return null;

        return firstUnit.GetComponent<Worker>();
    }
}
