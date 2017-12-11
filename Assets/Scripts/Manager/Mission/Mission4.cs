using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mission4 : Mission
{
    enum enumMission { NIGHT, INICIO, GATO_ACELERA, GATO_CORREDOR, FRENTE_CRIADO,
        MI_DESATIVADO, MI_ATIVADO, ARMARIO, GRANDE_BARULHO, VASO_GATO, VASO_SOZINHO, QUEBRADO,
        POP_UP, MAE_CHEGA_CORREDOR, MAE_CHEGA_QUARTO, VERDADE_MENTIRA, FINAL };
    enumMission secao;

    float portaMaeDefaultY, portaMaeDefaultX;
    float poteDefaultY, poteDefaultX;
    float vasoDefaultY, vasoDefaultX;

    GameObject pote, vaso, mom, bird;
    PlaceObject racao;
    MiniGameObject pedra;
    bool invertLocal = false, hasRacao = false;

    public override void InitMission()
    {
        sceneInit = "QuartoKid";
        MissionManager.initMission = true;
        MissionManager.initX = (float)-2.5;
        MissionManager.initY = (float)0.7;
        MissionManager.initDir = 1;
        SceneManager.LoadScene(sceneInit, LoadSceneMode.Single);
        secao = enumMission.NIGHT;
        if (Cat.instance != null) Cat.instance.DestroyCat();
        if (Corvo.instance != null) Corvo.instance.DestroyRaven();
        MissionManager.instance.invertWorldBlocked = false;
        if (MissionManager.instance.rpgTalk.isPlaying)
        {
            MissionManager.instance.rpgTalk.EndTalk();
        }

        racao = GameObject.Find("Player").transform.Find("Racao").GetComponent<PlaceObject>();
        pedra = GameObject.Find("Player").transform.Find("Pedra").GetComponent<MiniGameObject>();
    }

    public override void UpdateMission() //aqui coloca as ações do update específicas da missão
    {
        if (secao == enumMission.NIGHT)
        {
            if (!MissionManager.instance.GetMissionStart())
            {
                EspecificaEnum((int)enumMission.INICIO);
                MissionManager.instance.rpgTalk.NewTalk("M4KidRoomSceneStart", "M4KidRoomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountKidRoomDialog");
            }
        }
        else if (secao == enumMission.INICIO && !MissionManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.GATO_ACELERA);
        }

        if (MissionManager.instance.invertWorld && !invertLocal)
        {
            invertLocal = true;
            GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

            if (MissionManager.instance.currentSceneName.Equals("Corredor") && secao < enumMission.VASO_GATO)
            {
                AbrirPorta();
            }

            if (secao == enumMission.FRENTE_CRIADO)
            {
                EspecificaEnum((int)enumMission.MI_ATIVADO);
            }
            else if (secao <= enumMission.GRANDE_BARULHO)
            {
                //troca de mãe e corvo no quarto
                if (mom != null && bird != null)
                {
                    mom.SetActive(false);
                    bird.SetActive(true);
                }
            }
        }
        else if (!MissionManager.instance.invertWorld && invertLocal)
        {
            invertLocal = false;
            GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
            mainLight.transform.Rotate(new Vector3(40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

            if (MissionManager.instance.currentSceneName.Equals("Corredor") && secao < enumMission.VASO_GATO)
            {
                FecharPorta();
            }

            if (secao == enumMission.MI_ATIVADO)
            {
                EspecificaEnum((int)enumMission.MI_DESATIVADO);
            }
            else if (secao <= enumMission.GRANDE_BARULHO)
            {
                //troca de mãe e corvo no quarto
                if (mom != null && bird != null)
                {
                    mom.SetActive(true);
                    bird.SetActive(false);
                }
            }
        }

        if (MissionManager.instance.currentSceneName.Equals("Corredor") && 
            (secao == enumMission.VASO_GATO || secao == enumMission.VASO_SOZINHO) && !MissionManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.QUEBRADO);
        }

        if (secao == enumMission.GRANDE_BARULHO && !MissionManager.instance.mission4QuebraSozinho && 
            hasRacao && !Inventory.HasItemType(Inventory.InventoryItems.RACAO))
        {
            EspecificaEnum((int)enumMission.VASO_GATO);
        }


        if (secao == enumMission.GRANDE_BARULHO && MissionManager.instance.mission4QuebraSozinho && pedra.achievedGoal)
        {
            EspecificaEnum((int)enumMission.VASO_SOZINHO);
        }

        //livro encontrado
        if (Book.bookBlocked && Inventory.HasItemType(Inventory.InventoryItems.LIVRO))
        {
            Book.bookBlocked = false;
            EspecificaEnum((int)enumMission.POP_UP);
        }

        // só chamar a mãe depois que conseguiu ler o livro
        if (secao == enumMission.POP_UP && MissionManager.instance.invertWorld && Book.show)
        {
            InvokeChange();
        }

        if (secao == enumMission.VERDADE_MENTIRA && !MissionManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.FINAL);
        }

    }

    public override void SetCorredor()
    {
        if (MissionManager.instance.rpgTalk.isPlaying)
        {
            MissionManager.instance.rpgTalk.EndTalk();
        }

        if (MissionManager.instance.previousSceneName.Equals("GameOver") && Cat.instance == null)
        {
            // Gato
            GameObject player = GameObject.Find("Player").gameObject;
            GameObject cat = MissionManager.instance.AddObject(
                "catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }

        if (Corvo.instance != null)
        {
            Corvo.instance.DestroyRaven();
        }
        MissionManager.instance.scenerySounds.StopSound();

        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (MissionManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        }

        if (secao == enumMission.GATO_ACELERA || secao == enumMission.GATO_CORREDOR)
        {
            EspecificaEnum((int)enumMission.GATO_CORREDOR);
        }
        else if ((secao == enumMission.ARMARIO))
        {
            EspecificaEnum((int)enumMission.GRANDE_BARULHO);
        }

        vaso = GameObject.Find("VasoNaoEmpurravel").gameObject;
        vasoDefaultY = vaso.transform.position.y;
        vasoDefaultX = vaso.transform.position.x;

        if (secao >= enumMission.VASO_GATO)
        {
            //!!!!! OUTRAS MISSOES
            vaso.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/vasoPlanta_quebrado");
        }
        else if (secao == enumMission.GRANDE_BARULHO)
        {
            GameObject trigger2 = MissionManager.instance.AddObject("AreaTrigger", "", new Vector3(vasoDefaultX, vasoDefaultY, 0), new Vector3(1, 1, 1));
            trigger2.name = "VasoTrigger";
            trigger2.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            trigger2.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
        }

        if (secao == enumMission.VASO_GATO)
        {
            GameObject cat = MissionManager.instance.AddObject(
                "catFollower", "", new Vector3(-7f, -0.6f, 0), new Vector3(0.15f, 0.15f, 1));
        }

        if((secao == enumMission.FRENTE_CRIADO || secao == enumMission.MI_ATIVADO || secao == enumMission.MI_DESATIVADO)
            && MissionManager.instance.previousSceneName.Equals("Sala"))
        {
            MissionManager.instance.rpgTalk.NewTalk("M4DicaCorredor", "M4DicaCorredorEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountKidRoomDialog");
        }

        if (secao >= enumMission.QUEBRADO && secao < enumMission.MAE_CHEGA_CORREDOR)
        {
            SetMae();
        }
        else if (!MissionManager.instance.invertWorld)
        {
            // PortaMãe
            GameObject portaMae = GameObject.Find("DoorToMomRoom").gameObject;
            portaMaeDefaultY = portaMae.transform.position.y;
            portaMaeDefaultX = portaMae.transform.position.x;
            float posX = portaMae.GetComponent<SpriteRenderer>().bounds.size.x / 5;
            portaMae.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
            portaMae.GetComponent<Collider2D>().isTrigger = false;
            portaMae.transform.position = new Vector3(portaMae.transform.position.x + posX, portaMaeDefaultY, portaMae.transform.position.z);
        }

        if (secao == enumMission.MAE_CHEGA_CORREDOR)
        {
            MissionManager.instance.AddObject("mom", "", new Vector3(-3f, -0.5f, -0.5f), new Vector3(0.3f, 0.3f, 1));
            GameObject trigger3 = MissionManager.instance.AddObject("AreaTrigger", "", new Vector3(-2f, 0f, 1), new Vector3(1, 1, 1));
            trigger3.name = "MaeTrigger2";
            trigger3.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            trigger3.GetComponent<BoxCollider2D>().size = new Vector2(2f, 2f);
        }
    }

    public override void SetCozinha()
    {
        if (MissionManager.instance.rpgTalk.isPlaying)
        {
            MissionManager.instance.rpgTalk.EndTalk();
        }

        MissionManager.instance.scenerySounds.StopSound();

        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (MissionManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        }

        // Panela para caso ainda não tenha
        if (!Inventory.HasItemType(Inventory.InventoryItems.TAMPA))
        {
            GameObject panela = GameObject.Find("Panela").gameObject;
            panela.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/panela_tampa");
            panela.GetComponent<ScenePickUpObject>().enabled = true;
        }

        MissionManager.instance.scenerySounds.PlayDrop();

        // Ração no armário alto
        GameObject armario = GameObject.Find("Armario2").gameObject;
        armario.tag = "ScenePickUpObject";
        SceneObject sceneObject = armario.GetComponent<SceneObject>();
        sceneObject.enabled = false;
        ScenePickUpObject scenePickUpObject = armario.AddComponent<ScenePickUpObject>();
        scenePickUpObject.sprite1 = sceneObject.sprite1;
        scenePickUpObject.sprite2 = sceneObject.sprite2;
        scenePickUpObject.positionSprite = sceneObject.positionSprite;
        scenePickUpObject.scale = sceneObject.scale;
        scenePickUpObject.isUp = sceneObject.isUp;
        scenePickUpObject.item = Inventory.InventoryItems.RACAO;
    }

    public override void SetJardim()
    {
        if (MissionManager.instance.rpgTalk.isPlaying)
        {
            MissionManager.instance.rpgTalk.EndTalk();
        }

        MissionManager.instance.scenerySounds.StopSound();
        MissionManager.instance.scenerySounds.PlayWolf(1);

        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (MissionManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        }

        if (!Inventory.HasItemType(Inventory.InventoryItems.PEDRA))
        {
            GameObject pedra1 = GameObject.Find("monte_pedra").gameObject;
            pedra1.tag = "ScenePickUpObject";
            ScenePickUpObject scenePickUpObject = pedra1.AddComponent<ScenePickUpObject>();
            scenePickUpObject.sprite1 = pedra1.GetComponent<SpriteRenderer>().sprite;
            scenePickUpObject.sprite2 = pedra1.GetComponent<SpriteRenderer>().sprite;
            scenePickUpObject.blockAfterPick = true;
            scenePickUpObject.item = Inventory.InventoryItems.PEDRA;

            GameObject pedra2 = GameObject.Find("monte_pedra (1)").gameObject;
            pedra2.tag = "ScenePickUpObject";
            ScenePickUpObject scenePickUpObject2 = pedra2.AddComponent<ScenePickUpObject>();
            scenePickUpObject2.sprite1 = pedra2.GetComponent<SpriteRenderer>().sprite;
            scenePickUpObject2.sprite2 = pedra2.GetComponent<SpriteRenderer>().sprite;
            scenePickUpObject2.blockAfterPick = true;
            scenePickUpObject2.item = Inventory.InventoryItems.PEDRA;

            GameObject pedra = MissionManager.instance.AddObject("PickUp", "Sprites/Objects/Inventory/pedra", new Vector3((float)-3.59, (float)-0.45, 0), new Vector3((float)1.2, (float)1.2, 1.3f));
            pedra.GetComponent<PickUpObject>().item = Inventory.InventoryItems.PEDRA;
        }
    }

    public override void SetQuartoKid()
    {
        if (MissionManager.instance.rpgTalk.isPlaying)
        {
            MissionManager.instance.rpgTalk.EndTalk();
        }

        MissionManager.instance.scenerySounds.StopSound();
        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (MissionManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
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
    }

    public override void SetQuartoMae()
    {
        if (MissionManager.instance.rpgTalk.isPlaying)
        {
            MissionManager.instance.rpgTalk.EndTalk();
        }

        if (MissionManager.instance.previousSceneName.Equals("GameOver"))
        {
            MissionManager.instance.rpgTalk.NewTalk("M4DicaQuartoMae", "M4DicaQuartoMaeEnd");

            if (Cat.instance == null)
            {
                // Gato
                GameObject player = GameObject.Find("Player").gameObject;
                GameObject cat = MissionManager.instance.AddObject(
                    "catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
                cat.GetComponent<Cat>().FollowPlayer();
            }
        }

        MissionManager.instance.scenerySounds.StopSound();

        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (MissionManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        }

        //livro no inventário
        GameObject armario = GameObject.Find("Armario").gameObject;
        armario.tag = "ScenePickUpObject";
        SceneObject sceneObject = armario.GetComponent<SceneObject>();
        sceneObject.enabled = false;
        ScenePickUpObject scenePickUpObject = armario.AddComponent<ScenePickUpObject>();
        scenePickUpObject.sprite1 = sceneObject.sprite1;
        scenePickUpObject.sprite2 = sceneObject.sprite2;
        scenePickUpObject.positionSprite = sceneObject.positionSprite;
        scenePickUpObject.scale = sceneObject.scale;
        scenePickUpObject.isUp = sceneObject.isUp;
        scenePickUpObject.item = Inventory.InventoryItems.LIVRO;

        if (secao < enumMission.QUEBRADO)
        {
            mom = MissionManager.instance.AddObject("mom", "", new Vector3(2f, 1.5f, -0.5f), new Vector3(0.3f, 0.3f, 1));
            bird = MissionManager.instance.AddObject("Corvo", "", new Vector3(2f, 1.5f, -0.5f), new Vector3(3f, 3f, 1));
            mom.GetComponent<Patroller>().isPatroller = true;
            bird.GetComponent<SpriteRenderer>().color = Color.black;
            MissionManager.instance.AddObject("ActionPatroller", "", new Vector3(2f, 1.5f, 0), new Vector3(1, 1, 1));
            if (!invertLocal) bird.SetActive(false);
            else mom.SetActive(false);
        }

        if ((secao == enumMission.MI_ATIVADO || secao == enumMission.MI_DESATIVADO))
        {
            EspecificaEnum((int)enumMission.ARMARIO);
        }

    }

    public override void SetSala()
    {
        if (MissionManager.instance.rpgTalk.isPlaying)
        {
            MissionManager.instance.rpgTalk.EndTalk();
        }

        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (MissionManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        }

        //pote de ração vazio
        pote = GameObject.Find("PoteRação").gameObject;
        pote.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/pote-vazio");
        poteDefaultY = pote.transform.position.y;
        poteDefaultX = pote.transform.position.x;


        GameObject trigger = MissionManager.instance.AddObject("AreaTrigger", "", new Vector3(poteDefaultX, poteDefaultY, 0), new Vector3(1, 1, 1));
        trigger.name = "PoteTrigger";
        trigger.GetComponent<Collider2D>().offset = new Vector2(0, 0);
        trigger.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);

        //caso não tenha lanterna
        if (!Inventory.HasItemType(Inventory.InventoryItems.FLASHLIGHT))
        {
            GameObject criadoMudo = GameObject.Find("CriadoMudoSala").gameObject;
            criadoMudo.tag = "ScenePickUpObject";
            SceneObject sceneObject = criadoMudo.GetComponent<SceneObject>();
            sceneObject.enabled = false;
            ScenePickUpObject scenePickUpObject = criadoMudo.AddComponent<ScenePickUpObject>();
            scenePickUpObject.sprite1 = sceneObject.sprite1;
            scenePickUpObject.sprite2 = sceneObject.sprite2;
            scenePickUpObject.positionSprite = sceneObject.positionSprite;
            scenePickUpObject.scale = sceneObject.scale;
            scenePickUpObject.isUp = sceneObject.isUp;
            scenePickUpObject.item = Inventory.InventoryItems.FLASHLIGHT;
        }

        if (secao < enumMission.ARMARIO)
        {
            // Estante
            GameObject triggerE = MissionManager.instance.AddObject("AreaTrigger", "", new Vector3(-5.71f, 1.64f, 0), new Vector3(1, 1, 1));
            triggerE.name = "EstanteTrigger";
            triggerE.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            triggerE.GetComponent<BoxCollider2D>().size = new Vector2(1.8f, 1.6f);
        }
    }

    public override void EspecificaEnum(int pos)
    {
        secao = (enumMission)pos;
        MissionManager.instance.Print("SECAO: " + secao);
        if (secao == enumMission.NIGHT || secao == enumMission.INICIO)
        {
            
            MissionManager.instance.scenerySounds.PlayCat(3);
            GameObject cat = MissionManager.instance.AddObject("catFollower", "", new Vector3(-1f, 0.5f, -0.5f), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().followWhenClose = false;
            cat.GetComponent<Cat>().Patrol();
            Transform aux = new GameObject().transform;
            aux.position = new Vector3(1.8f, 0.8f, -0.5f);
            Transform[] catPos = { aux };
            cat.GetComponent<Cat>().targets = catPos;
            cat.GetComponent<Cat>().speed = 0.3f;
            cat.GetComponent<Cat>().destroyEndPath = true;

        }
        else if (secao == enumMission.GATO_ACELERA)
        {
            Cat.instance.speed = 1.2f;
        }
        else if (secao == enumMission.GATO_CORREDOR)
        {
            MissionManager.instance.scenerySounds.PlayCat(2);

            GameObject trigger = MissionManager.instance.AddObject("AreaTrigger", "", new Vector3(6.5f, -0.1f, 1), new Vector3(1, 2, 1));
            trigger.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            trigger.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);

            GameObject cat = MissionManager.instance.AddObject("catFollower", "", new Vector3(10f, -0.2f, -0.5f), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().Patrol();
            Transform aux = new GameObject().transform;
            aux.position = new Vector3(7f, -0.2f, -0.5f);
            Transform[] catPos = { aux };
            cat.GetComponent<Cat>().targets = catPos;
            cat.GetComponent<Cat>().speed = 1.6f;
            cat.GetComponent<Cat>().stopEndPath = true;
        }
        else if (secao == enumMission.FRENTE_CRIADO)
        {
            MissionManager.instance.rpgTalk.NewTalk("frenteCriadoStart", "frenteCriadoEnd");
        }
        else if (secao == enumMission.ARMARIO)
        {
            MissionManager.instance.rpgTalk.NewTalk("M4MomRoomSceneStart", "M4MomRoomSceneEnd");
        }
        else if (secao == enumMission.GRANDE_BARULHO)
        {
            MissionManager.instance.rpgTalk.NewTalk("GrandeBarulhoStart", "GrandeBarulhoEnd");
        }
        else if (secao == enumMission.VASO_GATO)
        {
            pote.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/pote-racao");

            MissionManager.instance.scenerySounds.PlayCat(4);
            Transform aux = new GameObject().transform;
            Cat.instance.followWhenClose = false;
            Cat.instance.Patrol();
            aux.position = new Vector3(-3f, 1.5f, -0.5f);
            Transform[] catPos = { aux };
            Cat.instance.targets = catPos;
            Cat.instance.speed = 1.6f;
            Cat.instance.destroyEndPath = true;

            MissionManager.instance.rpgTalk.NewTalk("OQ", "OQEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountLivingroomDialog", false);
        }
        else if (secao == enumMission.VASO_SOZINHO)
        {
            vaso.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/vasoPlanta_quebrado");

            MissionManager.instance.rpgTalk.NewTalk("OQ", "OQEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountLivingroomDialog", false);
        }
        else if (secao == enumMission.QUEBRADO)
        {
            AbrirPorta();
            SetMae();
        }
        else if (secao == enumMission.POP_UP)
        {
            Book.AddPage(); // livro encontrado
        }
        else if (secao == enumMission.MAE_CHEGA_QUARTO)
        {
            MissionManager.instance.AddObject("mom", "", new Vector3(-4f, 0f, -0.5f), new Vector3(0.3f, 0.3f, 1));
            GameObject trigger = MissionManager.instance.AddObject("AreaTrigger", "", new Vector3(-1f, 0f, 1), new Vector3(2, 4, 1));
            trigger.name = "MaeTrigger";
            trigger.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            trigger.GetComponent<BoxCollider2D>().size = new Vector2(2f, 2f);
        }
        else if (secao == enumMission.FINAL)
        {
            MissionManager.instance.InvertWorld(false);
            MissionManager.instance.ChangeMission(5);
        }
    }

    public override void AreaTriggered(string tag)
    {
        // frente do criado no corredor
        if (tag.Equals("EnterAreaTrigger(Clone)") && secao == enumMission.GATO_CORREDOR)
        {
            EspecificaEnum((int)enumMission.FRENTE_CRIADO);
        }
        if (tag.Equals("EnterEstanteTrigger"))
        {
            MissionManager.instance.rpgTalk.NewTalk("NadaAqui", "NadaAquiEnd");
        }
        // pote de ração vazio
        if (tag.Equals("EnterPoteTrigger") && secao == enumMission.GRANDE_BARULHO && !MissionManager.instance.mission4QuebraSozinho)
        {
            if (Inventory.HasItemType(Inventory.InventoryItems.RACAO))
            {
                hasRacao = true;
                racao.inArea = true;
            }
        }
        else if (tag.Equals("ExitPoteTrigger") && secao == enumMission.GRANDE_BARULHO && !MissionManager.instance.mission4QuebraSozinho)
        {
            if (Inventory.HasItemType(Inventory.InventoryItems.RACAO))
            {
                racao.inArea = false;
            }
        }
        //vaso antes de quebrar
        if (tag.Equals("EnterVasoTrigger") && secao == enumMission.GRANDE_BARULHO && MissionManager.instance.mission4QuebraSozinho)
        {
            if (Inventory.HasItemType(Inventory.InventoryItems.PEDRA))
            {
                pedra.activated = true;
            }
        }
        else if (tag.Equals("ExitVasoTrigger") && secao == enumMission.GRANDE_BARULHO && MissionManager.instance.mission4QuebraSozinho)
        {
            if (Inventory.HasItemType(Inventory.InventoryItems.PEDRA))
            {
                pedra.activated = false;
            }
        }
        // mãe mandar para o quarto
        if ((tag.Equals("MaeTrigger") && secao == enumMission.MAE_CHEGA_QUARTO) || (tag.Equals("MaeTrigger2") && secao == enumMission.MAE_CHEGA_CORREDOR))
        {
            MissionManager.instance.rpgTalk.NewTalk("VerdadeOuMentira", "VerdadeOuMentiraEnd");
            EspecificaEnum((int)enumMission.VERDADE_MENTIRA);
        }
    }

    void InvokeChange()
    {
        if (MissionManager.instance.pathCat >= MissionManager.instance.pathBird)
        {
            MissionManager.instance.scenerySounds.PlayCat(1);

            EspecificaEnum((int)enumMission.MAE_CHEGA_CORREDOR);
        }
        else
        {
            EspecificaEnum((int)enumMission.MAE_CHEGA_QUARTO);
        }
    }

    void AbrirPorta()
    {
        MissionManager.instance.scenerySounds2.PlayDoorOpen(1);
        GameObject porta = GameObject.Find("DoorToMomRoom").gameObject;
        porta.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-opened");
        porta.GetComponent<Collider2D>().isTrigger = true;
        porta.transform.position = new Vector3(portaMaeDefaultX, portaMaeDefaultY, porta.transform.position.z);
    }

    void FecharPorta()
    {
        MissionManager.instance.scenerySounds2.PlayDoorClose();
        GameObject porta = GameObject.Find("DoorToMomRoom").gameObject;
        float posX = porta.GetComponent<SpriteRenderer>().bounds.size.x / 5;
        porta.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
        porta.GetComponent<Collider2D>().isTrigger = false;
        porta.transform.position = new Vector3(portaMaeDefaultX + posX, portaMaeDefaultY, porta.transform.position.z);
    }

    void SetMae()
    {
        mom = MissionManager.instance.AddObject("mom", "", new Vector3(-1.8f, 0f, -0.5f), new Vector3(0.3f, 0.3f, 1));
        mom.GetComponent<Patroller>().isPatroller = true;
        Transform target1 = new GameObject().transform, target2 = new GameObject().transform;
        target1.position = new Vector3(6.8f, 0f, -0.5f);
        target2.position = new Vector3(vasoDefaultX, 0f, -0.5f);
        Transform[] momTargets = { target1, target2 };
        mom.GetComponent<Patroller>().targets = momTargets;
        MissionManager.instance.AddObject("ActionPatroller", "", new Vector3(-1.8f, 0, 0), new Vector3(0.7f, 0.7f, 1));
    }

}