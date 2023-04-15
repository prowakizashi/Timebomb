using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowButton : MonoBehaviour
{
    private GameManager GManager = null;
    private PlayerBoard PlayerBoard = null;

    public void Setup(PlayerBoard playerBoard, bool display)
    {
        GManager = GameManager.Instance;
        PlayerBoard = playerBoard;
        gameObject.SetActive(display);
    }

    private void OnMouseDown()
    {
        PlayerBoard.ShowCards();
    }
}
