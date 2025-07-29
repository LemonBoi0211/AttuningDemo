using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ActionCircleRotate : MonoBehaviour
{
    [SerializeField] private GameObject rotatePointer;
    [SerializeField] private GameObject goodHitArea;
    [SerializeField] private GameObject perfectHitArea;
    [SerializeField] private GameObject missHitArea;

    [SerializeField] public Rigidbody rotatorRB;

    [SerializeField] InputActionAsset playerControls;
    private InputAction actionAction;

    private float anglesPerSecond = 90.0f;
    public bool isRotating = true;
    public bool attuned = false;

    void Start()
    {
        actionAction = playerControls.FindActionMap("Player").FindAction("Action");

        //Randomly sets the rotation for the good/perfect hit areas
        Vector3 ranRotation = goodHitArea.transform.eulerAngles;
        ranRotation.z = Random.Range(0, 360);
        goodHitArea.transform.eulerAngles = ranRotation;

    }

    private void OnEnable()
    {
        actionAction.Enable();
    }

    private void OnDisable()
    {
        actionAction.Disable();
    }

    private void Update()
    {
        OnStopSkillRotator();
    }

    IEnumerator ActiveObject()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }

    void OnStopSkillRotator()
    {
        if (gameObject.activeInHierarchy && isRotating)
        {
            //rotates the pointer constantly
            Vector3 rotation = rotatorRB.transform.eulerAngles;
            rotation.z -= Time.deltaTime * anglesPerSecond;
            rotatorRB.transform.eulerAngles = rotation;

            if (actionAction.triggered)
            {
                isRotating = false;
                rotatorRB.freezeRotation = true;
                attuned = true;
                StartCoroutine(ActiveObject());
            }
        }
    }


}
