using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float castleHitAnimSens = 0.002f,
                 cannonAnimSens = 0.002f,
                 horizontalSens = 1f, 
                 xMin = 1f,
                 xMax = 1f,
                 mobMaxVelocity = 1f;

    public Image attackBar, attackBarYellow;
    public Transform player;
    public GameObject cannon, environment, center, char1, giant, enemy1, enemy2, castleLeft, castleRight, castleLef2, castleRight2, castleLast, movingGate;
    public float throwForce = 1f;
    public float defCharForwardForce = 1f;
    public float defEnemyForwardForce = 1f;
    public float hitTimeDiff = 0.5f;
    public float hitColorSense = 1f;
    public float hitColorDur = 1f;

    public float environmentMovementSens = 1f;
    public float environmentRotateSens = 1f;

    public int castleCount = 2;
    public bool controls = true;

    public Material charMat, enemyMat;

    int chapterCount = 2;

    // Start is called before the first frame update
    void Start()
    {
        cannon = player.GetChild(0).GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (controls)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCharSpawning();
            }

            if (Input.GetMouseButton(0))
            {
                UpdatePlayerPositionX();
            }

            if (Input.GetMouseButtonUp(0))
            {
                StopCharSpawning();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }


    // Update the players's horizontal/X position while mouse/finger is moving
    void UpdatePlayerPositionX()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float moveAmount = mouseX * horizontalSens;
        Vector3 newPosX = player.position + new Vector3(moveAmount, 0, 0);
        newPosX.x = Mathf.Clamp(newPosX.x, xMin, xMax);
        player.transform.position = newPosX;
    }

    void StartCharSpawning()
    {
        InvokeRepeating("SpawnAndThrowChar",0f , 0.33f);
    }
    void StopCharSpawning()
    {
        CancelInvoke("SpawnAndThrowChar");

        if(attackBar.fillAmount >= 1)
        {
            SpawnAndThrowGiant();
        }
    }

    void SpawnAndThrowGiant()
    {
        StartCoroutine(CannonAnimStart(giant, true));
    }

    void SpawnAndThrowChar()
    {
        StartCoroutine(CannonAnimStart(char1, false));
    }
    IEnumerator CannonAnimStart(GameObject throwedCharacter, bool isGiant)
    {
        for (int i = 0; i <= 100; i++)
        {
            cannon.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, i);
            i++;
            yield return new WaitForSeconds(cannonAnimSens);
        }

        for (int i = 0; i <= 100; i++)
        {
            cannon.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(1, i);
            cannon.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, 100 - i);
            i++;
            yield return new WaitForSeconds(cannonAnimSens);
        }

        SpawnChar(throwedCharacter, isGiant);

        for (int i = 100; i >= 0; i--)
        {
            cannon.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(1, i);
            i--;
            yield return new WaitForSeconds(cannonAnimSens);
        }
    }

    void SpawnChar(GameObject spawnedCharacter, bool isGiant)
    {
        GameObject spawnedChar = Instantiate(spawnedCharacter, player.position + player.transform.forward * 2.5f, Quaternion.identity);
        spawnedChar.GetComponent<CharSc>().ThrowChar();

        //spawnedChar.GetComponent<NavMeshAgent>().avoidancePriority = spawnedChar.CompareTag("Giant") ? 99 : UnityEngine.Random.Range(0, 50);

        if (!isGiant)
        {
            if(attackBar.fillAmount < 1)
            {
                attackBar.fillAmount += 0.04f;
            }
            else
            {
                attackBar.fillAmount = 1;
                attackBarYellow.enabled = true;
            }
        }
        else
        {
            attackBar.fillAmount = 0;
            attackBarYellow.enabled = false;
        }
    }

    public void DestroyCastle(GameObject destroyedCastle)
    {
        if(castleCount == 2)
        {
            castleLast = destroyedCastle == castleLeft ? castleRight : castleLeft;
            castleCount--;
            Destroy(destroyedCastle);
        }
        else if(castleCount == 1)
        {
            castleLast = destroyedCastle == castleLeft ? castleRight : castleLeft;
            castleCount--;
            Destroy(destroyedCastle);
            chapterCount--;
            if(chapterCount > 0)
            GoNextChapter();
        }
    } 

    void GoNextChapter()
    {
        InvokeRepeating("MoveEnvironment", 0, Time.fixedDeltaTime);
        CancelInvoke("SpawnAndThrowChar");
        Debug.Log("Go to next chapter!");
    }

    void MoveEnvironment()
    {
        controls = false;
        player.transform.position = player.transform.position.x != 0 ? Vector3.MoveTowards(player.transform.position, new Vector3(0, player.transform.position.y, player.transform.position.z), environmentMovementSens * 4 * Time.deltaTime) : player.transform.position;
        if(environment.transform.position.z > -72.4f)
        {
            environment.transform.position = Vector3.MoveTowards(environment.transform.position, Vector3.forward * -72.4f, environmentMovementSens * Time.deltaTime);
        }
        /*
        else if (environment.transform.rotation.eulerAngles.y == 0 || environment.transform.rotation.eulerAngles.y > 360 - 30.584f)
        {
            environment.transform.rotation = Quaternion.RotateTowards(environment.transform.rotation, Quaternion.Euler(0, -30.584f, 0), environmentRotateSens * Time.deltaTime);
            Debug.Log(environment.transform.rotation.eulerAngles.y);
        }
        */
        else if((environment.transform.position.z <= -72.4f && environment.transform.position.x < 2.06f) || (environment.transform.rotation.eulerAngles.y == 0 || environment.transform.rotation.eulerAngles.y > 360 - 30.584f))
        {
            environment.transform.position = Vector3.MoveTowards(environment.transform.position, new Vector3(2.07f, 0, - 92.156f), environmentMovementSens * Time.deltaTime);
            environment.transform.rotation = Quaternion.RotateTowards(environment.transform.rotation, Quaternion.Euler(0, -30.584f, 0), environmentRotateSens * Time.deltaTime);
        }
        else if(environment.transform.position.z <= -92.156f)
        {
            SetSecondChapter();
            environment.transform.position = new Vector3(2.07f, 0, -92.156f);
            environment.transform.rotation = Quaternion.Euler(0, -30.584f, 0);
            controls = true;
            CancelInvoke("MoveEnvironment");
        }

    }

    void SetSecondChapter()
    {
        //movingGate.GetComponent<MovingGateSc>().enabled = true;
        castleCount = 2;
        castleLeft = castleLef2;
        castleRight = castleRight2;
        castleLeft.GetComponent<CastleSc>().enabled = true;
        castleRight.GetComponent<CastleSc>().enabled = true;

    }

    // Reload the current scene to restart the game
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
