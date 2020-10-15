using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputScrip : MonoBehaviour
{
    public Transform target;
    public float timespeed = 1;

    private Camera cam;

    private float friction;

    // private double inertia = 0.001; // kg
    // private double inertiaRot = 0.001; // kg*m^2

    private Vector3 velocity;
    private Vector3 velocityLast;
    private float rotationspeed; // positive counterclockwise
    private float rotationspeedLast; // positive counterclockwise

    private Vector3 mousePos;
    private Vector3 mousePosLast;
    private Vector3 mouseVel;
    private Vector3 mouseVelLast;
    private Vector3 mouseAccel;

    private Vector3 vectrotate;
    Quaternion quatrotate;
    

    void Start()
    {
        cam = Camera.main;
        vectrotate.x = 0;
        vectrotate.y = 0;
        vectrotate.z = 0;
        quatrotate = Quaternion.Euler(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
        float step = timespeed * Time.deltaTime;

        float xpos = Input.mousePosition.x;
        float ypos = Input.mousePosition.y;

        mousePosLast = mousePos;
        mousePos.x = xpos;
        mousePos.y = ypos;

        // calk velocity of input
        mouseVelLast = mouseVel;
        mouseVel = (mousePos - mousePosLast) / step;

        // calk accelleration of input
        mouseAccel = (mouseVel - mouseVelLast) / step;

        // speed, acceleration, force friction and torque are guing into physical model
        // we need to split acceleration components in axis of target (Cog) 
        
        
        vectrotate.z +=1;
        //target.transform.Rotate(rotate);

        // calk force 


        
        // calk torque
        

        //update the position

        Quaternion tempquatrotate = Quaternion.Euler(vectrotate);
        target.rotation = tempquatrotate;
        // transform.position = cam.ScreenToWorldPoint(new Vector3(xpos, ypos, cam.nearClipPlane));
        target.position = cam.ScreenToWorldPoint(new Vector3(xpos, ypos, cam.nearClipPlane));
        //transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }

    void OnGUI()
    {
        Vector3 point = new Vector3();

        Event   currentEvent = Event.current;
        Vector2 mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;

        point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Screen pixels:  " + cam.pixelWidth + ":" + cam.pixelHeight);
        GUILayout.Label("Mouse position: " + mousePos);
        GUILayout.Label("World position: " + point.ToString("F3"));
        //GUILayout.Label("Speed:          " + velocity.ToString("F3"));
        GUILayout.EndArea();
    }
}


  
