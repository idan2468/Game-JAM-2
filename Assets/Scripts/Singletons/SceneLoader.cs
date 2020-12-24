using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
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
        MoveToScene(Enum.GetName(typeof(Scene),scene));
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
                break;
        }
        SceneManager.LoadScene(sceneName);
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
