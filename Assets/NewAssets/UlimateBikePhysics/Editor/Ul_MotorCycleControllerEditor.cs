
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(UL_MotorCycleController))]
public class Ul_MotorCycleControllerEditor : Editor
{
    public UL_MotorCycleController myscript;
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        myscript = target as UL_MotorCycleController;

        MotorCtrlCustomInspector(myscript);
        //base.OnInspectorGUI();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(myscript);
        }
    }
    //
    public void MotorCtrlCustomInspector(UL_MotorCycleController MTC)
    {
        EditorGUI.BeginChangeCheck();
        UL_MotorCycleController.ControllerType _controllertype = new UL_MotorCycleController.ControllerType();

        _controllertype = (UL_MotorCycleController.ControllerType)EditorGUILayout.EnumPopup("ControllerType", myscript.GetControllerType);

        if (EditorGUI.EndChangeCheck())
        {
            myscript.GetControllerType = _controllertype;
        }
        //
        myscript.tab = GUILayout.Toolbar(myscript.tab, new string[] { "Welcome", "AI Controller", "User Controller", "General Settings" });

        switch (myscript.tab)
        {

            case 0:
                {
                    GUILayout.Space(6);
                    GUILayout.Label("Thanks for Choosing UTC ,\n  now you can enjoy\n real Open World Traffic in your game ;)", EditorStyles.boldLabel);
                }
                break;
            case 1:
                {
                    EditorGUI.BeginChangeCheck();
                    GUILayout.Label("AI Now Fully Functional;)", EditorStyles.boldLabel);
                    #region Ai variable
                    //float fowardsensor = new float();
                    //float sidesensors = new float();
                    float SmoothTargetSpeed = new float();
                    float SmoothTargetAngle = new float();
                    float KillBrakeSpeed = new float();
                    float NodeDifference = new float();
                    Transform TargetNode = null;
                    Transform TargetParent = null;

                    float DistanceApart = new float();

                    #endregion
                    if (myscript.GetControllerType == UL_MotorCycleController.ControllerType.AI)
                    {
                        GUILayout.Space(6);
                        GUILayout.Label("Sensors/AI Navigate Settings", EditorStyles.boldLabel);
                        GUILayout.Space(1.5f);
                        TargetParent = EditorGUILayout.ObjectField("TargetNodeParent", myscript.TargetParent, typeof(Transform), true) as Transform;
                        //fowardsensor = EditorGUILayout.FloatField("FowardSensor", myscript.fowardsensor);
                        // sidesensors = EditorGUILayout.FloatField("SideSensor", myscript.sidesensor);
                        SmoothTargetSpeed = EditorGUILayout.FloatField("SmoothTargetSpeed", myscript.SmoothTargetSpeed);
                        KillBrakeSpeed = EditorGUILayout.FloatField("killBrakeSpeed", myscript.KillBrakeSpeed);
                        NodeDifference = EditorGUILayout.FloatField("NodeDifference", myscript.NodeDifference);
                        SmoothTargetAngle = EditorGUILayout.FloatField("SmoothTargetAngle", myscript.SmoothTargetAngle);
                        DistanceApart = EditorGUILayout.FloatField(" DistanceApart ", myscript.DistanceApart);

                        GUILayout.Space(6);
                        GUILayout.Label("AI WheelSetup", EditorStyles.boldLabel);

                        TargetNode = EditorGUILayout.ObjectField("TargetNode", myscript.TargetNode, typeof(Transform), true) as Transform;
                        GUILayout.Space(1.5f);

                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(myscript, "AI Bike Changes");
                            myscript.TargetNode = TargetNode;
                            myscript.SmoothTargetSpeed = SmoothTargetSpeed;
                            myscript.DistanceApart = DistanceApart;
                            myscript.SmoothTargetAngle = SmoothTargetAngle;
                            myscript.NodeDifference = NodeDifference;
                            myscript.KillBrakeSpeed = KillBrakeSpeed;
                            // myscript.sidesensor = sidesensors;
                            // myscript.fowardsensor = fowardsensor;
                            myscript.TargetParent = TargetParent;
                        }
                    }
                    else
                    {
                        GUILayout.Space(6);
                        GUILayout.Label("Please Change The Above Controller PopUp Menu to (Ai) \nBefore You Can Edit AI Settngs", EditorStyles.boldLabel);
                    }
                }
                break;
            case 2:
                {
                    EditorGUI.BeginChangeCheck();
                    #region Player variable
                    Ul_Suspension _frontwheel = null;
                    Ul_Suspension _rearwheel = null;
                    float OverallTorque = new float();
                    float RollTorque = new float();
                    float BrakeTorque = new float();
                    float ReverseTorque = new float();
                    float sidelerptorque = new float();
                    float balancingforce = new float();
                    float SteerBalanceFactor = new float();
                    float neckang = new float();
                    float driftspeed = new float();
                    float killdriftspeed = new float();
                    float FallAngle = new float();
                    float FallSpeed = new float();
                    bool UseInverseTilt = new bool();
                    float MaxLerpAngle = new float();
                    Transform bikehead = null;
                    Ul_SurfaceDetector surfaceDetector = null;


                    #endregion
                    if (myscript.GetControllerType == UL_MotorCycleController.ControllerType.PlayerCotrol)
                    {
                        GUILayout.Space(6);
                        GUILayout.Label("WheelSettings", EditorStyles.boldLabel);
                        _frontwheel = EditorGUILayout.ObjectField("FrontWheelSuspension", myscript.FrontWheel, typeof(Ul_Suspension), true) as Ul_Suspension;
                        _rearwheel = EditorGUILayout.ObjectField("RearWheelSuspension", myscript.RearWheel, typeof(Ul_Suspension), true) as Ul_Suspension;
                        GUILayout.Space(6);
                        GUILayout.Label("TorqueSettings", EditorStyles.boldLabel);
                        OverallTorque = EditorGUILayout.FloatField("OverallTorque", myscript.OverRallTorque);
                        SteerBalanceFactor = EditorGUILayout.FloatField("SteerBalanceFactor", myscript.SteerBalanceFactor);
                        sidelerptorque = EditorGUILayout.FloatField("SideLerpTorque", myscript.SideLerpTorque);
                        balancingforce = EditorGUILayout.FloatField("BalancingForce", myscript.Balancingforce);
                        RollTorque = EditorGUILayout.FloatField("RollTorque", myscript.RollTorque);
                        BrakeTorque = EditorGUILayout.FloatField("BrakeTorque", myscript.Braketorque);
                        ReverseTorque = EditorGUILayout.FloatField("ReverseTorque", myscript.ReverseTorque);
                        GUILayout.Space(6);
                        GUILayout.Label("Neck/Drift Setings", EditorStyles.boldLabel);
                        MaxLerpAngle = EditorGUILayout.Slider("MaxLerpAngle", myscript.MaxLerpAngle, 0.0f, 60f);
                        bikehead = EditorGUILayout.ObjectField("BikeHeadParent", myscript.BikeHead, typeof(Transform), true) as Transform;
                        neckang = EditorGUILayout.FloatField("NeckAngle", myscript.NeckAngle);
                        killdriftspeed = EditorGUILayout.FloatField("KillDriftSpeed", myscript.KillDriftSpeed);
                        driftspeed = EditorGUILayout.FloatField("DriftSpeed", myscript.DriftSpeed);
                        surfaceDetector = EditorGUILayout.ObjectField("SurfaceDetector", myscript.surfaceDetector, typeof(Ul_SurfaceDetector), true) as Ul_SurfaceDetector;
                        GUILayout.Label("Rider's Grip Setings", EditorStyles.boldLabel);
                        UseInverseTilt = EditorGUILayout.Toggle("UseInverseTilt", myscript.UseInverseTilt);
                        FallAngle = EditorGUILayout.FloatField("FallAngle", myscript.FallAngle);
                        FallSpeed = EditorGUILayout.FloatField("FallSpeed", myscript.FallSpeed);
                    }
                    else
                    {
                        GUILayout.Space(6);
                        GUILayout.Label("Please Change The Above Controller PopUp Menu to (PlayerController) \n Before You Can Edit Player Settngs", EditorStyles.boldLabel);

                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(myscript, "Bike Changes");
                        myscript.surfaceDetector = surfaceDetector;
                        myscript.SteerBalanceFactor = SteerBalanceFactor;
                        myscript.BikeHead = bikehead;
                        myscript.OverRallTorque = OverallTorque;
                        myscript.RollTorque = RollTorque;
                        myscript.SideLerpTorque = sidelerptorque;
                        myscript.NeckAngle = neckang;
                        myscript.DriftSpeed = driftspeed;
                        myscript.KillDriftSpeed = killdriftspeed;
                        myscript.Balancingforce = balancingforce;
                        myscript.ReverseTorque = ReverseTorque;
                        myscript.Braketorque = BrakeTorque;
                        myscript.FrontWheel = _frontwheel;
                        myscript.RearWheel = _rearwheel;
                        myscript.FallSpeed = FallSpeed;
                        myscript.FallAngle = FallAngle;
                        myscript.UseInverseTilt = UseInverseTilt;
                        myscript.MaxLerpAngle = MaxLerpAngle;
                    }
                }
                break;
            case 3:
                {
                    EditorGUI.BeginChangeCheck();
                    Transform FrontWheelMesh = null;
                    Transform RearWheelMesh = null;
                    GUILayout.Label("Settings here will affect both AI and player control", EditorStyles.boldLabel);
                    FrontWheelMesh = EditorGUILayout.ObjectField("FrontWheelMesh", myscript.FrontWheelMesh, typeof(Transform), true) as Transform;
                    RearWheelMesh = EditorGUILayout.ObjectField("RearWheelMesh", myscript.RearWheelMesh, typeof(Transform), true) as Transform;
                    if (EditorGUI.EndChangeCheck())
                    {
                        myscript.FrontWheelMesh = FrontWheelMesh;
                        myscript.RearWheelMesh = RearWheelMesh;
                    }
                }
                break;
        }
    }
}
#endif

