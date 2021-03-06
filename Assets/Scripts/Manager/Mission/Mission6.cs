using UnityEngine;
using CrowShadowManager;
using CrowShadowNPCs;
using CrowShadowObjects;
using CrowShadowPlayer;
using CrowShadowScenery;

public class Mission6 : Mission {
    enum enumMission { NIGHT, INICIO, LIGOULANTERNA, DESLIGOULANTERNA, SALA, GATOCOMFOLHA, DICA,
        ACENDE, TENTAOUTRAARVORE, TENTOUARVORECERTA, TEMTUDO, NAOTEMTUDO, TEMTUDOFORAJARDIM, FOGO, CHECOUMAE, FINAL };
    enumMission secao;

    GameObject fireEvent;

    bool arvore1Trigger = false, arvoreOtherTrigger = false;
    bool arvoreBurn = false, doorSet = false;

    public override void InitMission()
	{
		sceneInit = "QuartoMae";
		GameManager.initMission = true;
		GameManager.initX = (float) 3;
		GameManager.initY = (float) 0.2;
		GameManager.initDir = 3;
		GameManager.LoadScene(sceneInit);
        secao = enumMission.NIGHT;
        Book.bookBlocked = false;

        GameManager.instance.invertWorld = true;
        GameManager.instance.invertWorldBlocked = true;

        SetInitialSettings();

        fosforoMiniGame.posFlareX = -1.37f;
        fosforoMiniGame.posFlareY = 3.51f;
        isqueiroMiniGame.posFlareX = -1.37f;
        isqueiroMiniGame.posFlareY = 3.51f;
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
        if (secao == enumMission.INICIO && Inventory.IsCurrentItemType(Inventory.InventoryItems.FLASHLIGHT, true))
        {
            EspecificaEnum((int)enumMission.LIGOULANTERNA);
        }

        if (secao == enumMission.LIGOULANTERNA && !Inventory.IsCurrentItemType(Inventory.InventoryItems.FLASHLIGHT, true))
        {
            EspecificaEnum((int)enumMission.DESLIGOULANTERNA);
        }

        if (secao == enumMission.DESLIGOULANTERNA && GameManager.currentSceneName.Equals("Sala"))
        {
            EspecificaEnum((int)enumMission.SALA);
        }
        if(secao == enumMission.SALA)//&& Spirit.maxEvilKilled < 2)
        {
            EspecificaEnum((int)enumMission.DICA);
        }
        if ((secao == enumMission.SALA || secao == enumMission.DICA)
            && GameManager.currentSceneName.Equals("Jardim"))
        {
            EspecificaEnum((int)enumMission.GATOCOMFOLHA);
        }
        if ((secao == enumMission.SALA || secao == enumMission.DICA) &&
            !doorSet && GameManager.currentSceneName.Equals("Sala"))
        {
            GameObject portaJardim = GameObject.Find("DoorToGarden").gameObject;
            portaJardim.GetComponent<SceneDoor>().isOpened = true;
            doorSet = true;
        }
        
        if (secao == enumMission.GATOCOMFOLHA && arvoreOtherTrigger)
        {
            EspecificaEnum((int)enumMission.TENTAOUTRAARVORE);
        }

        if ((secao == enumMission.GATOCOMFOLHA || secao == enumMission.TENTAOUTRAARVORE) && arvore1Trigger)
        {
            EspecificaEnum((int)enumMission.TENTOUARVORECERTA);
        }

        if(secao == enumMission.NAOTEMTUDO && (Inventory.HasItemType(Inventory.InventoryItems.FOSFORO)
            || Inventory.HasItemType(Inventory.InventoryItems.ISQUEIRO)) && !GameManager.currentSceneName.Equals("Jardim"))
        {
            EspecificaEnum((int)enumMission.TEMTUDOFORAJARDIM);
            arvore1Trigger = false;
        }
        if(secao == enumMission.TEMTUDOFORAJARDIM && GameManager.currentSceneName.Equals("Jardim") && arvore1Trigger)
        {
            EspecificaEnum((int)enumMission.TEMTUDO);
        }

        if (secao == enumMission.TEMTUDO)
        {
            if (fosforoMiniGame.achievedGoal || isqueiroMiniGame.achievedGoal)
            {
                fosforoMiniGame.achievedGoal = false;
                isqueiroMiniGame.achievedGoal = false;

                fosforoMiniGame.active = false;
                isqueiroMiniGame.active = false;

                if (arvore1Trigger)
                {
                    arvoreBurn = true;
                    fireEvent = GameObject.Find("FireEventHolder").gameObject.transform.Find("FireEventTree").gameObject;
                    fireEvent.SetActive(true);
                    EspecificaEnum((int)enumMission.FOGO);
                }
            }
        }

        if(secao == enumMission.FOGO)
        {
            EspecificaEnum((int)enumMission.CHECOUMAE);
        }

        if(secao == enumMission.CHECOUMAE && !GameManager.instance.invertWorld)
        {
            EspecificaEnum((int)enumMission.FINAL);
        }

        if(secao == enumMission.FINAL && !GameManager.instance.rpgTalk.isPlaying)
        {
            GameManager.instance.InvertWorld(false);
            GameManager.instance.ChangeMission(7);
        }
    }

	public override void SetCorredor()
	{
        GameManager.instance.scenerySounds.StopSound();

        GameObject.Find("VasoNaoEmpurravel").gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/vasoPlanta_quebrado");
        //GameManager.instance.rpgTalk.NewTalk ("M6CorridorSceneStart", "M6CorridorSceneEnd");
    }

	public override void SetCozinha()
	{
        GameManager.instance.scenerySounds.PlayDrop();
        //GameManager.instance.rpgTalk.NewTalk ("M6KitchenSceneStart", "M6KitchenSceneEnd");

        // Panela para caso ainda não tenha
        if (!Inventory.HasItemType(Inventory.InventoryItems.TAMPA))
        {
            GameObject panela = GameObject.Find("Panela").gameObject;
            panela.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/panela_tampa");
            panela.GetComponent<ScenePickUpObject>().enabled = true;
        }
    }

	public override void SetJardim()
	{
        //GameManager.instance.rpgTalk.NewTalk ("M6GardenSceneStart", "M6GardenSceneEnd");
        GameManager.instance.AddObject("Scenery/Leafs", "", new Vector3(1.8f, 2.4f, -0.5f), new Vector3(1f, 1f, 1));
        GameManager.instance.AddObject("NPCs/catFollower", "", new Vector3(2f, 1.5f, -0.5f), new Vector3(0.15f, 0.15f, 1));

        GameObject triggerArv1 = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(-1.379f, 3.51f, 0), new Vector3(1, 1, 1));
        triggerArv1.name = "Arv1Trigger";
        triggerArv1.GetComponent<Collider2D>().offset = new Vector2(0, 0);
        triggerArv1.GetComponent<BoxCollider2D>().size = new Vector2(0.7f, 2f);

        GameObject triggerArv2 = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(-5.02f, 3.37f, 0), new Vector3(1, 1, 1));
        triggerArv2.name = "Arv2Trigger";
        triggerArv2.GetComponent<Collider2D>().offset = new Vector2(0, 0);
        triggerArv2.GetComponent<BoxCollider2D>().size = new Vector2(0.7f, 2.5f);

        GameObject triggerArv3 = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(-4.37f, -2.12f, 0), new Vector3(1, 1, 1));
        triggerArv3.name = "Arv3Trigger";
        triggerArv3.GetComponent<Collider2D>().offset = new Vector2(0, 0);
        triggerArv3.GetComponent<BoxCollider2D>().size = new Vector2(0.7f, 2f);

        GameObject triggerArv4 = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(4.91f, -1.48f, 0), new Vector3(1, 1, 1));
        triggerArv4.name = "Arv4Trigger";
        triggerArv4.GetComponent<Collider2D>().offset = new Vector2(0, 0);
        triggerArv4.GetComponent<BoxCollider2D>().size = new Vector2(0.7f, 2.5f);

        if (secao == enumMission.FOGO)
        {
            fireEvent = GameObject.Find("FireEventHolder").gameObject.transform.Find("FireEventTree").gameObject;
            fireEvent.SetActive(true);
        }
    }

    public override void SetQuartoKid()
	{
        //GameManager.instance.rpgTalk.NewTalk ("M6KidRoomSceneStart", "M6KidRoomSceneEnd");

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
    }

	public override void SetQuartoMae()
	{
        GameManager.instance.scenerySounds.StopSound();

        if (Cat.instance == null)
        {
            // Gato
            GameObject cat = GameManager.instance.AddObject(
                "NPCs/catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }

        if (GameManager.previousSceneName.Equals("GameOver"))
        {
            GameManager.instance.InvertWorld(true);
        }

        GameObject portaCorredor = GameObject.Find("DoorToAlley").gameObject;
        portaCorredor.GetComponent<SceneDoor>().isOpened = false;

        GameObject camaMae = GameObject.Find("Cama").gameObject;
        camaMae.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/camaMaeDoente");

        if (secao != enumMission.FOGO)
        {
            GameManager.instance.AddObject("Scenery/GloomBed", "", new Vector3(4.26f, 1.98f, 0), new Vector3(1f, 1f, 1));
        }
        else
        {
            // LUZ DO AMBIENTE
            mainLight.transform.Rotate(new Vector3(30, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

            GameManager.instance.AddObject("Objects/Pagina", "", new Vector3(3.59f, 1.73f, 0), new Vector3(0.535f, 0.483f, 1));
        }
    }


    public override void SetSala()
	{
        //GameManager.instance.AddObject("Objects/MovingObject", new Vector3(0, 0, 0), new Vector3(1, 1, 1));
        //GameObject.Find("PickUpLanterna").gameObject.SetActive(false);
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(30, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (Cat.instance == null)
        {
            // Gato
            GameObject cat = GameManager.instance.AddObject(
                "NPCs/catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }

        if (GameManager.previousSceneName.Equals("GameOver"))
        {
            GameManager.instance.InvertWorld(true);
        }

        GameManager.instance.CreateScenePickUp("escrivaninha", Inventory.InventoryItems.ISQUEIRO);

        if (secao == enumMission.DESLIGOULANTERNA || secao == enumMission.DICA || secao == enumMission.SALA)
        {

            //if (!Book.pages[2])
            //{
                GameObject portaJardim = GameObject.Find("DoorToGarden").gameObject;
                portaJardim.GetComponent<SceneDoor>().isOpened = false;

                GameManager.instance.AddObject("Objects/Pagina", "", new Vector3(6.2f, 0f, 0), new Vector3(0.535f, 0.483f, 1));
            //}

            //SpiritManager.GenerateSpiritMap();
        }

    }
    public override void SetBanheiro()
    {

    }
    public override void SetPorao()
    {

    }

    public override void EspecificaEnum(int pos)
    {
        secao = (enumMission)pos;
        GameManager.instance.Print("SECAO: " + secao);

        if (secao == enumMission.INICIO)
        {
            GameManager.instance.rpgTalk.NewTalk("M6MomRoomSceneStart", "M6MomRoomSceneEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.LIGOULANTERNA)
        {
            GameManager.instance.rpgTalk.NewTalk("M6AfterFlashlight", "M6AfterFlashlightEnd", GameManager.instance.rpgTalk.txtToParse);

        }
        else if (secao == enumMission.DESLIGOULANTERNA)
        {
            GameManager.instance.rpgTalk.NewTalk("M6AfterFlashlightShutdown", "M6AfterFlashlightShutdownEnd", GameManager.instance.rpgTalk.txtToParse);
            GameObject portaCorredor = GameObject.Find("DoorToAlley").gameObject;
            portaCorredor.GetComponent<SceneDoor>().isOpened = true;
        }
        else if (secao == enumMission.SALA)
        {
            GameManager.instance.rpgTalk.NewTalk("M6LivingroomSceneStart", "M6LivingroomSceneEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.DICA)
        {
            GameManager.instance.rpgTalk.NewTalk("M6LivingroomTipStart", "M6LivingroomTipEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.GATOCOMFOLHA)
        {
            GameManager.instance.scenerySounds.PlayCat(3);
            GameManager.instance.rpgTalk.NewTalk("M6GardenSceneStart", "M6GardenSceneEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.TENTAOUTRAARVORE)
        {
            GameManager.instance.rpgTalk.NewTalk("M6IncorrectTree", "M6IncorrectTreeEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.TENTOUARVORECERTA)
        {
            if (Inventory.HasItemType(Inventory.InventoryItems.FOSFORO) || Inventory.HasItemType(Inventory.InventoryItems.ISQUEIRO))
                EspecificaEnum((int)enumMission.TEMTUDO);
            else
                EspecificaEnum((int)enumMission.NAOTEMTUDO);
        }
        else if (secao == enumMission.TEMTUDO)
        {
            GameManager.instance.rpgTalk.NewTalk("M6HasEverything", "M6HasEverythingEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.NAOTEMTUDO)
        {
            GameManager.instance.rpgTalk.NewTalk("M6HasntEverything", "M6HasntEverythingEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.FOGO)
        {
            GameManager.instance.rpgTalk.NewTalk("M6FireTree", "M6FireTreeEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.CHECOUMAE)
        {
            GameManager.instance.invertWorldBlocked = false;
            GameManager.instance.rpgTalk.NewTalk("M6BeforeFinal", "M6BeforeFinalEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.FINAL)
        {
            GameManager.instance.rpgTalk.NewTalk("M6Final", "M6FinalEnd", GameManager.instance.rpgTalk.txtToParse);
        }
    }

    public override void AreaTriggered(string tag)
    {
        if (tag.Equals("EnterArv1Trigger") && !arvoreBurn)
        {
            arvore1Trigger = true;
            fosforoMiniGame.active = true;
            isqueiroMiniGame.active = true;
        }
        else if (tag.Equals("ExitArv1Trigger") && !arvoreBurn)
        {
            arvore1Trigger = false;
            fosforoMiniGame.active = false;
            isqueiroMiniGame.active = false;
        }
        else if (tag.Equals("EnterArv2Trigger") || tag.Equals("EnterArv3Trigger") || tag.Equals("EnterArv4Trigger"))
        {
            arvoreOtherTrigger = true;
        }
        else if (tag.Equals("ExitArv2Trigger") || tag.Equals("ExitArv3Trigger") || tag.Equals("ExitArv4Trigger"))
        {
            arvoreOtherTrigger = false;
        }
        
    }
    public override void ForneceDica()
    {

    }

}