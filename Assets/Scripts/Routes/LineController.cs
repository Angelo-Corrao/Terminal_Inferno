using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Data;

public class LineController : MonoBehaviour
{
    // public var
    public List<Point> points;
    public RouteColor color;
    public int maxAutonomy = 0; // max points can be added to the list
    public bool active = false;
    public bool crash = false;
    public bool canIncrease = true;

    // private var
    LineRenderer lr;
    int currentAutonomy = 0;
    Text autonomyText = null;
    bool updateAutonomy = false;
    readonly float maxDistance = 0.1f;
    
    public int CurrentAutonomy { get => currentAutonomy; }

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        SetUpLine(points);
    }

    // Add a Point to the internal list and to the line renderer
    private void AddPoint(Point t)
    {
        if (t.pointType == PointType.END)
            t.connectable = false;
        // add a point to list
        points.Add(t);
        // update line renderers points
        lr.positionCount++;
        // update the line renderers
        lr.SetPosition(lr.positionCount - 1, new Vector3(t.transform.position.x, t.transform.position.y, t.transform.position.z + 0.005f));
        currentAutonomy = maxAutonomy - (points.Count - 1) + 1;
    }

    // Add a temporary Point to show line direction 
    public void AddPointAround(Point t)
    {
        // compute distance between precedent point and the current one
        float distance = Vector2.Distance(t.transform.position, points[^1].transform.position);
        if(distance > maxDistance)
        {   // if the distance is above the threshold then compute the corrisponding point in the same direction but with max distance available 
            float sign = Mathf.Sign(t.transform.position.x - points[^1].transform.position.x);
            float angle = Mathf.Asin((t.transform.position.y - points[^1].transform.position.y)/distance);
            Vector3 newPos = new Vector3( points[^1].transform.position.x + maxDistance * Mathf.Cos(angle) * sign,
                                          points[^1].transform.position.y + maxDistance * Mathf.Sin(angle),
                                        points[^1].transform.position.z);
            t.transform.position = newPos;
        }
        AddPoint(t);
    }

    // Check if Point to add is at the next column
    public void CheckBeforAddPoint(Point p)
    {
        if (canIncrease && (p.indexColumn == points.Last().GetComponent<Point>().indexColumn + 1 ||
            p.indexColumn == points.Last().GetComponent<Point>().indexColumn ||
            p.indexColumn == points.Last().GetComponent<Point>().indexColumn - 1) &&
            Vector2.Distance(p.transform.position, points[^1].transform.position) < maxDistance &&
            lr.positionCount <= maxAutonomy &&
            points.Last().pointType != PointType.END)
            AddPoint(p);
    }

    public void ResetLastPoint()
    {   // remove invalid points (when adding - keep pressing):
        // -- objects not tagged as points
        // -- duplicates of the last points
        if(points.Count > 1)
        {
            Point point = points[^1].gameObject.GetComponent<Point>();
            if (point.indexColumn == -1 || points.FindAll(x => x.transform.position == points[^1].transform.position).Count > 1 )
            {
                points.RemoveAt(points.Count - 1);
                lr.positionCount--;
            }
        }
    }

    // Delete points to reset the line
    public void ResetLine()
    {
        if (points.Last().pointType == Data.PointType.END)
            points.Last().connectable = true;
        points.RemoveRange(1,points.Count - 1);
        lr.positionCount = points.Count;
        canIncrease = true;
    }

    // reset line after routes end
    public void TotalResetLine()
    {
        foreach (Point p in points)
            p.connectable = true;
        points.RemoveRange(1, points.Count - 1);
        lr.positionCount = points.Count;
        canIncrease = true;
        maxAutonomy = 0;
        currentAutonomy = 0;
        active = false;
    }

    // initialize the line
    public void SetUpLine(List<Point> points)
    {
        lr.positionCount = points.Count;
        this.points = points;
    }

    // initialize autonomy value
    public void SetUpAutonomy(Text text)
    {
        autonomyText = text;
        currentAutonomy = maxAutonomy;
        updateAutonomy = true;
    }

    public void UpdateLineAfterClickRealese()
    {   // remove points that are not valid to stop from the line and list 
        Point point;
        for (int i = 0; i < points.Count; i++)
        {
            point = points[i].gameObject.GetComponent<Point>();
            if (point.indexColumn == -1)
               points.Remove(points[i]);
        }
        // remove duplicates
        points = points.Distinct().ToList();
        lr.positionCount = points.Count;
        point = points[^1].gameObject.GetComponent<Point>();
        // check if reach the end or the treshold autonomy
        if (point != null && active && (point.pointType == PointType.END || lr.positionCount > maxAutonomy))
            canIncrease = false;
        currentAutonomy = maxAutonomy - (points.Count - 1);
    }

    private void Update()
    {
        // update the line every time a new point is added
        if (points.Count > 0)
        {
            int i = 0;
            foreach (Point p in points)
            {
                lr.SetPosition(i, new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z + 0.005f));
                i++;
            }
        }

        // update autonomy UI text
        if (updateAutonomy)
            if(currentAutonomy == 0)
                autonomyText.text = "";
            else
                autonomyText.text = currentAutonomy.ToString();
        
    }

}
