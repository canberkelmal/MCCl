using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySc : MonoBehaviour
{
    public int health = 2;
    GameManager gM;
    Rigidbody rb;
    float forwardForce = 4f;

    Vector3 direction;
    bool throwed = false;
    int hitCount = 0;

    float timer = 0;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = direction;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Char") || collision.transform.CompareTag("Giant"))
        {
            hitCount++;
            if (hitCount == health)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Char") || collision.transform.CompareTag("Giant"))
        {
            timer += Time.deltaTime;
        }

        if(timer >= gM.hitTimeDiff)
        {
            timer = 0;

            if (collision.transform.CompareTag("Char") || collision.transform.CompareTag("Giant"))
            {
                hitCount++;
                if (hitCount == health)
                {
                    Destroy(gameObject);
                }
            }
        }

    }

    public void ThrowEnemy(Vector3 throwDir)
    {
        direction = throwDir;
        Invoke("SetEnemyDef", 1f);
    }

    void SetEnemyDef()
    {
        forwardForce = gM.defEnemyForwardForce;
        direction = transform.forward * forwardForce;
    }
}