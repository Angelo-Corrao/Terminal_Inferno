using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Point class data container
[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(Image))]
public class Point : MonoBehaviour
{
    [HideInInspector] public Image image;
    [HideInInspector] public bool connectable = true;
    public int indexRow = -1;
    public int indexColumn = -1; // Index based on column order: start from right to left
    public Data.RouteColor color = Data.RouteColor.NULL;
    public Data.PointType pointType = Data.PointType.STANDARD;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void ResetPoint()
    {
        connectable = true;
        color = Data.RouteColor.NULL;
        pointType = Data.PointType.STANDARD;
        image.color = Data.standardPointColor;// Data.routeColorAssociation[Data.RouteColor.NULL];
    }
}
