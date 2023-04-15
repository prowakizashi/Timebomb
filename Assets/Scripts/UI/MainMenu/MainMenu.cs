using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject TitleOn = null;
    [SerializeField]
    private GameObject TitleOff = null;
    [SerializeField]
    private GameObject RTXOn = null;
    [SerializeField]
    private GameObject RTXOff = null;

    [SerializeField]
    private Text CreateButtonText = null;
    [SerializeField]
    private Color CreateButtonTextHoverColor;
    private Color CreateButtonTextBaseColor;

    [SerializeField]
    private InputField PlayerName = null;
    [SerializeField]
    private InputField JoinField = null;
    private bool codeFieldFocused = false;

    [SerializeField]
    private GameObject RulesWindow = null;
    [SerializeField]
    private GameObject CreditsWindow = null;
    [SerializeField]
    private ErrorWindow ErrorWindow = null;
    private NetworkController NetController = null;

    public void Awake()
    {
        var ctrls = FindObjectsOfType<NetworkController>();
        if (ctrls.Length > 0)
            NetController = ctrls[0];
    }

    public void Start()
    {
        if (PlayerName != null && PlayerPrefs.HasKey("playername"))
            PlayerName.text = PlayerPrefs.GetString("playername");

        CreateButtonTextBaseColor = CreateButtonText.color;
        if (PlayerPrefs.HasKey("RTX"))
        {
            SetRTX(PlayerPrefs.GetInt("RTX") == 1);
        }
        else
        {
            SetRTX(false);
            StartCoroutine(ShowRTXMessage());
        }

        NetController.UpdatePlayerName(PlayerName.text);
    }

    private IEnumerator ShowRTXMessage()
    {
        yield return new WaitForSeconds(3.0f);
        ErrorWindow.Open("Bienvenue sur Time Bomb!\n\nJ'aimerai vous faire part que ce jeu supporte le mode RTX!\n\nCliquez sur le bouton en haut à gauche pour essayer! ;).", null);
    }

    public void CreatePrivateRoom()
    {
        if (NetController == null)
            return;
        
        NetController.CreatePrivateRoom(8, (message) => {
            ErrorWindow.Open(message, null);
        });
    }

    public void JoinPrivateRoom()
    {
        if (JoinField.text.Length != 6)
        {
            ErrorWindow.Open("Le code doit faire 6 lettres pour être bon !", SelectCodeField);
            return;
        }

        NetController.JoinPrivateRoom(JoinField.text, (message) => {
            ErrorWindow.Open(message, SelectCodeField);
        });
    }

    public void ChangeCode(string name)
    {
        if (name == "")
            return;

        string upperName = name.ToUpper();
        string copy = "";
        for (int i = 0; i < upperName.Length; ++i)
        {
            for (int l = 0; l < NetController.AllowedLetters.Length; ++l)
            {
                if (upperName[i] == NetController.AllowedLetters[l])
                {
                    copy += upperName[i];
                }
            }
        }

        if (name != copy)
        {
            JoinField.text = copy;
        }
    }

    public void ChangePlayerName(string name)
    {
        PlayerPrefs.SetString("playername", name);
        NetController.UpdatePlayerName(name);

    }

    public void SelectCodeField()
    {
        JoinField.Select();
        JoinField.ActivateInputField();
    }

    public void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && codeFieldFocused)
        {
            codeFieldFocused = false;
            JoinPrivateRoom();

            JoinField.DeactivateInputField();
        }
        else
        {
            codeFieldFocused = JoinField.isFocused;
        }
    }

    public void SetRTX(bool value)
    {
        PlayerPrefs.SetInt("RTX", value ? 1 : 0);

        RTXOff.SetActive(!value);
        RTXOn.SetActive(value);
        TitleOff.SetActive(!value);
        TitleOn.SetActive(value);
    }

    public void ShowRules()
    {
        RulesWindow.SetActive(true);
    }

    public void HideRules()
    {
        RulesWindow.SetActive(false);
    }

    public void ShowCredits()
    {
        CreditsWindow.SetActive(true);
    }

    public void HideCredits()
    {
        CreditsWindow.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
