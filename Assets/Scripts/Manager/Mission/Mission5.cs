using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mission5 : Mission {
    enum enumMission { INICIO, DICA, FINAL };
    enumMission secao;

    public override void InitMission()
	{
		sceneInit = "Jardim";
		MissionManager.initMission = true;
		MissionManager.initX = (float) 3;
		MissionManager.initY = (float) 0.2;
		MissionManager.initDir = 3;
		SceneManager.LoadScene(sceneInit, LoadSceneMode.Single);
        secao = enumMission.INICIO;
    }

	public override void UpdateMission() //aqui coloca as ações do update específicas da missão
	{
        if (secao == enumMission.DICA)
        {
            MissionManager.instance.rpgTalk.NewTalk("Dica5", "Dica5End");
        }
    }

	public override void SetCorredor()
	{
		//MissionManager.instance.rpgTalk.NewTalk ("M5CorridorSceneStart", "M5CorridorSceneEnd");
	}

	public override void SetCozinha()
	{
        //MissionManager.instance.rpgTalk.NewTalk ("M5KitchenSceneStart", "M5KitchenSceneEnd");

        GameObject panela = GameObject.Find("Panela").gameObject;
        GameObject.Destroy(panela);
    }

	public override void SetJardim()
    {
        if (MissionManager.instance.countGardenDialog == 0)
        {
            MissionManager.instance.rpgTalk.NewTalk("M5GardenSceneStart", "M5GardenSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountGardenDialog");
        }
    }

	public override void SetQuartoKid()
	{
		//MissionManager.instance.rpgTalk.NewTalk ("M5KidRoomSceneStart", "M5KidRoomSceneEnd");
	}

	public override void SetQuartoMae()
	{
		//MissionManager.instance.rpgTalk.NewTalk ("M5MomRoomSceneStart", "M5MomRoomSceneEnd");
	}


	public override void SetSala()
	{
		//MissionManager.instance.AddObject("MovingObject", new Vector3(0, 0, 0), new Vector3(1, 1, 1));
		//GameObject.Find("PickUpLanterna").gameObject.SetActive(false);	
		//	MissionManager.instance.rpgTalk.NewTalk ("M5LivingRoomSceneStart", "M5LivingroomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountLivingroomDialog");
	}

    public override void EspecificaEnum(int pos)
    {
        secao = (enumMission)pos;
    }

    public void AddCountGardenDialog(){
		MissionManager.instance.countGardenDialog++;

	}
}