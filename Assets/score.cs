using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class score : MonoBehaviour
{
    
      public Text timerText;
     private float secondsCount = 0;
     private int minuteCount = 0;
     private int hourCount = 0;
     private float scoreStatus = 0;
     private float healthStatus = 100;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Update is called once per frame
     void Update(){
         UpdateTimerUI();
     }
 //call this on update
     public void UpdateTimerUI(){
         //set timer UI
         secondsCount += Time.deltaTime;
         timerText.text = hourCount +"h:"+ minuteCount +"m:"+(int)secondsCount + "s\nHealth:" + (int)healthStatus + "\nScore:"+(int)scoreStatus;
         if(secondsCount >= 60){
             minuteCount++;
             secondsCount = 0;
         }else if(minuteCount >= 60){
             hourCount++;
             minuteCount = 0;
         }    
     }
       void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle") {
    healthStatus -= 10;
    if(healthStatus <= 0) SceneManager.LoadScene("endGame");
    Destroy (collision.gameObject);
     }
      if (collision.gameObject.tag == "Border") {
    healthStatus -= 10;
    if(healthStatus <= 0) SceneManager.LoadScene("endGame");
     }
         if (collision.gameObject.tag == "Coin") {
    scoreStatus += 10;
    Destroy (collision.gameObject);
     }
 
    }
}