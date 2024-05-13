using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace pathFinding
{
    //public enum DirectionWeight
    //{
    //    None = 0,
    //    Up = 1,
    //    Down = 2,
    //    Right,
    //    Left
    //}
    public struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;
        //public DirectionWeight direction;
        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
            //direction = _direction;
        }
    }
    public struct PathResult
    {
        public Vector3[] path;
        public bool success;
        public Action<Vector3[], bool> callback;

        public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
        {
            this.path = path;
            this.success = success;
            this.callback = callback;
        }
    }

    public class PathRequestManager : MonoBehaviour
    {
        Queue<PathResult> results = new Queue<PathResult>();
        
        Pathfinding pathfinding;


        static PathRequestManager instance;

        private void Awake()
        {
            instance = this;
            pathfinding = GetComponent<Pathfinding>();
        }

        private void Update()
        {
            if(results.Count > 0)
            {
                int itemsInQueue = results.Count;
                lock(results)
                {
                    for(int i =0; i < itemsInQueue; i++)
                    {
                        PathResult result = results.Dequeue();
                        result.callback(result.path, result.success);
                    }
                }
            }
        }

        public static void RequestPath(PathRequest request)
        {
            ThreadStart threadStart = delegate
            {
                instance.pathfinding.FindPath(request, instance.FinishedProcessingPath);
            };
            threadStart.Invoke();

        }

        public void FinishedProcessingPath(PathResult result)
        {
            lock (results)
            {
                results.Enqueue(result);
            }
        }

        // 지정한 위치의 노드에 이동할 수 있는지 체크
        public static bool IsMovementPoint(Vector3 point)
        {
            // 포인트 까지 이동이 가능한가?.
            return instance.pathfinding.IsMovementPoint(point) ? true : false;
        }


    }

}
