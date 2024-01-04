using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallToAction : MonoBehaviour
{
    [SerializeField] private GameControllerScript gameController;

    public void OnMouseDown()
    {
        gameController.PrepareGame();
    }

}
