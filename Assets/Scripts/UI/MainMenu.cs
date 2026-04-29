using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Playables;
public class MainMenu : MonoBehaviour
{
    Button newGameBth;
    Button continueBth;
    Button quitBth;

    PlayableDirector director;

    private void Awake()
    {
        newGameBth = transform.GetChild(1).GetComponent<Button>();
        continueBth = transform.GetChild(2).GetComponent<Button>();
        quitBth = transform.GetChild(3).GetComponent<Button>();

        newGameBth.onClick.AddListener(PlayTimeline);
        continueBth.onClick.AddListener(ContinueGame);
        quitBth.onClick.AddListener(QuitGame);

        director = FindObjectOfType<PlayableDirector>();
        director.stopped += NewGame;
    }

    void PlayTimeline()
    {
        director.Play();
    }

    void NewGame(PlayableDirector obj)
    {
        PlayerPrefs.DeleteAll();
        SceneController.Instance.TransitionToFirstLevel();

    }
    void ContinueGame()
    {
        SceneController.Instance.TransitionToLoadGame();
    }
    void QuitGame()
    {
        Application.Quit();
        Debug.Log("ÍË³öÓÎÏ·");
    }
}
