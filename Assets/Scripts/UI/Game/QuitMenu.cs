using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitMenu : MonoBehaviour
{
    [SerializeField]
    private Button CancelButton = null;

    public bool IsOpened { get { return gameObject.activeSelf; } }

    public void Open()
    {
        gameObject.SetActive(true);
        CancelButton.Select();
    }

    public void QuitGame()
    {
        gameObject.SetActive(false);

        GameManager.Instance.NetworkController.Disconnect();
    }

    public void CancelQuit()
    {
        gameObject.SetActive(false);
    }
}
