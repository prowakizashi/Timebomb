using Assets.Scripts.GameNetwork;
using GNet;
using GNet.Packets;
using UnityEngine;

public class GameManager : MonoBehaviourNetCallBacks
{
    [SerializeField]
    private bool useSpriteCards = true;
    public bool UseSpriteCards { get { return useSpriteCards; } private set { useSpriteCards = value; } }

    [SerializeField]
    private int deZoom = 10;
    public int DeZoom { get { return deZoom; } private set { deZoom = value; } }

    [SerializeField]
    private Camera activeCamera = null;
    public Camera ActiveCamera { get { return activeCamera; } private set { activeCamera = value; } }

    [SerializeField]
    private GameBoard gameBoard = null;
    public GameBoard GameBoard { get { return gameBoard; } private set { gameBoard = value; } }

    public MapLoader MapLoader { get; private set; }
    public NetworkController NetworkController { get; private set; }

    private static GameManager instance;
    public static GameManager Instance { get { return instance; } private set { instance = value; } }

    private int playingPlayer = -1;

    #region INITIALIZATION

    public GameManager()
    {
        if (instance == null)
            instance = this;
    }

    private void Awake()
    {
        var loaders = FindObjectsOfType<MapLoader>();
        if (loaders.Length > 0)
            MapLoader = loaders[0];

        var networks = FindObjectsOfType<NetworkController>();
        if (networks.Length > 0)
            NetworkController = networks[0];
    }

    private void Start()
    {
        GamePacketWriter.IsGameReady();
    }

    #endregion

    #region PREPARE

    [BindPacket(Packet = (int)EGameServerPackets.sendRoles)]
    private void ReceiveRole(int[] roles)
    {
        GameBoard.SetupRoles(roles);
    }

    [BindPacket(Packet = (int)EGameServerPackets.sendCards)]
    private void ReceiveCards(int[] cards)
    {
        GameBoard.SetupCards(cards);
    }

    #endregion

    #region ROUNDS

    [BindPacket(Packet = (int)EGameServerPackets.startRound)]
    private void StartRound(int round, int[] cards)
    {
        Debug.Log("StartRound");
        StartCoroutine(GameBoard.DispatchGameCards(cards));
    }

    [BindPacket(Packet = (int)EGameServerPackets.startTurn)]
    private void StartTurn(int turn, int playerId)
    {
        Debug.Log("StartTurn");
        playingPlayer = playerId;
        GameBoard.GetPlayerBoard(playingPlayer).SetPlaying(true);
    }

    public void ChooseCard(int cardId)
    {
        Debug.Log("ChooseCard");

        if (playingPlayer == GameNetwork.LocalPlayer.PlayerId)
            GamePacketWriter.PickACard(cardId);
    }

    public void NotifyHoverCard(int cardId, bool value)
    {
        if (playingPlayer == GameNetwork.LocalPlayer.PlayerId)
        {
            GamePacketWriter.HoverACard(cardId, value);
        }
    }

    [BindPacket(Packet = (int)EGameServerPackets.hoverCard)]
    public void PlayerHoverCard(int cardId, bool value)
    {
        var card = GameBoard.GetGameCard(cardId);
        if (card != null)
        {
            card.Hover(value);
        }

    }

    [BindPacket(Packet = (int)EGameServerPackets.endTurn)]
    private void EndTurn(int cardId)
    {
        Debug.Log("EndTurn");
        GameBoard.GetPlayerBoard(playingPlayer).SetPlaying(false);
        GameBoard.RevealGameCard(cardId);

        playingPlayer = -1;
    }

    [BindPacket(Packet = (int)EGameServerPackets.endRound)]
    private void EndRound()
    {
        Debug.Log("EndRound");
        StartCoroutine(GameBoard.CollectAllCards());
    }

    [BindPacket(Packet = (int)EGameServerPackets.gameOver)]
    private void GameOver(int type, int count)
    {
        Debug.Log("GameOver");
        GameBoard.DisplayVictory(type != 0, count);
    }

    #endregion

    public void NotifyLookCards()
    {
        Debug.Log("NotifyLookCards");
        GamePacketWriter.WatchCards();
    }

    [BindPacket(Packet = (int)EGameServerPackets.watchCards)]
    private void OtherPlayerLookCards(int playerId)
    {
        GameBoard.GetPlayerBoard(playerId).LookCards();
    }
    
    public void AskBackToLobby()
    {
        GamePacketWriter.ReturnToLobby();
    }

    [BindPacket(Packet = (int)EGameServerPackets.returnToLobby)]
    private void BackToLobby()
    {
        MapLoader.LoadLobby();
    }
}
