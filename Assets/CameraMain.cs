using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMain : MonoBehaviour
{
    [SerializeField]Transform obs;
    [SerializeField] float aheadSpeed;
    [SerializeField] float followDamping;
    [SerializeField] float cameraHeight;

    Rigidbody _obsRigidBody;
    // Start is called before the first frame update
    void Start()
    {
        _obsRigidBody = obs.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (obs == null)
            return;

        Vector3 targetPosition = obs.position + Vector3.up * cameraHeight + _obsRigidBody.velocity * aheadSpeed;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followDamping * Time.deltaTime);
    }
}
 