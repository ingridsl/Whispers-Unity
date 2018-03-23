﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneDoor : MonoBehaviour
{
    public bool isOpened = true;

    void OnCollisionEnter2D(Collision2D other)
    {
        print("TRIGGER");
        if (other.gameObject.tag.Equals("Player"))
        {
            if (!isOpened)
            {
                MissionManager.instance.rpgTalk.NewTalk("Trancada", "TrancadaEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "", false);
                MissionManager.instance.scenerySounds2.PlayDoorClosed();
            }
            else
            {
                ChangeScene(other);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        print("STAYTRIGGER");

        if (other.gameObject.tag.Equals("Player") && isOpened && !MissionManager.instance.paused)
        {
            ChangeScene(other);
        }
    }

    private void ChangeScene(Collision2D other)
    {
        MissionManager.instance.paused = true;

        if (other.gameObject.tag.Equals("Player"))
        {
            switch (gameObject.tag)
            {
                case "DoorToLivingroom":
                    MissionManager.LoadScene(1);
                    break;
                case "DoorToAlley":
                    MissionManager.LoadScene(2);
                    break;
                case "DoorToGarden":
                    MissionManager.LoadScene(3);
                    break;
                case "DoorToKitchen":
                    MissionManager.LoadScene(4);
                    break;
                case "DoorToMomRoom":
                    MissionManager.LoadScene(5);
                    break;
                case "DoorToKidRoom":
                    MissionManager.LoadScene(6);
                    break;
                default:
                    MissionManager.instance.paused = false;
                    break;
            }
        }
    }
}