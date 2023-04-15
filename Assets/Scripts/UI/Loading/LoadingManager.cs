using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    private IDictionary<ELoadingType, LoadingScreen> LoadingScreens = new Dictionary<ELoadingType, LoadingScreen>();
    private LoadingScreen currentLS = null;

    void Awake()
    {
        var screens = GetComponentsInChildren<LoadingScreen>(true);
        foreach (var screen in screens)
        {
            LoadingScreens.Add(screen.GetLoadingType(), screen);
        }
    }

    public void StartLoading(ELoadingType loadingType, Action startCallback = null, bool immediatly = false)
    {
        LoadingScreen LS;
        LoadingScreens.TryGetValue(loadingType, out LS);

        if (LS == null)
            return;

        if (currentLS != null)
            currentLS.StopImmediatly();

        if (immediatly)
            LS.StartImmedialty(startCallback);
        else
            LS.StartLoading(startCallback);
        currentLS = LS;
    }

    public void EndLoading(bool immediatly = false)
    {
        if (currentLS)
        {
            if (immediatly)
                currentLS.StopImmediatly();
            else
                currentLS.EndLoading();
        }
        currentLS = null;
    }
}
