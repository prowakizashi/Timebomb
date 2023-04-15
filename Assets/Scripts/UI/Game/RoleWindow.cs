using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleWindow : MonoBehaviour
{
    [SerializeField]
    private Text RoleText = null;
    [SerializeField]
    private Text Description = null;

    [SerializeField]
    private Color PurpleColor = new Color();
    [SerializeField]
    private Color OrangeColor = new Color();

    public void ShowRole(bool isBadGuy)
    {
        gameObject.SetActive(true);
        RoleText.text = "Vous êtes dans l'équipe <color=#" + ColorUtility.ToHtmlStringRGB(isBadGuy ? OrangeColor : PurpleColor) + ">" + (isBadGuy ? "Orange" : "Violette") + "</color> !";

        if (isBadGuy)
        {
            Description.text = "Votre rôle est de faire exploser la bombe ou de faire en sorte que les gentils ne trouvent pas tous les cables rouges.";
        }
        else
        {
            Description.text = "Votre rôle est désamorcer la bombe en coupant tous les cables rouges. Attention à ne pas faire exploser la bombe!";
        }

    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
