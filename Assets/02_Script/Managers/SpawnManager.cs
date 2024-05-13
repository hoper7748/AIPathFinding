using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private List<GameObject> objs;
    // 스폰하는 기능
    // Start is called before the first frame update
    void Start()
    {
        GameObject obj;
        // 첫 스폰
        foreach (var atn in spawnPoints)
        {
            obj = Instantiate(prefab, atn.position, Quaternion.identity);
            objs.Add(obj);  
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
