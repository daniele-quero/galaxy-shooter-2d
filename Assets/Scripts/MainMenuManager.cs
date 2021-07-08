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
        if (_newGameButton == null)
            Debug.Log("No New Game Button found");

        _newGameButton.onClick.AddListener(delegate { StartGame(); });
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level_1");
    }


}
