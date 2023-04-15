using UnityEngine;
using UnityEngine.UI;
using GNet;

public class PlayerLobbyUI : MonoBehaviour
{
    [SerializeField]
    private Text PlayerName = null;
    [SerializeField]
    private Image ReadyOffImage = null;
    [SerializeField]
    private Image ReadyOnImage = null;
    [SerializeField]
    private Image LeaderImage = null;
    [SerializeField]
    private Button LeaderButton = null;
    [SerializeField]
    private Color SelfColor = new Color();

    private LobbyMenu LobbyMenu = null;
    private int PlayerId;

    public void SetInfos(LobbyMenu lobbyMenu, string playerName, bool isReady, int playerId)
    {
        LobbyMenu = lobbyMenu;

        PlayerId = playerId;

        ReadyOffImage.gameObject.SetActive(isReady);
        ReadyOnImage.gameObject.SetActive(false);

        if (PlayerName != null)
            PlayerName.text = playerName;
        if (GameNetwork.LocalPlayer.PlayerId == playerId)
        {
            PlayerName.color = SelfColor;
        }

        UpdateLeader();
        SetImageReady(isReady);
    }

    public void UpdateLeader()
    {
        if (PlayerId == GameNetwork.LeaderPlayer.PlayerId)
        {
            LeaderImage.gameObject.SetActive(true);
            LeaderButton.gameObject.SetActive(false);
        }
        else if (GameNetwork.IsLeaderPlayer)
        {
            LeaderImage.gameObject.SetActive(false);
            LeaderButton.gameObject.SetActive(true);
        }
        else
        {
            LeaderImage.gameObject.SetActive(false);
            LeaderButton.gameObject.SetActive(false);
        }
    }

    public void ClickLeaderButton()
    {
        if (GameNetwork.IsLeaderPlayer)
            GameNetwork.SetLeaderPlayer(GameNetwork.CurrentRoom.GetPlayer(PlayerId));
    }

    public void UpdateInfo(bool isReady)
    {
        SetImageReady(isReady);
    }

    private void SetImageReady(bool isReady)
    {
        ReadyOffImage.gameObject.SetActive(isReady);
        ReadyOnImage.gameObject.SetActive(false);
    }
}
