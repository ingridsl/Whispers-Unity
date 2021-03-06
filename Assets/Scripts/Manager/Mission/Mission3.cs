using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using CrowShadowManager;
using CrowShadowNPCs;
using CrowShadowObjects;
using CrowShadowPlayer;
using CrowShadowScenery;

public class Mission3 : Mission {
    enum enumMission { NIGHT, INICIO, GATO_CORREDOR, QUADRO, MI_DESBLOQUEADO, MI_ATIVADO,
        CORVO_APARECE, CORVO_ATACA, MI_TRAVADO, MAE_APARECE, PASSAROS_FINAL, FINAL };
    enumMission secao;

    GameObject livro, catShadow, person1, person2, crow, tipEmitter; // mesma logica do livro, colocar pros objetos de pessoas andando
    ZoomObject quadro1, quadro2;

    bool quadro1Seen = false, quadro2Seen = false;
    bool livroAtivado = false, specialTrigger = false;

    public override void InitMission()
	{
		sceneInit = "QuartoKid";
		GameManager.initMission = true;
		GameManager.initX = (float) -2.5;
		GameManager.initY = (float) -1.6;
		GameManager.initDir = 3;
		GameManager.LoadScene(sceneInit);
        secao = enumMission.NIGHT;
        Book.bookBlocked = true;

        GameManager.instance.InvertWorld(false);
        GameManager.instance.invertWorldBlocked = true;

        SetInitialSettings();
    }

    public override void UpdateMission() //aqui coloca as ações do update específicas da missão
    {

        if ((int)GameManager.instance.timer == tipTimerSmall || (int)GameManager.instance.timer == tipTimerMedium || (int)GameManager.instance.timer == tipTimerLonger)
        {
            ForneceDica();
        }

        if (secao == enumMission.NIGHT)
        {
            if (!GameManager.instance.showMissionStart)
            {
                EspecificaEnum((int)enumMission.INICIO);
            }
        }
        else if (secao == enumMission.QUADRO && GameManager.currentSceneName.Equals("Corredor"))
        {
            if (quadro1Seen && quadro2Seen)
            {
                EspecificaEnum((int)enumMission.MI_DESBLOQUEADO);
            }
            else if (quadro1.ObjectOpened() && !quadro1Seen)
            {
                GameManager.instance.scenerySounds.PlayCat(4);
                quadro1Seen = true;
            }
            else if (quadro2.ObjectOpened() && !quadro2Seen)
            {
                GameManager.instance.scenerySounds.PlayCat(2);
                quadro2Seen = true;
            }
        }
        else if (secao == enumMission.MI_DESBLOQUEADO)
        {
            if (GameManager.instance.invertWorld)
            {
                EspecificaEnum((int)enumMission.MI_ATIVADO);
            }
        }
        else if (secao == enumMission.MI_ATIVADO)
        {
            if (!GameManager.instance.rpgTalk.isPlaying && 
                (!GameManager.currentSceneName.Equals("GameOver") || !GameManager.currentSceneName.Equals("MainMenu")))
            {
                GameManager.instance.scenerySounds.StopSound();
                GameManager.instance.scenerySounds.PlayBird(1);
                EspecificaEnum((int)enumMission.CORVO_APARECE);
            }

            if (GameManager.instance.invertWorld && !livroAtivado)
            {
                livro.SetActive(true);
                person1.SetActive(true);
                person2.SetActive(true);
                Cat.instance.GetComponent<Cat>().GetComponent<SpriteRenderer>().gameObject.SetActive(false);
                catShadow.GetComponent<SpriteRenderer>().gameObject.SetActive(true);
                livroAtivado = true;
            }
            else if (!GameManager.instance.invertWorld && livroAtivado)
            {
                livro.SetActive(false);
                person1.SetActive(false);
                person2.SetActive(false);
                Cat.instance.GetComponent<Cat>().GetComponent<SpriteRenderer>().gameObject.SetActive(true);
                catShadow.GetComponent<SpriteRenderer>().gameObject.SetActive(false);
                livroAtivado = false;
            }
        }
        else if (secao == enumMission.MI_TRAVADO)
        {
            if (CrossPlatformInputManager.GetButtonDown("keyInvert"))
            {
                GameManager.instance.rpgTalk.NewTalk("M3MundoInvertido3", "M3MundoInvertido3End");
            }
        }
        else if (secao == enumMission.PASSAROS_FINAL)
        {
            if (!GameManager.instance.rpgTalk.isPlaying)
            {
                EspecificaEnum((int)enumMission.FINAL);
            }
        }
    }

	public override void SetCorredor()
	{
        if (GameManager.previousSceneName.Equals("GameOver"))
        {
            EspecificaEnum((int)enumMission.GATO_CORREDOR);
        }

        GameManager.instance.scenerySounds.StopSound();
        //GameManager.instance.rpgTalk.NewTalk ("M3CorridorSceneStart", "M3CorridorSceneEnd");

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(-20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z

        // Definir objetos dos quadros
        GameObject quadro1Object = GameObject.Find("Quadro1").gameObject;
        quadro1 = quadro1Object.GetComponent<ZoomObject>();
        GameObject quadro2Object = GameObject.Find("Quadro2").gameObject;
        quadro2 = quadro1Object.GetComponent<ZoomObject>();

        // Quarto da mãe bloqueado
        GameObject portaMae = GameObject.Find("DoorToMomRoom").gameObject;
        portaMae.GetComponent<SceneDoor>().isOpened = false;

        // Porta Banheiro bloqueada
        GameObject portaBanheiro = GameObject.Find("DoorToBathroom").gameObject;
        float portaBanheiroDefaultY = portaBanheiro.transform.position.y;
        var posX = portaBanheiro.GetComponent<SpriteRenderer>().bounds.size.x / 5;
        portaBanheiro.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
        portaBanheiro.GetComponent<SceneDoor>().isOpened = false;
        portaBanheiro.transform.position = new Vector3(portaBanheiro.transform.position.x - posX, portaBanheiroDefaultY, portaBanheiro.transform.position.z);

        if (secao == enumMission.INICIO || secao == enumMission.GATO_CORREDOR)
        {
            tipEmitter = GameManager.instance.AddObject("Effects/TipEmitter", new Vector3(1.3f, 0.4f, 0));
            GameObject trigger = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(0f, -0.5f, 1), new Vector3(1, 1, 1));
            trigger.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            trigger.GetComponent<BoxCollider2D>().size = new Vector2(1.2f, 1.2f);
            EspecificaEnum((int)enumMission.GATO_CORREDOR);
            GameManager.instance.invertWorldBlocked = true;
        }
    }


    public override void SetPorao()
    {

    }
    public override void SetCozinha()
    {
        GameManager.instance.scenerySounds.PlayDrop();
        //GameManager.instance.rpgTalk.NewTalk ("M3KitchenSceneStart", "M3KitchenSceneEnd");

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(-20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z

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
        if (GameManager.previousSceneName.Equals("GameOver"))
        {
            secao = enumMission.MAE_APARECE; // não chamar as definições do EspecificaEnum
        }

        GameManager.instance.scenerySounds.PlayBird(2);
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(-20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z

        if (secao == enumMission.MAE_APARECE)
        {
            GameObject porta = GameObject.Find("DoorToLivingRoom").gameObject;
            porta.GetComponent<SceneDoor>().isOpened = false;

            GameObject trigger = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(1.3f, 0.4f, 1), new Vector3(1, 1, 1));
            trigger.name = "AreaTriggerCrow";
            trigger.GetComponent<Collider2D>().offset = new Vector2(-4.4f, -2.1f);
            trigger.GetComponent<BoxCollider2D>().size = new Vector2(1.5f, 1.5f);

            GameManager.instance.invertWorldBlocked = false;

            GameManager.instance.rpgTalk.NewTalk("M3GardenSceneStart", "M3GardenSceneEnd");
        }

        GameObject triggerB = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(0f, 0f, 0), new Vector3(1, 1, 1));
        triggerB.name = "SpecialTrigger";
        triggerB.GetComponent<Collider2D>().offset = new Vector2(6f, 3.15f);
        triggerB.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
    }

	public override void SetQuartoKid()
	{
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(-20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        // Condicoes a partir da missao 2
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

        if(secao == enumMission.NIGHT || secao == enumMission.INICIO)
        {
            GameManager.instance.scenerySounds.PlayCat(3);
            GameManager.instance.AddObject("NPCs/catFollower", "", new Vector3(0f, 0f, -0.5f), new Vector3(0.15f, 0.15f, 1));
        }
	}

	public override void SetQuartoMae()
	{
        //GameManager.instance.rpgTalk.NewTalk ("M3MomRoomSceneStart", "M3MomRoomSceneEnd");

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(-20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z
    }


	public override void SetSala()
	{
        //GameManager.instance.rpgTalk.NewTalk ("M3LivingRoomSceneStart", "M3LivingroomSceneEnd", GameManager.instance.rpgTalk.txtToParse);

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z

        if (secao == enumMission.CORVO_ATACA)
        {
            GameObject porta = GameObject.Find("DoorToAlley").gameObject;
            porta.GetComponent<SceneDoor>().isOpened = false;

            GameObject portaG = GameObject.Find("DoorToGarden").gameObject;
            portaG.GetComponent<SceneDoor>().isOpened = false;

            EspecificaEnum((int)enumMission.MI_TRAVADO);
        }
        else if (secao == enumMission.FINAL)
        {
            GameManager.instance.InvertWorld(false);
            GameManager.instance.ChangeMission(4);
        }
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
            GameManager.instance.rpgTalk.NewTalk("M3KidRoomSceneStart", "M3KidRoomSceneEnd", GameManager.instance.rpgTalk.txtToParse);

            Cat.instance.GetComponent<Cat>().Patrol();
            Vector3 aux = new Vector3(1.8f, 0.8f, -0.5f);
            Vector3[] catPos = { aux };
            Cat.instance.targets = catPos;
            Cat.instance.speed = 1.2f;
            Cat.instance.destroyEndPath = true;
        }
        else if (secao == enumMission.GATO_CORREDOR)
        {
            GameObject cat = GameManager.instance.AddObject("NPCs/catFollower", "", new Vector3(8.2f, -0.2f, -0.5f), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().Patrol();
            Vector3 aux = new Vector3(1f, -0.1f, -0.5f);
            Vector3[] catPos = { aux };
            cat.GetComponent<Cat>().targets = catPos;
            cat.GetComponent<Cat>().speed = 1.6f;
            cat.GetComponent<Cat>().stopEndPath = true;
            cat.GetComponent<Cat>().destroyEndPath = false;
        }
        else if (secao == enumMission.MI_DESBLOQUEADO)
        {
            GameManager.instance.AddObject("Tutorial/E-key");
            GameManager.instance.rpgTalk.NewTalk("M3Painting", "M3PaintingEnd");
            GameManager.instance.invertWorldBlocked = false;

            // Porta Mae
            GameObject portaMae = GameObject.Find("DoorToMomRoom").gameObject;
            float portaMaeDefaultY = portaMae.transform.position.y;
            float posX = portaMae.GetComponent<SpriteRenderer>().bounds.size.x / 5;
            portaMae.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
            portaMae.GetComponent<SceneDoor>().isOpened = false;
            portaMae.transform.position = new Vector3(portaMae.transform.position.x - posX, portaMaeDefaultY, portaMae.transform.position.z);

            // Porta Kid
            GameObject portaKid = GameObject.Find("DoorToKidRoom").gameObject;
            float portaKidDefaultY = portaKid.transform.position.y;
            float posXKid = portaKid.GetComponent<SpriteRenderer>().bounds.size.x / 5;
            portaKid.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
            portaKid.GetComponent<SceneDoor>().isOpened = false;
            portaKid.transform.position = new Vector3(portaKid.transform.position.x - posXKid, portaKidDefaultY, portaKid.transform.position.z);

            // Porta Cozinha
            GameObject portaCozinha = GameObject.Find("DoorToKitchen").gameObject;
            portaCozinha.GetComponent<SceneDoor>().isOpened = false;

            // Porta Sala
            GameObject portaSala = GameObject.Find("DoorToLivingRoom").gameObject;
            portaSala.GetComponent<SceneDoor>().isOpened = false;
        }
        else if (secao == enumMission.MI_ATIVADO)
        {
            GameObject.Find("MainCamera").GetComponent<Camera>().orthographicSize = 4;

            // Objetos do mundo invertido
            // Livro
            livro = GameManager.instance.AddObject("Objects/FixedObject", "", new Vector3(6.8f, 0.68f, -0.5f), new Vector3(0.5f, 0.5f, 1));
            livro.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/livro");
            livro.GetComponent<SpriteRenderer>().color = Color.black;
            livro.AddComponent<Light>();
            livroAtivado = true;

            // Gato Sombra
            // VER MAIS CAMINHOS, COMO UMA HISTORIA
            Cat.instance.GetComponent<Cat>().GetComponent<SpriteRenderer>().gameObject.SetActive(false);
            catShadow = GameManager.instance.AddObject("NPCs/catShadow", "", new Vector3(8.2f, -0.2f, -0.5f), new Vector3(0.15f, 0.15f, 1));
            catShadow.GetComponent<Patroller>().isPatroller = true;
            Vector3 aux = new Vector3(6.8f, -0.2f, -0.5f);
            Vector3[] catPos = { aux };
            catShadow.GetComponent<Patroller>().targets = catPos;
            catShadow.GetComponent<Patroller>().speed = 0.9f;
            catShadow.GetComponent<Patroller>().stopEndPath = true;

            // Pessoa 1
            person1 = GameManager.instance.AddObject("NPCs/personShadow", "", new Vector3(10f, 0f, -0.5f), new Vector3(0.3f, 0.3f, 1));
            person1.GetComponent<Patroller>().isPatroller = true;
            Vector3 auxP1 = new Vector3(7.3f, 0f, -0.5f);
            Vector3[] p1Pos = { auxP1 };
            person1.GetComponent<Patroller>().targets = p1Pos;
            person1.GetComponent<Patroller>().speed = 0.9f;
            person1.GetComponent<Patroller>().stopEndPath = true;

            // Pessoa 2
            person2 = GameManager.instance.AddObject("NPCs/personShadow", "", new Vector3(-1f, 0f, -0.5f), new Vector3(0.3f, 0.3f, 1));
            person2.GetComponent<Patroller>().isPatroller = true;
            Vector3 auxP2 = new Vector3(6.5f, 0f, -0.5f);
            Vector3[] p2Pos = { auxP2 };
            person2.GetComponent<Patroller>().targets = p2Pos;
            person2.GetComponent<Patroller>().speed = 0.9f;
            person2.GetComponent<Patroller>().stopEndPath = true;

            GameManager.instance.rpgTalk.NewTalk("M3MundoInvertido", "M3MundoInvertidoEnd");
        }
        else if (secao == enumMission.CORVO_APARECE)
        {
            GameManager.instance.InvertWorld(true);
            GameManager.instance.invertWorldBlocked = true;

            GameManager.instance.scenerySounds.PlayBat(1);
            GameManager.instance.scenerySounds.PlayDemon(4);
            GameManager.instance.Invoke("InvokeMission", 3f);
        }
        else if (secao == enumMission.CORVO_ATACA)
        {
            GameManager.instance.rpgTalk.NewTalk("M3MundoInvertido2", "M3MundoInvertido2End", GameManager.instance.rpgTalk.txtToParse, false);

            // Porta Sala
            GameObject portaSala = GameObject.Find("DoorToLivingRoom").gameObject;
            portaSala.GetComponent<SceneDoor>().isOpened = true;

            GameObject.Find("BirdEmitterHolder(Corredor)").gameObject.transform.Find("BirdEmitterCollider").gameObject.SetActive(true);
            GameManager.instance.AddObject("Effects/PaperEmitter", "", new Vector3(6.8f, 0.68f, -0.5f), new Vector3(1, 1, 1));
        }
        else if (secao == enumMission.MI_TRAVADO)
        {
            if (GameManager.instance.rpgTalk.isPlaying)
            {
                GameManager.instance.rpgTalk.EndTalk();
            }

            GameManager.instance.InvertWorld(false);
            GameManager.instance.Invoke("InvokeMission", 5f);
        }
        else if (secao == enumMission.MAE_APARECE)
        {
            GameManager.instance.scenerySounds.PlayDemon(6);

            GameManager.instance.rpgTalk.NewTalk("M3VoltaMundoInvertido", "M3VoltaMundoInvertidoEnd");

            GameManager.instance.AddObject("NPCs/mom", "", new Vector3(-3.1f, 1f, -0.5f), new Vector3(0.3f, 0.3f, 1));
            camera.GetComponent<Camera>().orthographicSize = 4;

            GameObject portaG = GameObject.Find("DoorToGarden").gameObject;
            portaG.GetComponent<SceneDoor>().isOpened = true;
        }
        else if (secao == enumMission.PASSAROS_FINAL)
        {
            camera.GetComponent<Camera>().orthographicSize = 4;

            GameManager.instance.invertWorldBlocked = true;
            // Gato Sombra
            catShadow = GameManager.instance.AddObject("NPCs/catShadow", "", new Vector3(-6f, -2.1f, -0.5f), new Vector3(0.15f, 0.15f, 1));
            catShadow.GetComponent<Patroller>().isPatroller = true;
            Vector3 aux = new Vector3(-4.3f, -4.5f, -0.5f);
            Vector3[] catPos = { aux };
            catShadow.GetComponent<Patroller>().targets = catPos;
            catShadow.GetComponent<Patroller>().speed = 0.9f;
            catShadow.GetComponent<Patroller>().stopEndPath = true;

            // Pessoa 1
            person1 = GameManager.instance.AddObject("NPCs/personShadow", "", new Vector3(-5.5f, -2.5f, -0.5f), new Vector3(0.3f, 0.3f, 1));
            person1.GetComponent<Patroller>().isPatroller = true;
            Vector3 auxP1 = new Vector3(-4.8f, -4f, -0.5f);
            Vector3[] p1Pos = { auxP1 };
            person1.GetComponent<Patroller>().targets = p1Pos;
            person1.GetComponent<Patroller>().speed = 0.9f;
            person1.GetComponent<Patroller>().stopEndPath = true;

            GameManager.instance.rpgTalk.NewTalk("M3GardenSceneRepeat", "M3GardenSceneRepeatEnd");
        }
        else if (secao == enumMission.FINAL)
        {
            GameObject porta = GameObject.Find("DoorToLivingRoom").gameObject;
            porta.GetComponent<SceneDoor>().isOpened = true;

            GameObject.Find("BirdEmitterHolder(Jardim)").gameObject.transform.Find("BirdEmitterCollider").gameObject.SetActive(true);
            // Gato Sombra
            catShadow.GetComponent<Patroller>().isPatroller = true;
            Vector3 aux = new Vector3(3f, 2.5f, -0.5f);
            Vector3[] catPos = { aux };
            catShadow.GetComponent<Patroller>().targets = catPos;
            catShadow.GetComponent<Patroller>().speed = 1.2f;
            catShadow.GetComponent<Patroller>().destroyEndPath = true;

            // Pessoa 1
            person1.GetComponent<Patroller>().isPatroller = true;
            Vector3 auxP1 = new Vector3(3f, 2.3f, -0.5f);
            Vector3[] p1Pos = { auxP1 };
            person1.GetComponent<Patroller>().targets = p1Pos;
            person1.GetComponent<Patroller>().speed = 1.4f;
            person1.GetComponent<Patroller>().destroyEndPath = true;
            GameManager.instance.rpgTalk.NewTalk("M3GardenSceneRepeat2", "M3GardenSceneRepeat2End");
        }
    }

    public override void AreaTriggered(string tag)
    {
        if (tipEmitter)
        {
            tipEmitter.gameObject.SetActive(false);
        }
        if (tag.Equals("AreaTrigger(Clone)") && secao == enumMission.GATO_CORREDOR)
        {
            EspecificaEnum((int) enumMission.QUADRO);
        }
        else if (tag.Equals("AreaTriggerCrow") && secao == enumMission.MAE_APARECE && GameManager.instance.invertWorld)
        {
            EspecificaEnum((int)enumMission.PASSAROS_FINAL);
        }
        else if(tag.Equals("SpecialTrigger") && !specialTrigger)
        {
            GameManager.instance.rpgTalk.NewTalk("M3GardenSpecialBasement", "M3GardenSpecialBasementEnd", GameManager.instance.rpgTalk.txtToParse);
            specialTrigger = true;
        }
    }

    public override void InvokeMission()
    {
        if (secao == enumMission.CORVO_APARECE)
        {

            GameManager.instance.scenerySounds.PlayBird(4);
            EspecificaEnum((int)enumMission.CORVO_ATACA);
        }
        else if (secao == enumMission.MI_TRAVADO)
        {
            EspecificaEnum((int)enumMission.MAE_APARECE);
        }
        else if (secao == enumMission.PASSAROS_FINAL)
        {
            EspecificaEnum((int)enumMission.FINAL);
        }
    }

    public override void InvokeMissionChoice(int id)
    {
        
    }

    public override void ForneceDica()
    {
        
        if (GameManager.currentSceneName.Equals("Corredor") && secao == enumMission.GATO_CORREDOR)
        {
            GameManager.instance.timer = 0;
            GameManager.instance.rpgTalk.NewTalk("M3QuadroStart", "M3QuadroEnd", GameManager.instance.rpgTalk.txtToParse);
        }
    }


}