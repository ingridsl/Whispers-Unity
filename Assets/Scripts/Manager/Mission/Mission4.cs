using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using CrowShadowManager;
using CrowShadowNPCs;
using CrowShadowObjects;
using CrowShadowPlayer;
using CrowShadowScenery;

public class Mission4 : Mission
{
    enum enumMission { NIGHT, INICIO, GATO_ACELERA, GATO_CORREDOR, FRENTE_CRIADO,
        MI_DESATIVADO, MI_ATIVADO, ARMARIO, GRANDE_BARULHO, VASO_GATO, VASO_SOZINHO, QUEBRADO,
        POP_UP, MAE_CHEGA_CORREDOR, MAE_CHEGA_QUARTO, VERDADE_MENTIRA, FINAL };
    enumMission secao;

    GameObject pote, vaso, mom;
    SpriteRenderer momRenderer;
    PlaceObject racao;
    BreakableObject breakVaso;

    float portaMaeDefaultY, portaMaeDefaultX;
    float poteDefaultY, poteDefaultX;
    float vasoDefaultY, vasoDefaultX;
    bool invertLocal = false, hasRacao = false;
    bool specialTrigger1 = false, specialTrigger2 = false;

    public override void InitMission()
    {
        sceneInit = "QuartoKid";
        GameManager.initMission = true;
        GameManager.initX = (float)-2.5;
        GameManager.initY = (float)0.7;
        GameManager.initDir = 0;
        GameManager.LoadScene(sceneInit);
        secao = enumMission.NIGHT;
        Book.bookBlocked = true;

        GameManager.instance.InvertWorld(false);
        GameManager.instance.invertWorldBlocked = false;

        SetInitialSettings();

        //if (Inventory.HasItemType(Inventory.InventoryItems.LIVRO)) Inventory.DeleteItem(Inventory.InventoryItems.LIVRO);

        racao = GameManager.instance.transform.Find("Racao").GetComponent<PlaceObject>();
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
        else if (secao == enumMission.INICIO && !GameManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.GATO_ACELERA);
        }

        if (GameManager.instance.invertWorld && !invertLocal)
        {
            invertLocal = true;
            // LUZ DO AMBIENTE
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

            if ((GameManager.currentSceneName.Equals("Corredor") || GameManager.currentSceneName.Equals("QuartoMae"))
                && mom != null && momRenderer != null)
            {
               momRenderer.color = Color.black;
            }

            if (Cat.instance)
            {
                Cat.instance.GetComponent<SpriteRenderer>().color = Color.black;
            }

            if (GameManager.currentSceneName.Equals("Corredor") && secao < enumMission.VASO_GATO)
            {
                AbrirPorta();
            }

            if (secao == enumMission.FRENTE_CRIADO)
            {
                EspecificaEnum((int)enumMission.MI_ATIVADO);
            }
        }
        else if (!GameManager.instance.invertWorld && invertLocal)
        {
            invertLocal = false;
            // LUZ DO AMBIENTE
            mainLight.transform.Rotate(new Vector3(40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

            if ((GameManager.currentSceneName.Equals("Corredor") || GameManager.currentSceneName.Equals("QuartoMae"))
                && mom != null && momRenderer != null)
            {
               momRenderer.color = Color.white;
            }

            if (Cat.instance)
            {
                Cat.instance.GetComponent<SpriteRenderer>().color = Color.white;
            }

            if (GameManager.currentSceneName.Equals("Corredor") && secao < enumMission.VASO_GATO)
            {
                FecharPorta();
            }

            if (secao == enumMission.MI_ATIVADO)
            {
                EspecificaEnum((int)enumMission.MI_DESATIVADO);
            }
        }

        if (GameManager.currentSceneName.Equals("Corredor") && 
            (secao == enumMission.VASO_GATO || secao == enumMission.VASO_SOZINHO) && !GameManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.QUEBRADO);
        }

        if (secao == enumMission.GRANDE_BARULHO && !GameManager.instance.mission4QuebraSozinho && 
            hasRacao && racao.inArea)
        {
            if (!Inventory.HasItemType(Inventory.InventoryItems.RACAO)) {
                hasRacao = false;
                EspecificaEnum((int)enumMission.VASO_GATO);
            }
        }


        if (secao == enumMission.GRANDE_BARULHO && GameManager.instance.mission4QuebraSozinho && breakVaso.broke)
        {
            EspecificaEnum((int)enumMission.VASO_SOZINHO);
        }

        //livro encontrado
        if (secao == enumMission.QUEBRADO)
        {
            if (Inventory.HasItemType(Inventory.InventoryItems.LIVRO))
            {
                EspecificaEnum((int)enumMission.POP_UP);
            }
        }

        if (secao == enumMission.VERDADE_MENTIRA && !GameManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.FINAL);
        }

    }

    public override void SetCorredor()
    {
        if (GameManager.instance.rpgTalk.isPlaying)
        {
            GameManager.instance.rpgTalk.EndTalk();
        }

        if (GameManager.previousSceneName.Equals("GameOver") && Cat.instance == null)
        {
            // Gato
            GameObject cat = GameManager.instance.AddObject(
                "NPCs/catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }

        if (Crow.instance != null)
        {
            Crow.instance.DestroyRaven();
        }
        GameManager.instance.scenerySounds.StopSound();

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
            if (Cat.instance)
            {
                Cat.instance.GetComponent<SpriteRenderer>().color = Color.black;
            }
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
        breakVaso = vaso.GetComponent<BreakableObject>();
        vasoDefaultY = vaso.transform.position.y;
        vasoDefaultX = vaso.transform.position.x;

        if (secao >= enumMission.VASO_GATO)
        {
            vaso.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/vasoPlanta_quebrado");
        }
        else if (secao == enumMission.GRANDE_BARULHO)
        {
            vaso.GetComponent<RegularObject>().enabled = false;
            vaso.GetComponent<BreakableObject>().enabled = true;
        }

        if (secao == enumMission.VASO_GATO)
        {
            GameManager.instance.AddObject(
                "NPCs/catFollower", "", new Vector3(-7f, -0.6f, 0), new Vector3(0.15f, 0.15f, 1));
        }

        if((secao == enumMission.FRENTE_CRIADO || secao == enumMission.MI_ATIVADO || secao == enumMission.MI_DESATIVADO)
            && GameManager.previousSceneName.Equals("Sala"))
        {
            GameManager.instance.rpgTalk.NewTalk("M4DicaCorredor", "M4DicaCorredorEnd", GameManager.instance.rpgTalk.txtToParse);
        }

        if (secao >= enumMission.QUEBRADO && secao < enumMission.MAE_CHEGA_CORREDOR)
        {
            SetMae();
        }
        else if (!GameManager.instance.invertWorld)
        {
            // PortaMãe
            GameObject portaMae = GameObject.Find("DoorToMomRoom").gameObject;
            portaMaeDefaultY = portaMae.transform.position.y;
            portaMaeDefaultX = portaMae.transform.position.x;
            float posX = portaMae.GetComponent<SpriteRenderer>().bounds.size.x / 5;
            portaMae.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
            portaMae.GetComponent<SceneDoor>().isOpened = false;
            portaMae.transform.position = new Vector3(portaMae.transform.position.x + posX, portaMaeDefaultY, portaMae.transform.position.z);
        }

        if (secao == enumMission.MAE_CHEGA_CORREDOR)
        {
            GameManager.instance.AddObject("NPCs/mom", "", new Vector3(-3f, -0.5f, -0.5f), new Vector3(0.3f, 0.3f, 1));
            GameObject trigger3 = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(-2f, 0f, 1), new Vector3(1, 1, 1));
            trigger3.name = "MaeTrigger2";
            trigger3.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            trigger3.GetComponent<BoxCollider2D>().size = new Vector2(2f, 2f);
        }
    }

    public override void SetCozinha()
    {
        if (GameManager.instance.rpgTalk.isPlaying)
        {
            GameManager.instance.rpgTalk.EndTalk();
        }

        GameManager.instance.scenerySounds.StopSound();

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
            if (Cat.instance)
            {
                Cat.instance.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }

        // Panela para caso ainda não tenha
        if (!Inventory.HasItemType(Inventory.InventoryItems.TAMPA))
        {
            GameObject panela = GameObject.Find("Panela").gameObject;
            panela.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/panela_tampa");
            panela.GetComponent<ScenePickUpObject>().enabled = true;
        }

        GameManager.instance.scenerySounds.PlayDrop();

        // Ração no armário alto
        GameManager.instance.CreateScenePickUp("Armario2", Inventory.InventoryItems.RACAO);
    }

    public override void SetJardim()
    {
        if (GameManager.instance.rpgTalk.isPlaying)
        {
            GameManager.instance.rpgTalk.EndTalk();
        }

        GameManager.instance.scenerySounds.StopSound();
        GameManager.instance.scenerySounds.PlayWolf(1);

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
            if (Cat.instance)
            {
                Cat.instance.GetComponent<SpriteRenderer>().color = Color.black;
            }
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

            GameObject pedra = GameManager.instance.AddObject("Objects/PickUp", "Sprites/Objects/Inventory/pedra", new Vector3((float)-3.59, (float)-0.45, 0), new Vector3(0.6f, 0.6f, 1f));
            pedra.GetComponent<PickUpObject>().item = Inventory.InventoryItems.PEDRA;
        }
    }

    public override void SetQuartoKid()
    {
        if (GameManager.instance.rpgTalk.isPlaying)
        {
            GameManager.instance.rpgTalk.EndTalk();
        }

        GameManager.instance.scenerySounds.StopSound();
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
            if (Cat.instance)
            {
                Cat.instance.GetComponent<SpriteRenderer>().color = Color.black;
            }
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
    }

    public override void SetQuartoMae()
    {
        if (GameManager.instance.rpgTalk.isPlaying)
        {
            GameManager.instance.rpgTalk.EndTalk();
        }

        if (GameManager.previousSceneName.Equals("GameOver"))
        {
            GameManager.instance.rpgTalk.NewTalk("M4DicaQuartoMae", "M4DicaQuartoMaeEnd");

            if (Cat.instance == null)
            {
                // Gato
                GameObject cat = GameManager.instance.AddObject(
                    "NPCs/catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
                cat.GetComponent<Cat>().FollowPlayer();
            }
        }

        GameManager.instance.scenerySounds.StopSound();

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
            if (Cat.instance)
            {
                Cat.instance.GetComponent<SpriteRenderer>().color = Color.black;
            }
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
        scenePickUpObject.onlyInvertedWorld = true;

        GameManager.instance.AddObject("Scenery/Gloom", "", new Vector3(2.1f, 2.17f, -0.5f), new Vector3(1f, 1f, 1));

        if (secao < enumMission.QUEBRADO)
        {
            mom = GameManager.instance.AddObject("NPCs/mom", "", new Vector3(2f, 1.5f, -0.5f), new Vector3(0.3f, 0.3f, 1));
            momRenderer = mom.GetComponent<SpriteRenderer>();
            if (GameManager.instance.invertWorld)
            {
                momRenderer.color = Color.black;
            }

            mom.GetComponent<Patroller>().isPatroller = true;
            mom.GetComponent<Patroller>().speed = 0.5f;
            Vector3 target1 = new Vector3(3.2f, 1.5f, -0.5f);
            Vector3 target2 = new Vector3(2f, 0.5f, -0.5f);
            Vector3 target3 = new Vector3(2.75f, 0.15f, -0.5f);
            Vector3 target4 = new Vector3(2f, 1.5f, -0.5f);
            Vector3[] momTargets = { target1, target2, target3, target4};
            mom.GetComponent<Patroller>().targets = momTargets;

            mom.GetComponent<Patroller>().hasActionPatroller = true;
            mom.GetComponent<CircleCollider2D>().radius = 8;
        }

        if ((secao == enumMission.MI_ATIVADO || secao == enumMission.MI_DESATIVADO))
        {
            EspecificaEnum((int)enumMission.ARMARIO);
        }

    }

    public override void SetSala()
    {
        if (GameManager.instance.rpgTalk.isPlaying)
        {
            GameManager.instance.rpgTalk.EndTalk();
        }

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
            if (Cat.instance)
            {
                Cat.instance.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }

        //pote de ração vazio
        pote = GameObject.Find("PoteRação").gameObject;
        pote.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/pote-vazio");
        poteDefaultY = pote.transform.position.y;
        poteDefaultX = pote.transform.position.x;

        GameObject trigger = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(poteDefaultX, poteDefaultY, 0), new Vector3(1, 1, 1));
        trigger.name = "PoteTrigger";
        trigger.GetComponent<Collider2D>().offset = new Vector2(0, 0);
        trigger.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);

        GameObject triggerB = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(0f, 0f, 0), new Vector3(1, 1, 1));
        triggerB.name = "Special1Trigger";
        triggerB.GetComponent<Collider2D>().offset = new Vector2(-6.1f, -2f);
        triggerB.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);

        if (secao < enumMission.ARMARIO)
        {
            // Estante
            GameObject triggerE = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(-5.71f, 1.64f, 0), new Vector3(1, 1, 1));
            triggerE.name = "EstanteTrigger";
            triggerE.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            triggerE.GetComponent<BoxCollider2D>().size = new Vector2(1.8f, 1.6f);
        }
        else
        {
            GameObject triggerS = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(0f, 0f, 0), new Vector3(1, 1, 1));
            triggerS.name = "Special2Trigger";
            triggerS.GetComponent<Collider2D>().offset = new Vector2(-5.7f, 1.6f);
            triggerS.GetComponent<BoxCollider2D>().size = new Vector2(2f, 2f);
        }
    }
    public override void SetBanheiro()
    {
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
            if (Cat.instance)
            {
                Cat.instance.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
    }


    public override void SetPorao()
    {
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
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
            GameManager.instance.rpgTalk.NewTalk("M4KidRoomSceneStart", "M4KidRoomSceneEnd", GameManager.instance.rpgTalk.txtToParse);

            GameManager.instance.scenerySounds.PlayCat(3);
            GameObject cat = GameManager.instance.AddObject("NPCs/catFollower", "", new Vector3(-1f, 0.5f, -0.5f), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().followWhenClose = false;
            cat.GetComponent<Cat>().Patrol();
            Vector3 aux = new Vector3(1.8f, 0.8f, -0.5f);
            Vector3[] catPos = { aux };
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
            GameManager.instance.scenerySounds.PlayCat(2);

            GameObject trigger = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(6.5f, -0.1f, 1), new Vector3(1, 2, 1));
            trigger.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            trigger.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);

            GameObject cat = GameManager.instance.AddObject("NPCs/catFollower", "", new Vector3(10f, -0.6f, -0.5f), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().Patrol();
            Vector3 aux = new Vector3(7f, -0.2f, -0.5f);
            Vector3[] catPos = { aux };
            cat.GetComponent<Cat>().targets = catPos;
            cat.GetComponent<Cat>().speed = 1.6f;
            cat.GetComponent<Cat>().stopEndPath = true;
        }
        else if (secao == enumMission.FRENTE_CRIADO)
        {
            GameManager.instance.rpgTalk.NewTalk("frenteCriadoStart", "frenteCriadoEnd");
            Cat.instance.followWhenClose = true;
            Cat.instance.FollowPlayer();
        }
        else if (secao == enumMission.ARMARIO)
        {
            GameManager.instance.rpgTalk.NewTalk("M4MomRoomSceneStart", "M4MomRoomSceneEnd");
        }
        else if (secao == enumMission.GRANDE_BARULHO)
        {
            GameManager.instance.rpgTalk.NewTalk("GrandeBarulhoStart", "GrandeBarulhoEnd");
        }
        else if (secao == enumMission.VASO_GATO)
        {
            pote.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/pote-racao");

            GameManager.instance.scenerySounds.PlayCat(4);
            Cat.instance.followWhenClose = false;
            Cat.instance.GetComponent<Cat>().Patrol();
            Vector3 aux = new Vector3(-3f, 1.5f, -0.5f);
            Vector3[] catPos = { aux };
            Cat.instance.targets = catPos;
            Cat.instance.speed = 1.6f;
            Cat.instance.destroyEndPath = true;

            GameManager.instance.rpgTalk.NewTalk("OQ", "OQEnd", GameManager.instance.rpgTalk.txtToParse, false);
        }
        else if (secao == enumMission.VASO_SOZINHO)
        {
            vaso.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/vasoPlanta_quebrado");

            GameManager.instance.rpgTalk.NewTalk("OQ", "OQEnd", GameManager.instance.rpgTalk.txtToParse, false);
        }
        else if (secao == enumMission.QUEBRADO)
        {
            GameManager.instance.rpgTalk.NewTalk("M4Vase", "M4VaseEnd", GameManager.instance.rpgTalk.txtToParse, false);
            AbrirPorta();
            SetMae();
        }
        else if (secao == enumMission.POP_UP)
        {
            //Book.AddPage(); // livro encontrado

            if (GameManager.instance.pathCat >= GameManager.instance.pathBird)
            {
                EspecificaEnum((int)enumMission.MAE_CHEGA_CORREDOR);
            }
            else
            {
                EspecificaEnum((int)enumMission.MAE_CHEGA_QUARTO);
            }
        }
        else if (secao == enumMission.MAE_CHEGA_CORREDOR)
        {
            GameManager.instance.scenerySounds.PlayCat(1);
            GameManager.instance.rpgTalk.NewTalk("M4MaeChegouCorredor", "M4MaeChegouCorredorEnd", GameManager.instance.rpgTalk.txtToParse, false);
        }
        else if (secao == enumMission.MAE_CHEGA_QUARTO)
        {
            GameManager.instance.rpgTalk.NewTalk("M4MaeChegou", "M4MaeChegouEnd", GameManager.instance.rpgTalk.txtToParse, false);
            GameManager.instance.AddObject("NPCs/mom", "", new Vector3(-4f, 0f, -0.5f), new Vector3(0.3f, 0.3f, 1));
            GameObject trigger = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(-4f, 0f, 1), new Vector3(2, 2, 1));
            trigger.name = "MaeTrigger";
            trigger.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            trigger.GetComponent<BoxCollider2D>().size = new Vector2(2f, 2f);
        }
        else if (secao == enumMission.FINAL)
        {
            GameManager.instance.Invoke("InvokeMission", 2f);
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
            GameManager.instance.rpgTalk.NewTalk("NadaAqui", "NadaAquiEnd");
        }
        // pote de ração vazio
        if (tag.Equals("EnterPoteTrigger") && secao == enumMission.GRANDE_BARULHO && 
            !GameManager.instance.mission4QuebraSozinho && !racao.inArea)
        {
            if (Inventory.HasItemType(Inventory.InventoryItems.RACAO))
            {
                hasRacao = true;
                racao.inArea = true;
            }
        }
        else if (tag.Equals("ExitPoteTrigger") && secao == enumMission.GRANDE_BARULHO && 
            !GameManager.instance.mission4QuebraSozinho && racao.inArea)
        {
            if (Inventory.HasItemType(Inventory.InventoryItems.RACAO))
            {
                racao.inArea = false;
            }
        }
        // mãe mandar para o quarto
        if ((tag.Equals("MaeTrigger") && secao == enumMission.MAE_CHEGA_QUARTO) || 
            (tag.Equals("MaeTrigger2") && secao == enumMission.MAE_CHEGA_CORREDOR))
        {
            GameManager.instance.rpgTalk.NewTalk("VerdadeOuMentira", "VerdadeOuMentiraEnd");
            EspecificaEnum((int)enumMission.VERDADE_MENTIRA);
        }
        else if (tag.Equals("Special1Trigger") && !specialTrigger1)
        {
            GameManager.instance.rpgTalk.NewTalk("M4LivingroomSpecialBall", "M4LivingroomSpecialBallEnd", GameManager.instance.rpgTalk.txtToParse);
            specialTrigger1 = true;
        }
        else if (tag.Equals("Special2Trigger") && !specialTrigger2)
        {
            GameManager.instance.rpgTalk.NewTalk("M4LivingroomSpecialShelf", "M4LivingroomSpecialShelfEnd", GameManager.instance.rpgTalk.txtToParse);
            specialTrigger2 = true;
        }
    }

    public override void InvokeMission()
    {
        if (secao == enumMission.FINAL)
        {
            GameManager.instance.ChangeMission(5);
        }
    }

    void AbrirPorta()
    {
        GameManager.instance.scenerySounds2.PlayDoorOpen(1);
        GameObject porta = GameObject.Find("DoorToMomRoom").gameObject;
        porta.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-opened");
        porta.GetComponent<SceneDoor>().isOpened = true;
        porta.transform.position = new Vector3(portaMaeDefaultX, portaMaeDefaultY, porta.transform.position.z);
    }

    void FecharPorta()
    {
        GameManager.instance.scenerySounds2.PlayDoorClose();
        GameObject porta = GameObject.Find("DoorToMomRoom").gameObject;
        float posX = porta.GetComponent<SpriteRenderer>().bounds.size.x / 5;
        porta.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
        porta.GetComponent<SceneDoor>().isOpened = false;
        porta.transform.position = new Vector3(portaMaeDefaultX + posX, portaMaeDefaultY, porta.transform.position.z);
    }

    void SetMae()
    {
        //mae patrulha
        mom = GameManager.instance.AddObject("NPCs/mom", "", new Vector3(-2f, -0.5f, -0.5f), new Vector3(0.3f, 0.3f, 1));
        momRenderer = mom.GetComponent<SpriteRenderer>();
        if (GameManager.instance.invertWorld)
        {
            momRenderer.color = Color.black;
        }

        mom.GetComponent<Patroller>().isPatroller = true;
        Vector3 target1 = new Vector3(-7f, -0.3f, -0.5f);
        Vector3 target2 = new Vector3(-7f, 0.3f, -0.5f);
        Vector3 target3 = new Vector3(-7f, -0.3f, -0.5f);
        Vector3 target4 = new Vector3(0f, -0.3f, -0.5f);
        if (Random.value > 0) target4 = new Vector3(-3f, -0.3f, -0.5f);
        Vector3 target5 = new Vector3(3f, -0.5f, -0.3f);
        Vector3 target6 = new Vector3(5f, -0.5f, -0.3f);
        if (Random.value > 0) target6 = new Vector3(-5f, -0.3f, -0.5f);
        Vector3 target7 = new Vector3(-8f, -0.5f, -0.3f);
        Vector3[] momTargets = { target1, target2, target3, target4, target5, target6, target7 };

        mom.GetComponent<Patroller>().targets = momTargets;
        mom.GetComponent<Patroller>().hasActionPatroller = true;
    }

    public override void ForneceDica()
    {
        if (GameManager.currentSceneName.Equals("Corredor") && secao == enumMission.FRENTE_CRIADO)
        {
            GameManager.instance.timer = 0;
            GameManager.instance.rpgTalk.NewTalk("M4DicaInvertStart", "M4DicaInvertEnd", GameManager.instance.rpgTalk.txtToParse);
        }
    }

}