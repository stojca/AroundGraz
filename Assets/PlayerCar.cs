using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCar : MonoBehaviour
{
    [SerializeField] float turnSpeed = 5;
    [SerializeField] float acceleration = 8;
    Rigidbody _rigidBody;
    Vector3 lastPosition;
    private bool enter = false;
    Quaternion targetRotation;
    float _sideSlipAmount;

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
    
    Application.LoadLevel("MenuScreen");
    //Application.LoadLevel("spajopjan");
    yield return new WaitForSeconds(3.0f);
    //this.transform.position 0 = new Vector3(this.lastPosition);
    Debug.Log("pusim ga poslije");
    
    
    acceleration = 2200;
     turnSpeed = 80;
    yield return new WaitForSeconds(5.0f);
    collision.gameObject.GetComponent<Renderer> ().enabled = true;
     
     // Code to execute after the delay
 }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "enemy3")
        {

            acceleration = 0;
            turnSpeed = 0;  
            //xApplication.LoadLevel("spajopjan");
            //Destroy(collision.gameObject);
            collision.gameObject.GetComponent<Renderer> ().enabled = false;
            
            StartCoroutine(ExecuteAfterTime(collision));
            collision.gameObject.SetActive(true);
            
    
        }
       
    }
}
