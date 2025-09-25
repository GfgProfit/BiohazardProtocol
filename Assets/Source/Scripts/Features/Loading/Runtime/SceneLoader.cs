using UnityEngine;

public static class SceneLoader
{
    private const string LoadingPrefabName = "Loading Canvas Prefab";

    public static void Load(MapItem map)
    {
        if (map == null)
        {
            Debug.LogError("[SceneLoader] MapItem == null");
            return;
        }

        LoadingScreen prefab = Resources.Load<LoadingScreen>(LoadingPrefabName);
        
        if (prefab == null)
        {
            Debug.LogError($"[SceneLoader] �� ������ ������ '{LoadingPrefabName}' � Resources.");
            return;
        }

        LoadingScreen screen = Object.Instantiate(prefab);
        screen.Begin(map);
    }
}