/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour {

    private static Speedometer _instance;
    public static Speedometer Instance { get { return _instance; } }
    private static readonly object padlock = new object();

    private const float MAX_SPEED_ANGLE = -20;
    private const float ZERO_SPEED_ANGLE = 230;

    public Transform needleTranform;
    public Transform speedLabelTemplateTransform;

    private float speedMax;
    private float targetSpeed;

    // move to dictionary correlating to the speed; also change in SetTargetSpeed;
    private float accelerationRate = 0.5f;
    private float deccelerationRate = 1.5f;
    private float movementRate;

    private float speed;
    public float Speed { get => speed; }

    private void Awake()
    {
        Debug.Log("AWAKE");
        lock (padlock)
        {
            if (_instance != null && _instance != this)
            {
                Debug.Log("DESTROY");
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;

                //Here any additional initialization should occur:
                speedLabelTemplateTransform.gameObject.SetActive(false);

                speed = 0f;
                speedMax = 200f;

                CreateSpeedLabels();
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private float tParam;
    private void Update() {
        if (tParam < 1)
        {
            tParam += movementRate * Time.deltaTime;
            speed = Mathf.Lerp(speed, targetSpeed, tParam);
        }

        needleTranform.eulerAngles = new Vector3(0, 0, GetSpeedRotation());
    }

    private void HandlePlayerInput() {
        if (Input.GetKey(KeyCode.UpArrow)) {
            float acceleration = 80f;
            speed += acceleration * Time.deltaTime;
        } else {
            float deceleration = 20f;
            speed -= deceleration * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            float brakeSpeed = 100f;
            speed -= brakeSpeed * Time.deltaTime;
        }

        speed = Mathf.Clamp(speed, 0f, speedMax);
    }

    private void CreateSpeedLabels() {
        int labelAmount = 10;
        float totalAngleSize = ZERO_SPEED_ANGLE - MAX_SPEED_ANGLE;

        for (int i = 0; i <= labelAmount; i++) {
            Transform speedLabelTransform = Instantiate(speedLabelTemplateTransform, transform);
            float labelSpeedNormalized = (float)i / labelAmount;
            float speedLabelAngle = ZERO_SPEED_ANGLE - labelSpeedNormalized * totalAngleSize;
            speedLabelTransform.eulerAngles = new Vector3(0, 0, speedLabelAngle);
            speedLabelTransform.Find("speedText").GetComponent<Text>().text = Mathf.RoundToInt(labelSpeedNormalized * speedMax).ToString();
            speedLabelTransform.Find("speedText").eulerAngles = Vector3.zero;
            speedLabelTransform.gameObject.SetActive(true);
        }

        needleTranform.SetAsLastSibling();
    }

    private float GetSpeedRotation() {
        float totalAngleSize = ZERO_SPEED_ANGLE - MAX_SPEED_ANGLE;

        float speedNormalized = speed / speedMax;

        return ZERO_SPEED_ANGLE - speedNormalized * totalAngleSize;
    }

    public void SetTargetSpeed(ShipSpeed setSpeed)
    {
        Debug.Log("SetTargetSpeed");
        targetSpeed = (int)setSpeed;
        if (setSpeed == ShipSpeed.Stop){
            movementRate = deccelerationRate;
        }
        else{
            movementRate = accelerationRate;
        }
        tParam = 0;
    }
}
