using System.Collections;
using UnityEngine;

public class OpenDoorInteractable : Interactable
{
    private const float ROTATION_AMOUNT = 90;

    [SerializeField] private GameObject doorBody;

    [SerializeField] private bool isOpen;

    [SerializeField] private float rotateSpeed;

    public override void Interact()
    {
        isInteractable = false;

        if (isOpen)
        {
            StartCoroutine(Close());
        } else
        {
            float dot = Vector3.Dot(forward, (player.transform.position - transform.position).normalized);
            StartCoroutine(Open(dot));
        }
    }

    private IEnumerator Open(float forwardAmount)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation;

        if (forwardAmount >= 0) {
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

        isInteractable = true;
    }

    private IEnumerator Close()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(rotation);

        isOpen = false;

        float time = 0f;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * rotateSpeed;
        }

        isInteractable = true;
    }
}
