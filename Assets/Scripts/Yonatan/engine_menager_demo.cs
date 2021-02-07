using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class engine_menager_demo : MonoBehaviour
{
    /* assume gear between 1 to 3, wind between -2 to 2*/
    int[] s = { 15, 45, 100 }; /* speeds that are used as new target speed, such that:
    2s[0] < 5s[2]
    s[1] < 2s[2]
    2s[0] < 5s[1]
    and s[2] > s[1] > s[0]
    */
    int[] eff = { 50, 20, 10 };
    static bool Sail_Open;
    float calc_consumption_rate(int gear_eff, int target_speed) { return (float)gear_eff / target_speed; }
    int calc_target_speed(bool Sail_Open, int gear, int wind_speed)
    {
        int Sail_constant = 55; //negative effect of speed when sail is open , choose a constant that is smaller that -gear2's speed//
            switch (wind_speed)
            {
                case -2:
                    if (Sail_Open) { return s[0] - Sail_constant; }
                    else { return s[0]; }
                case -1:
                    if (Sail_Open) { return s[System.Math.Min(gear,1)] - Sail_constant; }
                    else { return s[System.Math.Min(gear, 1)]; }
                case 1:
                if (Sail_Open)
                {
                    if(gear ==2 || gear == 1)
                    {
                        return s[2];
                    }
                    return s[0];
                }
                else { return s[gear - 1]; }
            case 2:
                if (Sail_Open) { return s[2]; }
                else { return s[gear - 1]; }
            case 0:
                return s[gear - 1];
            default:
                return -1; //this is not supposed to happen//
        }
        }
    }


    