using UnityEngine;
using System.Collections.Generic;
using CrowShadowManager;
using CrowShadowNPCs;
using CrowShadowObjects;
using CrowShadowPlayer;
using CrowShadowScenery;

public class Mission8 : Mission {
    enum enumMission { NIGHT, INICIO, FINAL};
    enumMission secao;

    private bool em_corredor = false, em_cozinha = false, em_jardim1 = false, em_jardim2 = false,
        em_kid1 = false, em_kid2 = false, em_sala1 = false, em_sala2 = false, em_banheiro = false;
    MinionEmmitter minionEmitter_corredor = null, minionEmitter_cozinha = null, minionEmitter_jardim1 = null, minionEmitter_jardim2 = null,
        minionEmitter_kid1 = null, minionEmitter_kid2 = null, minionEmitter_sala1 = null, minionEmitter_sala2 = null, minionEmitter_banheiro = null;
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

        if (!GameManager.instance.mission2ContestaMae)
        {
            em_kid1 = em_kid2 = true;
        }

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

        // HACK - abre a cozinha, abre todas
        if (em_cozinha)
        {
            em_corredor = em_jardim1 = em_jardim2 = em_kid1 = em_kid2 = em_sala1 = em_sala2 = em_banheiro = true;
            ExtrasManager.canActivatePage4 = true;
        }
    }

	public override void SetCorredor()
	{
        GameManager.instance.scenerySounds.StopSound();
        
        GameObject.Find("VasoNaoEmpurravel").gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/vasoPlanta_quebrado");

        //GameManager.instance.rpgTalk.NewTalk ("M5CorridorSceneStart", "M5CorridorSceneEnd");

        if (!em_corredor) {
            //Minion Emitter no canto inferior esquerdo
            GameObject aux2 = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(5f, 0f, 0));
            minionEmitter_corredor = aux2.GetComponent<MinionEmmitter>();
            minionEmitter_corredor.destructible = true;
            minionEmitter_corredor.numMinions = 10;
            minionEmitter_corredor.hydraEffect = true;
            minionEmitter_corredor.limitX0 = 0.5f;
            minionEmitter_corredor.limitXF = 6.95f;
            minionEmitter_corredor.limitY0 = 0f;
            minionEmitter_corredor.limitYF = 3f;
        }
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

        if (!em_cozinha) {
            //Minion Emitter
            GameObject aux2 = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(2f, 0f, 0));
            minionEmitter_cozinha = aux2.GetComponent<MinionEmmitter>();
            minionEmitter_cozinha.destructible = true;
            minionEmitter_cozinha.numMinions = 12;
            minionEmitter_cozinha.hydraEffect = true;
            minionEmitter_cozinha.limitX0 = 0.5f;
            minionEmitter_cozinha.limitXF = 6.95f;
            minionEmitter_cozinha.limitY0 = 0f;
            minionEmitter_cozinha.limitYF = 3f;
        }
    }

	public override void SetJardim()
    {

        if (!em_jardim1) {
            //Minion Emitter ao lado da entrada do porão
            GameObject aux = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(6.05f, 3.15f, 0));
            minionEmitter_jardim1 = aux.GetComponent<MinionEmmitter>();
            minionEmitter_jardim1.destructible = true;
            minionEmitter_jardim1.numMinions = 30;
            minionEmitter_jardim1.hydraEffect = true;
            minionEmitter_jardim1.limitX0 = 0.5f;
            minionEmitter_jardim1.limitXF = 6.95f;
            minionEmitter_jardim1.limitY0 = 0f;
            minionEmitter_jardim1.limitYF = 3f;
        }

        if (!em_jardim2)
        {
            //Minion Emitter
            GameObject aux2 = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(-3f, 3f, 0));
            minionEmitter_jardim2 = aux2.GetComponent<MinionEmmitter>();
            minionEmitter_jardim2.destructible = true;
            minionEmitter_jardim2.numMinions = 20;
            minionEmitter_jardim2.hydraEffect = true;
            minionEmitter_jardim2.limitX0 = 0.5f;
            minionEmitter_jardim2.limitXF = 6.95f;
            minionEmitter_jardim2.limitY0 = 0f;
            minionEmitter_jardim2.limitYF = 3f;
        }
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

        if (!Inventory.HasItemType(Inventory.InventoryItems.BASTAO))
        {
            GameManager.instance.CreateScenePickUp("Bau", Inventory.InventoryItems.BASTAO);
        }

        if (Cat.instance == null)
        {
            // Gato
            GameObject cat = GameManager.instance.AddObject(
                "NPCs/catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }

        if (!em_kid1)
        {
            //Minion Emitter no canto inferior esquerdo
            GameObject aux = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(-2f, 0f, 0));
            minionEmitter_kid1 = aux.GetComponent<MinionEmmitter>();
            minionEmitter_kid1.destructible = true;
            minionEmitter_kid1.numMinions = 6;
            minionEmitter_kid1.hydraEffect = true;
            minionEmitter_kid1.limitX0 = 0.5f;
            minionEmitter_kid1.limitXF = 6.95f;
            minionEmitter_kid1.limitY0 = 0f;
            minionEmitter_kid1.limitYF = 3f;
        }

        if (!em_kid2) {
            //Minion Emitter no canto inferior esquerdo
            GameObject aux2 = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(2f, -1f, 0));
            minionEmitter_kid2 = aux2.GetComponent<MinionEmmitter>();
            minionEmitter_kid2.destructible = true;
            minionEmitter_kid2.numMinions = 6;
            minionEmitter_kid2.hydraEffect = true;
            minionEmitter_kid2.limitX0 = 0.5f;
            minionEmitter_kid2.limitXF = 6.95f;
            minionEmitter_kid2.limitY0 = 0f;
            minionEmitter_kid2.limitYF = 3f;
        }
    }

	public override void SetQuartoMae()
	{
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        GameObject porta = GameObject.Find("DoorToAlley").gameObject;
        porta.GetComponent<SceneDoor>().isOpened = true;

        GameObject Luminaria = GameObject.Find("Luminaria").gameObject;
        Luminaria.GetComponent<Light>().enabled = true;

        GameObject camaMae = GameObject.Find("Cama").gameObject;
        camaMae.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/camaMaeDoente");
    }


    public override void SetSala()
    {
        if (!em_sala1)
        {
            //Minion Emitter no meio da sala
            GameObject aux = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(0f, 0f, 0));
            minionEmitter_sala1 = aux.GetComponent<MinionEmmitter>();
            minionEmitter_sala1.destructible = true;
            minionEmitter_sala1.numMinions = 15;
            minionEmitter_sala1.hydraEffect = true;
            minionEmitter_sala1.limitX0 = 0.5f;
            minionEmitter_sala1.limitXF = 6.95f;
            minionEmitter_sala1.limitY0 = 0f;
            minionEmitter_sala1.limitYF = 3f;
        }

        if (!em_sala2) {
            //Minion Emitter no canto inferior esquerdo
            GameObject aux2 = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(-7f, -1f, 0));
            minionEmitter_sala2 = aux2.GetComponent<MinionEmmitter>();
            minionEmitter_sala2.destructible = true;
            minionEmitter_sala2.numMinions = 20;
            minionEmitter_sala2.hydraEffect = true;
            minionEmitter_sala2.limitX0 = 0.5f;
            minionEmitter_sala2.limitXF = 6.95f;
            minionEmitter_sala2.limitY0 = 0f;
            minionEmitter_sala2.limitYF = 3f;
        }

    }

    public override void SetBanheiro()
    {
        if (!em_banheiro)
        {
            //Minion Emitter no meio do banheiro
            GameObject aux = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(-1f, 0f, 0));
            minionEmitter_banheiro = aux.GetComponent<MinionEmmitter>();
            minionEmitter_banheiro.destructible = true;
            minionEmitter_banheiro.numMinions = 8;
            minionEmitter_banheiro.hydraEffect = true;
            minionEmitter_banheiro.limitX0 = 0.5f;
            minionEmitter_banheiro.limitXF = 6.95f;
            minionEmitter_banheiro.limitY0 = 0f;
            minionEmitter_banheiro.limitYF = 3f;
        }
    }

    public override void SetPorao()
    {
        EspecificaEnum((int)enumMission.FINAL);
    }

    public override void EspecificaEnum(int pos)
    {
        secao = (enumMission)pos;
        GameManager.instance.Print("SECAO: " + secao);

        if (secao == enumMission.FINAL)
        {
            GameManager.instance.ChangeMission(10);
        }
    }

    public override void ForneceDica()
    {

    }

    public override void InvokeMission()
    {
        if (GameManager.currentSceneName.Equals("Corredor") && minionEmitter_corredor == null)
        {
            em_corredor = true;
        }
        else if (GameManager.currentSceneName.Equals("Cozinha") && minionEmitter_cozinha == null)
        {
            em_cozinha = true;
        }
        else if (GameManager.currentSceneName.Equals("Jardim") && minionEmitter_jardim1 == null)
        {
            em_jardim1 = true;
        }
        else if (GameManager.currentSceneName.Equals("Jardim2") && minionEmitter_jardim2 == null)
        {
            em_jardim2 = true;
        }
        else if (GameManager.currentSceneName.Equals("QuartoKid") && minionEmitter_kid1 == null)
        {
            em_kid1 = true;
        }
        else if (GameManager.currentSceneName.Equals("QuartoKid") && minionEmitter_kid2 == null)
        {
            em_kid2 = true;
        }
        else if (GameManager.currentSceneName.Equals("Sala") && minionEmitter_sala1 == null)
        {
            em_sala1 = true;
        }
        else if (GameManager.currentSceneName.Equals("Sala") && minionEmitter_sala2 == null)
        {
            em_sala2 = true;
        }
        else if (GameManager.currentSceneName.Equals("Banheiro") && minionEmitter_banheiro == null)
        {
            em_banheiro = true;
        }
    }

}