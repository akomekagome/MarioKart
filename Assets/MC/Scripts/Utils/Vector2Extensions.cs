using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Utils
{

    public static class Vector2Extensions
    {
        public static Vector2 SetX(this Vector2 origin, float X)
        {
            return new Vector2(X, origin.y);
        }

        public static Vector2 SetY(this Vector2 origin, float Y)
        {
            return new Vector2(origin.x, Y);
        }

        public static Vector2 AddSetX(this Vector2 origin, float X)
        {
            return new Vector2(origin.x + X, origin.y);
        }

        public static Vector2 AddSetY(this Vector2 origin, float Y)
        {
            return new Vector2(origin.x, origin.y + Y);
        }

        public static bool LineSegmentsIntersection(
        Vector2 p1,
        Vector2 p2,
        Vector2 p3,
        Vector2 p4,
        out Vector2 intersection)
        {
            intersection = Vector2.zero;

            var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

            if (Mathf.Approximately(d,0f))
            {
                return false;
            }

            var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
            var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

            intersection.x = p1.x + u * (p2.x - p1.x);
            intersection.y = p1.y + u * (p2.y - p1.y);

            return true;
        }
    }
}
