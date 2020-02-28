using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerCar : MonoBehaviour
{
    [SerializeField] float turnSpeed = 5;
    [SerializeField] float acceleration = 8;
    Rigidbody _rigidBody;
    Vector3 lastPosition;
    private bool enter = false;
    Quaternion targetRotation;
    float _sideSlipAmount;

    public GameObject questionDisplay;
    public GameObject gameDisplay;
    public GameObject coolDown;

    public Text questionDisplayText;
    public Text scoreDisplayText;
    public Text timeRemainingDisplayText;
    public SimpleObjectPool answerButtonObjectPool;
    public Transform answerButtonParent;

    private DataController dataController;
    private RoundData currentRoundData;
    private QuestionData[] questionPool;

    private bool isRoundActive;
    private float timeRemaining;
    private int questionIndex;
    private int playerScore;
    private List<GameObject> answerButtonGameObjects = new List<GameObject>();

    // Use this for initialization
    public void Start_GAME () 
    {
        dataController = FindObjectOfType<DataController> ();
        currentRoundData = dataController.GetCurrentRoundData ();
        questionPool = currentRoundData.questions;
        timeRemaining = currentRoundData.timeLimitInSeconds;
        //UpdateTimeRemainingDisplay();

        playerScore = 0;
        questionIndex = 0;

        questionDisplay.SetActive (true);
        //gameDisplay.SetActive (false);

        ShowQuestion ();
        isRoundActive = true;

    }

    private void ShowQuestion()
    {
        RemoveAnswerButtons ();
        QuestionData questionData = questionPool [questionIndex];
        questionDisplayText.text = questionData.questionText;

        for (int i = 0; i < questionData.answers.Length; i++) 
        {
            GameObject answerButtonGameObject = answerButtonObjectPool.GetObject();
            answerButtonGameObjects.Add(answerButtonGameObject);
            answerButtonGameObject.transform.SetParent(answerButtonParent);

            AnswerButton answerButton = answerButtonGameObject.GetComponent<AnswerButton>();
            answerButton.Setup(questionData.answers[i]);

            Debug.Log("if" + questionData.answers.Length);
            if(i == questionData.answers.Length - 1)
            {
                Debug.Log("if");
                //EndRound();
               
            }
            
        }
    }

    private void RemoveAnswerButtons()
    {
        while (answerButtonGameObjects.Count > 0) 
        {
            answerButtonObjectPool.ReturnObject(answerButtonGameObjects[0]);
            answerButtonGameObjects.RemoveAt(0);
        }
    }

    public void AnswerButtonClicked(bool isCorrect)
    {
        if (isCorrect) 
        {
            playerScore += currentRoundData.pointsAddedForCorrectAnswer;
            scoreDisplayText.text = "Score: " + playerScore.ToString();
        }

        if (questionPool.Length > questionIndex + 1) {
            questionIndex++;
            ShowQuestion ();
        } else 
        {
            EndRound();
        }

    }
    
    public void EndRound()
    {
        isRoundActive = false;

        questionDisplay.SetActive (false);
        //gameDisplay.SetActive (true);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene ("MenuScreen");
    }

    private void UpdateTimeRemainingDisplay()
    {
        timeRemainingDisplayText.text = "Time: " + Mathf.Round (timeRemaining).ToString ();
    }


    // #################################################################
    public float SlideSlipAmount()
    {
            return _sideSlipAmount;
    }

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isRoundActive) 
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimeRemainingDisplay();

            if (timeRemaining <= 0f)
            {
                EndRound();
            }

        }
        SetRotationPoint();
        SetSlideSlip();
    }

    private void SetSlideSlip()
    {
        Vector3 direction = transform.position - lastPosition;
        Vector3 movement = transform.InverseTransformDirection(direction);
        lastPosition = transform.position;

        _sideSlipAmount = movement.x;
    }

    private void SetRotationPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float distance;

        if (plane.Raycast(ray, out distance))
        {
            Vector3 target = ray.GetPoint(distance);
            Vector3 direction = target - transform.position;
            float rotationAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            targetRotation = Quaternion.Euler(0, rotationAngle, 0);
        }
    }

    private void FixedUpdate()
    {

        float speed = _rigidBody.velocity.magnitude / 1000;
        float accelerationInput = acceleration * (Input.GetMouseButton(0) ? 1 : Input.GetMouseButton(1) ? -1 : 0) * Time.fixedDeltaTime;
        _rigidBody.AddRelativeForce(Vector3.forward * accelerationInput);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Mathf.Clamp(speed, -1, 1) * Time.fixedDeltaTime);
    }

IEnumerator ExecuteAfterTime(Collision collision)
 {
               
    Debug.Log("pusim ga");
    Debug.Log("Your enter Coroutine at" + Time.time);
    Debug.Log(this.lastPosition);
    
    //Application.LoadLevel("Game");
    //Application.LoadLevel("spajopjan");
    
    Start_GAME();
    

    yield return new WaitForSeconds(timeRemaining);
    //this.transform.position 0 = new Vector3(this.lastPosition);
    Debug.Log("pusim ga poslije");
    
    
    acceleration = 2200;
    turnSpeed = 80;
    yield return new WaitForSeconds(5.0f);

    collision.gameObject.GetComponent<Renderer> ().enabled = true;
     
     // Code to execute after the delay
 }

 IEnumerator ExecuteAfterEnemy()
 {
     Debug.Log("pusim ga prije");
    coolDown.SetActive(true);
    yield return new WaitForSeconds(6.0f);
    coolDown.SetActive(false);
    Debug.Log("pusim ga poslije");
    acceleration = 2200;
    turnSpeed = 80;
 }


    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Question")
        {

            acceleration = 0;
            turnSpeed = 0;  
            //xApplication.LoadLevel("spajopjan");
            //Destroy(collision.gameObject);
            collision.gameObject.GetComponent<Renderer> ().enabled = false;


            StartCoroutine(ExecuteAfterTime(collision));
            //collision.gameObject.SetActive(true);
            
    
        }
        if(collision.gameObject.tag == "Enemy")
        {
            acceleration = 800;
            turnSpeed = 20;
            //Start_Writing();
            StartCoroutine(ExecuteAfterEnemy());
        }
       
    }

}
