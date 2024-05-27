using UnityEngine;

//[]
public class Bullet : MonoBehaviour
{
    public Transform Oner;
    //Rigidbody rigidbody;
    int damage = 0;
    public float speed = 0;
    Vector3 directon;

    public Transform getOner
    {
        get { return Oner; }
    }

    private void Start()
    {
        damage = damage == 0 ? 10 : damage;
    }

    public void ShootTarget(Transform oner, Vector3 target, Vector3 origin, float time = 0.5f, float height = 1.5f)
    {
        Oner = oner;
        directon = (target- origin).normalized;
    }

    private void Update()
    {
        transform.Translate(directon * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter");
        if (other.transform == Oner)
            return;
        if (other.CompareTag("Player") )
        {
            other.GetComponent<BehaviourTree.AIAgent>().GetDamage(Oner);
        }

        Destroy(this.gameObject);
    }
}
