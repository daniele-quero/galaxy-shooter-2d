using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private Text _score;
    private Text _level;
    private Text _gameover;
    private Text _restart;
    private Image _livesDisplay;

    private bool _isGameOver = false;

    [SerializeField]
    private Sprite[] _livesSprite = new Sprite[4];
    public int scorePadding = 0;

    void Start()
    {
        _score = transform.Find("Score").GetComponent<Text>();
        if (_score == null)
            Debug.LogError("No Score Text found");

        _gameover = transform.Find("GameOver").GetComponent<Text>();
        if (_gameover == null)
            Debug.LogError("No Game Over Text found");

        _gameover.enabled = false;

        _restart = transform.Find("Restart").GetComponent<Text>();
        if (_restart == null)
            Debug.LogError("No Restart Text found");

        _restart.enabled = false;

        _level = transform.Find("Level").GetComponent<Text>();
        if (_level == null)
            Debug.LogError("No Level Text found");

        _level.text = "Level " + SceneManager.GetActiveScene().buildIndex;
        _level.rectTransform.sizeDelta = new Vector2(_level.rectTransform.sizeDelta.x / 7f * _level.text.Length, 
            _level.rectTransform.sizeDelta.y);
        _level.enabled = true;

        StartCoroutine(FadeLevelText());

        _livesDisplay = transform.Find("LivesDisplay").GetComponent<Image>();
        if (_livesDisplay == null)
            Debug.LogError("No Lives Sprite found");
    }

    private void Update()
    {
        if (_isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Level_1");
        }
    }

    private IEnumerator FadeLevelText()
    {
        Color textColor = _level.color;
       for(float a = 1; a >= 0; a -= 0.02f)
        {
            Color newColor = new Color(textColor.r, textColor.g, textColor.b, a);
            _level.color = newColor;
            yield return new WaitForSeconds(0.025f);
        }

        GameObject.Destroy(_level.gameObject);

    }

    public void UpdateScoreText(int score)
    {
        _score.text = "Score: " + score.ToString().PadLeft(scorePadding, '0');
    }

    public void UpdateLivesDisplay(int livesLeft)
    {
        if (livesLeft >= 0 && livesLeft < 4)
            _livesDisplay.sprite = _livesSprite[livesLeft];

        else if (livesLeft < 0)
        {
            _livesDisplay.enabled = false;
            StartCoroutine(GameOverFlick());
            _restart.enabled = true;
            _isGameOver = true;
        }
    }

    private IEnumerator GameOverFlick()
    {
        while (true)
        {
            _gameover.enabled = true;
            yield return new WaitForSeconds(0.5f);
            _gameover.enabled = false;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
