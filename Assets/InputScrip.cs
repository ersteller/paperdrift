using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputScrip : MonoBehaviour
{
    public Transform car;
    public Transform puller;
    public float timespeed;

    private Camera cam;

    private float friction;

    private Vector3 cartopullerrel;



    private Vector3 velocity;
    private Vector3 velocityLast;

    // those are in worldspace units 
    private Vector3 mousePos; 
    private Vector3 mousePosLast;
    private Vector3 mouseVel;
    private Vector3 mouseVelLast;
    private Vector3 mouseAccel;
    private Vector3 carPos;
    private Vector3 carPosLast;
    private Vector3 carVel;
    private Vector3 carVelLast;
    private Vector3 carAccel;
    private Vector3 accelComponents;

    private float caroffset = 1.5f; // 2 Worldunits offset from the car to the puller 

    private float mass = 1;       // mass of car 
    private Vector3 force;        // applied to car
    private float angle = 0;

    Quaternion quatrotate;

    void Start()
    {
        cam = Camera.main;
        quatrotate = Quaternion.Euler(0, 0, 0);
        timespeed = 1F;
        friction = 100;
        carVel = new Vector3(0, 0, 0);
        carPos = car.position;

        // car.position -2
        // initialize mouseposition so we dont get a spike in fisrt update
        float xpos = Input.mousePosition.x;
        float ypos = Input.mousePosition.y;
        mousePos = cam.ScreenToWorldPoint(new Vector3(xpos, ypos, cam.nearClipPlane));

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

        // speed, acceleration, force friction are guing into physical model
        // we need to split acceleration components in axis of car (CoG)
        // we do that by rotating the axis of the applied force by the amount of the car.rotation
        // so the components are aligned and othogonal with the car

        // angel of car = -atan2(Rx,Ry)  // found to be empirically correct (negative because 3d z-axis)
        cartopullerrel = puller.position - car.position;
        angle = -Mathf.Atan2(cartopullerrel.x, cartopullerrel.y);

        // Get acceleration component in car koordinates
        accelComponents = Vector3Extension.Rotate(mouseAccel, -angle);

        // calk car force F=m*a (y because of car initial orientation pointing up)
        float carforce = mass * accelComponents.y;
        carAccel = Vector3Extension.Rotate(new Vector3(0, carforce, 0), angle); // rotate acceleration back to world coordinates

        // integrate force to get velocity
        carVel.x = (carAccel.x / mass) * step + carVel.x;
        carVel.y = (carAccel.y / mass) * step + carVel.y;

        // constrain to mouse velocity in y axis(car)
        Vector3 carvelComponents = Vector3Extension.Rotate(carVel, -angle);
        Vector3 mousvelComponents = Vector3Extension.Rotate(mouseVel, -angle);
        carvelComponents.y = mousvelComponents.y;
        
        // reduce velocity by friction linearly in x axis(car)
        if (carvelComponents.x > friction * step)
        {
            carvelComponents.x -= friction * step;
        }
        else if (carvelComponents.x < -friction * step)
        {
            carvelComponents.x += friction * step;
        }
        else if ((carvelComponents.x < friction * step && carvelComponents.x > 0) || (carvelComponents.x > -friction * step && carvelComponents.x < 0))
        {
            carvelComponents.x = 0;
        }

        // friction is overwritten by mouse in y

        // convert new velocity to world koordinates
        carVel = Vector3Extension.Rotate(carvelComponents, angle);
   
        // integrate velocity to get position
        carPos.x = carVel.x * step + carPos.x;
        carPos.y = carVel.y * step + carPos.y;

        // set car to distance offset from puller 
        Vector3 pullertocar = carPos - puller.position;
        pullertocar.Normalize();
        pullertocar = pullertocar * caroffset; // TODO: this is still not working as expected
        // set car position to 
        carPos = pullertocar + puller.position;
        
        //update the position of puller (under the mouse)
        puller.position = cam.ScreenToWorldPoint(new Vector3(xpos, ypos, cam.nearClipPlane));

        // update rotation (always pointing to puller)
        Quaternion tempquatrotate = Quaternion.Euler(0,0, angle* Mathf.Rad2Deg);
        car.rotation = tempquatrotate;

        // update position of car
        float maxdistance = 2;
        
        car.position = Vector3.MoveTowards(car.position, carPos, maxdistance);

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

        GUILayout.BeginArea(new Rect(20, 20, 450, 220));
        GUILayout.Label("Mouse Pixel Position:  " + mousePos);
        GUILayout.Label("Mouse World Position:  " + point.ToString("F3"));
        GUILayout.Label("Mouse World Speed:     " + mouseVel.ToString("F3"));
        GUILayout.Label("Mouse World Accel.:    " + mouseAccel.ToString("F3"));
        GUILayout.Label("carkoord forces:       " + accelComponents.ToString("F3"));
        GUILayout.Label("carAccel.:             " + carAccel.ToString("F3"));
        GUILayout.Label("carspeed :             " + carVel.ToString("F3"));
        GUILayout.EndArea();
    }
}


  
