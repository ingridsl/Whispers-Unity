using UnityEngine;
using CrowShadowManager;
using CrowShadowNPCs;
using CrowShadowPlayer;

public class Mission12 : Mission {

    private float timeToTip = 2;
    private int timesInMI = 0;

    public override void InitMission()
	{
        sceneInit = "Jardim";//"QuartoKid";
		GameManager.initMission = true;
		GameManager.initX = (float) 3.0;
		GameManager.initY = (float) 0.2;
		GameManager.initDir = 3;
        GameManager.LoadScene(sceneInit);
        Book.bookBlocked = false;

        GameManager.instance.invertWorld = false;
        GameManager.instance.invertWorldBlocked = false;
        GameManager.instance.showMissionStart = false;

        GameManager.instance.sideQuests = 1; //comentar depois

        SetInitialSettings();
    }

	public override void UpdateMission() //aqui coloca as ações do update específicas da missão
	{

    }

	public override void SetCorredor()
	{
        GameManager.instance.scenerySounds.StopSound();
        
        GameObject.Find("VasoNaoEmpurravel").gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/vasoPlanta_quebrado");

        //GameManager.instance.rpgTalk.NewTalk ("M5CorridorSceneStart", "M5CorridorSceneEnd");
    }

	public override void SetCozinha()
	{
        GameManager.instance.scenerySounds.PlayDrop();
        //GameManager.instance.rpgTalk.NewTalk ("M5KitchenSceneStart", "M5KitchenSceneEnd");
    }

	public override void SetJardim()
    {

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
    }

	public override void SetQuartoMae()
	{
		//GameManager.instance.rpgTalk.NewTalk ("M5MomRoomSceneStart", "M5MomRoomSceneEnd");
	}


    public override void SetSala()
    {

    }
    public override void SetBanheiro()
    {

    }

    public override void SetPorao()
    {

    }

    public override void EspecificaEnum(int pos)
    {

    }

    public override void ForneceDica()
    {

    }


}