using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BT
{
    public class Cover : MonoBehaviour
    {
        [SerializeField] private Transform[] coverSpots;
        
        public Transform[] GetCoverSpots()
        {
            return coverSpots;
        }
        private void Awake()
        {

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
