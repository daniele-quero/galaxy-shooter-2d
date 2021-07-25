using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class UIManager : MonoBehaviour
{
    private Text _score;
    private Text _level;
    private Text _gameover;
    private Text _restart;
    private Image _livesDisplay;
    private Animator _pauseAnimator;

    private bool _isGameOver = false;

    GameObject _shieldBar;

    [SerializeField]
    private Sprite[] _livesSprite = new Sprite[4];
    public int scorePadding = 0;

    void Start()
    {
        _score = transform.Find("Score").GetComponent<Text>();
        Utilities.CheckNullGrabbed(_score, "Score Text");

        _gameover = transform.Find("GameOver").GetComponent<Text>();
        Utilities.CheckNullGrabbed(_gameover, "Game Over Text");
        _gameover.enabled = false;

        _restart = transform.Find("Restart").GetComponent<Text>();
        Utilities.CheckNullGrabbed(_restart, "Restart Text");
        _restart.enabled = false;

        _livesDisplay = transform.Find("LivesDisplay").GetComponent<Image>();
        Utilities.CheckNullGrabbed(_livesDisplay, "Lives Display Sprite");

        _level = transform.Find("Level").GetComponent<Text>();
        Utilities.CheckNullGrabbed(_level, "Level Text");
        _level.text = "Level " + SceneManager.GetActiveScene().buildIndex;
        _level.rectTransform.sizeDelta = new Vector2(_level.rectTransform.sizeDelta.x / 7f * _level.text.Length,
            _level.rectTransform.sizeDelta.y);
        _level.enabled = true;

        _pauseAnimator = transform.Find("PauseMenu").GetComponent<Animator>();
        Utilities.CheckNullGrabbed(_pauseAnimator, "Pause Animator");

        StartCoroutine(FadeLevelText());

        _shieldBar = transform.Find("Shields Bar").gameObject;
        Utilities.CheckNullGrabbed(_shieldBar, "Shields Bar");
    }

    private void Update()
    {
        if (_isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.SetFloat("BkgMusic", 0);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Level_1");
        }

        PauseMenu();
    }

    private IEnumerator FadeLevelText()
    {
        Color textColor = _level.color;
        for (float a = 1; a >= 0; a -= 0.02f)
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

    private void PauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            _pauseAnimator.SetTrigger("onPause");

        TogglePause();
    }

    private void TogglePause()
    {
        if (transform.Find("PauseMenu").GetComponent<RectTransform>().anchoredPosition.y == 0)
            Time.timeScale = 0;

        else if (transform.Find("PauseMenu").GetComponent<RectTransform>().anchoredPosition.y > 1500)
            Time.timeScale = 1f;
    }

    public void Resume()
    {
        _pauseAnimator.SetTrigger("onPause");
    }
    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ActivateShieldBarActivation()
    {  
        _shieldBar.SetActive(true);
    }

    public void DeactivateShieldBarActivation()
    {
        _shieldBar.SetActive(false);
    }

    public Text GetShieldBarText()
    {
        return _shieldBar.transform.Find("Shields Text").GetComponent<Text>();
    }

    public SimpleHealthBar GetShieldBar()
    {
        return _shieldBar.GetComponentInChildren<SimpleHealthBar>();
    }
}
