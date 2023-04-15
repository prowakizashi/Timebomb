using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorWindow : MonoBehaviour
{
    [SerializeField]
    private Text ErrorText = null;
    [SerializeField]
    private Button CloseButton = null;

    private Action OnClose = null;
    
    public void Open(string message, Action callback)
    {
        gameObject.SetActive(true);
        OnClose = callback;
        ErrorText.text = message;

        CloseButton.Select();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        OnClose?.Invoke();
    }
}
