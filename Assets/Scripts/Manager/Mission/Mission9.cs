﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Mission9 : Mission {
    enum enumMission { INICIO, DICA, FINAL };
    enumMission secao;

    public override void InitMission()
    {
        // colocar de volta para versão final
         sceneInit = "QuartoKid";
        MissionManager.initMission = true;
        MissionManager.initX = (float) 1.5;
        MissionManager.initY = (float) 0.2;
        MissionManager.initDir = 3;
        SceneManager.LoadScene(sceneInit, LoadSceneMode.Single);
        secao = enumMission.INICIO;
    }

    public override void UpdateMission() //aqui coloca as ações do update específicas da missão
    {
        if (secao == enumMission.DICA)
        {
            MissionManager.instance.rpgTalk.NewTalk("Dica9", "Dica9End");
        }
    }

    public override void SetCorredor()
    {
       
    }

    public override void SetCozinha()
    {

        GameObject panela = GameObject.Find("Panela").gameObject;
        GameObject.Destroy(panela);
    }

    public override void SetJardim()
    {
		
    }

    public override void SetQuartoKid()
    {

    }

    public override void SetQuartoMae()
    {
		
    }

    public override void SetSala()
    {
       
    }

    public override void EspecificaEnum(int pos)
    {
        secao = (enumMission)pos;
    }
}
