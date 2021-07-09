using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    private Button _newGameButton;
    void Start()
    {
        _newGameButton = transform.Find("NewGameButton").GetComponent<Button>();
        Utilities.CheckNullGrabbed(_newGameButton, "New Game Button");
        _newGameButton.onClick.AddListener(delegate { StartGame(); });
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level_1");
    }


}
