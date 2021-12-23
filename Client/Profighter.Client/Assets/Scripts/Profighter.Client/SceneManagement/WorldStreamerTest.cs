using System;
using Profighter.Client.Utils;
using UnityEngine;

namespace Profighter.Client.SceneManagement
{
    public class WorldStreamerTest : MonoBehaviour
    {
        private void Start()
        {
            Vector3[] polygon1 =
            {
                new Vector3(0, 0),
                new Vector3(10, 0),
                new Vector3(10, 10),
                new Vector3(0, 10)
            };
            int n = polygon1.Length;
            Vector3 p = new Vector3(20, 20);
            if (PositionUtils.IsInside(polygon1, n, p))
            {
                //Debug.Log("Yes");
            }
            else
            {
                //Debug.Log("No");
            }

            p = new Vector3(5, 5);
            if (PositionUtils.IsInside(polygon1, n, p))
            {
                //Debug.Log("Yes");
            }
            else
            {
                //Debug.Log("No");
            }

            Vector3[] polygon2 =
            {
                new Vector3(0, 0),
                new Vector3(5, 5),
                new Vector3(5, 0)
            };
            p = new Vector3(3, 3);
            n = polygon2.Length;
            if (PositionUtils.IsInside(polygon2, n, p))
            {
                //Debug.Log("Yes");
            }
            else
            {
                //Debug.Log("No");
            }

            p = new Vector3(5, 1);
            if (PositionUtils.IsInside(polygon2, n, p))
            {
                //Debug.Log("Yes");
            }
            else
            {
                //Debug.Log("No");
            }

            p = new Vector3(8, 1);
            if (PositionUtils.IsInside(polygon2, n, p))
            {
                //Debug.Log("Yes");
            }
            else
            {
                //Debug.Log("No");
            }

            Vector3[] polygon3 =
            {
                new Vector3(0, 0),
                new Vector3(10, 0),
                new Vector3(10, 10),
                new Vector3(0, 10)
            };
            p = new Vector3(-1, 10);
            n = polygon3.Length;
            if (PositionUtils.IsInside(polygon3, n, p))
            {
                ///Debug.Log("Yes");
            }
            else
            {
                //Debug.Log("No");
            }
        }
    }
}