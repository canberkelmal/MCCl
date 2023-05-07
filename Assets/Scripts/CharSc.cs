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
        forwardForce = gM.defCharForwardForce;
        float clonableDelayTime = gameObject.CompareTag("Giant") ? 0.65f : 0.3f;
        Invoke("SetClonable", clonableDelayTime);
    }
    void Start()
    {
        tempMat = new Material(gM.charMat);
        InvokeRepeating("CheckStuck", 0, 0.25f);
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
                GameObject partEffect = gameObject.CompareTag("Giant") ? Instantiate(gM.explosiveParticle, transform.position, Quaternion.identity) : Instantiate(gM.littleParticle, transform.position, Quaternion.identity);
                Destroy(partEffect, 1.5f);
                Destroy(gameObject);
                break;
        }
        rb.velocity = direction * forwardForce;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gate") && clonable)
        {
            DuplicateChar(other.GetComponent<GateSc>().gateMultipier);
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
        else if (collision.transform.CompareTag("Obs"))
        {
            HitBackFrom(collision.transform);
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
    void HitBackFrom(Transform refObj)
    {
        Vector3 dir = transform.position - refObj.position;
        rb.AddForce(direction.normalized * 5, ForceMode.Impulse);
    }
    void CheckStuck()
    {
        //Debug.Log(rb.velocity.magnitude);
        if (rb.velocity.magnitude < 1)
        {
            //Debug.Log(rb.velocity.magnitude);
            int rnd = (int)UnityEngine.Random.Range(0, 10);
            if (rnd < 5)
            {
                //Debug.Log("pushed right");
                rb.AddForce(transform.right * 5, ForceMode.Impulse);
            }
            else
            {
                //Debug.Log("pushed left");
                rb.AddForce(-transform.right * 5, ForceMode.Impulse);
            }
        }
    }

    void TakeHit()
    {
        if (!gameObject.CompareTag("Giant"))
        {
            whiting = true;
            tempMat.color = gM.charMat.color;
            colorTimer = 0;
            transform.GetChild(0).GetComponent<Renderer>().material = tempMat;
            InvokeRepeating("PaintWhite", 0, Time.fixedDeltaTime);
        }

        hitCount++;
        if (hitCount >= health)
        {
            GameObject partEffect = gameObject.CompareTag("Giant") ? Instantiate(gM.explosiveParticle, transform.position, Quaternion.identity) : Instantiate(gM.littleParticle, transform.position, Quaternion.identity);
            Destroy(partEffect, 1.5f);
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
            //float horRandom = (float)Random.Range(-duplicateMultiplier*3, duplicateMultiplier*3) / 10; 
            float verRandom = gameObject.CompareTag("Giant") ? (float)Random.Range(20, 25) / 10 : (float)Random.Range(6, 12) / 10;
            Instantiate(gameObject, transform.position + (Vector3.forward * verRandom), transform.rotation);
            //Instantiate(gameObject, transform.position + (Vector3.right * horRandom) + (Vector3.forward * verRandom), transform.rotation);
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
        forwardForce = gM.defCharForwardForce;
        throwed = false;
        clonable = true;
    }

    public void Punch()
    {
        GetComponent<Animator>().SetTrigger("PunchTrig");
    }
}
