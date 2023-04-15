using GNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class PlayerBoard : MonoBehaviour
{
    [SerializeField]
    private Transform[] Slots = new Transform[5];

    [SerializeField]
    private Canvas UI = null;
    [SerializeField]
    private Text PlayerName = null;
    [SerializeField]
    private Text TurnText = null;

    [SerializeField]
    private GameObject BottomPanel = null;

    [SerializeField]
    private GameObject LightOn = null;
    [SerializeField]
    private GameObject LightOff = null;

    [SerializeField]
    private GameObject ShowButton = null;
    [SerializeField]
    private Image ShowImage = null;

    [SerializeField]
    private Color OrangeTeamColor = new Color();
    [SerializeField]
    private Color PurpleTeamColor = new Color();

    GameCard[] Cards = new GameCard[5];
    public bool PlayerRole { get; private set; }

    public int OwnerId { get; private set; }
    private GameManager GManager;
    private GameBoard GameBoard;

    private bool isShowingCards = false;

    public void Initialize(GameBoard gameBoard, int playerId)
    {
        PlayerRole = false;
        OwnerId = playerId;
        GManager = GameManager.Instance;
        GameBoard = gameBoard;
        PlayerName.text = GameNetwork.CurrentRoom.Players[playerId].PlayerName;

        bool isLocalPlayer = GameNetwork.LocalPlayer.PlayerId == OwnerId;
        TurnText.gameObject.SetActive(isLocalPlayer);
        BottomPanel.gameObject.SetActive(isLocalPlayer);
        ShowButton.SetActive(isLocalPlayer);
        ShowImage.gameObject.SetActive(false);

        UI.worldCamera = GManager.ActiveCamera;
    }

    public void SetupBoard(int cardNumber)
    {
        TurnText.text = "Tour: " + (6 - cardNumber) + "/4";
    }

    public void SetPlaying(bool playing)
    {
        LightOn.SetActive(playing);
        LightOff.SetActive(!playing);
    }

    public IEnumerator ReceiveGameCardAnimation(int index, GameCard card, float moveTime)
    {
        var transform = card.GetComponent<Transform>();
        var initialPosition = transform.position;
        var initialRotation = transform.rotation;
        transform.parent = null;
        float timer = 0;

        while (timer < moveTime)
        {
            var position = Vector3.Lerp(initialPosition, Slots[index].position, timer / moveTime);
            var rotation = Quaternion.Lerp(initialRotation, Slots[index].rotation, timer / moveTime);
            transform.position = position;
            transform.rotation = rotation;
            timer += Time.deltaTime;
            yield return 0;
        }

        transform.position = Slots[index].position;
        transform.rotation = Slots[index].rotation;
        transform.parent = Slots[index];
        AddCard(index, card);
    }

    public void ShowCards()
    {
        if (!isShowingCards)
        {
            isShowingCards = true;
            GManager.NotifyLookCards();
            StartCoroutine(ShowCardsAnim());
        }
    }

    public void LookCards()
    {
        StartCoroutine(LookCardsAnim());
    }

    private IEnumerator LookCardsAnim()
    {
        ShowImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        ShowImage.gameObject.SetActive(false);
    }

    public IEnumerator ShowCardsAnim()
    {
        for (int i = 0; i < Cards.Length; ++i)
        {
            if (Cards[i] != null)
                Cards[i].ShowToPlayer();
        }

        yield return new WaitForSeconds(3.0f);

        for (int i = 0; i < Cards.Length; ++i)
        {
            if (Cards[i] != null)
                Cards[i].HideToPlayer();
        }
        isShowingCards = false;
    }

    public void SetRole(bool badGuy)
    {
        PlayerRole = badGuy;
        if (OwnerId == GameNetwork.LocalPlayer.PlayerId)
            PlayerName.color = badGuy ? OrangeTeamColor : PurpleTeamColor;
    }

    public void AddCard(int slot, GameCard card)
    {
        Cards[slot] = card;
        card.ChangeOwner(OwnerId, OwnerId == GameNetwork.LocalPlayer.PlayerId ? CardOwner.MINE : CardOwner.OTHER_PLAYER);
    }

    public void RemoveCard(int slot)
    {
        if (Cards[slot] != null)
            Cards[slot].ChangeOwner(-1, CardOwner.NONE);
        Cards[slot] = null;
    }

    public GameCard GetCard(int slot)
    {
        return Cards[slot];
    }

    public string GetPlayerName()
    {
        return PlayerName.text;
    }
}
