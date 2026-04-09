using System.Collections.Generic;
using UnityEngine;

public static class BuildingRegistry
{
    private static readonly HashSet<Building> constructedBuildings = new HashSet<Building>();

    public static void Register(Building building)
    {
        if (building != null)
            constructedBuildings.Add(building);
    }

    public static void Unregister(Building building)
    {
        if (building != null)
            constructedBuildings.Remove(building);
    }

    public static bool HasConstructedMainBuilding()
    {
        CleanupMissingEntries();

        foreach (Building building in constructedBuildings)
        {
            if (building == null || building.data == null)
                continue;

            if (building.data.isMainBuilding && building.IsConstructed)
                return true;
        }

        return false;
    }

    private static void CleanupMissingEntries()
    {
        constructedBuildings.RemoveWhere(b => b == null);
    }
}
