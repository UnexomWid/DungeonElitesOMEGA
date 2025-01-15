using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Pathfinding;
using UnityEngine.SceneManagement;
using System;

public class MapGeneration : MonoBehaviour
{
    public GameObject[] pathRooms;
    public GameObject[] bifurcations;
    public GameObject[] changeWay;
    public GameObject[] endRooms;
    public GameObject[] startPoints;
    public GameObject parentObj;
    public GameObject deadEnd;
    public int bifurcateChance;
    public int changeWayChance;
    public int endWayChance;
    public int voidChance;
    public int minRooms;
    public int maxRooms;
    public GameObject keyRoom;
    public GameObject chestRoom;
    public GameObject bossRoom;
    public GameObject rewardRoom;
    public GameObject shop;
    public GameObject wizard;
    bool generated = false;
    int voidRooms;

    public float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }

    float GetChance(float x)
    {
        float b = 1.2f;
        float m = 6.9f;

        float result = (Mathf.Pow(b, x) - 1) / (Mathf.Pow(b, m) - 1);

        float maxValue = (Mathf.Pow(b, 6) - 1) / (Mathf.Pow(b, m) - 1);

        result = Map(result, 0, maxValue, 0, 1);

        return result;
    }

    //Varianta 2
    /*float GetChance(float x)
    {
        x = Map(x, 0, 7, 0, 10);

        float b = 1.2f;
        float m = 6.9f;

        float result = (Mathf.Pow(b, x) - 1) / (Mathf.Pow(b, m) - 1);

        result = Map(result, 0, 2.0615f, 0, 1);

        return result;
    }*/

    public void Generate()
    {
        if (generated == false)
        {
            List<GameObject> hallMobs = new List<GameObject>();

            foreach(GameObject hall in pathRooms)
            {
                if (hall.GetComponent<room>().hasMobs)
                    hallMobs.Add(hall);
            }

            List<GameObject> newPathRoomsObj = new List<GameObject>(pathRooms);

            foreach(GameObject hall in hallMobs)
            {
                newPathRoomsObj.Remove(hall);
            }

            pathRooms = newPathRoomsObj.ToArray();

            List<char> roomTypes = new List<char>();
            for (int i = 0; i < voidChance; i++)
                roomTypes.Add('v');
            for (int i = 0; i < bifurcateChance; i++)
                roomTypes.Add('b');
            for (int i = 0; i < changeWayChance; i++)
                roomTypes.Add('c');
            if (hallMobs.Count != 0)
                for (int i = 0; i < 4; i++)
                {
                    roomTypes.Add('h');
                    Debug.Log("addedH");
                }



            generated = true;
            List<GameObject> startPoints = new List<GameObject>();
            foreach (GameObject startPoint in this.startPoints)
                startPoints.Add(startPoint);
            bool spawnedBossRoom = false;
            bool spawnedRewardRoom = false;
            bool spawnedKeyRoom = false;
            bool spawnedChestRoom = false;
            List<GameObject> chestRooms = new List<GameObject>();
            for (int i = 0; i < maxRooms; i++)
            {
                try
                {
                    if (startPoints[i].name.Contains("Unused") == false)
                    {
                        int mobRoomChance = startPoints[i].GetComponentInParent<room>().mobRoomChance;

                        bool spawnMobRoom = UnityEngine.Random.value > 1 - GetChance(mobRoomChance);
                        if (mobRoomChance == 6)
                            spawnMobRoom = true;

                        List<GameObject> bifurcationList = new List<GameObject>(bifurcations);
                        List<GameObject> changeWayList = new List<GameObject>(changeWay);
                        List<GameObject> hallMobsList = new List<GameObject>(hallMobs);
                        List<GameObject> newPathRooms = new List<GameObject>(this.pathRooms);

                        List<char> charList = new List<char>(roomTypes);

                        _start:

                        GameObject pathRoom = null;

                        bool isVoid = false;

                        char roomType = charList[UnityEngine.Random.Range(1, 999999) % charList.Count];

                        GameObject usedRoom = null;

                        if (spawnMobRoom)
                        {
                            switch(roomType)
                            {
                                case 'v':
                                    pathRoom = Instantiate(voidRoom);
                                    isVoid = true;
                                    break;
                                case 'b':
                                    int bifucationType = UnityEngine.Random.Range(0, 999999) % bifurcations.Length;
                                    usedRoom = bifurcations[bifucationType];
                                    pathRoom = Instantiate(usedRoom);
                                    break;
                                case 'c':
                                    int changeWayType = UnityEngine.Random.Range(0, 999999) % changeWay.Length;
                                    usedRoom = changeWay[changeWayType];
                                    pathRoom = Instantiate(usedRoom);
                                    break;
                                case 'h':
                                    Debug.Log("spawnedH");
                                    int hallMobsType = UnityEngine.Random.Range(0, 999999) % hallMobs.Count;
                                    usedRoom = hallMobs[hallMobsType];
                                    pathRoom = Instantiate(usedRoom);
                                    break;
                            }

                            pathRoom.GetComponent<room>().mobRoomChance = 0;

                        }
                        else
                        {
                            if (UnityEngine.Random.Range(1, 999999) % endWayChance == 0 && endRooms.Length != 0)
                            {
                                int type = UnityEngine.Random.Range(0, 999999) % endRooms.Length;
                                pathRoom = Instantiate(endRooms[type]);
                            }
                            else
                            {
                                int type = UnityEngine.Random.Range(0, 999999) % newPathRooms.Count;
                                pathRoom = Instantiate(newPathRooms[type]);
                            }
                            if (mobRoomChance < 6)
                                pathRoom.GetComponent<room>().mobRoomChance = mobRoomChance + 1;
                        }

                        pathRoom.transform.SetParent(parentObj.transform);
                        pathRoom.transform.localScale = new Vector3(1, 1, 1);
                        pathRoom.transform.localEulerAngles = new Vector3(0, 0, startPoints[i].transform.localEulerAngles.z);
                        Vector3 offset = Quaternion.Euler(0, 0, startPoints[i].transform.localEulerAngles.z) * pathRoom.GetComponent<room>().endPoint.transform.localPosition;
                        pathRoom.GetComponent<room>().endPoint.transform.position = startPoints[i].transform.position;
                        pathRoom.transform.position = pathRoom.GetComponent<room>().endPoint.transform.position - offset * 100;
                        pathRoom.GetComponent<room>().endPoint.transform.localPosition = offset;
                        pathRoom.SetActive(true);
                        List<thisisaroom> boxes = FindObjectsOfType<thisisaroom>().ToList();
                        boxes.Remove(pathRoom.GetComponent<thisisaroom>());
                        boxes.Remove(startPoints[i].GetComponentInParent<thisisaroom>());
                        bool ok = true;

                        GameObject parent = startPoints[i].transform.parent.gameObject;

                        foreach (thisisaroom box in boxes)
                        {


                            if (box.gameObject.GetComponent<SpriteRenderer>().bounds.Intersects(pathRoom.GetComponent<SpriteRenderer>().bounds) && box.gameObject != pathRoom)
                            {
                                Debug.Log(box.gameObject.transform.name + " " + pathRoom.transform.name, box);
                                ok = false;
                                break;
                            }

                        }
                        if (ok && ((isVoid && i > 60) || isVoid == false))
                        {
                            if (isVoid)
                            {
                                
                                voidRooms++;

                                List<char> toRemove = new List<char>();

                                foreach (char room in roomTypes)
                                {
                                    if (room == 'v')
                                    {
                                        toRemove.Add(room);
                                    }
                                }

                                foreach (char character in toRemove)
                                    roomTypes.Remove(character);
                            }
                            foreach (GameObject startPoint in pathRoom.GetComponent<room>().startPoints)
                            {
                                startPoint.transform.localEulerAngles = startPoint.transform.localEulerAngles + startPoints[i].transform.localEulerAngles;
                                startPoints.Add(startPoint);
                                if (pathRoom.name.ToLower().Contains("chestroom"))
                                {
                                    chestRooms.Add(pathRoom);
                                }
                            }
                        }
                        else
                        {
                            pathRoom.SetActive(false);
                            Destroy(pathRoom);
                            if(isVoid)
                            {
                                List<char> toRemove = new List<char>();

                                foreach (char room in charList)
                                {
                                    if (room == 'v')
                                    {
                                        toRemove.Add(room);
                                    }
                                }

                                foreach (char character in toRemove)
                                    charList.Remove(character);
                            }

                            if (bifurcationList.Contains(usedRoom))
                                bifurcationList.Remove(usedRoom);
                            if (changeWayList.Contains(usedRoom))
                                changeWayList.Remove(usedRoom);
                            if (hallMobsList.Contains(usedRoom))
                                hallMobsList.Remove(usedRoom);

                            if (spawnMobRoom && (bifurcationList.Count > 0 || changeWayList.Count > 0 || hallMobsList.Count > 0))
                                goto _start;
                        }
                        startPoints[i].name += " " + ok;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.ToString());
                    break;
                }


            }
            List<GameObject> nonEndedStartPoints = new List<GameObject>();
            foreach (GameObject startPoint in startPoints)
            {
                if (startPoint.name.Contains("True") == false && startPoint.name.Contains("Unused") == false)
                {
                    nonEndedStartPoints.Add(startPoint);
                }
            }
            int attempts = 0;
            while (nonEndedStartPoints.Count != 0 && spawnedBossRoom == false)
            {
                attempts++;
                int index = UnityEngine.Random.Range(1, 999999) % nonEndedStartPoints.Count;
                GameObject point = nonEndedStartPoints[index];
                GameObject pathRoom = null;
                pathRoom = Instantiate(bossRoom);
                pathRoom.transform.SetParent(parentObj.transform);
                pathRoom.transform.localScale = new Vector3(1, 1, 1);
                pathRoom.transform.localEulerAngles = new Vector3(0, 0, point.transform.localEulerAngles.z);
                Vector3 offset = Quaternion.Euler(0, 0, point.transform.localEulerAngles.z) * pathRoom.GetComponent<room>().endPoint.transform.localPosition;
                pathRoom.GetComponent<room>().endPoint.transform.position = point.transform.position;
                pathRoom.transform.position = pathRoom.GetComponent<room>().endPoint.transform.position - offset * 100;
                pathRoom.GetComponent<room>().endPoint.transform.localPosition = offset;
                pathRoom.SetActive(true);
                List<thisisaroom> boxes = FindObjectsOfType<thisisaroom>().ToList();
                boxes.Remove(pathRoom.GetComponent<thisisaroom>());
                boxes.Remove(point.GetComponentInParent<thisisaroom>());
                bool ok = true;
                GameObject parent = point.transform.parent.gameObject;

                foreach (thisisaroom box in boxes)
                {


                    if(box.gameObject.GetComponent<SpriteRenderer>().bounds.Intersects(pathRoom.GetComponent<SpriteRenderer>().bounds) && box.gameObject != pathRoom)
                            {
                        Debug.Log(box.gameObject.transform.name + " " + pathRoom.transform.name, box);
                        ok = false;
                        break;
                    }

                }
                if (ok == false)
                {
                    pathRoom.SetActive(false);
                    Destroy(pathRoom);
                    nonEndedStartPoints.RemoveAt(index);
                }
                else
                {
                    point.name = point.name.Split(' ')[0] + " True";
                    spawnedBossRoom = true;
                }
            }
            attempts = 0;
            while (nonEndedStartPoints.Count != 0 && spawnedRewardRoom == false)
            {
                attempts++;
                int index = UnityEngine.Random.Range(1, 999999) % nonEndedStartPoints.Count;
                GameObject point = nonEndedStartPoints[index];
                GameObject pathRoom = null;
                pathRoom = Instantiate(rewardRoom);
                pathRoom.transform.SetParent(parentObj.transform);
                pathRoom.transform.localScale = new Vector3(1, 1, 1);
                pathRoom.transform.localEulerAngles = new Vector3(0, 0, point.transform.localEulerAngles.z);
                Vector3 offset = Quaternion.Euler(0, 0, point.transform.localEulerAngles.z) * pathRoom.GetComponent<room>().endPoint.transform.localPosition;
                pathRoom.GetComponent<room>().endPoint.transform.position = point.transform.position;
                pathRoom.transform.position = pathRoom.GetComponent<room>().endPoint.transform.position - offset * 100;
                pathRoom.GetComponent<room>().endPoint.transform.localPosition = offset;
                pathRoom.SetActive(true);
                List<thisisaroom> boxes = FindObjectsOfType<thisisaroom>().ToList();
                boxes.Remove(pathRoom.GetComponent<thisisaroom>());
                boxes.Remove(point.GetComponentInParent<thisisaroom>());
                bool ok = true;
                GameObject parent = point.transform.parent.gameObject;

                foreach (thisisaroom box in boxes)
                {


                    if (box.gameObject.GetComponent<SpriteRenderer>().bounds.Intersects(pathRoom.GetComponent<SpriteRenderer>().bounds) && box.gameObject != pathRoom)
                    {
                        Debug.Log(box.gameObject.transform.name + " " + pathRoom.transform.name, box);
                        ok = false;
                        break;
                    }

                }
                if (ok == false)
                {
                    pathRoom.SetActive(false);
                    Destroy(pathRoom);
                    nonEndedStartPoints.RemoveAt(index);
                }
                else
                {
                    point.name = point.name.Split(' ')[0] + " True";
                    spawnedRewardRoom = true;
                }
            }

            GameObject door7 = null;
            GameObject chestRoom = null;

            attempts = 0;
            while (nonEndedStartPoints.Count != 0 && spawnedChestRoom == false)
            {
                attempts++;
                int index = UnityEngine.Random.Range(1, 999999) % nonEndedStartPoints.Count;
                GameObject point = nonEndedStartPoints[index];
                GameObject pathRoom = null;
                pathRoom = Instantiate(this.chestRoom);
                pathRoom.transform.SetParent(parentObj.transform);
                pathRoom.transform.localScale = new Vector3(1, 1, 1);
                pathRoom.transform.localEulerAngles = new Vector3(0, 0, point.transform.localEulerAngles.z);
                Vector3 offset = Quaternion.Euler(0, 0, point.transform.localEulerAngles.z) * pathRoom.GetComponent<room>().endPoint.transform.localPosition;
                pathRoom.GetComponent<room>().endPoint.transform.position = point.transform.position;
                pathRoom.transform.position = pathRoom.GetComponent<room>().endPoint.transform.position - offset * 100;
                pathRoom.GetComponent<room>().endPoint.transform.localPosition = offset;
                pathRoom.SetActive(true);
                List<thisisaroom> boxes = FindObjectsOfType<thisisaroom>().ToList();
                boxes.Remove(pathRoom.GetComponent<thisisaroom>());
                boxes.Remove(point.GetComponentInParent<thisisaroom>());
                bool ok = true;
                GameObject parent = point.transform.parent.gameObject;

                foreach (thisisaroom box in boxes)
                {


                    if(box.gameObject.GetComponent<SpriteRenderer>().bounds.Intersects(pathRoom.GetComponent<SpriteRenderer>().bounds) && box.gameObject != pathRoom)
                            {
                        Debug.Log(box.gameObject.transform.name + " " + pathRoom.transform.name, box);
                        ok = false;
                        break;
                    }

                }
                if (ok == false)
                {
                    pathRoom.SetActive(false);
                    Destroy(pathRoom);
                    nonEndedStartPoints.RemoveAt(index);
                }
                else
                {
                    chestRoom = pathRoom;
                    foreach(Transform child in pathRoom.transform)
                    {
                        if (child.transform.name == "door (7)")
                            door7 = child.gameObject;
                    }    
                    point.name = point.name.Split(' ')[0] + " True";
                    spawnedChestRoom = true;
                }
            }

            attempts = 0;
            while (nonEndedStartPoints.Count != 0 && spawnedKeyRoom == false)
            {
                attempts++;
                int index = UnityEngine.Random.Range(1, 999999) % nonEndedStartPoints.Count;
                GameObject point = nonEndedStartPoints[index];
                GameObject pathRoom = null;
                pathRoom = Instantiate(keyRoom);
                pathRoom.transform.SetParent(parentObj.transform);
                pathRoom.transform.localScale = new Vector3(1, 1, 1);
                pathRoom.transform.localEulerAngles = new Vector3(0, 0, point.transform.localEulerAngles.z);
                Vector3 offset = Quaternion.Euler(0, 0, point.transform.localEulerAngles.z) * pathRoom.GetComponent<room>().endPoint.transform.localPosition;
                pathRoom.GetComponent<room>().endPoint.transform.position = point.transform.position;
                pathRoom.transform.position = pathRoom.GetComponent<room>().endPoint.transform.position - offset * 100;
                pathRoom.GetComponent<room>().endPoint.transform.localPosition = offset;
                pathRoom.SetActive(true);
                List<thisisaroom> boxes = FindObjectsOfType<thisisaroom>().ToList();
                boxes.Remove(pathRoom.GetComponent<thisisaroom>());
                boxes.Remove(point.GetComponentInParent<thisisaroom>());
                bool ok = true;
                GameObject parent = point.transform.parent.gameObject;

                foreach (thisisaroom box in boxes)
                {


                    if (box.gameObject.GetComponent<SpriteRenderer>().bounds.Intersects(pathRoom.GetComponent<SpriteRenderer>().bounds) && box.gameObject != pathRoom)
                    {
                        Debug.Log(box.gameObject.transform.name + " " + pathRoom.transform.name, box);
                        ok = false;
                        break;
                    }

                }
                if (ok == false)
                {
                    pathRoom.SetActive(false);
                    Destroy(pathRoom);
                    nonEndedStartPoints.RemoveAt(index);
                }
                else
                {
                    pathRoom.GetComponent<LockRoom>().chestRoom = chestRoom;
                    pathRoom.GetComponent<LockRoom>().chestRoomDoor = door7;
                    point.name = point.name.Split(' ')[0] + " True";
                    spawnedKeyRoom = true;
                }
            }

            foreach (GameObject startPoint in startPoints)
            {
                if (startPoint.name.Contains("True") == false && startPoint.name.Contains("Unused") == false)
                {
                    Debug.Log(startPoint.name, startPoint);

                    startPoint.name = startPoint.name.Split(' ')[0];
                    GameObject pathRoom = null;
                    int type = UnityEngine.Random.Range(0, 999999) % endRooms.Length;
                    if(chestRooms.Count < 3)
                    {
                        for (int i = 0; i < endRooms.Length; i++)
                            if (endRooms[i].transform.name.ToLower().Contains("chest"))
                            {
                                type = i;
                                break;
                            }
                    }
                    pathRoom = Instantiate(endRooms[type]);
                    pathRoom.transform.SetParent(parentObj.transform);
                    pathRoom.transform.localScale = new Vector3(1, 1, 1);
                    pathRoom.transform.localEulerAngles = new Vector3(0, 0, startPoint.transform.localEulerAngles.z);
                    Vector3 offset = Quaternion.Euler(0, 0, startPoint.transform.localEulerAngles.z) * pathRoom.GetComponent<room>().endPoint.transform.localPosition;
                    pathRoom.GetComponent<room>().endPoint.transform.position = startPoint.transform.position;
                    pathRoom.transform.position = pathRoom.GetComponent<room>().endPoint.transform.position - offset * 100;
                    pathRoom.GetComponent<room>().endPoint.transform.localPosition = offset;
                    pathRoom.SetActive(true);
                    List<thisisaroom> boxes = FindObjectsOfType<thisisaroom>().ToList();
                    boxes.Remove(pathRoom.GetComponent<thisisaroom>());
                    boxes.Remove(startPoint.GetComponentInParent<thisisaroom>());
                    bool ok = true;
                    GameObject parent = startPoint.transform.parent.gameObject;

                    foreach (thisisaroom box in boxes)
                    {


                        if (box.gameObject.GetComponent<SpriteRenderer>().bounds.Intersects(pathRoom.GetComponent<SpriteRenderer>().bounds) && box.gameObject != pathRoom)
                        {
                            Debug.Log(box.gameObject.transform.name + " " + pathRoom.transform.name + " " + box.gameObject.transform.position, box.gameObject);
                            ok = false;
                            break;
                        }

                    }
                    if (ok == false)
                    {
                        pathRoom.SetActive(false);
                        Destroy(pathRoom);
                    }
                    else
                    {
                        if (pathRoom.name.ToLower().Contains("chestroom"))
                        {
                            chestRooms.Add(pathRoom);
                        }
                    }
                    startPoint.name += " " + ok;
                }
            }
            foreach (GameObject startPoint in startPoints)
            {
                if (startPoint.name.Contains("True") == false || startPoint.name.Contains("Unused"))
                {
                    GameObject pathRoom = null;
                    pathRoom = Instantiate(deadEnd);
                    pathRoom.transform.SetParent(parentObj.transform);
                    pathRoom.transform.localScale = new Vector3(1, 1, 1);
                    pathRoom.transform.localEulerAngles = new Vector3(0, 0, startPoint.transform.localEulerAngles.z);
                    Vector3 offset = Quaternion.Euler(0, 0, startPoint.transform.localEulerAngles.z) * pathRoom.GetComponent<room>().endPoint.transform.localPosition;
                    pathRoom.GetComponent<room>().endPoint.transform.position = startPoint.transform.position;
                    pathRoom.transform.position = pathRoom.GetComponent<room>().endPoint.transform.position - offset * 100;
                    pathRoom.GetComponent<room>().endPoint.transform.localPosition = offset;
                    pathRoom.SetActive(true);
                }
            }

            if ((startPoints.Count < minRooms || spawnedBossRoom == false || spawnedRewardRoom == false || chestRooms.Count < 3 || voidRooms != 1 || spawnedChestRoom == false || spawnedKeyRoom == false))
            //if ((startPoints.Count < minRooms || voidRooms != 1))
            {
                FindObjectOfType<DungeonData>().restarted = true;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                if (SceneManager.GetActiveScene().name != "Shops")
                    ServerNotification.instance.SendMessageToServer("User got into the " + SceneManager.GetActiveScene().name + " Dungeon.");

                GameObject shopRoom = chestRooms[UnityEngine.Random.Range(1, 9999999) % chestRooms.Count];
                chestRooms.Remove(shopRoom);
                GameObject wizardRoom = chestRooms[UnityEngine.Random.Range(1, 9999999) % chestRooms.Count];
                chestRooms.Remove(wizardRoom);

                shop.transform.position = shopRoom.transform.position;

                foreach (Transform child in wizardRoom.transform)
                {
                    if (child.name.Contains("npcSpawn"))
                    {
                        wizard.transform.position = child.transform.position;
                        wizard.transform.eulerAngles = new Vector3(0, 0, wizardRoom.transform.eulerAngles.z + 90);

                        if(wizard.transform.localEulerAngles.z == -180)
                        {
                            wizard.transform.localEulerAngles = Vector3.zero;
                            wizard.transform.localPosition = new Vector2(wizard.transform.localPosition.x, -0.375f);
                        }
                    }
                }

                shop.SetActive(true);

                wizard.SetActive(true);
            }
            parentObj.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            foreach (GameObject obj in FindObjectsOfType<GameObject>())
            {
                if (obj.name.Contains("torch"))
                {
                    if (UnityEngine.Random.Range(1, 9999999) % 10 == 0)
                    {
                        if (UnityEngine.Random.Range(1, 999999) % 2 == 0)
                        {
                            obj.GetComponent<SpriteRenderer>().sprite = brokenTorch;
                            obj.GetComponentInChildren<SpriteRenderer>().enabled = false;
                        }
                        else
                        {
                            obj.GetComponent<SpriteRenderer>().sprite = brokenTorch2;
                            obj.transform.position += (Vector3)(-obj.transform.up * new Vector2(0, 0.05f));
                            obj.transform.localEulerAngles = new Vector3(0, 0, obj.transform.localEulerAngles.z - 10);
                            obj.GetComponentInChildren<SpriteRenderer>().transform.localScale /= 2;

                        }
                    }
                }
            }
            float minY = 0f;
            float maxY = 0f;
            float minX = 0f;
            float maxX = 0f;
            foreach (thisisaroom room in FindObjectsOfType<thisisaroom>())
            {
                if (room.transform.position.x < minX)
                    minX = room.transform.position.x;
                if (room.transform.position.y < minY)
                    minY = room.transform.position.y;
                if (room.transform.position.x > maxX)
                    maxX = room.transform.position.x;
                if (room.transform.position.y > maxY)
                    maxY = room.transform.position.y;
            }

            float XDist = (maxX - minX) / 16f;
            float YDist = (maxY - minY) / 9f;

            if (XDist > YDist)
                FindObjectOfType<Megamap>().GetComponent<Camera>().orthographicSize = XDist * 16;
            else FindObjectOfType<Megamap>().GetComponent<Camera>().orthographicSize = YDist * 9;

            FindObjectOfType<Megamap>().gameObject.SetActive(false);

            GameObject electricity = null;

            foreach (GameObject obj in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
            {
                if (obj.transform.name.Contains("electricity (1)"))
                {
                    electricity = obj;
                    break;
                }
            }

            foreach (GameObject obj in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
            {
                if (obj.GetComponent<wizard>() != null)
                {
                    obj.GetComponent<wizard>().electricityBox.electricalBolt = electricity;
                }
            }
        }



    }
    public GameObject voidRoom;
    public Sprite brokenTorch;
    public Sprite brokenTorch2;
    public string[] mobRooms;
}
