using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace pathFinding
{
    public struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;
        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
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

        //Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
        //PathRequest currentPathRequest;

        Queue<PathResult> results = new Queue<PathResult>();
        


        Pathfinding pathfinding;

        //bool isProcessingPath;

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
            //originalRequest.callback(path, success);
            //PathResult result = new PathResult(path, success, originalRequest.callback);
            lock (results)
            {
                results.Enqueue(result);
            }
        }


        //public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
        //{
        //    PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        //    instance.pathRequestQueue.Enqueue(newRequest);
        //    instance.TryProcessNext();
        //}

        //void TryProcessNext()
        //{
        //    if (!isProcessingPath && pathRequestQueue.Count > 0)
        //    {
        //        currentPathRequest = pathRequestQueue.Dequeue();
        //        isProcessingPath = true;
        //        Pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        //    }
        //}

        //public void FinishedProcessingPath(Vector3[] path, bool success)
        //{
        //    currentPathRequest.callback(path, success);
        //    isProcessingPath = false;
        //    TryProcessNext();
        //}

        // 지정한 위치의 노드에 이동할 수 있는지 체크
        public static bool IsMovementPoint(Vector3 point)
        {
            // 포인트 까지 이동이 가능한가?.
            return instance.pathfinding.IsMovementPoint(point) ? true : false;
        }


    }

}
