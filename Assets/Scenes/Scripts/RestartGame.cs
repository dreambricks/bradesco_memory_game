using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{

    [SerializeField] private TMPro.TextMeshPro totalTime;
    [SerializeField] private TMPro.TextMeshPro timerText;
    [SerializeField] private GameObject preparegame;

    public float timeLeft;

    private void Update()
    {
        totalTime.text = timerText.text;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0.0f)
        {
            LogUtil.SendLog(StatusEnum.Jogou, totalTime.text);
            SceneManager.LoadScene("MainScene");
        }
    }
    public void OnMouseDown()
    {
        LogUtil.SendLog(StatusEnum.Jogou, totalTime.text);
        SceneManager.LoadScene("PlayAgain");
    }
}
