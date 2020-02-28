using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour {

    public Text answerText;

    private AnswerData answerData;
    private PlayerCar gameController;

    // Use this for initialization
    void Start () 
    {
        gameController = FindObjectOfType<PlayerCar> ();
    }

    public void Setup(AnswerData data)
    {
        answerData = data;
        answerText.text = answerData.answerText;
    }

    public void HandleClick()
    {
        gameController.AnswerButtonClicked(answerData.isCorrect);
    }
}