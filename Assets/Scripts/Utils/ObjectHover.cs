using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableHover : MonoBehaviour
{
    [SerializeField] GameObject hoverObject;

    [Space]
    [SerializeField] float maxY;
    [SerializeField] float minY;
    [SerializeField] float hoverSpeed; 

    [Space]
    [SerializeField] float rotationSpeed;

    [Space]
    [SerializeField] bool isAscending;

    private void FixedUpdate()
    {
        Hover();
    }

    private void Hover()
    {
        hoverObject.transform.Translate((isAscending ? Vector3.up : Vector3.down) * hoverSpeed * Time.deltaTime);
        hoverObject.transform.Rotate(new Vector3(0, rotationSpeed, 0));

        if (hoverObject.transform.localPosition.y <= minY)
            isAscending = true;
        else if (hoverObject.transform.localPosition.y >= maxY)
            isAscending = false;
    }
}
