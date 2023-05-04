using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleSc : MonoBehaviour
{
    GameManager gM;
    public int waveEnemyCount = 10;
    int tempThrowedCount = 0;
    public int hitCount = 0;
    float timer = 0;
    public int health = 100;
    // Start is called before the first frame update
    void Start()
    {
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        InvokeRepeating("SendWaves", 1, 7);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Char"))
        {
            hitCount++;
            if (hitCount == health)
            {
                gM.DestroyCastle(gameObject);
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Char"))
        {
            timer += Time.deltaTime;
        }

        if (timer >= gM.hitTimeDiff)
        {
            timer = 0;

            if (collision.transform.CompareTag("Char"))
            {
                hitCount++;
                if (hitCount == health)
                {
                    gM.DestroyCastle(gameObject);
                    //Destroy(gameObject);
                }
            }
        }

    }

    void SendWaves()
    {
        InvokeRepeating("SpawnAndThrowChar", 0, 0.1f);
    }

    void SpawnAndThrowChar()
    {
        if(tempThrowedCount < waveEnemyCount)
        {
            GameObject spawnedChar = Instantiate(gM.enemy1, transform.position - Vector3.forward * 5, transform.rotation);
            Vector3 throwDirection = transform.forward * gM.defEnemyForwardForce * 2 + transform.right * UnityEngine.Random.Range(-4f, 4f);
            spawnedChar.GetComponent<EnemySc>().ThrowEnemy(throwDirection);
            tempThrowedCount++;
        }
        else
        {
            tempThrowedCount = 0;
            CancelInvoke("SpawnAndThrowChar");
        }
    }
}
