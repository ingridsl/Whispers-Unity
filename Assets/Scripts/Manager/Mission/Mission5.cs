using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Mission5 : Mission {
    enum enumMission { NIGHT, INICIO, DICA, MIFIRST, SPIRITS_KILLED, BOOK_OPENED, BOOK_CLOSED, SALA, CORREDOR, TENTASAIR, FINAL};
    enumMission secao;

    private float timeToTip = 2;
    private int timesInMI = 0;

    public override void InitMission()
	{
		sceneInit = "Jardim";
		MissionManager.initMission = true;
		MissionManager.initX = (float) 3.0;
		MissionManager.initY = (float) 0.2;
		MissionManager.initDir = 3;
		MissionManager.LoadScene(sceneInit);
        secao = enumMission.NIGHT;
        Book.bookBlocked = false;

        MissionManager.instance.invertWorld = false;
        MissionManager.instance.invertWorldBlocked = false;

        SetInitialSettings();

        // Adiciona as páginas
        Book.pages[0] = true;
        Book.pages[1] = Book.pages[2] = Book.pages[3] = Book.pages[4] = false;
        Book.pageQuantity = 1;
    }

	public override void UpdateMission() //aqui coloca as ações do update específicas da missão
	{
        if (secao == enumMission.NIGHT)
        {
            if (!MissionManager.instance.showMissionStart)
            {
                EspecificaEnum((int)enumMission.INICIO);
            }
        }

        if (timesInMI == 0 && MissionManager.instance.invertWorld)
        {
            timesInMI++;
            EspecificaEnum((int)enumMission.MIFIRST);
        }

        if (secao != enumMission.DICA && timesInMI == 0)
        {
            timeToTip -= Time.deltaTime;
            if(timeToTip <= 0)
            {
                EspecificaEnum((int)enumMission.DICA);
            }
        }
        if (secao == enumMission.MIFIRST)
        {
            if(SpiritManager.goodSpiritGardenKilled == 5)
            {
                EspecificaEnum((int)enumMission.SPIRITS_KILLED);
            }
        }

        if (secao == enumMission.SPIRITS_KILLED && Book.pages[1/*0*/] == true && Book.show == true && MissionManager.instance.invertWorld)
        {
            EspecificaEnum((int)enumMission.BOOK_OPENED);
        }

        if(secao == enumMission.BOOK_OPENED)
        {
            if(Book.show == false)
            {
               EspecificaEnum((int)enumMission.BOOK_CLOSED);
            }
        }

        if(secao == enumMission.BOOK_CLOSED && MissionManager.instance.currentSceneName.Equals("Sala"))
        {
            EspecificaEnum((int)enumMission.SALA);
        }

        if (secao == enumMission.SALA && MissionManager.instance.currentSceneName.Equals("Corredor"))
        {
            EspecificaEnum((int)enumMission.CORREDOR);
        }

        if(secao == enumMission.CORREDOR && !MissionManager.instance.rpgTalk.isPlaying  && CrossPlatformInputManager.GetButtonDown("keyInvert")){
            EspecificaEnum((int)enumMission.TENTASAIR);
        }

        if(secao == enumMission.TENTASAIR && MissionManager.instance.currentSceneName.Equals("QuartoMae"))
        {
            MissionManager.instance.ChangeMission(6);
        }
    }

	public override void SetCorredor()
	{
        MissionManager.instance.scenerySounds.StopSound();

        if (secao != enumMission.TENTASAIR)
        {
            GameObject portaMae = GameObject.Find("DoorToMomRoom").gameObject;
            portaMae.GetComponent<SceneDoor>().isOpened = false;

            GameObject portaCozinha = GameObject.Find("DoorToKitchen").gameObject;
            portaCozinha.GetComponent<SceneDoor>().isOpened = false;

            GameObject portaQuarto = GameObject.Find("DoorToKidRoom").gameObject;
            portaQuarto.GetComponent<SceneDoor>().isOpened = false;
        }
        
        GameObject portaSala = GameObject.Find("DoorToLivingRoom").gameObject;
        portaSala.GetComponent<SceneDoor>().isOpened = false;
        
        GameObject.Find("VasoNaoEmpurravel").gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/vasoPlanta_quebrado");

        //MissionManager.instance.rpgTalk.NewTalk ("M5CorridorSceneStart", "M5CorridorSceneEnd");
    }

	public override void SetCozinha()
	{
        MissionManager.instance.scenerySounds.PlayDrop();
        //MissionManager.instance.rpgTalk.NewTalk ("M5KitchenSceneStart", "M5KitchenSceneEnd");

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
        if (Cat.instance == null)
        {
            // Gato
            GameObject player = GameObject.Find("Player").gameObject;
            GameObject cat = MissionManager.instance.AddObject(
                "catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }

        if (MissionManager.instance.previousSceneName.Equals("GameOver"))
        {
            if (secao == enumMission.MIFIRST)
            {
                SpiritManager.RefreshSpirits();
            }
        }

        if (secao >= enumMission.SPIRITS_KILLED)
        {
            GameObject.Find("SpiritHolderCase").gameObject.transform.Find("SpiritHolder").gameObject.SetActive(false);
        }
        else
        {
            GameObject.Find("SpiritHolderCase").gameObject.transform.Find("SpiritHolder").gameObject.SetActive(true);

            GameObject portaSala = GameObject.Find("DoorToLivingRoom").gameObject;
            portaSala.GetComponent<SceneDoor>().isOpened = false;
            SpiritManager.canSummom = true;
        }

    }

	public override void SetQuartoKid()
	{
        //MissionManager.instance.rpgTalk.NewTalk ("M5KidRoomSceneStart", "M5KidRoomSceneEnd");

        if (MissionManager.instance.mission2ContestaMae)
        {
            // Arranhao
            MissionManager.instance.AddObject("Scenery/Garra", "", new Vector3(-1.48f, 1.81f, 0), new Vector3(0.1f, 0.1f, 1));
        }
        else
        {
            // Vela
            GameObject velaFixa = MissionManager.instance.AddObject("Objects/EmptyObject", "", new Vector3(0.125f, -1.1f, 0), new Vector3(2.5f, 2.5f, 1));
            velaFixa.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Inventory/vela_acesa1");
            velaFixa.GetComponent<SpriteRenderer>().sortingOrder = 140;
            GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true);
        }
    }

	public override void SetQuartoMae()
	{
		//MissionManager.instance.rpgTalk.NewTalk ("M5MomRoomSceneStart", "M5MomRoomSceneEnd");
	}


    public override void SetSala()
    {
        //MissionManager.instance.AddObject("Objects/MovingObject", new Vector3(0, 0, 0), new Vector3(1, 1, 1));
        //GameObject.Find("PickUpLanterna").gameObject.SetActive(false);	
        //	MissionManager.instance.rpgTalk.NewTalk ("M5LivingRoomSceneStart", "M5LivingroomSceneEnd", MissionManager.instance.rpgTalk.txtToParse);

        if (MissionManager.instance.previousSceneName.Equals("GameOver") && Cat.instance == null)
        {
            // Gato
            GameObject player = GameObject.Find("Player").gameObject;
            GameObject cat = MissionManager.instance.AddObject(
                "catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }

        if (MissionManager.instance.previousSceneName.Equals("GameOver"))
        {
            if (secao == enumMission.SALA)
            {
                GameObject.Find("Player").gameObject.GetComponent<Player>().ChangePositionDefault(2.35f, -2f, 2);
                EspecificaEnum((int)enumMission.SALA);
            }
        }
        else if (MissionManager.instance.previousSceneName.Equals("Jardim"))
        {
            if (secao == enumMission.SALA)
            {
                EspecificaEnum((int)enumMission.SALA);
            }
        }
    }

    public override void EspecificaEnum(int pos)
    {
        secao = (enumMission)pos;
        MissionManager.instance.Print("SECAO: " + secao);

        if (secao == enumMission.INICIO)
        {
            MissionManager.instance.rpgTalk.NewTalk("M5GardenSceneStart", "M5GardenSceneEnd", MissionManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.DICA)
        {
            MissionManager.instance.rpgTalk.NewTalk("Dica5", "Dica5End", MissionManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.MIFIRST)
        {
            MissionManager.instance.rpgTalk.NewTalk("DicaMundoInvertido5", "DicaMundoInvertido5End", MissionManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.SPIRITS_KILLED)
        {
            GameObject.Find("SpiritHolderCase").gameObject.transform.Find("SpiritHolder").gameObject.SetActive(false);
            MissionManager.instance.rpgTalk.NewTalk("M5GardenSpiritKilled", "M5GardenSpiritKilledEnd", MissionManager.instance.rpgTalk.txtToParse);
            MissionManager.instance.AddObject("Objects/Pagina", "", new Vector3(-0.72f, -0.98f, 0), new Vector3(0.535f, 0.483f, 1));
        }
        else if (secao == enumMission.BOOK_CLOSED)
        {
            MissionManager.instance.rpgTalk.NewTalk("M5BookClosed", "M5BookClosedEnd", MissionManager.instance.rpgTalk.txtToParse);
            GameObject portaSala = GameObject.Find("DoorToLivingRoom").gameObject;
            portaSala.GetComponent<SceneDoor>().isOpened = true;
            MissionManager.instance.invertWorldBlocked = true;
        }
        else if (secao == enumMission.SALA)
        {
            MissionManager.instance.InvertWorld(true);
            MissionManager.instance.rpgTalk.NewTalk("M5LivingroomSceneStart", "M5LivingroomSceneEnd", MissionManager.instance.rpgTalk.txtToParse);
            MissionManager.instance.AddObject("Scenery/KillerSpirit", "", new Vector3(-3.878462f, -0.537204f, 0), new Vector3(1f, 1f, 1));
            MissionManager.instance.AddObject("Scenery/KillerSpirit", "", new Vector3(-5.441667f, -0.802685f, 0), new Vector3(1f, 1f, 1));
            MissionManager.instance.AddObject("Scenery/KillerSpirit", "", new Vector3(-2.008199f, 0.3697177f, 0), new Vector3(1f, 1f, 1));
            MissionManager.instance.AddObject("Scenery/KillerSpirit", "", new Vector3(-0.2775087f, -1.360973f, 0), new Vector3(1f, 1f, 1));
            MissionManager.instance.AddObject("Scenery/KillerSpirit", "", new Vector3(2.178955f, 0.1743171f, 0), new Vector3(1f, 1f, 1));
            MissionManager.instance.AddObject("Scenery/KillerSpirit", "", new Vector3(2.402271f, -1.27723f, 0), new Vector3(1f, 1f, 1));
            MissionManager.instance.AddObject("Scenery/KillerSpirit", "", new Vector3(-3.013116f, -1.388887f, 0), new Vector3(1f, 1f, 1));
            MissionManager.instance.AddObject("Scenery/KillerSpirit", "", new Vector3(-6.418669f, -0.02108344f, 0), new Vector3(1f, 1f, 1));
            MissionManager.instance.AddObject("Scenery/KillerSpirit", "", new Vector3(3.128043f, 1.067577f, 0), new Vector3(1f, 1f, 1));
            MissionManager.instance.AddObject("Scenery/KillerSpirit", "", new Vector3(0.6436655f, 1.430463f, 0), new Vector3(1f, 1f, 1));
        }
        else if (secao == enumMission.CORREDOR)
        {
            MissionManager.instance.rpgTalk.NewTalk("M5CorridorSceneStart", "M5CorridorSceneEnd", MissionManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.TENTASAIR)
        {
            MissionManager.instance.rpgTalk.NewTalk("M5TryQuit", "M5TryQuitEnd", MissionManager.instance.rpgTalk.txtToParse);

            GameObject portaMae = GameObject.Find("DoorToMomRoom").gameObject;
            portaMae.GetComponent<SceneDoor>().isOpened = true;

            GameObject portaKid = GameObject.Find("DoorToKidRoom").gameObject;
            portaKid.GetComponent<SceneDoor>().isOpened = true;

            GameObject portaCozinha = GameObject.Find("DoorToKitchen").gameObject;
            portaCozinha.GetComponent<SceneDoor>().isOpened = true;
        }
    }
    
}