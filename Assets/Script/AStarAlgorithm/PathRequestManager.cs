using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public class PathRequestManager : MonoBehaviour
    {

        Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
        PathRequest currentPathRequest;

        Pathfinding Pathfinding;

        bool isProcessingPath;

        static PathRequestManager instance;

        private void Awake()
        {
            instance = this;
            Pathfinding = GetComponent<Pathfinding>();
        }

        public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
        {
            PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
            instance.pathRequestQueue.Enqueue(newRequest);
            instance.TryProcessNext();
        }

        void TryProcessNext()
        {
            if (!isProcessingPath && pathRequestQueue.Count > 0)
            {
                currentPathRequest = pathRequestQueue.Dequeue();
                isProcessingPath = true;
                Pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
            }
        }

        public void FinishedProcessingPath(Vector3[] path, bool success)
        {
            currentPathRequest.callback(path, success);
            isProcessingPath = false;
            TryProcessNext();
        }

        // 지정한 위치의 노드에 이동할 수 있는지 체크
        public static bool IsMovementPoint(Vector3 point)
        {
            // 포인트 까지 이동이 가능한가?.
            return instance.Pathfinding.IsMovementPoint(point) ? true : false;
        }


    }

}
