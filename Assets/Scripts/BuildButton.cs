using UnityEngine;

public class BuildButton : MonoBehaviour
{
    public BuildingData buildingData;

    public void OnClick()
    {
        BuildingPlacer.Instance.selectedBuilding = buildingData;
    }
}