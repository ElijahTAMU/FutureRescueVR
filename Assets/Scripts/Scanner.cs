using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Scanner : MonoBehaviour
{
    public Transform startingTransform;

    public XRGrabInteractable grabInteractable;

    public Transform leftAttach;
    public Transform rightAttach;

    public Transform playerHead;   // Camera (XR Origin head)
    public Transform target;       // What you want to face

    public GameObject[] TwoHandDoors;

    public TextMeshProUGUI timerText;
    public float timer = 45;
    public bool isCountingDown = false;

    public float timeHeld = 0;

    public float leftHaptic;
    public float rightHaptic;

    public TextMeshProUGUI HapticText;

    private void Awake()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    private void Start()
    {
        startingTransform = gameObject.transform;
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        Transform interactorTransform = args.interactorObject.transform;

        float leftDist = Vector3.Distance(interactorTransform.position, leftAttach.position);
        float rightDist = Vector3.Distance(interactorTransform.position, rightAttach.position);

        grabInteractable.attachTransform = leftDist < rightDist ? leftAttach : rightAttach;
    }

    public IEnumerator PickupRumble()
    {
        for(int i = 0; i < 3; i++)
        {
            foreach (var interactor in grabInteractable.interactorsSelecting)
            {
                var controllerInteractor = interactor as XRBaseInputInteractor;
                controllerInteractor.SendHapticImpulse(1, 0.2f);
                leftHaptic = 1;
                rightHaptic = 1;
            }
            yield return new WaitForSeconds(0.2f);
        }

    }

    void Update()
    {

        leftHaptic = 0; rightHaptic = 0;
        if (isCountingDown && GameObject.FindGameObjectWithTag("NPCHandler").GetComponent<HumanController>().GameIsInPlay)
        {
            timer -= Time.deltaTime;
            timerText.text = "" + Mathf.Round(timer);
            if(timer < 0)
            {
                Debug.Log("FAILED");
                isCountingDown = false;

                GameObject.FindGameObjectWithTag("NPCHandler").GetComponent<HumanController>().GameIsInPlay = false;
                GameObject.FindGameObjectWithTag("NPCHandler").GetComponent<HumanController>().LoseGame();
            }
        }

        GameObject[] humans = GameObject.FindGameObjectsWithTag("NPC");
        foreach(GameObject g in humans)
        {
            if (g.activeSelf)
            {
                target = g.transform;
            }
        }


        // Only run haptics if BOTH hands are holding
        if (grabInteractable.interactorsSelecting.Count < 2) {
            foreach (GameObject g in TwoHandDoors)
            {
                g.SetActive(true);
               
            }
            timeHeld = 0;
            return;
        }


        if (timeHeld < 0.01)
        {
            StartCoroutine("PickupRumble");
            GetComponent<AudioSource>().Play();
        }
        timeHeld += Time.deltaTime;

        foreach (GameObject g in TwoHandDoors)
        {
            g.SetActive(false);
        }

        isCountingDown = true;
        if (GameObject.FindGameObjectWithTag("NPCHandler").GetComponent<HumanController>().GameIsInPlay != true)
        {
            GameObject.FindGameObjectWithTag("NPCHandler").GetComponent<HumanController>().GameIsInPlay = true;
            GameObject.FindGameObjectWithTag("NPCHandler").GetComponent<HumanController>().StartGame();
        }

        if (playerHead == null || target == null)
            return;

        // Direction to target
        Vector3 toTarget = target.position - playerHead.position;
        toTarget.y = 0f; // ignore vertical
        toTarget.Normalize();

        // Left/right direction
        float direction = Vector3.Dot(playerHead.right, toTarget);
        // > 0 = right, < 0 = left

        float leftIntensity = Mathf.Clamp01(-direction);
        float rightIntensity = Mathf.Clamp01(direction);

        if(Mathf.Abs(direction) < 0.05f)
        {
            leftIntensity = 1;
            rightIntensity = 1;
            gameObject.GetComponent<AudioSource>().Play();
            leftHaptic = 1;
            rightHaptic = 1;
        }

        if(timeHeld > 1f)
        {
            // Send haptics to each hand
            foreach (var interactor in grabInteractable.interactorsSelecting)
            {
                var controllerInteractor = interactor as XRBaseInputInteractor;
                if (controllerInteractor == null) continue;

                if (controllerInteractor.handedness == InteractorHandedness.Left)
                {
                    controllerInteractor.SendHapticImpulse(leftIntensity, 0.05f);
                }
                else if (controllerInteractor.handedness == InteractorHandedness.Right)
                {
                    controllerInteractor.SendHapticImpulse(rightIntensity, 0.05f);
                }

                rightHaptic = rightIntensity;
                leftHaptic = leftIntensity;
            }
        }

        HapticText.text = "HAPTIC\nLEFT INTENSITY: " + leftHaptic + "\nRIGHT INTENSITY: " + rightHaptic;

    }
}