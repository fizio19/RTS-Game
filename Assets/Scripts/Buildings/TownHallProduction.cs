using UnityEngine;

public class TownHallProduction : MonoBehaviour
{
    [Header("Rekrutacja Worker")]
    [SerializeField] private UnitData workerData;
    [SerializeField] private int workerFoodCost = 100;
    [SerializeField] private float workerTrainTime = 10f;
    [SerializeField] private Transform spawnPoint;

    private float trainTimer;
    private bool isTraining;

    public bool IsTraining => isTraining;
    public float Progress01 => isTraining ? Mathf.Clamp01(trainTimer / Mathf.Max(0.01f, workerTrainTime)) : 0f;
    public int WorkerFoodCost => workerFoodCost;

    private void Update()
    {
        if (!isTraining)
            return;

        trainTimer += Time.deltaTime;
        if (trainTimer < workerTrainTime)
            return;

        SpawnWorker();
        isTraining = false;
        trainTimer = 0f;
    }

    public bool TryQueueWorker()
    {
        if (isTraining || workerData == null || workerData.prefab == null)
            return false;

        bool paid = ResourceManager.Instance.SpendResources(0, workerFoodCost, 0, 0);
        if (!paid)
            return false;

        isTraining = true;
        trainTimer = 0f;
        return true;
    }

    private void SpawnWorker()
    {
        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position + Vector3.down * 1.2f;
        Instantiate(workerData.prefab, spawnPosition, Quaternion.identity);
    }
}
