using System.Collections;
using UnityEngine;

public class OpenDoorInteractable : Interactable
{
    private const float ROTATION_AMOUNT = 90;

    [SerializeField] private bool canInteractOnce = false;

    [SerializeField] private GameObject doorBody;

    [SerializeField] private bool isOpen;
    [SerializeField] private bool isOpenableByLever = false;
    [SerializeField] private bool interactionInProgress = false;

    [SerializeField] private float rotateSpeed;

    private Vector3 rotation;
    private Vector3 forward;

    public bool InteractionInProgress { get { return interactionInProgress; } }
    public bool IsOpen { get { return isOpen; } }

    private void Start()
    {
        rotation = doorBody.transform.rotation.eulerAngles;
        forward = doorBody.transform.forward;
    }

    public override void Interact()
    {
        isInteractable = false;
        interactionInProgress = true;

        if (isOpen)
        {
            StartCoroutine(Close());
        } else
        {
            float dot = Vector3.Dot(forward, (player.transform.position - doorBody.transform.position).normalized);
            StartCoroutine(Open(dot));
        }
    }

    private IEnumerator Open(float forwardAmount)
    {
        Quaternion startRotation = doorBody.transform.rotation;
        Quaternion endRotation;

        if (forwardAmount >= 0)
        {
            endRotation = Quaternion.Euler(new Vector3(0, rotation.y - ROTATION_AMOUNT, 0));
        } else
        {
            endRotation = Quaternion.Euler(new Vector3(0, rotation.y + ROTATION_AMOUNT, 0));
        }

        isOpen = true;

        float time = 0f;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * rotateSpeed;
        }

        isInteractable = !isOpenableByLever && !canInteractOnce;
        interactionInProgress = false;
    }

    private IEnumerator Close()
    {
        Quaternion startRotation = doorBody.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(rotation);

        isOpen = false;

        float time = 0f;
        while (time < 1)
        {
            doorBody.transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * rotateSpeed;
        }

        isInteractable = !isOpenableByLever;
        interactionInProgress = false;
    }
}
