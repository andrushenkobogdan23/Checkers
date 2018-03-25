using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject MainMenuPanel;
    [SerializeField] GameObject GamePanel;
    [SerializeField] Text textPlayer1count;
    [SerializeField] Text textPlayer2count;
    [SerializeField] Text textStep;
    [SerializeField] Text textWarning;
    [SerializeField] Board board;
    private int player1CheckerCount;
    private int player2CheckerCount;

    public void Start()
    {
        textStep.text = "Player 1 Step";
        textPlayer1count.text = "12";
        textPlayer2count.text = "12";
    }

    public void StartGame()
    {
        MainMenuPanel.SetActive(false);
        GamePanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    public void UpdeteText()
    {
        player1CheckerCount =
            board.DictionaryChecker.Values.Count(c => c != null && c.ChekerColor == ChekerColor.White);
        player2CheckerCount =
            board.DictionaryChecker.Values.Count(c => c != null && c.ChekerColor == ChekerColor.Black);
        textPlayer1count.text = player1CheckerCount.ToString();
        textPlayer2count.text = player2CheckerCount.ToString();

        textStep.text = board.curentStroke == ChekerColor.White ? "Player 1 Step" : "Player 2 Step";

        if (board.StepIsLocked)
        {
            textWarning.gameObject.SetActive(true);
            textWarning.text = "Lock Step must beat";
        }
        else textWarning.gameObject.SetActive(false);

        if (!board.DictionaryChecker.Values.Any(c => c != null && c.ChekerColor == ChekerColor.Black))
        {
            textWarning.gameObject.SetActive(true);
            textWarning.text = "  Player 1 win!";
        }
        else if (!board.DictionaryChecker.Values.Any(c => c != null && c.ChekerColor == ChekerColor.White))
        {
            textWarning.gameObject.SetActive(true);
            textWarning.text = "  Player 2 win!";
        }
    }
}