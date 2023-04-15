using Assets.Scripts.GameNetwork;
using GNet;
using System;
using UnityEngine;

public class NetworkController : MonoBehaviourNetCallBacks
{
    [SerializeField]
    private int RoomNameLength = 6;
    [SerializeField]
    private GameObject ChatPrefab = null;

    private MapLoader MapLoader = null;

    private Action<string> CreatePrivateFail = null;
    private Action<string> JoinPrivateFail = null;
    
    private char[] Letters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    public char[] AllowedLetters { get { return Letters; } }


    public ChatWindow ChatWindow { get; private set; }


    private void Awake()
    {
        ChatWindow = null;

        var loaders = FindObjectsOfType<MapLoader>();
        if (loaders.Length > 0)
            MapLoader = loaders[0];

        GameNetwork.AddPacketReader(typeof(GamePacketReader));
    }

    private void OnDestroy()
    {
        GameNetwork.RemovePacketReader(typeof(GamePacketReader));
    }

    private void Start()
    {
        UnityEngine.Object.DontDestroyOnLoad(this);
        GameNetwork.ConnectToServer();
    }

    public void CreatePrivateRoom(int roomSize, Action<string> failCallback)
    {
        CreatePrivateFail = failCallback;
        GameNetwork.CreateRoom(GenerateRoomName(), roomSize, false, true);
    }

    public override void OnFailedToCreateRoom()
    {
        CreatePrivateFail?.Invoke("Oooooh non ! Je n'ai pas pu trouvé la partie !");
    }

    public void JoinPrivateRoom(string roomName, Action<string> failCallback)
    {
        JoinPrivateFail = failCallback;
        GameNetwork.JoinRoom(roomName);
    }

    public override void OnJoinRoom(Room room)
    {
        MapLoader.LoadLobby();

        var go = Instantiate(ChatPrefab);
        ChatWindow = go.GetComponent<ChatWindow>();
        UnityEngine.Object.DontDestroyOnLoad(go);
    }

    public override void OnLeaveRoom()
    {
        RemoveChat();
    }

    public override void OnFailedToJoinRoom(int code)
    {
        switch (code)
        {
            case 2:
                JoinPrivateFail?.Invoke("Quel manque d'originalité, ton nom est déjà pris dans cette partie!");
                break;
            case 1:
                JoinPrivateFail?.Invoke("Zut la partie est pleine!");
                break;
            case 0:
            default:
                JoinPrivateFail?.Invoke("Tu as probablement entré un code incorrect!");
                break;
        }
    }

    public string GenerateRoomName()
    {
        UInt64 timestamp = (UInt64)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
        var modValue = (UInt64)Mathf.Pow(Letters.Length, RoomNameLength);

        int letterIndex = 0;
        string roomName = "";
        for (int i = 0; i < RoomNameLength; ++i)
        {
            timestamp %= modValue;
            modValue /= (ulong)Letters.Length;
            letterIndex = Mathf.Clamp((int)(timestamp / modValue), 0, Letters.Length - 1);
            roomName += Letters[letterIndex];
        }

        return roomName;
    }

    public void Disconnect()
    {
        GameNetwork.LeaveRoom();
        RemoveChat();
        MapLoader.LoadMainMenu();
    }

    private void RemoveChat()
    {
        if (ChatWindow != null)
        {
            DestroyImmediate(ChatWindow.gameObject);
            ChatWindow = null;
        }
    }

    public void UpdatePlayerName(string nickName)
    {
        if (nickName == "")
            nickName = "J'ai pas de nom";
        GameNetwork.PlayerName = nickName;
    }
}
