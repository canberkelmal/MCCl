using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public float castleHitAnimSens = 0.002f,
                 iconMovingSens = 1f,
                 cannonAnimSens = 0.002f,
                 horizontalSens = 1f, 
                 xMin = 1f,
                 xMax = 1f,
                 mobMaxVelocity = 1f;

    public UnityEngine.UI.Image attackBar, attackBarYellow;
    public Transform player;
    public GameObject mainCam, boxFrame, coinFrame, boxIcon, coinIcon, cannon, cannonBase, environment, center, center2, char1, giant, enemy1, enemy2, castleLeft, castleRight, castleLef2, castleRight2, castleLast, movingGate, chapter1, chapter2;
    public GameObject littleParticle, explosiveParticle;
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

    public int coinCount = 0, boxCount = 0;

    public Material charMat, enemyMat;

    int chapterCount = 2;
    bool movementCenter1 = false, movementCenter2 = false;
    Vector3 cannonMovementDirection = Vector3.zero;

    float zMin, zMax;
    float camOffsetY;
    float camOffsetZ;
    private void Awake()
    {
        camOffsetY = mainCam.transform.position.y - player.transform.position.y;
        camOffsetZ = mainCam.transform.position.z - player.transform.position.z;
    }
    // Start is called before the first frame update
    void Start()
    {
        cannon = player.GetChild(0).GetChild(0).gameObject;
        cannonBase = player.GetChild(0).GetChild(1).gameObject;
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
        Vector3 newPosX = player.position + player.transform.right * moveAmount;
        if(chapterCount < 2)
        {
            newPosX.x = Mathf.Clamp(newPosX.x, xMin, xMax);
            newPosX.z = Mathf.Clamp(newPosX.z, zMin, zMax);
        }
        else
        {
            newPosX.x = Mathf.Clamp(newPosX.x, xMin, xMax);
        }
        player.position = newPosX;  
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
        spawnedChar.transform.rotation = Quaternion.Euler(0, player.transform.rotation.eulerAngles.y, 0);
        spawnedChar.GetComponent<CharSc>().ThrowChar();
        spawnedChar.GetComponent<NavMeshAgent>().avoidancePriority = spawnedChar.CompareTag("Giant") ? UnityEngine.Random.Range(51, 99) : UnityEngine.Random.Range(0, 50);

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

    void GoNextChapter()
    {
        if(chapterCount == 1)
        {
            Destroy(chapter1);
            InvokeRepeating("MoveCannon", 0, Time.fixedDeltaTime);
            CancelInvoke("SpawnAndThrowChar");
            Debug.Log("Go to next chapter!");
        }
        else if(chapterCount == 0)
        {
            Destroy(chapter2);
            FinishChapters();
        }
    }

    void MoveCannon()
    {
        controls = false;
        if(!movementCenter1  && !movementCenter2)
        {
            cannonMovementDirection = center.transform.position;
            player.transform.position = Vector3.MoveTowards(player.transform.position, new Vector3(center.transform.position.x, player.transform.position.y, player.transform.position.z), environmentMovementSens * Time.fixedDeltaTime);

            cannonBase.transform.localRotation = Quaternion.RotateTowards(cannonBase.transform.localRotation, Quaternion.Euler(-90, 0, -90), environmentRotateSens * 3 * Time.deltaTime);

            if(player.transform.position.x == center.transform.position.x && mainCam.transform.parent != player.transform)
            {
                mainCam.transform.parent = player.transform;
            }
        }
        else if(movementCenter1 && !movementCenter2)
        {
            cannonMovementDirection = center2.transform.position;

            player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, Quaternion.Euler(0, 30.584f, 0), environmentRotateSens * Time.deltaTime);
            cannonBase.transform.localRotation = Quaternion.RotateTowards(cannonBase.transform.localRotation, Quaternion.Euler(-90, 0, 0), environmentRotateSens* 2 * Time.deltaTime);
        }
        else if(movementCenter1 && movementCenter2)
        {
            player.transform.position = center2.transform.position;
            player.transform.rotation = Quaternion.Euler(0, 30.584f, 0);
            cannonBase.transform.localRotation = Quaternion.Euler(-90, 0, 0);
            mainCam.transform.parent = null;
            SetSecondChapter();
            controls = true;
            CancelInvoke("MoveCannon");
        }
        player.transform.position = Vector3.MoveTowards(player.transform.position, cannonMovementDirection, environmentMovementSens * Time.fixedDeltaTime);
    }

    public void TrigCenter1()
    {
        movementCenter1 = true;
    }

    public void TrigCenter2()
    {
        movementCenter2 = true;
    }

    void MoveEnvironment()
    {
        controls = false;
        player.transform.position = player.transform.position.x != 0 ? Vector3.MoveTowards(player.transform.position, new Vector3(0, player.transform.position.y, player.transform.position.z), environmentMovementSens * 4 * Time.deltaTime) : player.transform.position;
        
        if(environment.transform.position.z > -72.4f)
        {
            environment.transform.position = Vector3.MoveTowards(environment.transform.position, Vector3.forward * -72.4f, environmentMovementSens * Time.deltaTime);
            cannonBase.transform.rotation = Quaternion.RotateTowards(cannonBase.transform.rotation, Quaternion.Euler(-90, 0, -90), environmentRotateSens * 3f * Time.deltaTime);
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
            cannonBase.transform.rotation = Quaternion.RotateTowards(cannonBase.transform.rotation, Quaternion.Euler(-90, 0, 0), environmentRotateSens * 3f * Time.deltaTime);
        }
        else if(environment.transform.position.z <= -92.156f)
        {
            environment.transform.position = new Vector3(2.07f, 0, -92.156f);
            environment.transform.rotation = Quaternion.Euler(0, -30.584f, 0);
            SetSecondChapter();
            controls = true;
            CancelInvoke("MoveEnvironment");
        }
    }

    void SetSecondChapter()
    {
        //movingGate.GetComponent<MovingGateSc>().enabled = true;
        zMax = player.position.z + (xMin * player.transform.right).z;
        zMin = player.position.z + (xMax * player.transform.right).z;

        xMax = player.position.x + (xMax * player.transform.right).x;
        xMin = player.position.x + (xMin * player.transform.right).x;

        Debug.Log(zMax + " - " + zMin + " - " + xMax + " - " + xMin);


        castleCount = 2;
        castleLeft = castleLef2;
        castleRight = castleRight2;
        castleLeft.GetComponent<CastleSc>().enabled = true;
        castleRight.GetComponent<CastleSc>().enabled = true;
    }

    public void DestroyCastle(GameObject destroyedCastle)
    {
        GetCoinFromCastle(destroyedCastle);
        if (castleCount == 2)
        {
            castleLast = destroyedCastle == castleLeft ? castleRight : castleLeft;
            castleCount--;

            Destroy(destroyedCastle);
        }
        else if (castleCount == 1)
        {
            castleLast = destroyedCastle == castleLeft ? castleRight : castleLeft;
            castleCount--;
            Destroy(destroyedCastle);
            chapterCount--;
            if (chapterCount >= 0)
            {
                GoNextChapter();
            }
        }
    }

    public void GetCoinFromCastle(GameObject castle)
    {
        Vector3 castleScreenPosition = Camera.main.WorldToScreenPoint(castle.transform.position);

        GameObject movingCoin = Instantiate(coinIcon, castleScreenPosition, Quaternion.identity);
        movingCoin.transform.parent = coinFrame.transform;
    }

    public void HitToAnyCastle(GameObject hitCastle)
    {
        Vector3 castleScreenPosition = Camera.main.WorldToScreenPoint(hitCastle.transform.position);

        GameObject movingBox = Instantiate(boxIcon, castleScreenPosition, Quaternion.identity);
        movingBox.transform.parent = boxFrame.transform;
    }

    public void IncreaseBoxCount()
    {
        boxCount++;
        boxFrame.transform.GetChild(2).GetComponent<Text>().text = boxCount.ToString();
    }
    public void IncreaseCoinCount()
    {
        coinCount+=200;
        coinFrame.transform.GetChild(2).GetComponent<Text>().text = coinCount.ToString();
    }

    void FinishChapters()
    {

    }

    // Reload the current scene to restart the game
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
