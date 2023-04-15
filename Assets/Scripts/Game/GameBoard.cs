using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using GNet;

public class GameBoard : MonoBehaviour
{
    [SerializeField]
    private Transform Pile = null;
    [SerializeField]
    private Transform Trash = null;
    [SerializeField]
    private Transform[] Slots = new Transform[8];

    [SerializeField]
    private GameObject PlayerBoardPrefab = null;
    [SerializeField]
    private GameObject GameCardPrefab = null;

    [SerializeField]
    private float MoveCardTime = 0.25f;
    public float moveCardTime { get { return MoveCardTime; } private set { MoveCardTime = value; }  }

    [SerializeField]
    private QuitMenu QuitMenu = null;
    [SerializeField]
    private VictoryWindow VictoryWindow = null;
    [SerializeField]
    private RoleWindow RoleWindow = null;
    [SerializeField]
    private Text CountText = null;
    [SerializeField]
    private Text CountLiers = null;

    private GameManager GManager;
    private IDictionary<int, PlayerBoard> PlayerBoards = new Dictionary<int, PlayerBoard>();
    private GameCard[] GameCards = null;
    private List<int> CardsInPile = new List<int>();
    private int NumberOfPlayer;

    public int RedWiresFoundCount { get; private set; }
    public bool IsInMenu { get { return QuitMenu.IsOpened; } }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleQuitMenu();
        }
    }

    private void ToggleQuitMenu()
    {
        if (!QuitMenu.IsOpened)
        {
            QuitMenu.Open();
        }
        else
        {
            QuitMenu.CancelQuit();
        }
    }

    private void Awake()
    {
        RedWiresFoundCount = 0;

        GManager = GameManager.Instance;
        NumberOfPlayer = GameNetwork.CurrentRoom.Players.Count;

        UpdateCount();

        var boardDispatch = new BoardDispatch();
        CreatePlayerBoards(boardDispatch.GetPositions(NumberOfPlayer));
    }

    private void CreatePlayerBoards(Vector3[] positions)
    {
        float z = transform.position.z;

        for (int i = 0; i < NumberOfPlayer; ++i)
        {
            Player player = GameNetwork.PlayerList[i];

            int posId = (NumberOfPlayer - GameNetwork.CurrentRoom.GetPlayerPos(GameNetwork.LocalPlayer.PlayerId) + i) % NumberOfPlayer;
            PlayerBoard board = Instantiate(PlayerBoardPrefab, positions[posId], Quaternion.identity).GetComponent<PlayerBoard>();

            PlayerBoards.Add(player.PlayerId, board);
            board.Initialize(this, player.PlayerId);
        }
    }

    public PlayerBoard GetPlayerBoard(int playerId)
    {
        return PlayerBoards[playerId];
    }

    public void SetupCards(int[] cards)
    {
        GameCards = new GameCard[cards.Length];

        GameObject go;
        for (int i = 0; i < cards.Length; ++i)
        {
            go = Instantiate(GameCardPrefab, Pile);
            go.transform.parent = Pile;
            GameCards[i] = go.GetComponent<GameCard>();
            GameCards[i].Initialize(i, (GameCardType)cards[i]);
            CardsInPile.Add(i);
        }
    }

    public GameCard GetGameCard(int carId)
    {
        return GameCards[carId];
    }

    public void SetupRoles(int[] roles)
    {
        for (int i = 0; i < NumberOfPlayer; ++i)
        {
            Player player = GameNetwork.PlayerList[i];
            PlayerBoards[player.PlayerId].SetRole(roles[i] == 1);

            if (player.IsLocal)
                RoleWindow.ShowRole(roles[i] == 1);
        }

    }

    public IEnumerator DispatchGameCards(int[] cards)
    {
        foreach (PlayerBoard board in PlayerBoards.Values)
        {
            board.SetupBoard(cards.Length / NumberOfPlayer);
        }

        for (int i = 0; i < cards.Length; ++i)
        {
            yield return new WaitForSeconds(0.15f);
            int posId = i % NumberOfPlayer;
            var playerBoard = PlayerBoards[GameNetwork.PlayerList[posId].PlayerId];
            StartCoroutine(playerBoard.ReceiveGameCardAnimation(i / NumberOfPlayer, GameCards[cards[i]], MoveCardTime));
        }
    }

    public void RevealGameCard(int cardId)
    {
        GameCards[cardId].Reveal();
        GameCards[cardId].Hover(false);
    }

    public IEnumerator CollectAllCards()
    {
        yield return StartCoroutine(CollectGreenCards());
        yield return new WaitForSeconds(0.5f);
        CollectTrashCards();
        yield return new WaitForSeconds(0.5f);
        CollectPileCards();
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator CollectGreenCards()
    {
        GameCard card = null;
        int i;

        int slotNum = 0;
        foreach (var slot in Slots)
        {
            if (slot.childCount == 0)
                break;
            ++slotNum;
        }

        foreach (Player player in GameNetwork.PlayerList)
        {
            PlayerBoard board = PlayerBoards[player.PlayerId];
            for (i = 0; i < 5; ++i)
            {
                card = board.GetCard(i);
                if (card != null && card.Revealed && card.CardType == GameCardType.RED_WIRE)
                {
                    StartCoroutine(MoveToGreenCardSlotAnimation(slotNum++, card));
                    board.RemoveCard(i);
                    yield return new WaitForSeconds(0.20f);
                }
            }
        }
    }

    public void CollectTrashCards()
    {
        GameCard card = null;
        int i;
        foreach (Player player in GameNetwork.PlayerList)
        {
            PlayerBoard board = PlayerBoards[player.PlayerId];
            for (i = 0; i < 5; ++i)
            {
                card = board.GetCard(i);
                if (card != null && card.Revealed)
                {
                    StartCoroutine(MoveToTrashAnimation(card));
                    board.RemoveCard(i);
                }
            }
        }
    }

    public void CollectPileCards()
    {
        CardsInPile.Clear();
        GameCard card = null;
        int i;

        foreach (Player player in GameNetwork.PlayerList)
        {
            PlayerBoard board = PlayerBoards[player.PlayerId];
            for (i = 0; i < 5; ++i)
            {
                card = board.GetCard(i);
                if (card != null && !card.Revealed)
                {
                    card.HideToPlayer();
                    StartCoroutine(MoveToPileAnimation(card));
                    board.RemoveCard(i);
                    CardsInPile.Add(card.Id);
                }
            }
        }
    }

    private IEnumerator MoveToGreenCardSlotAnimation(int slot, GameCard card)
    {
        var transform = card.GetComponent<Transform>();
        var initialPosition = transform.position;
        transform.SetParent(null);
        float timer = 0;

        while (timer < MoveCardTime)
        {
            var position = Vector3.Lerp(initialPosition, Slots[slot].position, timer / MoveCardTime);
            transform.position = position;
            timer += Time.deltaTime;
            yield return 0;
        }

        transform.SetParent(Slots[slot]);
        transform.position = Slots[slot].position;
        RedWiresFoundCount++;
        UpdateCount();
    }

    private void UpdateCount()
    {
        CountText.text = "Fils rouges trouvés: " + RedWiresFoundCount + " / " + NumberOfPlayer;

        int lierNum = NumberOfPlayer <= 4 ? 1 : NumberOfPlayer == 8 ? 3 : 2;
        string lierCount = NumberOfPlayer == 4 || NumberOfPlayer == 7 ? lierNum + " ou " + (lierNum + 1) : lierNum.ToString();
        CountLiers.text = "Traîtres: " + lierCount;
    }

    private IEnumerator MoveToPileAnimation(GameCard card)
    {
        var transform = card.GetComponent<Transform>();
        var initialPosition = transform.position;
        transform.SetParent(null);
        float timer = 0;

        while (timer < MoveCardTime)
        {
            var position = Vector3.Lerp(initialPosition, Pile.position, timer / MoveCardTime);
            transform.position = position;
            timer += Time.deltaTime;
            yield return 0;
        }

        transform.SetParent(Pile);
        transform.position = Pile.position;
    }

    private IEnumerator MoveToTrashAnimation(GameCard card)
    {
        var transform = card.GetComponent<Transform>();
        var initialPosition = transform.position;
        transform.SetParent(null);
        float timer = 0;

        while (timer < MoveCardTime)
        {
            var position = Vector3.Lerp(initialPosition, Trash.position, timer / MoveCardTime);
            transform.position = position;
            timer += Time.deltaTime;
            yield return 0;
        }

        transform.SetParent(Trash);
        transform.position = Trash.position;
    }

    public void DisplayVictory(bool orangeWins, int wireCount)
    {
        foreach(var card in GameCards)
        {
            card.Reveal();
        }

        StartCoroutine(DisplayVictoryAnim(orangeWins, wireCount));
    }

    public IEnumerator DisplayVictoryAnim(bool orangeWins, int wireCount)
    {
        yield return new WaitForSeconds(1.0f);
        if (!orangeWins)
        {
            yield return StartCoroutine(CollectGreenCards());
            yield return new WaitForSeconds(1.0f);
        }

        VictoryWindow.ShowVictory(orangeWins, wireCount);
        yield return new WaitForSeconds(3.0f);
        VictoryWindow.ShowRoomButton();
    }
}
