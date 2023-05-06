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
    float colorTimer;

    int hitCount = 0;
    public int health = 4;

    Material defMat, tempMat;
    bool whiting = true;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        forwardForce = gM.defCharForwardForce * 2;
        Invoke("SetClonable", 0.2f);
    }
    void Start()
    {
        tempMat = new Material(gM.charMat);
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
        if (collision.transform.CompareTag("Enemy") || collision.transform.CompareTag("Castle"))
        {
            TakeHit();
        }
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Enemy") || collision.transform.CompareTag("Castle"))
        {
            timer += Time.deltaTime;
        }

        if (timer >= gM.hitTimeDiff)
        {
            timer = 0;

            if (collision.transform.CompareTag("Enemy") || collision.transform.CompareTag("Castle"))
            {
                TakeHit();
            }
        }
    }

    void TakeHit()
    {
        whiting = true;
        tempMat.color = gM.charMat.color;
        colorTimer = 0;
        transform.GetChild(0).GetComponent<Renderer>().material = tempMat;
        InvokeRepeating("PaintWhite", 0, Time.fixedDeltaTime);

        hitCount++;
        if (hitCount == health)
        {
            Destroy(gameObject);
        }
    }


    void PaintWhite()
    {
        colorTimer += Time.deltaTime;
        if (colorTimer > gM.hitColorDur * 0.4f)
        {
            whiting = false;
        }

        if (colorTimer < gM.hitColorDur)
        {
            tempMat.color = whiting ? Color.Lerp(tempMat.color, Color.white, gM.hitColorSense * Time.deltaTime) : Color.Lerp(tempMat.color, gM.charMat.color, gM.hitColorSense * Time.deltaTime);
        }
        else
        {
            tempMat.color = gM.charMat.color;
            transform.GetChild(0).GetComponent<Renderer>().material = gM.charMat;
            CancelInvoke("PaintWhite");
        }
    }

    public void DuplicateChar(int duplicateMultiplier)
    {
        for(int i = 0; i < duplicateMultiplier; i++)
        {
            float horRandom = (float)Random.Range(-duplicateMultiplier*3, duplicateMultiplier*3) / 10;
            float verRandom = (float)Random.Range(6, 12) / 10;
            Instantiate(gameObject, transform.position + (Vector3.right * horRandom) + (Vector3.forward * verRandom), transform.rotation);
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

    public void Punch()
    {
        GetComponent<Animator>().SetTrigger("PunchTrig");
    }
}
