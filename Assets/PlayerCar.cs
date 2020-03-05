using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class PlayerCar : MonoBehaviour
{
    [SerializeField] float turnSpeed = 5;
    [SerializeField] float acceleration = 8;
    Rigidbody _rigidBody;
    Vector3 lastPosition;
    private bool enter = false;
    Quaternion targetRotation;
    float _sideSlipAmount;
    [SerializeField] GameObject gameOverObject;

    public GameObject questionDisplay;
    public GameObject gameDisplay;
    public GameObject coolDown;

    public Text questionDisplayText;
    public Text timeRemainingDisplayText;

    public TextMeshProUGUI scoreDisplayText;
    public TextMeshProUGUI gameTimeDisplay;

    public SimpleObjectPool answerButtonObjectPool;
    public Transform answerButtonParent;

    private DataController dataController;
    private RoundData currentRoundData;
    private QuestionData[] questionPool;

    private bool isRoundActive;
    private bool isCarActive = true;
    private float timeRemainingQuestion;
    private float timeRemainingGame = 45.0f;
    //private int questionIndex = 0;
    //private int playerScore = 0;
    private List<GameObject> answerButtonGameObjects = new List<GameObject>();
    public GameManager gm;

    private float secondsCount = 0;
    private int minuteCount = 0;
    private int hourCount = 0;
    private float scoreStatus = 0;
    private float healthStatus = 100;

    public GameObject pauseScreen;
    public static bool isPaused = false;


    // Use this for initialization
    public void start_questions_game()
    {
        dataController = FindObjectOfType<DataController>();
        currentRoundData = dataController.GetCurrentRoundData();
        questionPool = currentRoundData.questions;
        timeRemainingQuestion = 3.0f;
        UpdateTimeRemainingDisplay();

        //playerScore = 0;
        //questionIndex = 0;

        questionDisplay.SetActive(true);
        //gameDisplay.SetActive (false);

        ShowQuestion();
        isRoundActive = true;

    }

    private void ShowQuestion()
    {
        RemoveAnswerButtons();
        QuestionData questionData = questionPool[PlayerStats.questionIndex];
        //Debug.Log(questionPool);
        //Debug.Log(questionIndex);
        questionDisplayText.text = questionData.questionText;

        for (int i = 0; i < questionData.answers.Length; i++)
        {
            GameObject answerButtonGameObject = answerButtonObjectPool.GetObject();
            answerButtonGameObjects.Add(answerButtonGameObject);
            answerButtonGameObject.transform.SetParent(answerButtonParent);

            AnswerButton answerButton = answerButtonGameObject.GetComponent<AnswerButton>();
            answerButton.Setup(questionData.answers[i]);

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
            PlayerStats.playerScore += currentRoundData.pointsAddedForCorrectAnswer;
            UpdateScore();
            checkScore();
        }
        else
        {
            PlayerStats.playerScore -= 3;
            UpdateScore();
        }

        if (questionPool.Length > PlayerStats.questionIndex + 1)
        {
            PlayerStats.questionIndex++;
            //ShowQuestion();
        }
        else
        {
            EndRound();
        }

    }

    public void EndRound()
    {
        isRoundActive = false;

        questionDisplay.SetActive(false);
        //gameDisplay.SetActive (true);
    }

    public void EndGame()
    {
        isCarActive = false;
        gameOverObject.SetActive(true);
        acceleration = 0;
        turnSpeed = 0;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScreen");
    }

    private void UpdateTimeRemainingDisplay()
    {
        timeRemainingDisplayText.text = "Time: " + Mathf.Round(timeRemainingQuestion).ToString();
    }

    private void UpdateTimeRemainingDisplayGAME()
    {
        gameTimeDisplay.text = "Time: " + Mathf.Round(timeRemainingGame).ToString();
    }

    private void UpdateScore()
    {
        scoreDisplayText.text = "Score: " + PlayerStats.playerScore.ToString();
    }


    // #################################################################
    public float SlideSlipAmount()
    {
        return _sideSlipAmount;
    }

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        UpdateScore();
    }

    public void UpdateTimerUI()
    {
        //set timer UI
        secondsCount += Time.deltaTime;
        //timerText.text = "m:"+(int)secondsCount + "s\nHealth:" + (int)healthStatus + "\nScore:"+(int)scoreStatus;
        if (secondsCount >= 60)
        {
            minuteCount++;
            secondsCount = 0;
        }
        else if (minuteCount >= 60)
        {
            hourCount++;
            minuteCount = 0;
        }
    }

    public void continueGame()
    {

        acceleration = 2200;
        turnSpeed = 80;
        isPaused = false;
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
    }

    public void retryGame()
    {
        SceneManager.LoadScene("InnerStadt");
        Time.timeScale = 1f;
        acceleration = 2200;
        turnSpeed = 80;
        PlayerStats.playerScore = 0;
        PlayerStats.questionIndex = 0; 

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                continueGame();
            }
            else
            {
                print("space key was pressed");
                pauseScreen.SetActive(true);
                isPaused = true;
                acceleration = 0;
                turnSpeed = 0;
                Time.timeScale = 0f;
            }

        }

        //UpdateTimerUI();
        if (isCarActive)
        {
            timeRemainingGame -= Time.deltaTime;
            UpdateTimeRemainingDisplayGAME();
            //UpdateScore();
            if (timeRemainingGame <= 0f)
            {
                EndGame();
            }
        }


        if (isRoundActive)
        {
            timeRemainingQuestion -= Time.deltaTime;
            UpdateTimeRemainingDisplay();

            if (timeRemainingQuestion <= 0f)
            {
                EndRound();
            }
        }
        SetRotationPoint();
        SetSlideSlip();
    }

    private void checkScore()
    {
        if(PlayerStats.playerScore > 9)
        {
            SceneManager.LoadScene("Jakomini");
        }
        if(PlayerStats.playerScore > 25)
        {
            SceneManager.LoadScene("StPeter");
        }
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
        Debug.Log("Your enter Coroutine at" + Time.time);
        Debug.Log(this.lastPosition);

        start_questions_game();

        yield return new WaitForSeconds(timeRemainingQuestion);
        acceleration = 2200;
        turnSpeed = 80;
        yield return new WaitForSeconds(5.0f);
        collision.gameObject.SetActive(true);
    }

    IEnumerator ExecuteAfterEnemy()
    {
        coolDown.SetActive(true);
        acceleration = 1000;
        turnSpeed = 25;
        yield return new WaitForSeconds(3.0f);
        acceleration = 2200;
        turnSpeed = 80;
        coolDown.SetActive(false);

    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Coin")
        {
            PlayerStats.playerScore += 10;
            UpdateScore();
            Destroy(collision.gameObject);
            checkScore();
        }
          if (collision.gameObject.tag == "Enemy")
        {
            PlayerStats.playerScore -= 10;
            UpdateScore();
            Destroy(collision.gameObject);
        }
         if(collision.gameObject.tag == "Time")
        {
             UpdateTimeRemainingDisplayGAME();
             timeRemainingGame += 5f;
            Destroy(collision.gameObject);
        }   
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Question")
        {

            acceleration = 0;
            turnSpeed = 0;
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
            //collision.gameObject.GetComponent<Renderer> ().enabled = false;
            StartCoroutine(ExecuteAfterTime(collision));
            //collision.gameObject.SetActive(true);
        }
        if (collision.gameObject.tag == "Roadsign")
        {
            Destroy(collision.gameObject);

            StartCoroutine(ExecuteAfterEnemy());
        }
        if (collision.gameObject.tag == "Border")
        {
            healthStatus -= 10;
            if (healthStatus <= 0)
                gameOverObject.SetActive(true);
        }
    }

}