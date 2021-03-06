using UnityEngine;
using CrowShadowManager;
using CrowShadowNPCs;
using CrowShadowObjects;
using CrowShadowPlayer;
using CrowShadowScenery;

public class Mission7 : Mission
{
    enum enumMission { NIGHT, INICIO, DEFAULT, PISTA_JARDIM, PISTA_BANHEIRO, PISTA_QUARTO_MAE, HOW, PERGUNTA_GATO, ACUSA_GATO, PERGUNTA1, PERGUNTA10, PERGUNTA2, PERGUNTA20, FINE, FINAL };
    enumMission secao;

    bool pistaJardim = false, pistaBanheiro = false, pistaQuartoMae = false;

    GameObject person1, person2, catShadow, tipEmitter;

    float timeToDeath = 0.5f;
    float portaKidDefaultY, portaKidDefaultX, portaAlleyDefaultY, portaAlleyDefaultX;

    public override void InitMission()
    {
        sceneInit = "Jardim";
        GameManager.initMission = true;
        GameManager.initX = (float)3.5;
        GameManager.initY = (float)1.7;
        //GameManager.initDir = 3;
        GameManager.LoadScene(sceneInit);
        secao = enumMission.NIGHT;
        Book.bookBlocked = false;

        GameManager.instance.invertWorld = true;
        GameManager.instance.invertWorldBlocked = false;

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
        if (secao == enumMission.PISTA_JARDIM && !GameManager.instance.rpgTalk.isPlaying)
        {
            person1.SetActive(false);
            person2.SetActive(false);
            EspecificaEnum((int)enumMission.DEFAULT);
        }
        if (secao == enumMission.PISTA_BANHEIRO && !GameManager.instance.rpgTalk.isPlaying)
        {
            person1.SetActive(false);
            catShadow.SetActive(false);
            EspecificaEnum((int)enumMission.DEFAULT);
        }
        if (secao == enumMission.PISTA_QUARTO_MAE && !GameManager.instance.rpgTalk.isPlaying)
        {
            person1.SetActive(false);
            catShadow.SetActive(false);
            EspecificaEnum((int)enumMission.DEFAULT);
        }
        if (pistaBanheiro && pistaJardim && pistaQuartoMae && secao == enumMission.DEFAULT && !GameManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.HOW);

        }
        if(secao == enumMission.FINE && !GameManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.FINAL);
        }

    }

    public override void SetCorredor()
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

        }

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        GameObject.Find("VasoNaoEmpurravel").gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/vasoPlanta_quebrado");

        // porta corredor
        GameObject portaCorredor = GameObject.Find("DoorToKitchen").gameObject;
        portaCorredor.GetComponent<SceneDoor>().isOpened = true;

        // porta sala
        GameObject portaSala = GameObject.Find("DoorToLivingRoom").gameObject;
        portaSala.GetComponent<SceneDoor>().isOpened = true;

        // porta quarto criança
        GameObject portaQuarto = GameObject.Find("DoorToKidRoom").gameObject;
        portaQuarto.GetComponent<SceneDoor>().isOpened = true;

        // porta quarto mãe
        GameObject portaQuartoMae = GameObject.Find("DoorToMomRoom").gameObject;
        portaQuartoMae.GetComponent<SceneDoor>().isOpened = true;

        //Minion Emitter no canto inferior esquerdo
        GameObject aux2 = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(5f, 0f, 0));
        MinionEmmitter minionEmitter2 = aux2.GetComponent<MinionEmmitter>();
        minionEmitter2.numMinions = 6;
        minionEmitter2.hydraEffect = true;
        minionEmitter2.limitX0 = 0.5f;
        minionEmitter2.limitXF = 6.95f;
        minionEmitter2.limitY0 = 0f;
        minionEmitter2.limitYF = 3f;
    }

    public override void SetCozinha()
    {
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        GameManager.instance.scenerySounds.PlayDrop();

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
        minionEmitter2.numMinions = 8;
        minionEmitter2.hydraEffect = true;
        minionEmitter2.limitX0 = 0.5f;
        minionEmitter2.limitXF = 6.95f;
        minionEmitter2.limitY0 = 0f;
        minionEmitter2.limitYF = 3f;
        if (Cat.instance == null)
        {
            // Gato
            GameObject cat = GameManager.instance.AddObject(
                "NPCs/catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }
    }

    public override void SetJardim()
    {
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (!pistaJardim)
        {
            tipEmitter = GameManager.instance.AddObject("Effects/TipEmitter", new Vector3(-6f, -3f, 0));
            GameObject trigger = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(-6f, -3f, 0), new Vector3(1, 1, 1));
            trigger.name = "JardimTrigger";
            trigger.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            trigger.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
        }

        //Minion Emitter ao lado da entrada do porão
        GameObject aux = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(6.05f, 3.15f, 0));
        MinionEmmitter minionEmitter = aux.GetComponent<MinionEmmitter>();
        minionEmitter.numMinions = 20;
        minionEmitter.hydraEffect = true;
        minionEmitter.limitX0 = 0.5f;
        minionEmitter.limitXF = 6.95f;
        minionEmitter.limitY0 = 0f;
        minionEmitter.limitYF = 3f;

        //Minion Emitter
        GameObject aux2 = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(-3f, 3f, 0));
        MinionEmmitter minionEmitter2 = aux2.GetComponent<MinionEmmitter>();
        minionEmitter2.numMinions = 15;
        minionEmitter2.hydraEffect = true;
        minionEmitter2.limitX0 = 0.5f;
        minionEmitter2.limitXF = 6.95f;
        minionEmitter2.limitY0 = 0f;
        minionEmitter2.limitYF = 3f;
        if (Cat.instance == null)
        {
            // Gato
            GameObject cat = GameManager.instance.AddObject(
                "NPCs/catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }
    }

    public override void SetQuartoKid()
    {
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        GameManager.instance.rpgTalk.NewTalk("M7KidRoomSceneStart", "M7KidRoomSceneEnd", GameManager.instance.rpgTalk.txtToParse);

        GameObject porta = GameObject.Find("DoorToAlley").gameObject;
        porta.GetComponent<SceneDoor>().isOpened = true;

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

        if (GameManager.instance.mission2ContestaMae)
        {
            //Minion Emitter no canto inferior esquerdo
            GameObject aux = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(-2f, 0f, 0));
            MinionEmmitter minionEmitter = aux.GetComponent<MinionEmmitter>();
            minionEmitter.numMinions = 4;
            minionEmitter.hydraEffect = true;
            minionEmitter.limitX0 = 0.5f;
            minionEmitter.limitXF = 6.95f;
            minionEmitter.limitY0 = 0f;
            minionEmitter.limitYF = 3f;

            //Minion Emitter no canto inferior esquerdo
            GameObject aux2 = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(2f, -1f, 0));
            MinionEmmitter minionEmitter2 = aux2.GetComponent<MinionEmmitter>();
            minionEmitter2.numMinions = 4;
            minionEmitter2.hydraEffect = true;
            minionEmitter2.limitX0 = 0.5f;
            minionEmitter2.limitXF = 6.95f;
            minionEmitter2.limitY0 = 0f;
            minionEmitter2.limitYF = 3f;
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

        if (GameManager.previousSceneName.Equals("GameOver"))
        {
            GameManager.instance.ChangeMission(7);
        }

        if (!pistaQuartoMae)
        {
            tipEmitter = GameManager.instance.AddObject("Effects/TipEmitter", new Vector3(4f, -2f, 0));
            GameObject trigger = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(4f, -2f, 0), new Vector3(1, 1, 1));
            trigger.name = "QuartoTrigger";
            trigger.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            trigger.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
        }

        if (Cat.instance == null)
        {
            // Gato
            GameObject cat = GameManager.instance.AddObject(
                "NPCs/catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }
    }

    public override void SetSala()
    {

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        GameObject portaCorredor = GameObject.Find("DoorToAlley").gameObject;
        portaCorredor.GetComponent<SceneDoor>().isOpened = true;

        GameObject portaG = GameObject.Find("DoorToGarden").gameObject;
        portaG.GetComponent<SceneDoor>().isOpened = true;

        if (GameManager.previousSceneName.Equals("GameOver"))
        {
            // EspecificaEnum((int)enumMission.LUZSALA);
        }

        //Minion Emitter no meio da sala
        GameObject aux = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(0f, 0f, 0));
        MinionEmmitter minionEmitter = aux.GetComponent<MinionEmmitter>();
        minionEmitter.numMinions = 10;
        minionEmitter.hydraEffect = true;
        minionEmitter.limitX0 = 0.5f;
        minionEmitter.limitXF = 6.95f;
        minionEmitter.limitY0 = 0f;
        minionEmitter.limitYF = 3f;

        //Minion Emitter no canto inferior esquerdo
        GameObject aux2 = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(-7f, -1f, 0));
        MinionEmmitter minionEmitter2 = aux2.GetComponent<MinionEmmitter>();
        minionEmitter2.numMinions = 12;
        minionEmitter2.hydraEffect = true;
        minionEmitter2.limitX0 = 0.5f;
        minionEmitter2.limitXF = 6.95f;
        minionEmitter2.limitY0 = 0f;
        minionEmitter2.limitYF = 3f;

        if (Cat.instance == null)
        {
            // Gato
            GameObject cat = GameManager.instance.AddObject(
                "NPCs/catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }

    }

    public override void SetBanheiro()
    {
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (!pistaBanheiro)
        {
            tipEmitter = GameManager.instance.AddObject("Effects/TipEmitter", new Vector3(0f, -1.5f, 0));
            GameObject trigger = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(0f, -1.5f, 0), new Vector3(1, 1, 1));
            trigger.name = "BanheiroTrigger";
            trigger.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            trigger.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
        }

        if (Cat.instance == null)
        {
            // Gato
            GameObject cat = GameManager.instance.AddObject(
                "NPCs/catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }

        //Minion Emitter no meio do banheiro
        GameObject aux = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(-1f, 0f, 0));
        MinionEmmitter minionEmitter = aux.GetComponent<MinionEmmitter>();
        minionEmitter.numMinions = 6;
        minionEmitter.hydraEffect = true;
        minionEmitter.limitX0 = 0.5f;
        minionEmitter.limitXF = 6.95f;
        minionEmitter.limitY0 = 0f;
        minionEmitter.limitYF = 3f;
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
            GameManager.instance.rpgTalk.NewTalk("M7GardenSceneStart", "M7GardenSceneEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.FINAL)
        {
            GameManager.instance.ChangeMission(8);
        }
        else if (secao == enumMission.PISTA_JARDIM)
        {
            GameManager.instance.InvertWorld(false);
            GameManager.instance.rpgTalk.NewTalk("M7GardenShadowSceneStart", "M7GardenShadowSceneEnd", GameManager.instance.rpgTalk.txtToParse);
            // Pessoa 1
            person1 = GameManager.instance.AddObject("NPCs/personShadow", "", new Vector3(-6f, 4f, -0.5f), new Vector3(0.3f, 0.3f, 1));
            person1.GetComponent<Patroller>().isPatroller = true;
            Vector3 auxP1 = new Vector3(-6f, -2.5f, -0.5f);
            Vector3 auxP2 = new Vector3(-5f, -2.4f, -0.5f);
            Vector3 auxP3 = new Vector3(-7f, -2.6f, -0.5f);
            Vector3 auxP4 = new Vector3(-7f, -2.5f, -0.5f);
            Vector3[] p1Pos = { auxP1, auxP2, auxP3, auxP4 };
            person1.GetComponent<Patroller>().targets = p1Pos;
            person1.GetComponent<Patroller>().speed = 0.9f;
            person1.GetComponent<Patroller>().stopEndPath = true;

            // Pessoa 2
            person2 = GameManager.instance.AddObject("NPCs/personShadow", "", new Vector3(0f, -3.5f, -0.5f), new Vector3(0.3f, 0.3f, 1));
            person2.GetComponent<Patroller>().isPatroller = true;
            Vector3 aux2P1 = new Vector3(-3f, -3f, -0.5f);
            Vector3 aux2P2 = new Vector3(-4f, -3f, -0.5f);
            Vector3 aux2P3 = new Vector3(-5f, -2.5f, -0.5f);
            Vector3 aux2P4 = new Vector3(-5f, -3f, -0.5f);
            Vector3[] p2Pos = { aux2P1, aux2P2, aux2P3, aux2P4 };
            person2.GetComponent<Patroller>().targets = p2Pos;
            person2.GetComponent<Patroller>().speed = 0.6f;
            person2.GetComponent<Patroller>().stopEndPath = true;

        }
        else if (secao == enumMission.PISTA_BANHEIRO)
        {
            GameManager.instance.InvertWorld(false);
            GameManager.instance.rpgTalk.NewTalk("M7BathroomShadowSceneStart", "M7BathroomShadowSceneEnd", GameManager.instance.rpgTalk.txtToParse);
            // Pessoa 1
            person1 = GameManager.instance.AddObject("NPCs/personShadow", "", new Vector3(2f, 0.5f, -0.5f), new Vector3(0.3f, 0.3f, 1));
            person1.GetComponent<Patroller>().isPatroller = true;
            Vector3 auxP1 = new Vector3(2f, 0f, -0.5f);
            Vector3 auxP2 = new Vector3(-2f, 0f, -0.5f);
            Vector3 auxP3 = new Vector3(2f, 0f, -0.5f);
            Vector3 auxP4 = new Vector3(-2f, 0f, -0.5f);
            Vector3[] p1Pos = { auxP1, auxP2, auxP3, auxP4 };
            person1.GetComponent<Patroller>().targets = p1Pos;
            person1.GetComponent<Patroller>().speed = 0.9f;
            person1.GetComponent<Patroller>().stopEndPath = true;

            // Gato Sombra
            catShadow = GameManager.instance.AddObject("NPCs/catShadow", "", new Vector3(2f, 0.5f, -0.5f), new Vector3(0.15f, 0.15f, 1));
            catShadow.GetComponent<Patroller>().isPatroller = true;
            Vector3 aux = new Vector3(2f, -1f, -0.5f);
            Vector3 aux1 = new Vector3(1f, -1f, -0.5f);
            Vector3[] catPos = { aux, aux1 };
            catShadow.GetComponent<Patroller>().targets = catPos;
            catShadow.GetComponent<Patroller>().speed = 0.3f;
            catShadow.GetComponent<Patroller>().stopEndPath = true;
        }
        else if (secao == enumMission.PISTA_QUARTO_MAE)
        {
            GameManager.instance.InvertWorld(false);
            GameManager.instance.rpgTalk.NewTalk("M7MomRoomShadowSceneStart", "M7MomRoomShadowSceneEnd", GameManager.instance.rpgTalk.txtToParse);
            // Pessoa 1
            person1 = GameManager.instance.AddObject("NPCs/personShadow", "", new Vector3(2f, 0.5f, -0.5f), new Vector3(0.3f, 0.3f, 1));
            person1.GetComponent<Patroller>().isPatroller = true;
            Vector3 auxP1 = new Vector3(2f, 1.5f, -0.5f);
            Vector3 auxP2 = new Vector3(2f, -0.5f, -0.5f);
            Vector3 auxP3 = new Vector3(4f, -0.5f, -0.5f);
            Vector3 auxP4 = new Vector3(2f, -0.5f, -0.5f);
            Vector3[] p1Pos = { auxP1, auxP2, auxP3, auxP4 };
            person1.GetComponent<Patroller>().targets = p1Pos;
            person1.GetComponent<Patroller>().speed = 0.9f;
            person1.GetComponent<Patroller>().stopEndPath = true;

            // Gato Sombra
            catShadow = GameManager.instance.AddObject("NPCs/catShadow", "", new Vector3(-4f, 0f, -0.5f), new Vector3(0.15f, 0.15f, 1));
            catShadow.GetComponent<Patroller>().isPatroller = true;
            Vector3 aux = new Vector3(5f, -1f, -0.5f);
            Vector3[] catPos = { aux };
            catShadow.GetComponent<Patroller>().targets = catPos;
            catShadow.GetComponent<Patroller>().speed = 0.6f;
            catShadow.GetComponent<Patroller>().stopEndPath = true;
        }
        else if (secao == enumMission.HOW)
        {
            GameManager.instance.rpgTalk.NewTalk("M7HowStart", "M7HowEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.DEFAULT) {
            GameManager.instance.InvertWorld(true);
        }
        else if (secao == enumMission.ACUSA_GATO)
        {
            //GameManager.instance.rpgTalk.NewTalk("M7AcusaStart", "M7AcusaEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.PERGUNTA_GATO)
        {
           // GameManager.instance.rpgTalk.NewTalk("M7PerguntaStart", "M7PerguntaEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.PERGUNTA1)
        {
            //GameManager.instance.rpgTalk.NewTalk("M7Pergunta1Start", "M7Pergunta1End", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.PERGUNTA10)
        {
           // GameManager.instance.rpgTalk.NewTalk("M7Pergunta10Start", "M7Pergunta10End", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.PERGUNTA2)
        {
           // GameManager.instance.rpgTalk.NewTalk("M7Pergunta2Start", "M7Pergunta2End", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.PERGUNTA20)
        {
           // GameManager.instance.rpgTalk.NewTalk("M7Pergunta20Start", "M7Pergunta20End", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.FINE)
        {
           // GameManager.instance.rpgTalk.NewTalk("M7FineStart", "M7FineEnd", GameManager.instance.rpgTalk.txtToParse);

        }
    }

    public override void ForneceDica()
    {

    }
    public override void InvokeMissionChoice(int id)
    {
        if (secao == enumMission.HOW)
        {
            if (id == 0)
            {
                GameManager.instance.scenerySounds.PlayCat(2);
                EspecificaEnum((int)enumMission.ACUSA_GATO);
            }
            else
            {
                GameManager.instance.scenerySounds.PlayCat(1);
                EspecificaEnum((int)enumMission.PERGUNTA_GATO);
            }
        }
        else if (secao == enumMission.PERGUNTA_GATO)
        {
            switch (id)
            {
                case 0:
                    EspecificaEnum((int)enumMission.PERGUNTA1);
                    GameManager.instance.scenerySounds.PlayCat(2);
                    break;
                case 1:
                    GameManager.instance.scenerySounds.PlayCat(1);
                    EspecificaEnum((int)enumMission.PERGUNTA2);
                    break;
                default:
                    GameManager.instance.scenerySounds.PlayCat(1);
                    EspecificaEnum((int)enumMission.FINE);
                    break;

            }
        }
        else if (secao == enumMission.PERGUNTA1)
        {
            switch (id)
            {
                case 0:
                    EspecificaEnum((int)enumMission.PERGUNTA20);
                    break;
                default:
                    EspecificaEnum((int)enumMission.FINE);
                    break;

            }
        }
        else if (secao == enumMission.PERGUNTA2)
        {
            switch (id)
            {
                case 0:
                    EspecificaEnum((int)enumMission.PERGUNTA10);
                    break;
                default:
                    EspecificaEnum((int)enumMission.FINE);
                    break;

            }
        }
    }

    public override void AreaTriggered(string tag)
    {
        tipEmitter.gameObject.SetActive(false);
        if (tag.Equals("JardimTrigger") && !pistaJardim)
        {
            pistaJardim = true;
            EspecificaEnum((int)enumMission.PISTA_JARDIM);
        } else if (tag.Equals("BanheiroTrigger") && !pistaBanheiro)
        {
            pistaBanheiro = true;
            EspecificaEnum((int)enumMission.PISTA_BANHEIRO);
        }
        else if (tag.Equals("QuartoTrigger") && !pistaQuartoMae)
        {
            pistaQuartoMae = true;
            EspecificaEnum((int)enumMission.PISTA_QUARTO_MAE);
        }
    }

}