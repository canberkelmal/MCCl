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
        direction = transform.position.x<0 ? (gM.castleLeft.transform.position - transform.position).normalized : (gM.castleRight.transform.position - transform.position).normalized;
        rb.velocity = direction * forwardForce;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gate") && clonable)
        {
            DuplicateChar(4);
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
