﻿using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MissionManager : MonoBehaviour {

    public static MissionManager instance;
    public Mission mission;
    public int missionSelected; // colocar 1 quando for a versão final, para começar na missão 1 quando clicar em new game
    public string previousSceneName, currentSceneName;

    public bool paused = false;
    public bool blocked = false;

    public static bool initMission = false;
    public static float initX = 0, initY = 0;
    public static int initDir = 0;

    public float pathBird, pathCat;
    private GameObject hud, menu, text1, text2;
    private int optionSelected;

    //float startMissionDelay = 3f;

    public void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            currentSceneName = SceneManager.GetActiveScene().name;
            previousSceneName = currentSceneName;
            hud = GameObject.Find("HUDCanvas").gameObject;
            menu = hud.transform.Find("DecisionMenu").gameObject;
            text1 = menu.transform.Find("Option1").gameObject;
            text2 = menu.transform.Find("Option2").gameObject;
            SetMission(missionSelected);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        if(mission != null) mission.UpdateMission();

        // teste, depois colocar pelo menu
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            ChangeMission(1);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            ChangeMission(2);
        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            LoadGame(0);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        previousSceneName = currentSceneName;
        currentSceneName = scene.name;
        print("OLDSCENE" + previousSceneName);
        print("NEWSCENE" + currentSceneName);
        if (!initMission) {
            GetComponent<Player>().ChangePosition();
        }
        else {
            GetComponent<Player>().ChangePositionDefault(initX, initY, initDir);
            if (currentSceneName.Equals(mission.sceneInit))
            {
                initMission = false;
                initX = initY = 0;
            }
        }
        if(mission != null) mission.LoadMissionScene();
    }

    public void AddObject(string name, Vector3 position, Vector3 scale)
    {
        GameObject moveInstance =
            Instantiate(Resources.Load("Prefab/" + name),
            position, Quaternion.identity) as GameObject;
        moveInstance.transform.localScale = scale;
    }

    public void SetDecision(string opt1, string opt2)
    {
        blocked = true;
        menu.SetActive(true);
        text1.GetComponent<Text>().text = opt1;
        text2.GetComponent<Text>().text = opt2;
        SelectOption(text1, text2);
        optionSelected = 1;
    }

    public int MakeDecision()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (optionSelected == 1)
            {
                SelectOption(text2, text1);
                optionSelected = 2;
            }
            else if (optionSelected == 2)
            {
                SelectOption(text1, text2);
                optionSelected = 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            blocked = false;
            menu.SetActive(false);
            return optionSelected;
        }

        return -1;
    }

    private void SelectOption(GameObject textSel, GameObject textNon)
    {
        textSel.GetComponent<Text>().fontStyle = FontStyle.BoldAndItalic;
        textSel.GetComponent<Text>().color = Color.red;
        textNon.GetComponent<Text>().fontStyle = FontStyle.Normal;
        textNon.GetComponent<Text>().color = Color.white;
    }

    private Save CreateSaveGameObject()
    {
        Save save = new Save();
        
        save.inventory = Inventory.GetInventory();
        save.mission = missionSelected;
        save.currentItem = Inventory.GetCurrentItem();

        return save;
    }

    public void SaveGame(int m)
    {
        Save save = CreateSaveGameObject();
        
        BinaryFormatter bf = new BinaryFormatter();
        // m = 0 -> continue
        // m > 0 -> select mission
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave" + m + ".save");
        bf.Serialize(file, save);
        file.Close();
        
        Debug.Log("Game Saved " + m);
    }

    public void LoadGame(int m)
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave" + m + ".save"))
        {
            paused = true;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave" + m + ".save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            SetMission(save.mission);
            Inventory.SetInventory(save.inventory);
            if(save.currentItem != -1) Inventory.SetCurrentItem(save.currentItem);

            Debug.Log("Game Loaded " + m);

            paused = false;
        }
        else
        {
            Debug.Log("No game saved!");
        }
    }

    public void SetMission(int m)
    {
        missionSelected = m;
        if (missionSelected == 1)
        {
            mission = new Mission1();
        }
        else if (missionSelected == 2)
        {
            mission = new Mission2();
        }
        if (mission != null) mission.InitMission();
    }

    public void ChangeMission(int m)
    {
        SetMission(m);
        SaveGame(0);
        SaveGame(missionSelected);
    }
    
}
