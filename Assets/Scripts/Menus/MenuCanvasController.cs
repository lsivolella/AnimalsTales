using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCanvasController : MonoBehaviour
{
    [Header("Formating")]
    [SerializeField]
    private float yPadding = 1f;
    [SerializeField]
    private GameObject leftPaw;
    [SerializeField]
    private GameObject rightPaw;
    [Header("Menu Options")]
    [SerializeField]
    private TMP_Text startGame;
    [SerializeField]
    private TMP_Text controlls;
    [SerializeField]
    private TMP_Text credits;
    [SerializeField]
    private TMP_Text backToMenu;

    private int index = 0;
    private int menuOptions = 3;
    private Vector2 leftPawPosition;
    private Vector2 rightPawPosition;
    private Color originalColor;
    private Color selectedColor;
    private const string menuScene = "MenuScene";
    private const string gameScene = "TownScene";
    private const string controlsScene = "ControlsScene";
    private const string creditsScene = "CreditsScene";
    private string activeScene;

    private float convertedYPadding;

    // Start is called before the first frame update
    void Start()
    {
        SetDefaultVariables();
    }

    private void SetDefaultVariables()
    {
        originalColor = Color.white;
        selectedColor = Color.black;
        convertedYPadding = yPadding * Screen.height / 1080;
        activeScene = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
        GetNavigationInput();
        GetSelectionInput();
        IdentifySelectedOption();
    }

    private void GetNavigationInput()
    { 
        leftPawPosition = leftPaw.transform.position;
        rightPawPosition = rightPaw.transform.position;

        switch (activeScene)
        {
            case menuScene:
                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                {
                    if (index == menuOptions - 1)
                    {
                        leftPaw.transform.position = new Vector2(leftPawPosition.x, leftPawPosition.y + 2 * convertedYPadding);
                        rightPaw.transform.position = new Vector2(rightPawPosition.x, rightPawPosition.y + 2 * convertedYPadding);
                        index = 0;
                    }
                    else
                    {
                        leftPaw.transform.position = new Vector2(leftPawPosition.x, leftPawPosition.y - convertedYPadding);
                        rightPaw.transform.position = new Vector2(rightPawPosition.x, rightPawPosition.y - convertedYPadding);
                        index++;
                    }
                }
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                {
                    if (index == 0)
                    {
                        leftPaw.transform.position = new Vector2(leftPawPosition.x, leftPawPosition.y - 2 * convertedYPadding);
                        rightPaw.transform.position = new Vector2(rightPawPosition.x, rightPawPosition.y - 2 * convertedYPadding);
                        index = 2;
                    }
                    else
                    {
                        leftPaw.transform.position = new Vector2(leftPawPosition.x, leftPawPosition.y + convertedYPadding);
                        rightPaw.transform.position = new Vector2(rightPawPosition.x, rightPawPosition.y + convertedYPadding);
                        index--;
                    }
                }
                break;

            default:
                break;
        }
    }

    protected virtual void IdentifySelectedOption()
    {
        switch (activeScene)
        {
            case menuScene:
                if (index == 0)
                { 
                    startGame.color = selectedColor;
                    controlls.color = originalColor;
                    credits.color = originalColor;
                }
                else if (index == 1)
                {
                    startGame.color = originalColor;
                    controlls.color = selectedColor;
                    credits.color = originalColor;
                }
                else if (index == 2)
                {
                    startGame.color = originalColor;
                    controlls.color = originalColor;
                    credits.color = selectedColor;
                }
                break;

            default:
                backToMenu.color = selectedColor;
                break;
        }
    }

    protected virtual void GetSelectionInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            switch (activeScene)
            {
                case menuScene:
                    if (index == 0)
                    {
                        SceneManager.LoadScene(gameScene);
                    }
                    else if (index == 1)
                    {
                        SceneManager.LoadScene(controlsScene);
                    }
                    else if (index == 2)
                    {
                        SceneManager.LoadScene(creditsScene);
                    }
                    break;

                default:
                    SceneManager.LoadScene(menuScene);
                    break;
            } 
        }
    }
}
