using System;
using DG.Tweening;
using Singletons;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    private static Transform m_DestroyOnLoadGO;

    private void Start()
    {
        MusicController.Instance.PlayMenuBGM();
    }

    public enum Scene
    {
        StartScene,
        GameScene,
    };

    public void MoveToScene(Scene scene)
    {
        MoveToScene(Enum.GetName(typeof(Scene), scene));
    }

    public static void DestroyOnLoad(GameObject aGO)
    {
        if (m_DestroyOnLoadGO == null)
            m_DestroyOnLoadGO = (new GameObject("DestroyOnLoad")).transform;
        aGO.transform.parent = m_DestroyOnLoadGO;
    }

    public void MoveToScene(string sceneName)
    {
        switch (sceneName)
        {
            case "StartScene":
                MusicController.Instance.PlayMenuBGM();
                break;
            case "GameScene":
                MusicController.Instance.PlayGameBGM();
                ResetGameSceneObjects();
                break;
        }

        DOTween.KillAll();
        SceneManager.LoadScene(sceneName);
    }

    private void ResetGameSceneObjects()
    {
        var gameManager = FindObjectOfType<GameManager>();
        var uiController = FindObjectOfType<UIController>();
        if (gameManager == null)
        {
            Debug.LogWarning("Game manager not found in scene");
        }
        else
        {
            DestroyOnLoad(gameManager.gameObject);
        }

        if (uiController == null)
        {
            Debug.LogWarning("UIController not found in scene");
        }
        else
        {
            DestroyOnLoad(uiController.gameObject);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }


    public Scene GetActiveScene()
    {
        var sceneName = SceneManager.GetActiveScene().name;
        return Enum.GetName(typeof(Scene), Scene.GameScene) == sceneName ? Scene.GameScene : Scene.StartScene;
    }
}