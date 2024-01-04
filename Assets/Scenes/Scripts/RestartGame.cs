using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{

    [SerializeField] private TMPro.TextMeshPro totalTime;
    [SerializeField] private TMPro.TextMeshPro timerText;

    private void Update()
    {
        totalTime.text = timerText.text;
    }
    public void OnMouseDown()
    {
        SceneManager.LoadScene("MainScene");
    }
}
