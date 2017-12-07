using UnityEngine;
using UnityEngine.SceneManagement;

public class Mission3 : Mission {
    enum enumMission { NIGHT, INICIO, GATO_CORREDOR, QUADRO, MI_DESBLOQUEADO, CORVO_ATACA, MI_TRAVADO, MAE_APARECE, FINAL };
    enumMission secao;

    ZoomObject quadro1, quadro2;
    bool quadro1Seen = false, quadro2Seen = false;

    public override void InitMission()
	{
		sceneInit = "QuartoKid";
		MissionManager.initMission = true;
		MissionManager.initX = (float) -2.5;
		MissionManager.initY = (float) -1.6;
		MissionManager.initDir = 3;
		SceneManager.LoadScene(sceneInit, LoadSceneMode.Single);
        secao = enumMission.NIGHT;
        if (Cat.instance != null) Cat.instance.DestroyCat();
    }

    public override void UpdateMission() //aqui coloca as ações do update específicas da missão
    {

        if (secao == enumMission.NIGHT)
        {
            if (!MissionManager.instance.GetMissionStart())
            {
                EspecificaEnum((int)enumMission.INICIO);
                MissionManager.instance.rpgTalk.NewTalk("M3KidRoomSceneStart", "M3KidRoomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountKidRoomDialog");
            }
        }
        else if (secao == enumMission.QUADRO && MissionManager.instance.currentSceneName.Equals("Corredor"))
        {
            if (quadro1Seen && quadro2Seen)
            {
                EspecificaEnum((int)enumMission.MI_DESBLOQUEADO);
            }
            else if (quadro1.ObjectOpened() && !quadro1Seen)
            {
                MissionManager.instance.Print("QUADRO1");
                quadro1Seen = true;
            }
            else if (quadro2.ObjectOpened() && !quadro2Seen)
            {
                MissionManager.instance.Print("QUADRO2");
                quadro2Seen = true;
            }
        }
    }

	public override void SetCorredor()
	{
        //MissionManager.instance.rpgTalk.NewTalk ("M3CorridorSceneStart", "M3CorridorSceneEnd");

        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(-20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z

        // Definir objetos dos quadros
        GameObject quadro1Object = GameObject.Find("Quadro1").gameObject;
        quadro1 = quadro1Object.GetComponent<ZoomObject>();
        GameObject quadro2Object = GameObject.Find("Quadro2").gameObject;
        quadro2 = quadro1Object.GetComponent<ZoomObject>();

        if (secao == enumMission.INICIO)
        {
            GameObject trigger = MissionManager.instance.AddObject("AreaTrigger", "", new Vector3(1.3f, 0.4f, 1), new Vector3(1, 1, 1));
            trigger.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            trigger.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
            EspecificaEnum((int)enumMission.GATO_CORREDOR);
        }
    }

    public override void SetCozinha()
    {
        //MissionManager.instance.rpgTalk.NewTalk ("M3KitchenSceneStart", "M3KitchenSceneEnd");

        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(-20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z

        // Panela para caso ainda não tenha
        if (!Inventory.HasItemType(Inventory.InventoryItems.TAMPA))
        {
            GameObject panela = GameObject.Find("Panela").gameObject;
            panela.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/panela_tampa");
            panela.GetComponent<ScenePickUpObject>().gameObject.SetActive(true);
        }
    }

	public override void SetJardim()
	{
        //MissionManager.instance.rpgTalk.NewTalk ("M3GardenSceneStart", "M3GardenSceneEnd");

        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(-20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z
    }

	public override void SetQuartoKid()
	{
        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(-20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z

        // Condicoes a partir da missao 2
        if (MissionManager.instance.mission2ContestaMae)
        {
            // colocar arranhao
        }
        else
        {
            // colocar luz
        }

        if(secao == enumMission.NIGHT || secao == enumMission.INICIO)
        {
            MissionManager.instance.AddObject("catFollower", "", new Vector3(0f, 0f, -0.5f), new Vector3(0.15f, 0.15f, 1));
        }
	}

	public override void SetQuartoMae()
	{
        //MissionManager.instance.rpgTalk.NewTalk ("M3MomRoomSceneStart", "M3MomRoomSceneEnd");

        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(-20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z
    }


	public override void SetSala()
	{
        //MissionManager.instance.rpgTalk.NewTalk ("M3LivingRoomSceneStart", "M3LivingroomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountLivingroomDialog");

        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(-20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z
    }

    public override void EspecificaEnum(int pos)
    {
        secao = (enumMission)pos;
        MissionManager.instance.Print("SECAO: " + secao);

        if (secao == enumMission.INICIO)
        {
            Cat.instance.Patrol();
            Transform aux = new GameObject().transform;
            aux.position = new Vector3(1.8f, 0.8f, -0.5f);
            Transform[] catPos = { aux };
            Cat.instance.targets = catPos;
            Cat.instance.speed = 1.2f;
            Cat.instance.destroyEndPath = true;
        }
        else if (secao == enumMission.GATO_CORREDOR)
        {
            GameObject cat = MissionManager.instance.AddObject("catFollower", "", new Vector3(8.2f, -0.2f, -0.5f), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().Patrol();
            Transform aux = new GameObject().transform;
            aux.position = new Vector3(1f, -0.1f, -0.5f);
            Transform[] catPos = { aux };
            cat.GetComponent<Cat>().targets = catPos;
            cat.GetComponent<Cat>().speed = 1.6f;
            cat.GetComponent<Cat>().stopEndPath = true;
        }
        else if (secao == enumMission.QUADRO)
        {
            MissionManager.instance.rpgTalk.NewTalk("M3Painting", "M3PaintingEnd");
        }
        else if (secao == enumMission.MI_DESBLOQUEADO)
        {
            MissionManager.instance.rpgTalk.NewTalk("M3MundoInvertido", "M3MundoInvertidoEnd");
        }
        else if (secao == enumMission.MI_TRAVADO)
        {
            MissionManager.instance.rpgTalk.NewTalk("M3VoltaMundoInvertido", "M3VoltaMundoInvertidoEnd");
        }
    }

    public override void AreaTriggered(string tag)
    {
        if (tag.Equals("AreaTrigger(Clone)") && secao == enumMission.GATO_CORREDOR)
        {
            EspecificaEnum((int) enumMission.QUADRO);
        }
    }

    public override void InvokeMission()
    {

    }

    public override void InvokeMissionChoice(int id)
    {
        
    }

    public void AddCountKidRoomDialog()
    {
        MissionManager.instance.countKidRoomDialog++;
    }
}