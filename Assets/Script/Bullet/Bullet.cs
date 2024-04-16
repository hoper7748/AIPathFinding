using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

//[]
public class Bullet : MonoBehaviour
{
    Transform Oner;
    Rigidbody rigidbody;
    int damage = 0;

    public Transform getOner
    {
        get { return Oner; }
    }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        if (rigidbody == null)
        {
            this.AddComponent<Rigidbody>();
            rigidbody = GetComponent<Rigidbody>();
        }
        damage = damage == 0 ? 10 : damage;
    }

    public void ShootTarget(Transform oner, Vector3 target, Vector3 origin, float time, float height = 1.5f)
    {
        Oner = oner;
        if (rigidbody == null)
            rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = EnemyAI.CaculateVelocity(target, origin, time, height);
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"AA = {other.name}");
        if (other.transform == Oner)
            return;
        if (other.CompareTag("Player") )
        {
            other.GetComponent<EnemyAI>().GetDamage();
        }
            Destroy(this.gameObject);
    }
}
