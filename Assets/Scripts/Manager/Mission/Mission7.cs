using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mission7 : Mission {
    enum enumMission { NIGHT, INICIO, APARECELUZ, LUZCHEIA, LUZANDANDO, LUZSALA, LUZSALAANDANDO, LUZKID,
        LUZKIDANDANDO, FINAL };
    enumMission secao;

    //Luz do quarto da mãe
    GameObject luz1;
    //Luz do quarto 
    GameObject[] luz2;
    GameObject[] luz3;
    private float timeToDeath = 1.5f;

    float portaMaeDefaultY, portaMaeDefaultX, portaKidDefaultY, portaKidDefaultX;

    public override void InitMission()
	{
		sceneInit = "QuartoMae";
		MissionManager.initMission = true;
		MissionManager.initX = (float) 3.5;
		MissionManager.initY = (float) 1.7;
		MissionManager.initDir = 3;
		SceneManager.LoadScene(sceneInit, LoadSceneMode.Single);
        secao = enumMission.NIGHT;
        if (Cat.instance != null) Cat.instance.DestroyCat();
        if (Corvo.instance != null) Corvo.instance.DestroyRaven();
        Book.bookBlocked = false;

        MissionManager.instance.invertWorld = false;
        MissionManager.instance.invertWorldBlocked = false;

        // Adiciona as páginas
        Book.pages[0] = Book.pages[1] = Book.pages[2] = Book.pages[3] = true;
        Book.pages[4] = false;
        Book.pageQuantity = 4;

        if (MissionManager.instance.rpgTalk.isPlaying)
        {
            MissionManager.instance.rpgTalk.EndTalk();
        }
    }

	public override void UpdateMission() //aqui coloca as ações do update específicas da missão
	{
        if (secao == enumMission.NIGHT)
        {
            if (!MissionManager.instance.GetMissionStart())
            {
                EspecificaEnum((int)enumMission.INICIO);
            }
        }

        if(secao == enumMission.INICIO && !MissionManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.APARECELUZ);
        }
        if(secao == enumMission.APARECELUZ)
        {
            if(luz1.GetComponent<Light>().range < 0.8)
            {
                luz1.GetComponent<Light>().range += Time.deltaTime;
            }
            else
            {
                EspecificaEnum((int)enumMission.LUZCHEIA);
            }
        }
        if(secao == enumMission.LUZCHEIA && !MissionManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.LUZANDANDO);
        }

        if (secao == enumMission.LUZANDANDO && luz1 != null)
        {
            if (luz1.GetComponent<HelpingLight>().stoped)
            {
                GameObject portaCorredor = GameObject.Find("DoorToAlley").gameObject;
                portaCorredor.GetComponent<Collider2D>().isTrigger = true;
            }
            if (!luz1.GetComponent<HelpingLight>().PlayerInside)
            {
                KillInDarkness();
            }
            else
            {
                timeToDeath = 1.5f;
            }
        }

        if (secao == enumMission.LUZANDANDO && MissionManager.instance.currentSceneName.Equals("Corredor"))
        {
            EspecificaEnum((int)enumMission.LUZSALA);
        }
        if (secao == enumMission.LUZSALA && !MissionManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.LUZSALAANDANDO);
        }

        if (secao == enumMission.LUZSALAANDANDO && luz2[0] != null && luz2[1] != null)
        {
            if (luz2[0].GetComponent<HelpingLight>().stoped)
            {
                GameObject portaSala = GameObject.Find("DoorToLivingRoom").gameObject;
                portaSala.GetComponent<Collider2D>().isTrigger = true;
            }
            if (luz2[1].GetComponent<HelpingLight>().stoped)
            {
                GameObject portaQuarto = GameObject.Find("DoorToKidRoom").gameObject;
                portaQuarto.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-opened");
                portaQuarto.GetComponent<Collider2D>().isTrigger = true;
                portaQuarto.transform.position = new Vector3(portaKidDefaultX, portaKidDefaultY, portaQuarto.transform.position.z);
            }

            if (!luz2[0].GetComponent<HelpingLight>().PlayerInside &&
                !luz2[0].GetComponent<HelpingLight>().PlayerInside)
            {
                KillInDarkness();
            }
            else
            {
                timeToDeath = 1.5f;
            }
        }
        if (MissionManager.instance.currentSceneName.Equals("Sala"))
        {
            KillInDarkness();
        }

        if (secao == enumMission.LUZSALAANDANDO && MissionManager.instance.currentSceneName.Equals("QuartoKid"))
        {
            EspecificaEnum((int)enumMission.LUZKID);
        }
        if (secao == enumMission.LUZKID && !MissionManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.LUZKIDANDANDO);
        }

        if(secao == enumMission.LUZKIDANDANDO && Book.pageQuantity == 5)
        {
            EspecificaEnum((int)enumMission.FINAL);
        }

        if (secao == enumMission.FINAL && !MissionManager.instance.rpgTalk.isPlaying)
        {
            MissionManager.instance.ChangeMission(8);
        }


    }


    public override void SetCorredor()
	{
        MissionManager.instance.scenerySounds.StopSound();

        if (MissionManager.instance.previousSceneName.Equals("GameOver"))
            EspecificaEnum((int)enumMission.LUZSALA);
        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        GameObject.Find("VasoNaoEmpurravel").gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/vasoPlanta_quebrado");
        GameObject portaCorredor = GameObject.Find("DoorToKitchen").gameObject;
        portaCorredor.GetComponent<Collider2D>().isTrigger = false;
        GameObject portaSala = GameObject.Find("DoorToLivingRoom").gameObject;
        portaSala.GetComponent<Collider2D>().isTrigger = false;

        //tranca porta quarto criança
        GameObject portaQuarto = GameObject.Find("DoorToKidRoom").gameObject;
        portaKidDefaultY = portaQuarto.transform.position.y;
        portaKidDefaultX = portaQuarto.transform.position.x;
        float posKidX = portaQuarto.GetComponent<SpriteRenderer>().bounds.size.x / 5;
        portaQuarto.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
        portaQuarto.GetComponent<Collider2D>().isTrigger = false;
        portaQuarto.transform.position = new Vector3(portaQuarto.transform.position.x + posKidX, portaKidDefaultY, portaQuarto.transform.position.z);

        //tranca porta quarto mãe
        GameObject portaQuartoMae = GameObject.Find("DoorToMomRoom").gameObject;
        portaMaeDefaultY = portaQuartoMae.transform.position.y;
        portaMaeDefaultX = portaQuartoMae.transform.position.x;
        float posMaeX = portaQuartoMae.GetComponent<SpriteRenderer>().bounds.size.x / 5;
        portaQuartoMae.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
        portaQuartoMae.GetComponent<Collider2D>().isTrigger = false;
        portaQuartoMae.transform.position = new Vector3(portaQuartoMae.transform.position.x + posMaeX, portaMaeDefaultY, portaQuartoMae.transform.position.z);

        MissionManager.instance.AddObject("HelpingLight", "", new Vector3(-2.083f, -0.372f, -0.03f), new Vector3(1f, 1f, 1));
        MissionManager.instance.AddObject("HelpingLight", "", new Vector3(-1.14f, -0.372f, -0.03f), new Vector3(1f, 1f, 1));
        luz2 = GameObject.FindGameObjectsWithTag("HelpingLight");

    }

	public override void SetCozinha()
	{
        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        MissionManager.instance.scenerySounds.PlayDrop();
        

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
        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
    }

    public override void SetQuartoKid()
	{
        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (MissionManager.instance.countLivingroomDialog == 0)
        {
            MissionManager.instance.rpgTalk.NewTalk("M7KidRoomSceneStart", "M7KidRoomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountKidRoomDialog");
        }

        if (MissionManager.instance.mission2ContestaMae)
        {
            // Arranhao
            MissionManager.instance.AddObject("Garra", "", new Vector3(-1.48f, 1.81f, 0), new Vector3(0.1f, 0.1f, 1));
        }
        else
        {
            // Vela
            GameObject velaFixa = MissionManager.instance.AddObject("EmptyObject", "", new Vector3(0.125f, -1.1f, 0), new Vector3(2.5f, 2.5f, 1));
            velaFixa.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Inventory/vela_acesa1");
            velaFixa.GetComponent<SpriteRenderer>().sortingOrder = 140;
            GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true);
        }

        MissionManager.instance.AddObject("HelpingLight", "", new Vector3(1f, -0.3f, -0.4f), new Vector3(1f, 1f, 1));
        MissionManager.instance.AddObject("HelpingLight", "", new Vector3(1.73f, -0.87f, -0.2f), new Vector3(1f, 1f, 1));
        MissionManager.instance.AddObject("HelpingLight", "", new Vector3(2.38f, -0.3f, -0.4f), new Vector3(1f, 1f, 1));
        MissionManager.instance.AddObject("HelpingLight", "", new Vector3(1.73f, -1.3f, -0.2f), new Vector3(1f, 1f, 1));
        luz3 = GameObject.FindGameObjectsWithTag("HelpingLight");

        MissionManager.instance.AddObject("Pagina", "", new Vector3(1.709751f, -0.2f, 0), new Vector3(0.535f, 0.483f, 1));    
    }

	public override void SetQuartoMae()
	{
        GameObject portaCorredor = GameObject.Find("DoorToAlley").gameObject;
        portaCorredor.GetComponent<Collider2D>().isTrigger = false;
        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        GameObject Luminaria = GameObject.Find("Luminaria").gameObject;
        Luminaria.GetComponent<Light>().enabled = false;

        GameObject camaMae = GameObject.Find("Cama").gameObject;
        camaMae.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/camaMaeDoente");

        if (MissionManager.instance.previousSceneName.Equals("GameOver"))
        {
            MissionManager.instance.ChangeMission(7);
        }
    }


    public override void SetSala()
	{
        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        //MissionManager.instance.AddObject("MovingObject", new Vector3(0, 0, 0), new Vector3(1, 1, 1));
        //GameObject.Find("PickUpLanterna").gameObject.SetActive(false);	
        //	MissionManager.instance.rpgTalk.NewTalk ("M7LivingRoomSceneStart", "M7LivingroomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountLivingroomDialog");
    }

    public override void EspecificaEnum(int pos)
    {
        secao = (enumMission)pos;
        MissionManager.instance.Print("SECAO: " + secao);

        if (secao == enumMission.INICIO)
        {
            MissionManager.instance.rpgTalk.NewTalk("M7MomRoomSceneStart", "M7MomRoomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountMomRoomDialog");
        }
        if (secao == enumMission.APARECELUZ)
        {
            MissionManager.instance.AddObject("HelpingLight", "", new Vector3(3.5f, 1.7f, -0.03f), new Vector3(1f, 1f, 1));
            luz1 = GameObject.Find("HelpingLight(Clone)").gameObject;
            luz1.GetComponent<Light>().range = 0;
        }
        if (secao == enumMission.LUZCHEIA)
        {
            MissionManager.instance.rpgTalk.NewTalk("M7LightFull", "M7LightFullEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountMomRoomDialog");
        }
        if (secao == enumMission.LUZANDANDO)
        {
            luz1.GetComponent<HelpingLight>().targets = new Vector3[3];
            luz1.GetComponent<HelpingLight>().targets[0] = new Vector3(3.5f, -1, -0.03f);
            luz1.GetComponent<HelpingLight>().targets[1] = new Vector3(-0.9f, -2.07f, -0.03f);
            luz1.GetComponent<HelpingLight>().targets[2] = new Vector3(-3.8f, -0.23f, -0.03f);

            luz1.GetComponent<HelpingLight>().active = true;
            luz1.GetComponent<HelpingLight>().stopEndPath = true;
        }
        if (secao == enumMission.LUZSALA)
        {
            MissionManager.instance.rpgTalk.NewTalk("M7CorridorSceneStart", "M7CorridorSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountMomRoomDialog");
        }
        if (secao == enumMission.LUZSALAANDANDO)
        {
            luz2[0].GetComponent<HelpingLight>().targets = new Vector3[1];
            luz2[0].GetComponent<HelpingLight>().targets[0] = new Vector3(-9.8f, -0.62f, -0.03f);
            luz2[0].GetComponent<HelpingLight>().active = true;
            luz2[0].GetComponent<HelpingLight>().stopEndPath = true;

            luz2[1].GetComponent<HelpingLight>().targets = new Vector3[1];
            luz2[1].GetComponent<HelpingLight>().targets[0] = new Vector3(11.88f, -0.462f, -0.03f);
            luz2[1].GetComponent<HelpingLight>().active = true;
            luz2[1].GetComponent<HelpingLight>().stopEndPath = true;
        }

        if (secao == enumMission.LUZKID)
        {
            MissionManager.instance.rpgTalk.NewTalk("M7KidRoomSceneStart", "M7KidRoomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountMomRoomDialog");
        }

        if (secao == enumMission.LUZKIDANDANDO) {
            Vector3 aux1 = new Vector3(1f, -0.3f, -0.4f);
            Vector3 aux2 = new Vector3(1.73f, -0.87f, -0.2f);
            Vector3 aux3 = new Vector3(2.38f, -0.3f, -0.4f);
            Vector3 aux4 = new Vector3(1.73f, -1.3f, -0.2f);

            luz3[0].GetComponent<HelpingLight>().targets = new Vector3[4];
            luz3[1].GetComponent<HelpingLight>().targets = new Vector3[4];
            luz3[2].GetComponent<HelpingLight>().targets = new Vector3[4];
            luz3[3].GetComponent<HelpingLight>().targets = new Vector3[4];

            luz3[0].GetComponent<HelpingLight>().targets[0] = aux2; luz3[0].GetComponent<HelpingLight>().targets[1] = aux3;
            luz3[0].GetComponent<HelpingLight>().targets[2] = aux4; luz3[0].GetComponent<HelpingLight>().targets[3] = aux1;

            luz3[1].GetComponent<HelpingLight>().targets[0] = aux3; luz3[1].GetComponent<HelpingLight>().targets[1] = aux4;
            luz3[1].GetComponent<HelpingLight>().targets[2] = aux1; luz3[1].GetComponent<HelpingLight>().targets[3] = aux2;

            luz3[2].GetComponent<HelpingLight>().targets[0] = aux4; luz3[2].GetComponent<HelpingLight>().targets[1] = aux1;
            luz3[2].GetComponent<HelpingLight>().targets[2] = aux2; luz3[2].GetComponent<HelpingLight>().targets[3] = aux3;

            luz3[3].GetComponent<HelpingLight>().targets[0] = aux1; luz3[3].GetComponent<HelpingLight>().targets[1] = aux2;
            luz3[3].GetComponent<HelpingLight>().targets[2] = aux3; luz3[3].GetComponent<HelpingLight>().targets[3] = aux4;

            luz3[0].GetComponent<HelpingLight>().active = true;
            luz3[1].GetComponent<HelpingLight>().active = true;
            luz3[2].GetComponent<HelpingLight>().active = true;
            luz3[3].GetComponent<HelpingLight>().active = true;
        }

        if (secao == enumMission.FINAL)
        {
            MissionManager.instance.rpgTalk.NewTalk("M7MomRoomSceneStart", "M7MomRoomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountMomRoomDialog");
        }
    }

    public void AddCountKidRoomDialog(){
		MissionManager.instance.countKidRoomDialog++;
	}
    public void AddCountMomRoomDialog()
    {
        MissionManager.instance.countMomRoomDialog++;
    }
    public void AddCountCorridorDialog()
    {
        MissionManager.instance.countCorridorDialog++;
    }


    private void KillInDarkness()
    {
        timeToDeath -= Time.deltaTime;
        if (timeToDeath <= 0)
        {
            MissionManager.instance.GameOver();
        }
        
    }
}