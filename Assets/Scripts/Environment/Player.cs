using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;

public class Player : MonoBehaviour
{
    public bool rocketsEquipped;
    public RocketInfo leftRocket;
    public RocketInfo rightRocket;

    public InputActionReference triggerAction;
    public CharacterController characterController;
    public GravityProvider gravityProvider;

    public float speed = 0.02f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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

        if (rocketsEquipped)
        {
            MoveThrust();
        }
    }

    public void LeftThrust(InputAction.CallbackContext context)
    {
        if (rocketsEquipped && context.action.triggered)
        {
            Debug.Log("Left Rocket On");
            leftRocket.isThrust = true;
        }

        if (!context.action.triggered)
        {
            Debug.Log("Left Rocket OFF");
            leftRocket.isThrust = false;
        }

    }

    public void MoveThrust()
    {
        gravityProvider.useGravity = false;
        Vector3 leftDirection = leftRocket.rocket.transform.forward;
        Vector3 rightDirection = rightRocket.rocket.transform.forward;

        leftDirection = leftDirection.normalized;
        rightDirection = rightDirection.normalized;

        Vector3 combined = leftDirection + rightDirection;

        //characterController.SimpleMove(combined * speed);
        Debug.Log(combined);
        combined = new Vector3(combined.x, combined.y, combined.z);
        transform.Translate(combined * speed);
    }
}

[Serializable]
public class RocketInfo
{
    public bool isThrust;
    public GameObject hand;
    public GameObject rocket;
}
