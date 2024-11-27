using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MonsterDataLoader loader = new MonsterDataLoader("MonsterData");
        Debug.Log(loader.Get(0).Attack);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
