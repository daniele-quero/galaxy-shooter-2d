using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Text _score;
    private Text _gameover;
    private Image _livesDisplay;

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

        _livesDisplay = transform.Find("LivesDisplay").GetComponent<Image>();
        if (_livesDisplay == null)
            Debug.LogError("No Lives Sprite found");
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
