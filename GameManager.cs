using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public CanvasGroup gameCanvas;
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public int gameTime;
    public GameObject starPS;
    public Button startBtn;
    public TMP_Text titleText;
    public CanvasGroup titlePanel, endPanel;
    public Transform buttons;
    public AudioClip finishSFX;
    public TMP_Text countdownText;
    public SpriteRenderer chicken;
    public CanvasGroup fadePanel;
    public static int score;
    AudioSource audioSource;
    Vector2 btnsOrigianlPos;
    Vector2 scoreOriginalPos;
    float gameEndT;
    bool gameStart;
    Coroutine scoreAnimation;
    bool firstStart;

    void Awake()
    {
        fadePanel.alpha = 1;
        Instance = this;
    }

    void Start()
    {
        fadePanel.DOFade(0, 1f);
        chicken.DOFade(1, 1f);
        firstStart = true;
        btnsOrigianlPos = buttons.transform.localPosition;
        scoreOriginalPos = scoreText.transform.parent.localPosition;
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        audioSource.DOFade(1, 2f);
    }

    void Update()
    {
        scoreText.text = score.ToString();
        if (gameStart)
            timeText.text = ((int)Mathf.Clamp(gameEndT - Time.time, 0, gameTime)).ToString();
        else
            timeText.text = "";

        if (gameStart && Time.time > gameEndT)
        {
            countdownText.text = "";
            gameStart = false;
            gameCanvas.blocksRaycasts = false;

            StartCoroutine(GameFinish());
        }
        else if (gameStart && Time.time >= gameEndT - 10)
        {
            countdownText.text = timeText.text;
        }

        if (!gameStart)
            gameCanvas.blocksRaycasts = false;
    }

    public void StartGame()
    {
        if (firstStart)
        {
            titlePanel.DOFade(0, 0.3f);
            titlePanel.blocksRaycasts = false;
            titleText.DOFade(0, 0.3f);
            buttons.DOLocalMoveX(730, 0.4f);
            firstStart = false;
        }
        else
        {
            endPanel.DOFade(0, 0.3f);
            endPanel.blocksRaycasts = false;
            scoreText.transform.parent.DOLocalMove(scoreOriginalPos, 0.3f);
            buttons.DOLocalMoveX(730, 0.4f);
        }


        score = 0;
        gameStart = true;
        gameEndT = Time.time + gameTime;
        startBtn.interactable = false;
        LevelManager.Instance.StartLevel();
        gameCanvas.blocksRaycasts = true;
    }

    IEnumerator GameFinish()
    {
        endPanel.DOFade(1, 0.5f);
        endPanel.blocksRaycasts = true;
        scoreText.transform.parent.DOLocalMove(titleText.transform.localPosition + new Vector3(0, 40, 0), 0.7f);
        buttons.DOLocalMove(btnsOrigianlPos, 0.7f);
        yield return new WaitForSeconds(0.7f);
        yield return new WaitUntil(() => !Board.isMatching);
        audioSource.PlayOneShot(finishSFX, 0.2f);
        RegisterTween(scoreText.gameObject, scoreText.transform.DOPunchScale(new Vector3(1.2f, 1.2f, 0), 0.8f, 10, 1));
        Instantiate(starPS, scoreText.transform.position, Quaternion.identity, scoreText.transform);
        yield return new WaitForSeconds(0.4f);
        startBtn.interactable = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void AddScore(int num)
    {
        score += 100 * num;
        RegisterTween(scoreText.gameObject, scoreText.transform.DOPunchScale(new Vector3(0.6f, 0.6f, 0), 0.5f, 10, 1));
        Instantiate(starPS, scoreText.transform.position, Quaternion.identity, scoreText.transform);
    }

    static Dictionary<GameObject, Tween> tweenDict = new Dictionary<GameObject, Tween>();

    public static void RegisterTween(GameObject gameObject, Tween tween)
    {
        if (tweenDict.ContainsKey(gameObject))
        {
            tweenDict[gameObject].Kill(false);
            tweenDict[gameObject] = tween;
        }
        else
            tweenDict.Add(gameObject, tween);
    }
}
