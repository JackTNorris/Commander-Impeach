using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Globalization;

public class Parallaxer : MonoBehaviour
{
        
    class PoolObject
    {
        public Transform transform; //transform relates to the position, rotation, and scale of an object
        public bool inUse;
       
        public PoolObject(Transform t) { transform = t; }
        public void Use() { inUse = true; }
        //public bool isConsumable = false;
        public bool isConsumed = false;
        public void Dispose() { inUse = false; isConsumed = false; }
        public string offense;
        //public bool isConsumable = false;
    }


    [System.Serializable]//making sure viewable in unity inspector
    public struct YSpawnRange
    {
        public float min;
        public float max;
    }
    [System.Serializable]
    public struct Label_Prefab
    {
        public GameObject prefab;
        public bool beenChosen;
        public bool inUse;
        public void Dispose() { inUse = false;}
        public string name;
        public bool isPassed;
    }
    int rnd;
    public GameObject Prefab; //a prefab is a premade game object you configured in the scene

    int labelCounter = 0;
    int counter = 0;
    public int poolSize;
    public float shiftSpeed;
    public float spawnRate;
    public bool Include_Consumable;
    public bool Include_Label;
    bool holder;
    int savedRand;
    Vector3 rotate = new Vector3(0, 0, 1);
    
    public Label_Prefab[] Label_Prefabs;

    //public GameObject[] Label_Prefabs;

    public GameObject Consumable_Prefab;
    public bool Save_Pos;

    public YSpawnRange ySpawnRange;
    public Vector3 defaultSpawnPos;
    public bool spawnImmediate; //particle prewarm
    public Vector3 immediateSpawnPos;
    public Vector2 targetAspectRatio;    //for different mobile device aspect ratios

    float spawnTimer;
    float targetAspect;
    PoolObject[] poolObjects;       //group of objects that make up this set
    PoolObject[] consumableObjects;
    PoolObject[] labelObjects;
    GameManager game;
    ImpeachableOffenses impeach;

    void Awake()
    {
        Configure();
    }

    void Start()
    {
        game = GameManager.Instance; //sets Gamemanager object game to the originanal gamemanager
        impeach = ImpeachableOffenses.Instance;
        for (int i = 0; i < Label_Prefabs.Length; i++)
        {

            Label_Prefabs[i].isPassed = false;

        }
        for(int j = 0; j < Label_Prefabs.Length; j++)
        {
            if(Label_Prefabs[j].beenChosen)
            {
                Label_Prefabs[j].isPassed = true;
            }
        }
    }






    void OnEnable()
    {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed; // subscribing to gameoverconfirmed definition in gamemanager class
       // ImpeachableOffenses.LabelName += LabelName;
        TapController.OnConsumed += OnConsumed;
        TapController.OnPassLabel += OnPassLabel;
        TapController.OnPlayerScored += OnPlayerScored;

        for(int i = 0; i < Label_Prefabs.Length; i ++)
        {
            Label_Prefabs[i].name = Label_Prefabs[i].prefab.name;
        }
    }

    void OnDisable()
    {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed; // unsubscribing to gameoverconfirmed
        TapController.OnConsumed -= OnConsumed;
        //ImpeachableOffenses.LabelName -= LabelName;
        TapController.OnPassLabel -= OnPassLabel;
        TapController.OnPlayerScored -= OnPlayerScored;
        
    }

    void OnGameOverConfirmed()
    {
        

        if (!Save_Pos)
        {
            for (int i = 0; i < poolObjects.Length; i++)
            {
                poolObjects[i].Dispose();
                poolObjects[i].transform.position = Vector3.one * 1000;
            }

            if (Include_Consumable)
            {
                for (int i = 0; i < consumableObjects.Length; i++)
                {
                    consumableObjects[i].Dispose();
                    consumableObjects[i].transform.position = Vector3.one * 1000;
                }
            }

            if (Include_Label)
            {
                for(int i = 0; i < Label_Prefabs.Length; i++)
                {
                    Label_Prefabs[i].Dispose();
                    Label_Prefabs[i].prefab.transform.position = Vector3.one * 1000;
                }
                /*
                for (int i = 0; i < labelObjects.Length; i++)
                {
                    labelObjects[i].Dispose();
                    labelObjects[i].transform.position = Vector3.one * 1000;
                }
                */


            }
        }




        /*if (spawnImmediate)
        {
            SpawnImmediate();
        }*/
    }










    void Update()
    {
        if (game.GameOver) { return; }
        if (!game.InMotion) { return; }
        Shift();

        spawnTimer += Time.deltaTime;//delta time = time it took to complete the last frame;
        if (spawnTimer > spawnRate)
        {
            Spawn();
            spawnTimer = 0;
        }
        if (Include_Consumable)
        {
            for (int i = 0; i < consumableObjects.Length; i++)
            {
                if (consumableObjects[i].isConsumed)
                {
                    consumableObjects[i].transform.position += Vector3.up * shiftSpeed * Time.deltaTime;
                    consumableObjects[i].transform.Rotate(Vector3.up * 1500 * Time.deltaTime, Space.World);
                }
                else
                {
                    consumableObjects[i].transform.Rotate(Vector3.up * 500 * Time.deltaTime, Space.World);
                }
                
            }
        }

        if (GameManager.Player_Lives == 1 && Prefab.name != "street dynamic")
        {
            if(Save_Pos)
            {
                holder = true;
            }
            
            Save_Pos = false;
        }
        if(GameManager.Player_Lives > 1 &&holder == true)
        {
            Save_Pos = true;
        }




    }

    void Configure()
    {

        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        poolObjects = new PoolObject[poolSize];
        for (int i = 0; i < poolObjects.Length; i++)
        {
            GameObject go = Instantiate(Prefab) as GameObject;
            Transform t = go.transform;
            t.SetParent(transform);
            t.position = Vector3.one * 1000;
            poolObjects[i] = new PoolObject(t);
        }


        if (Include_Consumable)
        {
            consumableObjects = new PoolObject[poolSize];
            for (int i = 0; i < poolObjects.Length; i++)
            {
                GameObject go = Instantiate(Consumable_Prefab) as GameObject;
                Transform t = go.transform;
                t.SetParent(transform);
                t.position = Vector3.one * 1000;
                consumableObjects[i] = new PoolObject(t);
                //consumableObjects[i].isConsumable = true;
            }
        }

        
        if (Include_Label)
        {
            /*
            for(int i = 0; i < Label_Prefabs.Length; i++)
            {
                Label_Prefabs[i].beenChosen = false;
                                    
            }
            Configure_Label();
            */
            for (int i = 0; i < Label_Prefabs.Length; i++)
            {
                Label_Prefabs[i].prefab.transform.position = Vector3.one * 1000;
            }


        }


        if (spawnImmediate)
        {
            SpawnImmediate();
        }
        

        
    }






    void Configure_Label()
    {

        
        labelObjects = new PoolObject[poolSize];
        int counter = 0;





        for (int i = 0; i < poolObjects.Length; i++)
        {


            randomizer: int rnd = Random.Range(0, Label_Prefabs.Length);

            if (!Label_Prefabs[rnd].beenChosen)
            {
                counter++;
                if (counter == Label_Prefabs.Length)
                {

                    counter = 0;
                    int savedInt = rnd;
                    for (int j = 0; j < Label_Prefabs.Length; j++)
                    {
                        Label_Prefabs[j].beenChosen = false;
                        if (j == savedInt)
                        {
                            Label_Prefabs[j].beenChosen = true;
                        }
                    }

                }


            }
            else
            {
                goto randomizer;
            }


            GameObject go = Instantiate(Label_Prefabs[rnd].prefab) as GameObject;
            Label_Prefabs[rnd].beenChosen = true;

            Transform t = go.transform;
            t.SetParent(transform);
            t.position = Vector3.one * 1000;
            labelObjects[i] = new PoolObject(t);




        }
    }






    void Spawn()
    {
        
        Transform t = GetPoolObject();
        if (t == null) return; //if ture, this indicates that poolSize is too small
        Vector3 pos = Vector3.zero;
        pos.x = defaultSpawnPos.x;
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);

        t.position = pos;


        /*
        if(Label_Prefabs.Length > 0 && pipeCounter%Label_Prefabs.Length == 0)
        {
            Configure_Label();
        }
        */


        if (Include_Consumable)
        {
            Transform j = GetConsumable();
            if (j == null) return; //if ture, this indicates that poolSize is too small
            j.position = pos;
        }

        if (Include_Label)//must be at bottom in this code line
        {
            GameObject label = GetLabel();

            if (labelCounter   == Label_Prefabs.Length)
            {
                labelCounter = 0;
                for (int i = 0; i < Label_Prefabs.Length; i++)
                {
                    Label_Prefabs[i].beenChosen = false;
                }
                //Label_Prefabs[rnd].beenChosen = true;
            }


            if (label == null) return;
            if (pos.y >= .55)
            {
                pos.y -= 3;
            }
            else if (pos.y < .55)
            {
                pos.y += 3;
            }

            else
            {
                pos.y += 3;
            }
            label.transform.position = pos;


            /*
            labelCounter++;
            randomizer: int rnd = Random.Range(0, Label_Prefabs.Length);
            if (!Label_Prefabs[rnd].beenChosen)
            {
                GameObject go = Instantiate(Label_Prefabs[rnd].prefab) as GameObject;
                Label_Prefabs[rnd].beenChosen = true;
                Transform g = go.transform;
                g.SetParent(transform);
                
                Label_Prefabs[rnd].beenChosen = true;
                if(labelCounter%Label_Prefabs.Length == 0)
                {
                    for(int i = 0; i < Label_Prefabs.Length; i++)
                    {
                        Label_Prefabs[i].beenChosen = false;
                    }
                }
                g.position = pos;
                labelObjects[counter] = new PoolObject(g);
            }
            else
            {
                goto randomizer;
            }


            if (counter == poolSize)
            {
                counter = 0;
            }
            else
            {
                counter++;
            }
            


            */


            /*
            Transform k = GetLabel();
            if (k == null) return;
            if(pos.y >= .55)
            {
                pos.y -= 3;
            }
            else if (pos.y < .55)
            {
                pos.y += 3;
            }

            else
            {
                pos.y += 3;
            }
            k.position = pos;
            */
        }

    }



    void SpawnImmediate()
    {
        Transform t = GetPoolObject();
        if (t == null) return; //if ture, this indicates that poolSize is too small
        Vector3 pos = Vector3.zero;
        pos.x = immediateSpawnPos.x;
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);

        t.position = pos;
        

        if (Include_Consumable)
        {
            Transform j = GetConsumable();
            if (j == null) return; //if ture, this indicates that poolSize is too small
            //Vector3 pos = Vector3.zero;
           // pos.x = defaultSpawnPos.x;
           // pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);

            j.position = pos;
        }

        if (Include_Label)
        {
            GameObject label = GetLabel();
            if (label == null) return;
            if (pos.y >= .55)
            {
                pos.y -= 3;
            }
            else if (pos.y < .55)
            {
                pos.y += 3;
            }

            else
            {
                pos.y += 3;
            }
            label.transform.position = pos;
        }
        Spawn();

    }



    void Shift()//move objects
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].transform.position += -Vector3.right * shiftSpeed * Time.deltaTime;
            CheckDisposeObject(poolObjects[i]);
            
        }
        if (Include_Consumable)
        {
            for (int i = 0; i < poolObjects.Length; i++)
            {
                consumableObjects[i].transform.position += -Vector3.right * shiftSpeed * Time.deltaTime;
                CheckDisposeObject(consumableObjects[i]);
            }
        }

        if (Include_Label)
        {
            for (int i = 0; i < Label_Prefabs.Length; i++)
            {
                Label_Prefabs[i].prefab.transform.position += -Vector3.right * shiftSpeed * Time.deltaTime;
                Check(Label_Prefabs[i]);
            }
        }


    }

    void CheckDisposeObject(PoolObject poolObject) //check if object needs to be erased
    {
        //if (!poolObject.isConsumable)
        //{
            if(poolObject.transform.position.x < -defaultSpawnPos.x)
            {
                poolObject.Dispose();//dispose will take them off screen
                poolObject.transform.position = Vector3.one * 1000;
            }
        //}

       /* if (poolObject.isConsumable)
        {
            if (poolObject.transform.position.x < -defaultSpawnPos.x)
            {
                poolObject.Dispose();//dispose will take them off screen
                poolObject.transform.position = Vector3.one * 1000;
            }
        }*/



    }

    void Check (Label_Prefab label)
    {
        if(label.prefab.transform.position.x < -defaultSpawnPos.x)
        {
            label.Dispose();
            label.prefab.transform.position = Vector3.one * 1000;
        }
    }

    Transform GetPoolObject()
    {
        for (int i = 0; i< poolObjects.Length; i++)
        {
            if (!poolObjects[i].inUse)
            {
                poolObjects[i].Use();
                return poolObjects[i].transform;    //might want to look at this my friend
            }
        }

        return null;

    }


    Transform GetConsumable()
    {
        for (int i = 0; i < consumableObjects.Length; i++)
        {
            if (!consumableObjects[i].inUse)
            {
                consumableObjects[i].Use();
                return consumableObjects[i].transform;    //might want to look at this my friend
            }
        }

        return null;
    }

    GameObject GetLabel()
    {
        
        labelCounter++;
        randomizer: rnd = Random.Range(0, Label_Prefabs.Length);
        if(savedRand == rnd)
        {
            goto randomizer;
        }
                
        if(!Label_Prefabs[rnd].beenChosen /*&& !Label_Prefabs[rnd].inUse*/)
        {
            /*
            if (labelCounter % Label_Prefabs.Length == 0 )
            {
                
                for(int i = 0; i < Label_Prefabs.Length; i++)
                {
                    Label_Prefabs[i].beenChosen = false;
                }
                
            }
            */
            Label_Prefabs[rnd].beenChosen = true;
            //Label_Prefabs[rnd].inUse = true;
            savedRand = rnd;
            return Label_Prefabs[rnd].prefab;
        }
        else
        {
            goto randomizer;
        }
        
        
            
        


    }


    void OnConsumed(Collider2D col)
    {
        if (Include_Consumable)
        {
            for (int i = 0; i < consumableObjects.Length; i++)
            {
                if(col.transform.position == consumableObjects[i].transform.position)
                {
                    //consumableObjects[i].transform.position = Vector3.one*1000;
                    consumableObjects[i].isConsumed = true;
                    //consumableObjects[i].inUse = false;
                    
                }
            }
        }

        if(Include_Label)
        {
            /*
            for (int i = 0; i < Label_Prefabs.Length; i++)
            {

                Label_Prefabs[i].isPassed = false;

            }
            
            for (int i = 0; i < Label_Prefabs.Length; i ++)
            {
                if(Label_Prefabs[i].prefab.transform.position.x >= col.transform.position.x - 1 && Label_Prefabs[i].prefab.transform.position.x <= col.transform.position.x + 1)
                {
                    Label_Prefabs[i].isPassed = true;
                }
            }
            */
        }

        //prevPos = consumable.transform.position;
        // consumable.transfrom.position = newPos;
    }

    void OnPassLabel(Collider2D col)
    {
        for (int i = 0; i < Label_Prefabs.Length; i++)
        {
            if (Label_Prefabs[i].prefab.transform.position.x == col.transform.position.x)
            {
                Label_Prefabs[i].isPassed = true;
            }
            else
            {
                Label_Prefabs[i].isPassed = false;
            }
        }
    }

    void OnPlayerScored()
    {
        for (int i = 0; i < Label_Prefabs.Length; i++)
        {
            if (Label_Prefabs[i].isPassed)
            {
                /*
                if(Label_Prefabs[i].name == "china")
                {
                    impeach.PlaySound(Label_Prefabs[i].name);
                }
                if (Label_Prefabs[i].name == "immigrants")
                {
                    impeach.PlaySound(Label_Prefabs[i].name);
                }
                if (Label_Prefabs[i].name == "putin")
                {
                    impeach.PlaySound(Label_Prefabs[i].name);
                }
                if (Label_Prefabs[i].name == "comey")
                {
                    impeach.PlaySound(Label_Prefabs[i].name);
                }
                if (Label_Prefabs[i].name == "collusion")
                {
                    impeach.PlaySound(Label_Prefabs[i].name);
                }
                if (Label_Prefabs[i].name == "cnn")
                {
                    impeach.PlaySound(Label_Prefabs[i].name);
                }
                if (Label_Prefabs[i].name == "kkk")
                {
                    impeach.PlaySound(Label_Prefabs[i].name);
                }
               */
                impeach.PlaySound(Label_Prefabs[i].name);

            }
        }
    }
    /*
    string LabelName()
    {
        string thing = " ";
        for(int i = 0; i < Label_Prefabs.Length; i++)
        {
            if(Label_Prefabs[i].isPassed == true)
            {

                //Label_Prefabs[i].isPassed = false;

                thing = Label_Prefabs[i].name.ToString();
                
            }
        }
        return thing;
    }
    */


}
