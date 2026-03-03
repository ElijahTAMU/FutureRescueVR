using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;

public class Player1 : MonoBehaviour
{
    public bool rocketsEquipped;
    public RocketInfo2 leftRocket;
    public RocketInfo2 rightRocket;

    public InputActionReference triggerAction;
    public CharacterController characterController;
    public GravityProvider gravityProvider;

    // public Rigidbody rb;

    public float speed = 0.05f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        triggerAction.action.Enable();
        triggerAction.action.performed += LeftThrust;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rocketsEquipped)
        {
            if (!leftRocket.rocket.activeSelf)
            {
                leftRocket.rocket.SetActive(true);
            }

            if (!rightRocket.rocket.activeSelf)
            {
                rightRocket.rocket.SetActive(true);
            }
        }

        if (!rocketsEquipped)
        {
            if (leftRocket.rocket.activeSelf)
            {
                leftRocket.rocket.SetActive(false);
            }

            if (rightRocket.rocket.activeSelf)
            {
                rightRocket.rocket.SetActive(false);
            }
        }

        if (rocketsEquipped && leftRocket.isThrust)
        {
            MoveThrust();
        }
    }

    public void LeftThrust(InputAction.CallbackContext context)
    {
        Debug.Log("TRIGGERED");

        if (rocketsEquipped && context.action.triggered)
        {
            if (!leftRocket.isThrust)
            {
                //rb.useGravity = false;
                gravityProvider.useGravity = false;
                Debug.Log("Left Rocket On");
                leftRocket.isThrust = true;
            }
            else
            {
               // rb.useGravity = true;
                gravityProvider.useGravity = true;
                Debug.Log("Left Rocket OFF");
                leftRocket.isThrust = false;
            }
        }

    }

    public void UnequipRockets()
    {
        rocketsEquipped = false;
        gravityProvider.useGravity = true;
        // rb.useGravity = true;
    }

    public void MoveThrust()
    {
        Vector3 leftDirection = leftRocket.hand.transform.forward;
        Vector3 rightDirection = rightRocket.hand.transform.forward;

        Vector3 combined = leftDirection + rightDirection;

        combined = combined.normalized;

        //characterController.SimpleMove(combined * speed);
        Debug.Log(combined);
        combined = new Vector3(combined.x, combined.y, combined.z);

        //combined = transform.localToWorldMatrix * combined;
        characterController.Move(combined * speed);
        //transform.Translate(combined * speed, Space.World);
    }
}

[Serializable]
public class RocketInfo2
{
    public bool isThrust;
    public GameObject hand;
    public GameObject rocket;
}
