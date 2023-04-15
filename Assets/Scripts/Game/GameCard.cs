using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameCardType
{
    GREY_WIRE = 0,
    RED_WIRE,
    BOMB
}

public enum CardOwner
{
    NONE,
    OTHER_PLAYER,
    MINE
}

public class GameCard : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer SpriteRenderer = null;
    [SerializeField]
    private GameObject Border = null;

    [SerializeField]
    private Sprite BombSprite = null;
    [SerializeField]
    private Sprite RedWireSprite = null;
    [SerializeField]
    private Sprite GreyWireSprite = null;
    [SerializeField]
    private Sprite BackSprite = null;

    public GameCardType CardType { get; private set; }
    public bool Revealed { get; private set; }
    public bool Shown { get; private set; }
    public CardOwner Owner { get; private set; }
    public int OwnerId { get; private set; }
    public int Id { get; private set; }
    private GameManager GManager;

    private bool Hovered = false;

    public void Initialize(int id, GameCardType type)
    {
        Id = id;
        CardType = type;
        GManager = GameManager.Instance;
        
        SpriteRenderer.gameObject.SetActive(GameManager.Instance.UseSpriteCards);

        Shown = false;
        UpdateDisplay();
    }

    public void ChangeOwner(int ownerId, CardOwner owner)
    {
        OwnerId = ownerId;
        Owner = owner;
    }

    public void Reveal()
    {
        Revealed = true;
        UpdateDisplay();
    }

    public void ShowToPlayer()
    {
        Shown = true;
        UpdateDisplay();
    }

    public void HideToPlayer()
    {
        Shown = false;
        UpdateDisplay();
    }

    public void Hover(bool value)
    {
        if (Hovered == value)
            return;

        Hovered = value;
        Border.SetActive(value);
    }

    private void UpdateDisplay()
    {
        if (Revealed || Shown)
        {
            if (Revealed)
                Hover(false);

            switch (CardType)
            {
                case GameCardType.RED_WIRE:
                    SpriteRenderer.sprite = RedWireSprite;
                    break;
                case GameCardType.BOMB:
                    SpriteRenderer.sprite = BombSprite;
                    break;
                case GameCardType.GREY_WIRE:
                    SpriteRenderer.sprite = GreyWireSprite;
                    break;
            }
        }
        else
        {
            SpriteRenderer.sprite = BackSprite;
        }
    }

    void OnMouseOver()
    {
        if (Hovered)
            return;

        if (!Revealed && Owner == CardOwner.OTHER_PLAYER && GManager.GameBoard.IsInMenu)
        {
            Hover(true);
            GManager.NotifyHoverCard(Id, true);
        }
    }

    void OnMouseExit()
    {
        if (!Hovered)
            return;

        if (!Revealed && Owner == CardOwner.OTHER_PLAYER && !GManager.GameBoard.IsInMenu)
        {
            Hover(false);
            GManager.NotifyHoverCard(Id, false);
        }
    }

    private void OnMouseDown()
    {
        if (!Revealed && Owner == CardOwner.OTHER_PLAYER && !GManager.GameBoard.IsInMenu)
        {
            GManager.ChooseCard(Id);
        }
    }
}
