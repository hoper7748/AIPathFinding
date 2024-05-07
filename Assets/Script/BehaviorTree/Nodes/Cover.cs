using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BT
{
    public class Cover : MonoBehaviour
    {
        [SerializeField] private Transform[] coverSpots;

        public Rect MeshSize;

        public Transform[] GetCoverSpots()
        {
            return coverSpots;
        }
        private void Awake()
        {
            Mesh cols = GetComponent<MeshFilter>().mesh;
            Vector3 offset = new Vector3(cols.bounds.size.x, cols.bounds.size.y, cols.bounds.size.z);
            Vector3 center = cols.bounds.center + transform.position;
            MeshSize.Set((center.x + offset.x) * 0.5f, (center.z + offset.z) * 0.5f, (center.x - offset.x) * 0.5f, (center.z - offset.z) * 0.5f);

        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
