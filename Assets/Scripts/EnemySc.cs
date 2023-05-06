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
    float colorTimer;

    Material defMat, tempMat;
    bool whiting = true;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    void Start()
    {
        tempMat = new Material(gM.enemyMat);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(transform.position + Vector3.back);
        rb.velocity = throwed ? direction : transform.forward * forwardForce;
        if (gM.castleCount == 0)
        {
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Char") || collision.transform.CompareTag("Giant"))
        {
            TakeHit();
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
                TakeHit();
            }
        }
    }

    void TakeHit()
    {
        whiting = true;
        tempMat.color = gM.enemyMat.color;
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
            tempMat.color = whiting ? Color.Lerp(tempMat.color, Color.white, gM.hitColorSense * Time.deltaTime) : Color.Lerp(tempMat.color, gM.enemyMat.color, gM.hitColorSense * Time.deltaTime);
        }
        else
        {
            tempMat.color = gM.enemyMat.color;
            transform.GetChild(0).GetComponent<Renderer>().material = gM.enemyMat;
            CancelInvoke("PaintWhite");
        }
    }

    public void ThrowEnemy(Vector3 throwDir)
    {
        throwed = true;
        direction = throwDir;
        Invoke("SetEnemyDef", 1f);
    }

    void SetEnemyDef()
    {
        forwardForce = gM.defEnemyForwardForce;
        direction = transform.forward * forwardForce;
        throwed = false;
    }
}