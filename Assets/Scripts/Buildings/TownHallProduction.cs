using System.Collections.Generic;
using UnityEngine;

public class TownHallProduction : MonoBehaviour
{
    [Header("Rekrutacja Worker")]
    [SerializeField] private UnitData workerData;
    [SerializeField] private int workerFoodCost = 100;
    [SerializeField] private float workerTrainTime = 10f;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int maxQueueSize = 5;

    private readonly Queue<UnitData> recruitQueue = new Queue<UnitData>();
    private float trainTimer;

    public bool IsTraining => recruitQueue.Count > 0;
    public int QueueCount => recruitQueue.Count;
    public int MaxQueueSize => maxQueueSize;
    public int WorkerFoodCost => workerFoodCost;

    // Postęp aktualnie szkolonej jednostki.
    public float Progress01 => IsTraining ? Mathf.Clamp01(trainTimer / Mathf.Max(0.01f, workerTrainTime)) : 0f;

    private void Update()
    {
        if (!IsTraining)
            return;

        trainTimer += Time.deltaTime;
        if (trainTimer < workerTrainTime)
            return;

        UnitData trainedUnit = recruitQueue.Dequeue();
        SpawnWorker(trainedUnit);

        // Kolejny element zaczyna z timerem od zera.
        trainTimer = 0f;
    }

    public bool TryQueueWorker()
    {
        if (workerData == null || workerData.prefab == null)
            return false;

        if (recruitQueue.Count >= maxQueueSize)
            return false;

        bool paid = ResourceManager.Instance.SpendResources(0, workerFoodCost, 0, 0);
        if (!paid)
            return false;

        recruitQueue.Enqueue(workerData);

        // Gdy kolejka była pusta, od razu startuje produkcja pierwszej jednostki.
        if (recruitQueue.Count == 1)
            trainTimer = 0f;

        return true;
    }

    public bool CancelLastQueuedWorker()
    {
        if (recruitQueue.Count == 0)
            return false;

        List<UnitData> temp = new List<UnitData>(recruitQueue);
        temp.RemoveAt(temp.Count - 1);

        recruitQueue.Clear();
        foreach (UnitData item in temp)
            recruitQueue.Enqueue(item);

        // Zwrot kosztu anulowanej pozycji kolejki.
        ResourceManager.Instance.AddFood(workerFoodCost);

        if (!IsTraining)
            trainTimer = 0f;

        return true;
    }

    private void SpawnWorker(UnitData unit)
    {
        if (unit == null || unit.prefab == null)
            return;

        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position + Vector3.down * 1.2f;
        Instantiate(unit.prefab, spawnPosition, Quaternion.identity);
    }
}
