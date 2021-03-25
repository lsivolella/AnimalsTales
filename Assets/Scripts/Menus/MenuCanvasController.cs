using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCanvasController : MonoBehaviour
{
    [Header("Formating")]
    [SerializeField]
    private float yPadding = 1f;
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
    private Vector2 pawLocation;
    private Color originalColor;
    private Color selectedColor;
    private const string menuScene = "MenuScene";
    private const string gameScene = "TownScene";
    private const string controlsScene = "ControlsScene";
    private const string creditsScene = "CreditsScene";
    private string activeScene;

    // Start is called before the first frame update
    void Start()
    {
        originalColor = Color.white;
        selectedColor = Color.black;
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
        activeScene = SceneManager.GetActiveScene().name;

        switch (activeScene)
        {
            case menuScene:
                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                {
                    pawLocation = transform.position;

                    if (index == 2)
                    {
                        transform.position = new Vector2(pawLocation.x, pawLocation.y + 2 * yPadding);
                        index = 0;
                    }
                    else
                    {
                        transform.position = new Vector2(pawLocation.x, pawLocation.y - yPadding);
                        index++;
                    }
                }
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                {
                    pawLocation = transform.position;

                    if (index == 0)
                    {
                        transform.position = new Vector2(pawLocation.x, pawLocation.y - 2 * yPadding);
                        index = 2;
                    }
                    else
                    {
                        transform.position = new Vector2(pawLocation.x, pawLocation.y + yPadding);
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
