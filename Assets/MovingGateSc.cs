using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGateSc : MonoBehaviour
{
    public float maxX, minX;
    Vector3 targetPos;
    bool movingRight = true;
    public float movingSense = 1f;

    private void Start()
    {
        targetPos = transform.localPosition;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (movingRight && transform.localPosition.x > maxX - 0.15f)
        {
            movingRight = false;
        }
        
        if(!movingRight && transform.localPosition.x < minX + 0.15f)
        {
            movingRight = true;
        }

        targetPos.x = movingRight ? maxX : minX;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, movingSense * Time.deltaTime);
    }
}
