﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SickCrow : MonoBehaviour
{
    public bool fly = false;
    public GameObject armario;

    public Animation animationSick { get { return GetComponent<Animation>(); } }

    void Start()
    {
        /* if(MissionManager.instance.currentMission != 1)
         {
             this.gameObject.SetActive(false);
         }
         else
         {*/

            animationSick.Play("CrowSick");
            armario.GetComponent<SceneObject>().isCrowSick = true;

        //}
    }

    private void Update()
    {
        if (fly)
        {
            animationSick.Stop();
            animationSick.Play("SickCrowImage");
        }
    }
    

}
