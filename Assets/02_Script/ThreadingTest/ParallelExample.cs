using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class ParallelExample : MonoBehaviour
{
    float curTimer = 0;
    public float tempTimer = 10f;
    // Start is called before the first frame update
    void Start()
    {

        Parallel.For(0, 5000, (i) => {
            Debug.Log($"{this.name} : Hello World! Threading");
        });
    }

    // Update is called once per frame
    void Update()
    {
        //curTimer += Time.deltaTime;
        //if(curTimer > tempTimer)
        //{
        //    Hello();
        //    curTimer = 0;
        //}
    }

    public void Hello()
    {

        //for(int i = 0; i < 5000; i++)
        //{
        //    Debug.Log($"{this.name} : Hello World!");
        //}
    }
}
