using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static float calculateDistanceSqr(Vector3 from, Vector3 to) {
        return (from - to).sqrMagnitude;
    }

    public static int roundNumber(float number) {
        int intNumber = (int)number;
        if ((float)intNumber == number)
        {
            return intNumber;
        }
        return intNumber + 1;
    }
}
