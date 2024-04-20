using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static Action<string, bool> SetScreen;
    
    [SerializeField]
    Animator mainScreenAnim;
    [SerializeField]
    Animator loseScreenAnim;
    [SerializeField]
    Animator winScreenAnim;
    [SerializeField]
    Animator pauseScreenAnim;

    string activeScreen = "";
    bool paused = false;

    private void Awake()
    {
        // in case I leave these off in the scene...
        mainScreenAnim.gameObject.SetActive(true);
        loseScreenAnim.gameObject.SetActive(true);
        winScreenAnim.gameObject.SetActive(true);
        pauseScreenAnim.gameObject.SetActive(true);

        SetScreen += SetScreenState;
        GameManager.OnGameOver += () => SetScreenState("lose", true);
        GameManager.OnGameWin += () => SetScreenState("win", true);
        mainScreenAnim.SetTrigger("forceOn");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && GameManager.InGame)
        {
            Pause();
        }
    }

    public void Pause(bool setInput = true)
    {
        paused = !paused;
        GameManager.AllowInput = !paused;
        Time.timeScale = paused ? 0F : 1F;
        pauseScreenAnim.SetTrigger(paused ? "on" : "off");
    }

    void SetScreenState(string key, bool enabled)
    {
        switch (key)
        {
            case "main":
                if (enabled)
                {
                    mainScreenAnim.SetTrigger("on");
                    if (activeScreen != "")
                        SetScreenState(activeScreen, false);
                    if (paused) Pause(false);
                }
                else
                {
                    if (activeScreen != "") SetScreenState(activeScreen, false);
                    else mainScreenAnim.SetTrigger("off");
                }
                break;
            case "lose":
                if (enabled)
                {
                    loseScreenAnim.SetTrigger("on");
                    AudioManager.PlayClip?.Invoke("lose");
                    activeScreen = key;
                }
                else
                {
                    loseScreenAnim.SetTrigger("off");
                    activeScreen = "";
                }
                break;
            case "win":
                if (enabled)
                {
                    winScreenAnim.SetTrigger("on");
                    AudioManager.PlayClip?.Invoke("win");
                    activeScreen = key;
                }
                else
                {
                    winScreenAnim.SetTrigger("off");
                    activeScreen = "";
                }
                break;

            default:
                Debug.LogError("Invalid screen name! " + key);
                break;
        }
    }

    void SetScreenStats()
    {
        // todo
    }
}
