using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData : MonoBehaviour
{
    public enum BuildingType
    {
        Academic,
        Sports,
        Residential,
        Hostel,
        GirlsHostel,
        BoysHostel,
        Halls,
        Others,
    }

    [System.Serializable]
    public class Building
    {
        public string name;
        public BuildingType buildingType;
        public float area_sqm;
    }

    public Building[] scheme;
}
