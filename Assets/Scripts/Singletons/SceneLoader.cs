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
        EndScene,
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
            case "EndScene":
                MusicController.Instance.PlayGameBGM();
                break;
        }
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
}
