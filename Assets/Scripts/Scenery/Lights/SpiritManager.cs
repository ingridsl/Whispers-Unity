﻿using System.Collections.Generic;
using UnityEngine;

public class SpiritManager : MonoBehaviour {
    private int goodSpiritCount = 0;
    private int evilSpiritCount = 0;
    private int killerSpiritCount = 0;

    private int goodSpiritKilled = 0;
    private int evilSpiritKilled = 0;
    private int killerSpiritKilled = 0;

    private int maxEvilKilled = 0;
    private int maxKillerKilled = 0;
    private int totalGoodSpirit = 0;

    public bool success = false;

    private Dictionary<int, GameObject> goodSpiritDictionary = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> evilSpiritDictionary = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> killerSpiritDictionary = new Dictionary<int, GameObject>();

    private void Start()
    {
        goodSpiritKilled = 0;
        evilSpiritKilled = 0;
        killerSpiritKilled = 0;
        success = false;
    }

    private void Update()
    {

    }

    // GERA O MAPA DE ESPÍRITOS
    // PARÂMETROS: raio, ponto de origem X e Y, número máximo de evil e killed para destruir, marcador se possui killer
    // número máximo de espíritos em sequência, diferença máxima do número de evil e good (killer e evil, killer e good) ao construir mapa
    public void GenerateSpiritMap(float radius, float originX, float originY, int maxEvil, int maxSequencia = 2, int difEvilGood = 1,
        bool hasKiller = false, int maxKiller = 0, int difKillerEvil = 3, int difKillerGood = 6)
    {
        maxEvilKilled = maxEvil;
        maxKillerKilled = maxKiller;

        int maxRange = 3, sequencia = 0, aux2 = -1;
        if (hasKiller) maxRange = 4;

        for (float y = -radius; y <= radius; y += 1f)
        {
            for (float x = -radius; x <= radius; x += 1f)
            {
                if (x * x + y * y <= radius * radius) {
                    int aux = Random.Range(1, maxRange);

                    if (aux2 == aux)
                    {
                        sequencia++;
                        if (sequencia == maxSequencia)
                        {
                            aux = ((aux + 1) % maxRange);
                        }
                    }
                    else
                    {
                        sequencia = 0;
                    }

                    if (aux == 1 && evilSpiritCount < (goodSpiritCount+difEvilGood))
                    {
                        aux = 2;
                    }
                    else if (hasKiller && aux == 2 && killerSpiritCount < (evilSpiritCount+difKillerEvil))
                    {
                        aux = 3;
                    }
                    else if (hasKiller && aux == 3 && goodSpiritCount < (killerSpiritCount-difKillerGood))
                    {
                        aux = 1;
                    }

                    AddSpirit(originX + x, originY + y, aux);
                    aux2 = aux;
                }
            }
        }

        totalGoodSpirit = goodSpiritCount;
    }

    private void AddSpirit(float x, float y, int type)
    {
        switch (type)
        {
            case 1:
                GameObject goodSpirit = MissionManager.instance.AddObjectWithParent("Scenery/GoodSpirit", "", new Vector3(x, y, 0), new Vector3(1f, 1f, 1), transform);
                goodSpirit.GetComponent<Spirit>().number = goodSpiritCount;
                goodSpirit.GetComponent<Spirit>().spiritManager = this;
                goodSpiritDictionary.Add(goodSpiritCount, goodSpirit);
                goodSpiritCount++;
                break;
            case 2:
                GameObject evilSpirit = MissionManager.instance.AddObjectWithParent("Scenery/EvilSpirit", "", new Vector3(x, y, 0), new Vector3(1f, 1f, 1), transform);
                evilSpirit.GetComponent<Spirit>().number = evilSpiritCount;
                evilSpirit.GetComponent<Spirit>().spiritManager = this;
                evilSpiritDictionary.Add(evilSpiritCount, evilSpirit);
                evilSpiritCount++;
                break;
            default:
                GameObject killerSpirit = MissionManager.instance.AddObjectWithParent("Scenery/KillerSpirit", "", new Vector3(x, y, 0), new Vector3(1f, 1f, 1), transform);
                killerSpirit.GetComponent<Spirit>().number = killerSpiritCount;
                killerSpirit.GetComponent<Spirit>().spiritManager = this;
                killerSpiritDictionary.Add(killerSpiritCount, killerSpirit);
                killerSpiritCount++;
                break;
        }
    }

    public void DestroyGoodSpirit()
    {
        goodSpiritKilled++;

        if (goodSpiritKilled >= totalGoodSpirit)
        {
            success = true;
        }
    }

    public void DestroyEvilSpirit()
    {
        evilSpiritKilled++;

        if (evilSpiritKilled >= maxEvilKilled)
        {
            MissionManager.instance.GameOver();
        }
    }

    public void DestroyKillerSpirit()
    {
        killerSpiritKilled++;

        if (killerSpiritKilled >= maxKillerKilled)
        {
            MissionManager.instance.GameOver();
        }
    }

}
