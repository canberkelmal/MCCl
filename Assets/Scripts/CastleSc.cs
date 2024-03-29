using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CastleSc : MonoBehaviour
{
    GameManager gM;
    public int waveEnemyCount = 10;
    int tempThrowedCount = 0;
    public int hitCount = 0;
    float timer = 0;
    public int health = 100;

    bool animating = false;
    // Start is called before the first frame update
    void Start()
    {
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        InvokeRepeating("SendWaves", 3, 7);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Char") || collision.transform.CompareTag("Giant"))
        {
            HitToCastle(collision.gameObject);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Char") || collision.transform.CompareTag("Giant"))
        {
            timer += Time.deltaTime;
        }

        if (timer >= gM.hitTimeDiff)
        {
            timer = 0;

            if (collision.transform.CompareTag("Char") || collision.transform.CompareTag("Giant"))
            {
                HitToCastle(collision.gameObject);
            }
        }
    }

    void HitToCastle(GameObject hittingChar)
    {
        gM.HitToAnyCastle(gameObject);
        if (!animating)
        {
            StartCoroutine(CastleHitAnimation());
        }
        hitCount++;
        transform.GetChild(0).GetChild(0).GetComponent<Text>().text = (health - hitCount).ToString();
        if (hittingChar.transform.CompareTag("Giant"))
        {
            hittingChar.GetComponent<CharSc>().Punch();
        }
        if (hitCount == health)
        {
            GameObject partEffect = Instantiate(gM.explosiveParticle, transform.position, Quaternion.identity);
            Destroy(partEffect, 1.5f);
            gM.DestroyCastle(gameObject);
        }
    }


    IEnumerator CastleHitAnimation()
    {
        animating = true;
        SkinnedMeshRenderer renderer = gameObject.GetComponent<SkinnedMeshRenderer>();
        for(int i = 0; i < 101; i++)
        {
            i += 2;
            renderer.SetBlendShapeWeight(0, i);
            yield return new WaitForSeconds(gM.castleHitAnimSens / 10000);
        }
        for(int i = 0;i < 101; i++)
        {
            i += 2;
            renderer.SetBlendShapeWeight(1, i);
            renderer.SetBlendShapeWeight(0, 100-i);
            yield return new WaitForSeconds(gM.castleHitAnimSens / 10000);
        }
        for (int i = 0; i < 101; i++)
        {
            i += 2;
            renderer.SetBlendShapeWeight(1, 100-i);
            yield return new WaitForSeconds(gM.castleHitAnimSens / 10000);
        }
        animating = false;
    }

    void SendWaves()
    {
        InvokeRepeating("SpawnAndThrowChar", 0, 0.1f);
    }
     
    void SpawnAndThrowChar()
    {
        if(tempThrowedCount < waveEnemyCount) 
        {
            Vector3 spawnPoint = transform.position - transform.up * 3;
            spawnPoint.y = 1.1f;
            GameObject spawnedChar = Instantiate(gM.enemy1, spawnPoint, Quaternion.identity);
            Debug.Log(transform.rotation.eulerAngles.y);
            spawnedChar.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            //spawnedChar.GetComponent<NavMeshAgent>().avoidancePriority = UnityEngine.Random.Range(0, 50);
            Vector3 throwDirection = -transform.up * gM.defEnemyForwardForce * 2 + transform.right * (float)Random.Range(-3f, 3f);
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
