using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    bool gameHasEnded = false;
    public void endGame() {
        if(gameHasEnded == false)
        {
            gameHasEnded = true;
            SceneManager.LoadScene("EndGame");
            Debug.Log("end");
            //restartGame();
        }
    }

    void restartGame() {
        SceneManager.LoadScene("MainScene");
    }
}
