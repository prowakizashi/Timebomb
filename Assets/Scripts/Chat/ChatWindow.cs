using Assets.Scripts.GameNetwork;
using GNet;
using GNet.Packets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatWindow : MonoBehaviourNetCallBacks
{
    [SerializeField]
    private RectTransform Panel = null;

    [SerializeField]
    private Transform MessageList = null;
    [SerializeField]
    private GameObject MessagePrefab = null;

    [SerializeField]
    private Scrollbar VerticalScrollbar = null;
    
    [SerializeField]
    private InputField MessageField = null;

    [SerializeField]
    private Button ExitButton = null;
    [SerializeField]
    private Button LobbyButton = null;

    [SerializeField]
    private float ShowTime = 0.5f;

    private bool visible = false;
    private float initialPositionX;
    private Coroutine ShowCoroutine = null;

    private void Start()
    {
        VerticalScrollbar.value = 0;
        initialPositionX = Panel.localPosition.x;

        var mapLoader = FindObjectOfType<MapLoader>();
        mapLoader.ChangeMapEvent += OnMapChanged;

        LobbyButton.onClick.AddListener(() =>
        {
            GamePacketWriter.ReturnToLobby();
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            visible = !visible;

            if (ShowCoroutine != null)
                StopCoroutine(ShowCoroutine);

            ShowCoroutine = StartCoroutine(ToggleShow());
            MessageField.text = "";
        }

        if (visible && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            && (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)))
        {
            SendMessage();
        }
    }

    public void SendMessage()
    {
        string message = MessageField.text;
        if (message.Length > 0 && message[message.Length - 1] == '\n')
        {
            message = message.Substring(0, message.Length - 1);
        }

        if (message.Length != 0)
        {
            PacketWriter.SendPlayerMessage(message);
            MessageField.text = "";
        }

        MessageField.Select();
        MessageField.ActivateInputField();
    }

    [BindPacket(Packet = (int)ServerPackets.playerMessage)]
    private void UpdatePlayer(Player player, string message)
    {
        var value = VerticalScrollbar.value;
        var go = Instantiate(MessagePrefab, MessageList);
        go.GetComponentInChildren<Text>().text = "<b>" + player.PlayerName + ":</b> " + message;
        VerticalScrollbar.value = value;
    }

    private IEnumerator ToggleShow()
    {
        float width = Panel.rect.width;
        float speed = width / ShowTime * (visible ? 1 : - 1);
        while ((visible && Panel.localPosition.x < initialPositionX + width) || (!visible && Panel.localPosition.x > initialPositionX))
        {
            Panel.Translate(speed * Time.deltaTime, 0, 0, Space.Self);
            yield return null;
        }

        Panel.localPosition = new Vector3(visible ? initialPositionX + width : initialPositionX, Panel.localPosition.y, Panel.localPosition.z);
        ShowCoroutine = null;

        if (visible)
        {
            MessageField.Select();
            MessageField.ActivateInputField();
        }
    }

    private void OnMapChanged(string mapName)
    {
        ExitButton.onClick.RemoveAllListeners();
        if (mapName == "Game")
        {
            var QM = FindObjectOfType<QuitMenu>();
            if (QM != null)
                ExitButton.onClick.AddListener(QM.Open);
            LobbyButton.gameObject.SetActive(true);
        }
        else if (mapName == "Lobby")
        {
            var LM = FindObjectOfType<LobbyMenu>();
            if (LM != null)
                ExitButton.onClick.AddListener(LM.QuitLobby);
            LobbyButton.gameObject.SetActive(false);
        }
    }
}
