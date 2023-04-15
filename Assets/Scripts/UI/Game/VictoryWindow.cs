using GNet;
using UnityEngine;
using UnityEngine.UI;

public class VictoryWindow : MonoBehaviour
{
    [SerializeField]
    private Text VictoryText = null;
    [SerializeField]
    private GameBoard Gameboard = null;

    [SerializeField]
    private Text OrangeList = null;
    [SerializeField]
    private Text PurpleList = null;
    [SerializeField]
    private Text RedWiresCount = null;

    [SerializeField]
    private Button RoomButton = null;
    [SerializeField]
    private Color PurpleColor = new Color();
    [SerializeField]
    private Color OrangeColor = new Color();

    public void ShowVictory(bool orangeWins, int wireCount)
    {
        gameObject.SetActive(true);

        VictoryText.text = "L'équipe <color=#" + ColorUtility.ToHtmlStringRGB(orangeWins ? OrangeColor : PurpleColor) + ">" +  (orangeWins ? "Orange" : "Violette") + "</color> a gagné !";

        string orangeNames = "";
        string purplesNames = "";
        
        foreach (Player player in GameNetwork.PlayerList)
        {
            var board = Gameboard.GetPlayerBoard(player.PlayerId);
            if (board.PlayerRole)
            {
                if (orangeNames != "")
                    orangeNames += "\n";
                orangeNames += board.GetPlayerName();
            }
            else
            {
                if (purplesNames != "")
                    purplesNames += "\n";
                purplesNames += board.GetPlayerName();
            }
        }

        RedWiresCount.text = "x" + wireCount;

        OrangeList.text = orangeNames;
        PurpleList.text = purplesNames;

        RoomButton.interactable = false;
        RoomButton.gameObject.SetActive(GameNetwork.IsLeaderPlayer);
    }

    public void ShowRoomButton()
    {
        if (GameNetwork.IsLeaderPlayer)
            RoomButton.interactable = true;
    }


    public void BackToLobby()
    {
        GameManager.Instance.AskBackToLobby();
    }
}
