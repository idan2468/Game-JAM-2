using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
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
    private int _currLife = TOTAL_NUN_HEARTS;
    private int _currScore = 0;
    private Tweener _tween;
    private TextMeshProUGUI _score;

    private void Start()
    {
        AddScore(0);
        playerLife = new Image[]{};
        LoadGameSceneUIObjects();
        SceneManager.activeSceneChanged += ((arg0, scene) =>  LoadGameSceneUIObjects());
    }

    public int GetHeartAmount()
    {
        return _currLife;
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
        _currLife--;
        if (_currLife >= 0)
        {
            _tween = playerLife[_currLife].DOFillAmount(0, heartAnimationTime).SetEase(Ease.OutQuad);
        }
    }

    public void AddScore(int scoreToAdd)
    {
        _currScore += scoreToAdd;
        if (_score != null)
        {
            _score.text = "Score: " + _currScore;
        }
    }

    public void SwitchToEndGameUI()
    {
        endGameUI.SetActive(true);
        gameSceneUI.SetActive(false);
    }
}