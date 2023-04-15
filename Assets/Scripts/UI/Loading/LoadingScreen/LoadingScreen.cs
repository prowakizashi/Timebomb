using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LoadingScreen : MonoBehaviour
{
    public abstract ELoadingType GetLoadingType();

    public abstract void StartLoading(Action startCallback = null);
    public abstract void EndLoading();
    public abstract void StartImmedialty(Action startCallback = null);
    public abstract void StopImmediatly();
}
