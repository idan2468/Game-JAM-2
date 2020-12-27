using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIController : Singleton<UIController>
{
    [SerializeField] private  float heartAnimationTime = 2f;
    [SerializeField] private Image[] playerLife;
    [Header("UI Canvas")] 
    [SerializeField] private GameObject gameSceneUI;
    [SerializeField] private GameObject endGameUI;
    private const int TOTAL_NUN_HEARTS = 3;
    private int _currHeartIndex;
    private int _currScore = 0;
    private Tweener _tween;
    private TextMeshProUGUI _score;

    private void Start()
    {
        _currHeartIndex = GameManager.Instance.PlayerLives - 1;
        playerLife = new Image[]{};
        LoadGameSceneUIObjects();
        SceneManager.activeSceneChanged += ((arg0, scene) =>  LoadGameSceneUIObjects());
    }

    public int GetHeartAmount()
    {
        return _currHeartIndex;
    }

    public int GetScore()
    {
        return _currScore;
    }

    private void LoadGameSceneUIObjects()
    {
        //if (SceneLoader.Instance.GetActiveScene() == SceneLoader.Scene.GameScene && playerLife.Length == 0)
        //{
        //    playerLife = GameObject.FindGameObjectsWithTag("Heart").Select(go => go.GetComponent<Image>())
        //        .ToArray();
        //}
        playerLife = GameObject.FindGameObjectsWithTag("Heart").Select(go => go.GetComponent<Image>())
                .ToArray();

        if (_score == null)
        {
            _score = GameObject.FindGameObjectWithTag("Score").GetComponent<TextMeshProUGUI>();
        }
    }

    public void LoseLife()
    {
        StartCoroutine(LoseLifeCoroutine());
    }

    private IEnumerator LoseLifeCoroutine()
    {
        if (_tween.IsActive())
        {
            yield return _tween.WaitForCompletion();    
        }
        if (_currHeartIndex >= 0)
        {
            _tween = playerLife[_currHeartIndex].DOFillAmount(0, heartAnimationTime).SetEase(Ease.OutQuad);
        }
        _currHeartIndex--;
    }

    public void UpdateScoreUI(int score)
    {
        if (_score != null)
        {
            _score.text = "Score: " + score;
        }
    }

    public void SwitchToEndGameUI()
    {
        endGameUI.SetActive(true);
        gameSceneUI.SetActive(false);
    }
}