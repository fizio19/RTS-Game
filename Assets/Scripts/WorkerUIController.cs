using UnityEngine;
using UnityEngine.UI;

public class WorkerUIController : MonoBehaviour
{
    public GameObject workerPanel;

    private Image backgroundImage;

    void Awake()
    {
        backgroundImage = workerPanel.GetComponent<Image>();
    }

    void Update()
    {
        if (SelectionManager.Instance == null)
            return;

        bool hasAnyUnit = SelectionManager.Instance.selectedUnits.Count > 0;

        // ukryj/poka¿ t³o
        if (backgroundImage != null)
            backgroundImage.enabled = hasAnyUnit;

        // ukryj/poka¿ dzieci
        foreach (Transform child in workerPanel.transform)
        {
            child.gameObject.SetActive(hasAnyUnit);
        }
    }
}