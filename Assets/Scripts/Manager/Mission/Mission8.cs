using UnityEngine;
using System.Collections.Generic;
using CrowShadowManager;
using CrowShadowNPCs;
using CrowShadowObjects;
using CrowShadowPlayer;
using CrowShadowScenery;

public class Mission8 : Mission {
    enum enumMission { NIGHT, INICIO, ENCONTRAR_MATERIAIS, FENDA1, FENDA2, FENDA_PORAO, FINAL};
    enumMission secao;

    private float timeToTip = 2;
    private int timesInMI = 0;

    public override void InitMission()
	{
		sceneInit = "QuartoKid";
		GameManager.initMission = true;
		GameManager.initX = (float) 3.0;
		GameManager.initY = (float) 0.2;
		GameManager.initDir = 3;
        GameManager.LoadScene(sceneInit);
        secao = enumMission.NIGHT;
        Book.bookBlocked = false;

        GameManager.instance.invertWorld = false;
        GameManager.instance.invertWorldBlocked = false;

        GameManager.instance.RandomObjectsPlaces(Inventory.InventoryItems.ISQUEIRO, new List<string> { "QuartoKid" });

        SetInitialSettings();
    }

	public override void UpdateMission() //aqui coloca as ações do update específicas da missão
	{
        if (secao == enumMission.NIGHT)
        {
            if (!GameManager.instance.showMissionStart)
            {
                EspecificaEnum((int)enumMission.INICIO);
            }
        }
    }

	public override void SetCorredor()
	{
        GameManager.instance.scenerySounds.StopSound();

        GameObject portaMae = GameObject.Find("DoorToMomRoom").gameObject;
        portaMae.GetComponent<SceneDoor>().isOpened = false;

        GameObject portaCozinha = GameObject.Find("DoorToKitchen").gameObject;
        portaCozinha.GetComponent<SceneDoor>().isOpened = false;

        GameObject portaQuarto = GameObject.Find("DoorToKidRoom").gameObject;
        portaQuarto.GetComponent<SceneDoor>().isOpened = false;
        
        GameObject portaSala = GameObject.Find("DoorToLivingRoom").gameObject;
        portaSala.GetComponent<SceneDoor>().isOpened = false;
        
        GameObject.Find("VasoNaoEmpurravel").gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/vasoPlanta_quebrado");

        //GameManager.instance.rpgTalk.NewTalk ("M5CorridorSceneStart", "M5CorridorSceneEnd");

        //Minion Emitter no canto inferior esquerdo
        GameObject aux2 = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(5f, 0f, 0));
        MinionEmmitter minionEmitter2 = aux2.GetComponent<MinionEmmitter>();
        minionEmitter2.numMinions = 3;
        minionEmitter2.hydraEffect = true;
        minionEmitter2.limitX0 = 0.5f;
        minionEmitter2.limitXF = 6.95f;
        minionEmitter2.limitY0 = 0f;
        minionEmitter2.limitYF = 3f;
    }

	public override void SetCozinha()
	{
        GameManager.instance.scenerySounds.PlayDrop();
        //GameManager.instance.rpgTalk.NewTalk ("M5KitchenSceneStart", "M5KitchenSceneEnd");

        // Panela para caso ainda não tenha
        if (!Inventory.HasItemType(Inventory.InventoryItems.TAMPA))
        {
            GameObject panela = GameObject.Find("Panela").gameObject;
            panela.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/panela_tampa");
            panela.GetComponent<ScenePickUpObject>().enabled = true;
        }

        //Minion Emitter
        GameObject aux2 = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(2f, 0f, 0));
        MinionEmmitter minionEmitter2 = aux2.GetComponent<MinionEmmitter>();
        minionEmitter2.numMinions = 3;
        minionEmitter2.hydraEffect = true;
        minionEmitter2.limitX0 = 0.5f;
        minionEmitter2.limitXF = 6.95f;
        minionEmitter2.limitY0 = 0f;
        minionEmitter2.limitYF = 3f;
    }

	public override void SetJardim()
    {
        GameObject portaSala = GameObject.Find("DoorToLivingRoom").gameObject;
        portaSala.GetComponent<SceneDoor>().isOpened = false;

        //Minion Emitter ao lado da entrada do porão
        GameObject aux = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(6.05f, 3.15f, 0));
        MinionEmmitter minionEmitter = aux.GetComponent<MinionEmmitter>();
        minionEmitter.numMinions = 5;
        minionEmitter.hydraEffect = true;
        minionEmitter.limitX0 = 0.5f;
        minionEmitter.limitXF = 6.95f;
        minionEmitter.limitY0 = 0f;
        minionEmitter.limitYF = 3f;

        //Minion Emitter
        GameObject aux2 = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(-3f, 3f, 0));
        MinionEmmitter minionEmitter2 = aux2.GetComponent<MinionEmmitter>();
        minionEmitter2.numMinions = 3;
        minionEmitter2.hydraEffect = true;
        minionEmitter2.limitX0 = 0.5f;
        minionEmitter2.limitXF = 6.95f;
        minionEmitter2.limitY0 = 0f;
        minionEmitter2.limitYF = 3f;
    }

	public override void SetQuartoKid()
	{
        //GameManager.instance.rpgTalk.NewTalk ("M5KidRoomSceneStart", "M5KidRoomSceneEnd");

        if (GameManager.instance.mission2ContestaMae)
        {
            // Arranhao
            GameManager.instance.AddObject("Scenery/Garra", "", new Vector3(-1.48f, 1.81f, 0), new Vector3(0.1f, 0.1f, 1));
        }
        else
        {
            // Vela
            GameObject velaFixa = GameObject.Find("velaMesa").gameObject;
            velaFixa.transform.GetChild(0).gameObject.SetActive(true);
            velaFixa.transform.GetChild(1).gameObject.SetActive(true);
            velaFixa.transform.GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = 140;
        }

        if (Cat.instance == null)
        {
            // Gato
            GameObject cat = GameManager.instance.AddObject(
                "NPCs/catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }


        //Minion Emitter no canto inferior esquerdo
        GameObject aux = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(-2f, 0f, 0));
        MinionEmmitter minionEmitter = aux.GetComponent<MinionEmmitter>();
        minionEmitter.numMinions = 3;
        minionEmitter.hydraEffect = true;
        minionEmitter.limitX0 = 0.5f;
        minionEmitter.limitXF = 6.95f;
        minionEmitter.limitY0 = 0f;
        minionEmitter.limitYF = 3f;

        //Minion Emitter no canto inferior esquerdo
        GameObject aux2 = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(2f, -1f, 0));
        MinionEmmitter minionEmitter2 = aux2.GetComponent<MinionEmmitter>();
        minionEmitter2.numMinions = 3;
        minionEmitter2.hydraEffect = true;
        minionEmitter2.limitX0 = 0.5f;
        minionEmitter2.limitXF = 6.95f;
        minionEmitter2.limitY0 = 0f;
        minionEmitter2.limitYF = 3f;
    }

	public override void SetQuartoMae()
	{
		//GameManager.instance.rpgTalk.NewTalk ("M5MomRoomSceneStart", "M5MomRoomSceneEnd");
	}


    public override void SetSala()
    {
        //Minion Emitter no meio da sala
        GameObject aux = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(0f, 0f, 0));
        MinionEmmitter minionEmitter = aux.GetComponent<MinionEmmitter>();
        minionEmitter.numMinions = 3;
        minionEmitter.hydraEffect = true;
        minionEmitter.limitX0 = 0.5f;
        minionEmitter.limitXF = 6.95f;
        minionEmitter.limitY0 = 0f;
        minionEmitter.limitYF = 3f;

        //Minion Emitter no canto inferior esquerdo
        GameObject aux2 = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(-7f, -1f, 0));
        MinionEmmitter minionEmitter2 = aux2.GetComponent<MinionEmmitter>();
        minionEmitter2.numMinions = 3;
        minionEmitter2.hydraEffect = true;
        minionEmitter2.limitX0 = 0.5f;
        minionEmitter2.limitXF = 6.95f;
        minionEmitter2.limitY0 = 0f;
        minionEmitter2.limitYF = 3f;

    }
    public override void SetBanheiro()
    {

    }

    public override void EspecificaEnum(int pos)
    {
        secao = (enumMission)pos;
        GameManager.instance.Print("SECAO: " + secao);

        if (secao == enumMission.INICIO)
        {
            
        }
    }

    public override void ForneceDica()
    {

    }

}