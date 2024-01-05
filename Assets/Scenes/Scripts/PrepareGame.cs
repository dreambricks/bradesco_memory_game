using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareGame : MonoBehaviour
{
    [SerializeField] private GameControllerScript gameController;
    [SerializeField] private GameObject backgroundred;
    [SerializeField] private ParticleSystem particles;

    private void OnEnable()
    {
        particles.Stop();
    }

    public void OnMouseDown()
    {
        backgroundred.SetActive(false);
        gameController.StartGame();
    }

}
