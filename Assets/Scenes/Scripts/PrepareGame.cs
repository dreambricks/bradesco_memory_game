using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareGame : MonoBehaviour
{
    [SerializeField] private GameControllerScript gameController;

    public void OnMouseDown()
    {
        gameController.StartGame();
    }

}
