using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputScrip : MonoBehaviour
{
    public Transform target;
    public float timespeed;

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

    private float offset = 2; // 2 units offset

    private Vector3 force;
    private float mass = 0.1F;   // mass of target
    private float inertia = 0.1F; // rotaional mass
    private float angularspeed = 0;
    private float angle = 0;
    private float angularacceleration = 0;


    private Vector3 vectrotate;
    Quaternion quatrotate;

    void Start()
    {
        cam = Camera.main;
        vectrotate.x = 0;
        vectrotate.y = 0;
        vectrotate.z = 0;
        quatrotate = Quaternion.Euler(0, 0, 0);
        timespeed = 1F;
        friction = 100;
    }

    // Update is called once per frame
    void Update()
    {
        
        float step = timespeed * Time.deltaTime;

        float xpos = Input.mousePosition.x;
        float ypos = Input.mousePosition.y;

        mousePosLast = mousePos;
        mousePos = cam.ScreenToWorldPoint(new Vector3(xpos, ypos, cam.nearClipPlane));

        // calk velocity of input
        mouseVelLast = mouseVel;
        mouseVel = (mousePos - mousePosLast) / step;

        // calk accelleration of input
        mouseAccel = (mouseVel - mouseVelLast) / step;

        // speed, acceleration, force friction and torque are guing into physical model
        // we need to split acceleration components in axis of target (CoG)
        // we do that by rotating the axis of the applied force by the amount of the target 
        // rotation so the components are aligned and othogonal with the target
        
        // calk force F=m*a
        force.x = mouseAccel.x * mass;
        force.y = mouseAccel.y * mass;

        // calk torque = (F)x(r)
        angle = target.eulerAngles.z * Mathf.Deg2Rad;
        Vector3 forceComponents = Vector3Extension.Rotate(force, angle);
        float torque = forceComponents.x * offset; // only this component is adding spin to the target.
        // calc angular acceleration     Torque = inertia * omega' 
        angularacceleration = torque / inertia;

        // dont forget dt when integrating (this is no simulation but anyways)
        angularspeed += angularacceleration * step;
        angle += angularspeed * step;

        // also reduce angular speed by friction linearly
        if (angularspeed > 0)
        {
            angularspeed -= friction * step;
        } else if (angularspeed < 0)
        {
            angularspeed += friction * step;
        }
         
        //update the position

        Quaternion tempquatrotate = Quaternion.Euler(0,0, angle* Mathf.Rad2Deg);
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

        GUILayout.BeginArea(new Rect(20, 20, 350, 220));
        GUILayout.Label("Screen pixels:         " + cam.pixelWidth + ":" + cam.pixelHeight);
        GUILayout.Label("Mouse Pixel Position:  " + mousePos);
        GUILayout.Label("Mouse World Position:  " + point.ToString("F3"));
        GUILayout.Label("Mouse World Speed:     " + mouseVel.ToString("F3"));
        GUILayout.Label("Mouse World Accel.:    " + mouseAccel.ToString("F3"));
        GUILayout.Label("Target World Rot.:     " + target.rotation.ToString("F3") + angle);
        GUILayout.Label("Target World RotSpeed: " + angularspeed);
        GUILayout.Label("Target WorldRotAccel.: " + angularacceleration);
        GUILayout.EndArea();
    }
}


  
