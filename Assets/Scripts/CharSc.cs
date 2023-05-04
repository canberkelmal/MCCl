using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CharSc : MonoBehaviour
{
    GameManager gM;
    Rigidbody rb;
    float forwardForce = 4f;
    Vector3 direction;
    bool clonable = false;
    bool throwed = false;
    float timer = 0;

    int hitCount = 0;
    public int health = 4;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        forwardForce = gM.defCharForwardForce * 2;
        Invoke("SetClonable", 0.2f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {        
        switch (gM.castleCount)
        {
            case 2:
                direction = transform.position.x < 0 ? (gM.castleLeft.transform.position - transform.position).normalized : (gM.castleRight.transform.position - transform.position).normalized;
                break;
            case 1:
                direction = (gM.castleLast.transform.position - transform.position).normalized;
                break;
            case 0:
                Destroy(gameObject);
                break;
        }
        rb.velocity = direction * forwardForce;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gate") && clonable)
        {
            DuplicateChar(2);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Enemy"))
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
        if (collision.transform.CompareTag("Enemy"))
        {
            timer += Time.deltaTime;
        }

        if (timer >= gM.hitTimeDiff)
        {
            timer = 0;

            if (collision.transform.CompareTag("Enemy"))
            {
                hitCount++;
                if (hitCount == health)
                {
                    Destroy(gameObject);
                }
            }
        }

    }





    public void DuplicateChar(int duplicateMultiplier)
    {
        for(int i = 0; i < duplicateMultiplier; i++)
        {
            Instantiate(gameObject, transform.position + (Vector3.right * 0.6f * i) + (Vector3.forward), transform.rotation);
        }
        Destroy(gameObject);
    }
    public void ThrowChar()
    {
        forwardForce = gM.throwForce;
        throwed = true;
    }
    void SetClonable()
    {
        forwardForce = throwed ? forwardForce : gM.defCharForwardForce;
        throwed = false;
        clonable = true;
    }
}
