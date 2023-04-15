using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColoredTextButton : MonoBehaviour
{
    [SerializeField]
    private Text CreateButtonText = null;
    [SerializeField]
    private Color CreateButtonTextHoverColor = new Color();
    private Color CreateButtonTextBaseColor = new Color();

    private bool Hovered = false;
    private bool Selected = false;

    void Start()
    {
        if (CreateButtonText)
            CreateButtonTextBaseColor = CreateButtonText.color;
    }

    public void SetMouseHover(bool value)
    {
        Hovered = value;
        UpdateTextColor();
    }

    public void SetMouseSelect(bool value)
    {
        Selected = value;
        UpdateTextColor();
    }

    private void UpdateTextColor()
    {
        if (Selected || Hovered)
            CreateButtonText.color = CreateButtonTextHoverColor;
        else
            CreateButtonText.color = CreateButtonTextBaseColor;
    }
}
