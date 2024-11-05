//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2020 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RCC_LogitechSteeringWheel : MonoBehaviour{

	#region singleton
	private static RCC_LogitechSteeringWheel instance;
	public static RCC_LogitechSteeringWheel Instance{

		get{

			if (instance == null) {

				instance = FindObjectOfType<RCC_LogitechSteeringWheel> ();

				if (instance == null) {

					GameObject sceneManager = new GameObject ("_RCCLogitechSteeringWheelManager");
					instance = sceneManager.AddComponent<RCC_LogitechSteeringWheel> ();

				}

			}

			return instance;

		}

	}

	#endregion

	private LogitechGSDK.LogiControllerPropertiesData properties;
	internal LogitechGSDK.DIJOYSTATE2ENGINES rec;

	public RCC_Inputs inputs = new RCC_Inputs();

	public bool useForceFeedback = true;
	public bool useHShifter = true;
	public bool atNGear = true;

    void Start(){

		LogitechGSDK.LogiSteeringInitialize (false);

    }

	void OnEnable(){

		RCC_CarControllerV3.OnRCCPlayerCollision += RCC_CarControllerV3_OnRCCPlayerCollision;

	}

	void RCC_CarControllerV3_OnRCCPlayerCollision (RCC_CarControllerV3 RCC, Collision collision){
		
		if(RCC == RCC_SceneManager.Instance.activePlayerVehicle)
			LogitechGSDK.LogiPlayFrontalCollisionForce (0, Mathf.CeilToInt(collision.relativeVelocity.magnitude * 3f));
		
	}
		
    void Update(){

		if (LogitechGSDK.LogiUpdate () && LogitechGSDK.LogiIsConnected (0)) {

			rec = LogitechGSDK.LogiGetStateUnity (0);

            if (useHShifter)
            {
				
				HShifter(rec);
			}
				

			if (useForceFeedback)
				ForceFeedback ();

			inputs.steerInput = Mathf.Clamp(rec.lX / 32768f, -1f, 1f);
			inputs.throttleInput = Mathf.Clamp01 (rec.lY / -32768f);
			inputs.brakeInput = Mathf.Clamp01 ((1f - Mathf.Abs(rec.lRz / -32768f)));
			inputs.clutchInput = Mathf.Clamp01 (rec.rglSlider [0] / -32768f);
			inputs.Ngear = atNGear;

		}
        
    }

	void HShifter(LogitechGSDK.DIJOYSTATE2ENGINES shifter){

		bool atGear = false;

		for (int i = 0; i < 128; i++) {
			//Debug.Log(" here in joy stick " + shifter.rgbButtons[i]);
			if (shifter.rgbButtons [i] == 128) {
				
				switch (i) {

				case 12:

					inputs.gearInput = 0;		
						atGear = true;
					break;

				case 13:

					inputs.gearInput = 1;		
						atGear = true;
					break;

				case 14:

					inputs.gearInput = 2; 	
						atGear = true;
					break;

				case 15:

					inputs.gearInput = 3;
					atGear = true;
					break;

				case 16:

					inputs.gearInput = 4;
					atGear = true;
					break;

				case 17:

					inputs.gearInput = 5;
					atGear = true;
					break;

				case 18:

					inputs.gearInput = -1;
					atGear = true;
					break;

				}
					
			}

		}

		atNGear = !atGear;

	}

	void ForceFeedback(){
		
		RCC_CarControllerV3 playerVehicle = RCC_SceneManager.Instance.activePlayerVehicle;

		if (!playerVehicle)
			return;

		LogitechGSDK.LogiStopConstantForce(0);
		LogitechGSDK.LogiPlayConstantForce (0, (int)(-playerVehicle.FrontLeftWheelCollider.wheelHit.sidewaysSlip * 200f));
		 
	}

	public static bool GetKeyTriggered(int controllerIndex, int keycode){

		return LogitechGSDK.LogiButtonTriggered (controllerIndex, keycode);

	}

	public static bool GetKeyPressed(int controllerIndex, int keycode){
	
		return LogitechGSDK.LogiButtonIsPressed (controllerIndex, keycode);
      


    }

    public static bool GetKeyReleased(int controllerIndex, int keycode){

		return LogitechGSDK.LogiButtonReleased (controllerIndex, keycode);

	}
	//added
	public static void PlayBumpyRoadEffect()
    {
			LogitechGSDK.LogiPlayBumpyRoadEffect(0, 10);	
	}
	public static void StopBumpyRoadEffect()
    {
		if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_BUMPY_ROAD))
		{
			LogitechGSDK.LogiStopBumpyRoadEffect(0);

		}
	}

	void OnDisable(){

		RCC_CarControllerV3.OnRCCPlayerCollision -= RCC_CarControllerV3_OnRCCPlayerCollision;

	}

}
