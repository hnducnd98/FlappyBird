using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duc
{
    public class ObstacleSpawner : MonoBehaviour
    {
        [SerializeField] private float waitTime;
        [SerializeField] private GameObject obstaclePrefabs;
        [SerializeField] private Transform transPointDown;
        [SerializeField] private Transform transPointUp;

        [SerializeField] private List<Transform> lstTransObstacle = new List<Transform>();
        private float tempTime;
        private float maxDistanceObstacle;
        [SerializeField] private int indexObstacle = 0;
        private int indexMode = 0;
        private List<bool> lstModeEasy = new List<bool>();

        void Start()
        {
            tempTime = waitTime - Time.deltaTime;
            maxDistanceObstacle = Vector2.Distance(transPointDown.position, transPointUp.position) / 2;
            Debug.LogError("maxDistanceObstacle: " + maxDistanceObstacle);
            for (int i = 0; i < 3; i++)
            {
                lstModeEasy.Add(true);
            }
            for (int i = 0; i < 2; i++)
            {
                lstModeEasy.Add(false);
            }
        }

        void LateUpdate()
        {
            if (GameManager.Instance.GameState())
            {
                tempTime += Time.deltaTime;
                if (tempTime > waitTime)
                {
                    // Wait for some time, create an obstacle, then set wait time to 0 and start again
                    tempTime = 0;
                    if (lstTransObstacle.Count < 3)
                    {
                        GameObject pipeClone = Instantiate(obstaclePrefabs, transform.position, transform.rotation);
                        lstTransObstacle.Add(pipeClone.transform);
                    }
                    SetPositionObstacle(lstTransObstacle[indexObstacle], indexObstacle);
                    indexObstacle += 1;
                    if (indexObstacle >= 3)
                    {
                        indexObstacle = 0;
                    }
                }
            }
        }

        //void OnTriggerEnter2D(Collider2D col)
        //{
        //    if (col.gameObject.transform.parent != null)
        //    {
        //        Destroy(col.gameObject.transform.parent.gameObject);
        //    }
        //    else
        //    {
        //        Destroy(col.gameObject);
        //    }
        //}

        private void SetPositionObstacle(Transform transObstacle, int indexObstacle)
        {
            var beginPos = Vector3.zero;

            var isEassy = lstModeEasy[indexMode];
            if (lstTransObstacle.Count > 1)
            {
                var preIndexObstacle = indexObstacle == 0 ? lstTransObstacle.Count - 1 : indexObstacle - 1;
                var preTransObstacle = lstTransObstacle[preIndexObstacle];
                var yMinUp = preTransObstacle.position.y + maxDistanceObstacle / 2;
                var yMaxDown = preTransObstacle.position.y - maxDistanceObstacle / 2;
                if (isEassy)
                {
                    beginPos = ReturnEasyBeginPos(preTransObstacle.position);
                }
                else
                {
                    if (yMinUp < transPointUp.position.y && yMaxDown > transPointDown.position.y)
                    {
                        var yRandomUp = UnityEngine.Random.Range(yMinUp, Mathf.Min(yMinUp + maxDistanceObstacle / 2, transPointUp.position.y));
                        var yRandomDown = UnityEngine.Random.Range(Mathf.Max(yMaxDown - maxDistanceObstacle / 2, transPointDown.position.y), yMaxDown);
                        var yRandom = UnityEngine.Random.Range(0, 2) == 0 ? yRandomUp : yRandomDown;
                        beginPos.y = yRandom;
                    }
                    else if (yMinUp < transPointUp.position.y)
                    {
                        var yRandomUp = UnityEngine.Random.Range(yMinUp, Mathf.Min(yMinUp + maxDistanceObstacle / 2, transPointUp.position.y));
                        beginPos.y = yRandomUp;
                    }
                    else if (yMaxDown > transPointDown.position.y)
                    {
                        var yRandomDown = UnityEngine.Random.Range(Mathf.Max(yMaxDown - maxDistanceObstacle / 2, transPointDown.position.y), yMaxDown);
                        beginPos.y = yRandomDown;
                    }
                }
            }
            else
            {
                beginPos = ReturnEasyBeginPos(Vector3.zero);
            }

            beginPos.x = transform.position.x;
            transObstacle.position = beginPos;
            indexMode += 1;
            if (indexMode >= lstModeEasy.Count)
            {
                indexMode = 0;
            }
        }

        private Vector3 ReturnEasyBeginPos(Vector3 prePos)
        {
            var yMax = Mathf.Min(prePos.y + maxDistanceObstacle / 2, transPointUp.position.y);
            var yMin = Mathf.Max(prePos.y - maxDistanceObstacle / 2, transPointDown.position.y);
            var yObstacle = UnityEngine.Random.Range(yMin, yMax);
            return new Vector3(0, yObstacle, 0);
        }
    }
}
