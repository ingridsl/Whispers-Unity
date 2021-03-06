using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using CrowShadowManager;
using CrowShadowNPCs;
using CrowShadowObjects;
using CrowShadowPlayer;
using CrowShadowScenery;

public class Mission5 : Mission
{
    enum enumMission { NIGHT, INICIO, CLOSED, OPENED, INSIDE, FINISHED, ALMOST_ATTACK, END_ATTACK, ATTACK_MOM, ATTACK_MOM2, ATTACK_CAT, ATTACK_CAT2, FINAL };
    enumMission secao;

    private MinionEmmitter minionEmitter = null;
    private GameObject mom = null;
    private SpriteRenderer momRenderer;

    private float timeToTip = 2;
    private int timesInMI = 0, hintNumber = 0;
    bool invertLocal = false, specialTrigger = false;

    public override void InitMission()
    {
        sceneInit = "QuartoKid";
        GameManager.initMission = true;
        GameManager.initX = (float)3.0;
        GameManager.initY = (float)0.2;
        GameManager.initDir = 3;
        GameManager.LoadScene(sceneInit);
        secao = enumMission.NIGHT;
        Book.bookBlocked = false;

        GameManager.instance.InvertWorld(false);
        GameManager.instance.invertWorldBlocked = false;

        SetInitialSettings();
    }

    public override void UpdateMission() //aqui coloca as ações do update específicas da missão
    {
        if ((int)GameManager.instance.timer == tipTimerSmall ||
            (int)GameManager.instance.timer == tipTimerMedium || (int)GameManager.instance.timer == tipTimerLonger)
        {
            ForneceDica();
        }

        if (GameManager.instance.invertWorld && !invertLocal)
        {
            invertLocal = true;
            // LUZ DO AMBIENTE
            mainLight.transform.Rotate(new Vector3(-20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

            if (GameManager.currentSceneName.Equals("Cozinha") && mom != null && momRenderer != null)
            {
                mom.GetComponent<SpriteRenderer>().gameObject.SetActive(false);
            }
            if (Cat.instance)
            {
                Cat.instance.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
        else if (!GameManager.instance.invertWorld && invertLocal)
        {
            invertLocal = false;
            // LUZ DO AMBIENTE
            mainLight.transform.Rotate(new Vector3(20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

            if (GameManager.currentSceneName.Equals("Cozinha") && mom != null && momRenderer != null)
            {
                mom.GetComponent<SpriteRenderer>().gameObject.SetActive(true);
            }
            if (Cat.instance)
            {
                Cat.instance.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }

        if (secao == enumMission.NIGHT)
        {
            if (!GameManager.instance.showMissionStart)
            {
                EspecificaEnum((int)enumMission.INICIO);
            }
        }
        else if (secao == enumMission.INICIO && Book.show)
        {
            EspecificaEnum((int)enumMission.OPENED);
        }
        else if (secao == enumMission.OPENED && (CrossPlatformInputManager.GetButtonDown("keyJournal")))
        {
            EspecificaEnum((int)enumMission.CLOSED);
        }
        else if (secao == enumMission.CLOSED && GameManager.currentSceneName.Equals("SideQuest"))
        {
            EspecificaEnum((int)enumMission.INSIDE);
        }
        else if (secao == enumMission.INSIDE && GameManager.currentSceneName.Equals("SideQuest")
           && (hintNumber == 2 || hintNumber == 3) && Inventory.IsCurrentItemType(Inventory.InventoryItems.FLASHLIGHT, true))
        {
            GameManager.instance.rpgTalk.NewTalk("M5Side4Start", "M5Side4End", false);
            hintNumber = 4;
            GameManager.instance.Invoke("InvokeMission", 8f);
        }
        else if (secao == enumMission.INSIDE && GameManager.instance.sideQuests == 1)
        {
            EspecificaEnum((int)enumMission.FINISHED);
        }
        else if (secao == enumMission.FINISHED && GameManager.currentSceneName.Equals("Jardim")
            && minionEmitter != null && (minionEmitter.numMinions >= 40 || minionEmitter.colliding))
        {
            EspecificaEnum((int)enumMission.END_ATTACK);
        }
        else if (secao == enumMission.END_ATTACK && !GameManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.ALMOST_ATTACK);
        }
        else if (secao == enumMission.ATTACK_MOM && !GameManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.ATTACK_MOM2);
        }
        else if (secao == enumMission.ATTACK_CAT && !GameManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.ATTACK_CAT2);
        }
    }

    public override void SetCorredor()
    {
        mainLight.transform.Rotate(new Vector3(30, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
            if (Cat.instance)
            {
                Cat.instance.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }

        GameManager.instance.scenerySounds.StopSound();

        // Porta Mae
        GameObject portaMae = GameObject.Find("DoorToMomRoom").gameObject;
        float portaMaeDefaultY = portaMae.transform.position.y;
        float posX = portaMae.GetComponent<SpriteRenderer>().bounds.size.x / 5;
        portaMae.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
        portaMae.GetComponent<SceneDoor>().isOpened = false;
        portaMae.transform.position = new Vector3(portaMae.transform.position.x - posX, portaMaeDefaultY, portaMae.transform.position.z);

        GameObject.Find("VasoNaoEmpurravel").gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/vasoPlanta_quebrado");

        //GameManager.instance.rpgTalk.NewTalk ("M5CorridorSceneStart", "M5CorridorSceneEnd");
    }

    public override void SetCozinha()
    {
        mainLight.transform.Rotate(new Vector3(30, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
            if (Cat.instance)
            {
                Cat.instance.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }

        GameManager.instance.scenerySounds.PlayDrop();
        //GameManager.instance.rpgTalk.NewTalk ("M5KitchenSceneStart", "M5KitchenSceneEnd");

        // Panela para caso ainda não tenha
        if (!Inventory.HasItemType(Inventory.InventoryItems.TAMPA))
        {
            GameObject panela = GameObject.Find("Panela").gameObject;
            panela.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/panela_tampa");
            panela.GetComponent<ScenePickUpObject>().enabled = true;
        }

        mom = GameManager.instance.AddObject("NPCs/mom", "", new Vector3(-3.15f, 0.2f, -0.5f), new Vector3(0.3f, 0.3f, 1));
        momRenderer = mom.GetComponent<SpriteRenderer>();
        if (GameManager.instance.invertWorld)
        {
            mom.GetComponent<SpriteRenderer>().gameObject.SetActive(true);
        }

        mom.GetComponent<Patroller>().isPatroller = true;
        mom.GetComponent<Patroller>().speed = 0.8f;
        Vector3 target1 = new Vector3(-2.95f, -1.77f, -0.5f);
        Vector3 target2 = new Vector3(2.95f, -1.80f, -0.5f);
        Vector3 target3 = new Vector3(3.63f, 0.49f, -0.5f);
        Vector3 target4 = new Vector3(1.9f, -0.69f, -0.5f);
        Vector3 target5 = new Vector3(1.8f, 0.37f, -0.5f);
        Vector3 target6 = new Vector3(-0.98f, 0.54f, -0.5f);
        Vector3 target7 = new Vector3(-3.17f, -0.87f, -0.5f);
        Vector3 target8 = new Vector3(-1.5f, -0.75f, -0.5f);
        Vector3[] momTargets = { target1, target2, target3, target4, target5, target6, target7, target8 };
        mom.GetComponent<Patroller>().targets = momTargets;

        mom.GetComponent<Patroller>().hasActionPatroller = true;
        mom.GetComponent<Patroller>().isAreaTrigger = true;
        mom.GetComponent<CircleCollider2D>().radius = 4;
    }

    public override void SetJardim()
    {
        if (Cat.instance == null)
        {
            // Gato
            GameObject cat = GameManager.instance.AddObject(
                "NPCs/catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }

        mainLight.transform.Rotate(new Vector3(30, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
            if (Cat.instance)
            {
                Cat.instance.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }

        //GameObject portaSala = GameObject.Find("DoorToLivingRoom").gameObject;
        //portaSala.GetComponent<SceneDoor>().isOpened = false;

        GameManager.instance.rpgTalk.NewTalk("M5KidRoomSceneStart", "M5KidRoomSceneEnd"); // tirar isso depois
        // Problema de performance com os minions
        GameObject aux = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(6.05f, 3.15f, 0));
        minionEmitter = aux.GetComponent<MinionEmmitter>();
        minionEmitter.numMinions = 20;
        minionEmitter.hydraEffect = true;
        minionEmitter.limitX0 = 0.5f;
        minionEmitter.limitXF = 6.95f;
        minionEmitter.limitY0 = 0f;
        minionEmitter.limitYF = 3f;
    }

    public override void SetQuartoKid()
    {
        if (Cat.instance == null)
        {
            // Gato
            GameObject cat = GameManager.instance.AddObject(
                "NPCs/catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }

        mainLight.transform.Rotate(new Vector3(30, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
            if (Cat.instance)
            {
                Cat.instance.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }

        if (secao <= enumMission.FINISHED && ((GameManager.instance.sideQuest != null) ? !GameManager.instance.sideQuest.success : true))
        {
            // Porta
            GameObject porta = GameObject.Find("DoorToAlley").gameObject;
            float portaDefaultX = porta.transform.position.x;
            float portaDefaultY = porta.transform.position.y;
            float posX = porta.GetComponent<SpriteRenderer>().bounds.size.x / 5;
            porta.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
            porta.GetComponent<SceneDoor>().isOpened = false;
            porta.transform.position = new Vector3(porta.transform.position.x - posX, portaDefaultY, porta.transform.position.z);
        }

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
        mainLight.transform.Rotate(new Vector3(30, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
            if (Cat.instance)
            {
                Cat.instance.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
    }

    public override void SetSala()
    {
        mainLight.transform.Rotate(new Vector3(30, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
            if (Cat.instance)
            {
                Cat.instance.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
    }

    public override void SetBanheiro()
    {
        mainLight.transform.Rotate(new Vector3(30, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
            if (Cat.instance)
            {
                Cat.instance.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
    }

    public override void SetPorao()
    {
        mainLight.transform.Rotate(new Vector3(30, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
            if (Cat.instance)
            {
                Cat.instance.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
    }

    public override void EspecificaEnum(int pos)
    {
        secao = (enumMission)pos;
        GameManager.instance.Print("SECAO: " + secao);

        if (secao == enumMission.INICIO)
        {
            GameManager.instance.rpgTalk.NewTalk("M5KidRoomSceneStart", "M5KidRoomSceneEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.OPENED)
        {
            ExtrasManager.canActivateSide1 = true;
            ExtrasManager.SideQuestsManager();
        }
        else if (secao == enumMission.CLOSED)
        {
            GameManager.instance.rpgTalk.NewTalk("M5KidRoomSceneSideStart", "M5KidRoomSceneSideEnd");
        }
        else if (secao == enumMission.INSIDE)
        {
            GameManager.instance.rpgTalk.NewTalk("M5Side1Start", "M5Side1End", false);
        }
        else if (secao == enumMission.FINISHED)
        {
            GameManager.instance.scenerySounds.PlayBird(1);
            //GameManager.instance.rpgTalk.NewTalk("M5KidRoomSceneRepeat", "M5KidRoomSceneRepeatEnd");
        }
        else if (secao == enumMission.END_ATTACK)
        {
            GameManager.instance.blocked = true;
            minionEmitter.StopAllMinions();
            GameObject.Find("MainCamera").GetComponent<Camera>().orthographicSize = 6;
            GameObject.Find("MainCamera").GetComponent<Camera>().transform.position = new Vector3(0f, 0f, -20f);
            mom = GameManager.instance.AddObject("NPCs/mom", "", new Vector3(2.65f, 2.5f, -0.5f), new Vector3(0.3f, 0.3f, 1));
            GameManager.instance.rpgTalk.NewTalk("M5HelpMomStart", "M5HelpMomEnd");

        }
        else if (secao == enumMission.ALMOST_ATTACK)
        {
            GameManager.instance.rpgTalk.NewTalk("M5HelpMom2Start", "M5HelpMom2End");
        }
        else if (secao == enumMission.ATTACK_MOM)
        {
            //final ruim
            minionEmitter.MoveAllMinionsAround(mom.transform.position);
        }
        else if (secao == enumMission.ATTACK_MOM2)
        {
            Cat.instance.GetComponent<Cat>().Patrol();
            Vector3 aux = mom.transform.position;
            Vector3[] catPos = { aux };
            Cat.instance.targets = catPos;
            Cat.instance.destroyEndPath = false;

            GameManager.instance.Invoke("InvokeMission", 8f);
        }
        else if (secao == enumMission.ATTACK_CAT)
        {
            //final bom
            minionEmitter.MoveAllMinionsAround(player.transform.position);
        }
        else if (secao == enumMission.ATTACK_CAT2)
        {
            GameManager.instance.Invoke("InvokeMission", 2f);
            // tela piscando
            Cat.instance.GetComponent<Cat>().Patrol();
            Vector3 aux = player.transform.position;
            Vector3[] catPos = { aux };
            Cat.instance.targets = catPos;
            Cat.instance.destroyEndPath = false;
        }
        else if (secao == enumMission.FINAL)
        {
            //GameManager.instance.ChangeMission(7);
            GameManager.LoadScene("Credits");
        }
    }

    public override void AreaTriggered(string tag)
    {
        if (tag.Equals("MomPatrollerTrigger") && !specialTrigger && !Inventory.HasItemType(Inventory.InventoryItems.BASTAO))
        {
            GameManager.instance.rpgTalk.NewTalk("M5KitchenSpecialStick", "M5KitchenSpecialStickEnd");
            specialTrigger = true;
        }
        else if (tag.Equals("Trigger1Side") && hintNumber <= 0)
        {
            GameManager.instance.rpgTalk.NewTalk("M5Side2Start", "M5Side2End", false);
            hintNumber = 1;
            GameManager.instance.Invoke("InvokeMission", 10f);
        }
    }

    public override void ForneceDica()
    {

        if (secao == enumMission.INICIO || secao == enumMission.NIGHT)
        {
            GameManager.instance.timer = 0;
            GameManager.instance.rpgTalk.NewTalk("DicaMundoInvertido5Start", "DicaMundoInvertido5End", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.CLOSED)
        {
            GameManager.instance.timer = 0;
            GameManager.instance.rpgTalk.NewTalk("DicaSide5Start", "DicaSide5End", GameManager.instance.rpgTalk.txtToParse);
        }
    }

    public override void InvokeMissionChoice(int id)
    {
        if (secao == enumMission.ALMOST_ATTACK)
        {
            if (id == 0)
            {
                EspecificaEnum((int)enumMission.ATTACK_CAT);
            }
            else
            {
                EspecificaEnum((int)enumMission.ATTACK_MOM);
            }
            GameManager.instance.Invoke("InvokeMission", 8f);
        }
    }

    public override void InvokeMission()
    {
        if (secao == enumMission.ATTACK_CAT2)
        {
            EspecificaEnum((int)enumMission.FINAL);
        }
        else if (secao == enumMission.ATTACK_MOM2)
        {
            EspecificaEnum((int)enumMission.FINAL);
        }
        else if (GameManager.currentSceneName.Equals("SideQuest"))
        {
            if (hintNumber <= 1)
            {
                GameManager.instance.rpgTalk.NewTalk("M5Side3Start", "M5Side3End", false);
                hintNumber = 2;
                GameManager.instance.Invoke("InvokeMission", 15f); //ajustar a questão das duas chamadas, para não adiantar do 4
            }
            else if (hintNumber == 2)
            {
                GameManager.instance.rpgTalk.NewTalk("M5Side3-2Start", "M5Side3-2End", false);
                hintNumber = 3;
            }
            else if (hintNumber == 4)
            {
                GameManager.instance.rpgTalk.NewTalk("M5Side5Start", "M5Side5End", false);
                hintNumber = 5;
            }
        }
    }

}