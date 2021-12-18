using UnityEngine;

namespace Profighter.Client.Utils
{
    public class PositionUtils
    {
        // Define Infinite (Using INT_MAX
        // caused overflow problems)
        static int INF = 10000;

        // Given three collinear points p, q, r,
        // the function checks if point q lies
        // on line segment 'pr'
        public static bool IsOnSegment(Vector3 p, Vector3 q, Vector3 r)
        {
            if (q.x <= Mathf.Max(p.x, r.x) &&
                q.x >= Mathf.Min(p.x, r.x) &&
                q.z <= Mathf.Max(p.z, r.z) &&
                q.z >= Mathf.Min(p.z, r.z))
            {
                return true;
            }

            return false;
        }

        // To find orientation of ordered triplet (p, q, r).
        // The function returns following values
        // 0 --> p, q and r are collinear
        // 1 --> Clockwise
        // 2 --> Counterclockwise
        public static int GetOrientation(Vector3 p, Vector3 q, Vector3 r)
        {
            int val = (int)((q.z - p.z) * (r.x - q.x) -
                            (q.x - p.x) * (r.z - q.z));

            if (val == 0)
            {
                return 0; // collinear
            }

            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }

        // The function that returns true if
        // line segment 'p1q1' and 'p2q2' intersect.
        public static bool DoLineSegmentsIntersect(Vector3 p1, Vector3 q1, Vector3 p2, Vector3 q2)
        {
            // Find the four orientations needed for
            // general and special cases
            int o1 = GetOrientation(p1, q1, p2);
            int o2 = GetOrientation(p1, q1, q2);
            int o3 = GetOrientation(p2, q2, p1);
            int o4 = GetOrientation(p2, q2, q1);

            // General case
            if (o1 != o2 && o3 != o4)
            {
                return true;
            }

            // Special Cases
            // p1, q1 and p2 are collinear and
            // p2 lies on segment p1q1
            if (o1 == 0 && IsOnSegment(p1, p2, q1))
            {
                return true;
            }

            // p1, q1 and p2 are collinear and
            // q2 lies on segment p1q1
            if (o2 == 0 && IsOnSegment(p1, q2, q1))
            {
                return true;
            }

            // p2, q2 and p1 are collinear and
            // p1 lies on segment p2q2
            if (o3 == 0 && IsOnSegment(p2, p1, q2))
            {
                return true;
            }

            // p2, q2 and q1 are collinear and
            // q1 lies on segment p2q2
            if (o4 == 0 && IsOnSegment(p2, q1, q2))
            {
                return true;
            }

            // Doesn't fall in any of the above cases
            return false;
        }

        // Returns true if the point p lies
        // inside the polygon[] with n vertices
        public static bool IsInside(Vector3[] polygon, int n, Vector3 point)
        {
            // There must be at least 3 vertices in polygon[]
            if (n < 3)
            {
                return false;
            }

            // Create a point for line segment from p to infinite
            Vector3 extreme = new Vector3(INF, point.y, point.z);

            // Count intersections of the above line
            // with sides of polygon
            int count = 0, i = 0;
            do
            {
                int next = (i + 1) % n;

                // Check if the line segment from 'p' to
                // 'extreme' intersects with the line
                // segment from 'polygon[i]' to 'polygon[next]'
                if (DoLineSegmentsIntersect(polygon[i],
                    polygon[next], point, extreme))
                {
                    // If the point 'p' is collinear with line
                    // segment 'i-next', then check if it lies
                    // on segment. If it lies, return true, otherwise false
                    if (GetOrientation(polygon[i], point, polygon[next]) == 0)
                    {
                        return IsOnSegment(polygon[i], point,
                            polygon[next]);
                    }

                    count++;
                }

                i = next;
            } while (i != 0);

            // Return true if count is odd, false otherwise
            return (count % 2 == 1); // Same as (count%2 == 1)
        }
    }
}