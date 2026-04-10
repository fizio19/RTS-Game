using UnityEngine;

public class TownHallProduction : MonoBehaviour
{
    [Header("Rekrutacja Worker")]
    [SerializeField] private UnitData workerData;
    [SerializeField] private int workerFoodCost = 100;
    [SerializeField] private float workerTrainTime = 10f;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int maxQueueSize = 10;

    private float trainTimer;
    private int queuedWorkers;

    public bool IsTraining => queuedWorkers > 0;
    public float Progress01 => IsTraining ? Mathf.Clamp01(trainTimer / Mathf.Max(0.01f, workerTrainTime)) : 0f;
    public int WorkerFoodCost => workerFoodCost;
    public int QueuedWorkers => queuedWorkers;
    public int MaxQueueSize => maxQueueSize;

    private void Update()
    {
        if (!IsTraining)
            return;

        trainTimer += Time.deltaTime;
        if (trainTimer < workerTrainTime)
            return;

        SpawnWorker();
        queuedWorkers = Mathf.Max(queuedWorkers - 1, 0);
        trainTimer = 0f;
    }

    public bool TryQueueWorker()
    {
        if (workerData == null || workerData.prefab == null)
            return false;

        if (queuedWorkers >= maxQueueSize)
            return false;

        bool paid = ResourceManager.Instance.SpendResources(0, workerFoodCost, 0, 0);
        if (!paid)
            return false;

        bool queueWasEmpty = queuedWorkers == 0;
        queuedWorkers++;

        if (queueWasEmpty)
            trainTimer = 0f;

        return true;
    }

    public bool CancelLastQueuedWorker()
    {
        if (queuedWorkers <= 0)
            return false;

        queuedWorkers--;
        ResourceManager.Instance.AddFood(workerFoodCost);

        if (!IsTraining)
            trainTimer = 0f;

        return true;
    }

    private void SpawnWorker()
    {
        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position + Vector3.down * 1.2f;
        Instantiate(workerData.prefab, spawnPosition, Quaternion.identity);
    }
}
