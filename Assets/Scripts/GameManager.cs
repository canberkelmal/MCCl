using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float cannonAnimSens = 0.002f,
                 horizontalSens = 1f, 
                 xMin = 1f,
                 xMax = 1f;

    public Image attackBar, attackBarYellow;
    public Transform player;
    public GameObject cannon, char1, giant, enemy1, enemy2, castleLeft, castleRight, castleLast;
    public float throwForce = 1f;
    public float defCharForwardForce = 1f;
    public float defEnemyForwardForce = 1f;
    public float hitTimeDiff = 0.5f;

    public int castleCount = 2;

    // Start is called before the first frame update
    void Start()
    {
        cannon = player.GetChild(0).GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
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
        GameObject spawnedChar = Instantiate(spawnedCharacter, player.position + Vector3.forward * 2.5f, Quaternion.identity);
        spawnedChar.GetComponent<CharSc>().ThrowChar();

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
            GoNextChapter();
        }
    }

    void GoNextChapter()
    {
        Debug.Log("Go to next chapter!");
    }

    // Reload the current scene to restart the game
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
