using GNet;
using GNet.Packets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviourNetCallBacks
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
    private Button StartButton = null;

    [SerializeField]
    private Transform PlayerGrid = null;
    [SerializeField]
    private GameObject PlayerUIPrefab = null;
    [SerializeField]
    private Text PlayerCount = null;
    [SerializeField]
    private InputField RoomName = null;

    private NetworkController NetController = null;

    private IDictionary<int, PlayerLobbyUI> PlayerUIs = new Dictionary<int, PlayerLobbyUI>();
    private IDictionary<int, bool> ReadyPlayers = new Dictionary<int, bool>();

    private MapLoader MapLoader = null;

    private bool StartingGame = false;
    private int MinimumOfPlayer = 4;

    public void Awake()
    {
        MapLoader = FindObjectsOfType<MapLoader>()[0];
        NetController = FindObjectsOfType<NetworkController>()[0];

        if (PlayerPrefs.HasKey("RTX"))
            SetRTX(PlayerPrefs.GetInt("RTX") == 1);
        else
            SetRTX(false);
    }

    public void Start()
    {
        RoomName.text = GameNetwork.CurrentRoom.Name;

        foreach (var player in GameNetwork.PlayerList)
        {
            CreatePlayerUI(player);
        }

        if (GameNetwork.PlayerList.Count > 0)
            UpdateDisplay();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            MinimumOfPlayer = 2;
            UpdateDisplay();
        }
    }

    public override void OnPlayerJoinedRoom(Player newPlayer)
    {
        if (!newPlayer.IsLocal)
        {
            CreatePlayerUI(newPlayer);
            UpdateDisplay();
        }
    }

    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        RemovePlayer(newPlayer.PlayerId);
        UpdateDisplay();
    }

    public override void OnLeaderRoomChange(Player newMasterClient)
    {
        foreach (var UI in PlayerUIs.Values)
        {
            UI.UpdateLeader();
        }

        UpdateDisplay();
    }

    private void CreatePlayerUI(Player player)
    {
        GameObject playerObj = Instantiate(PlayerUIPrefab, PlayerGrid);
        var playerUI = playerObj.GetComponent<PlayerLobbyUI>();

        playerUI.SetInfos(this, player.PlayerName, player.IsReady, player.PlayerId);
        PlayerUIs.Add(player.PlayerId, playerUI);
        ReadyPlayers.Add(player.PlayerId, player.IsReady);
    }
    
    private void RemovePlayer(int playerID)
    {
        PlayerLobbyUI playerUI;
        if (!PlayerUIs.TryGetValue(playerID, out playerUI))
            return;

        Destroy(playerUI.gameObject);
        PlayerUIs.Remove(playerID);
        ReadyPlayers.Remove(playerID);
    }

    public void TogglePlayerReady()
    {
        GameNetwork.LocalPlayer.IsReady = !GameNetwork.LocalPlayer.IsReady;
    }
    
    [BindPacket(Packet = (int)ServerPackets.updatePlayer)]
    private void UpdatePlayer(Player player)
    {
        PlayerUIs[player.PlayerId].UpdateInfo(player.IsReady);
        ReadyPlayers[player.PlayerId] = player.IsReady;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (GameNetwork.IsLeaderPlayer)
        {
            StartButton.gameObject.SetActive(true);
            if (!StartingGame && GameNetwork.PlayerList.Count >= MinimumOfPlayer && CheckAllPlayersReady())
                StartButton.interactable = true;
            else
                StartButton.interactable = false;
        }
        else
        {
            StartButton.gameObject.SetActive(false);
        }

        if (GameNetwork.PlayerList.Count < MinimumOfPlayer)
            PlayerCount.color = Color.red;
        else
            PlayerCount.color = Color.green;
        PlayerCount.text = GameNetwork.PlayerList.Count + " / " + GameNetwork.CurrentRoom.Size;
    }

    private bool CheckAllPlayersReady()
    {
        foreach (var pair in ReadyPlayers)
        {
            if (!pair.Value)
                return false;
        }
        return true;
    }

    public void TryStartGame()
    {
        StartingGame = true;
        UpdateDisplay();
        PacketWriter.TryStartGame();
    }

    [BindPacket(Packet = (int)ServerPackets.startGame)]
    private void StartGame(bool start)
    {
        if (start)
        {
            MapLoader.LoadGame();
        }
        else
        {
            StartingGame = false;
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

    public void QuitLobby()
    {
        if (NetController)
            NetController.Disconnect();
    }
}
