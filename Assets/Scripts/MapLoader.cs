using GNet;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapLoader : MonoBehaviour
{
    [SerializeField]
    private LoadingManager LoadingManager = null;

    private AsyncOperation LoadingOperation = null;
    private string CurrentMap = null;

    public delegate void ChangeMapHandler(string mapName);
    public ChangeMapHandler ChangeMapEvent;

    void Start()
    {
        Object.DontDestroyOnLoad(this);
        Object.DontDestroyOnLoad(LoadingManager);
        LoadMainMenu();
    }

    public bool LoadMainMenu()
    {
        return LoadAsyncMap("MainMenu", ELoadingType.BlackScreen, true, 1.0f);
    }

    public bool LoadLobby()
    {
        return LoadAsyncMap("Lobby", ELoadingType.BlackScreen, false, 1.0f);
    }

    public bool LoadGame()
    {
        return LoadAsyncMap("Game", ELoadingType.BlackScreen, false, 1.0f);
    }

    private bool LoadAsyncMap(string mapName, ELoadingType loadingScreen, bool immediatly, float minLoadingTime)
    {
        if (mapName == CurrentMap)
        {
            Debug.LogError("[MapLoader] Trying to load a map already loaded");
            return false;
        }
        
        GameNetwork.Paused = true;
        if (LoadingManager)
        {
            LoadingManager.StartLoading(loadingScreen, () => {
                LoadingOperation = SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Single);
                StartCoroutine(CheckLoadingEnd(minLoadingTime));
            }, immediatly);
        }
        else
        {
            Debug.LogError("[MapLoader] No loading screen manager found.");
            LoadingOperation = SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Single);
            StartCoroutine(CheckLoadingEnd(1.0f));
        }

        CurrentMap = mapName;
        return true;
    }

    private IEnumerator CheckLoadingEnd(float minLoadingTime)
    {
        yield return new WaitForSeconds(minLoadingTime);

        if (LoadingOperation != null)
        {
            while (!LoadingOperation.isDone)
                yield return null;
        }

        LoadingOperation = null;
        LoadingManager.EndLoading();
        ChangeMapEvent?.Invoke(CurrentMap);

        yield return null;

        GameNetwork.Paused = false;
    }
}
