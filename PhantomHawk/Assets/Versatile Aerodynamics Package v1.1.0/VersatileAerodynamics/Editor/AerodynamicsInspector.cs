using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Aerodynamics))]
[CanEditMultipleObjects]
public class AerodynamicsInspector : Editor
{
	Aerodynamics aerodynamics;
	Aircraft aircraft;
	int warningNumber = 0;

	public override void OnInspectorGUI()
	{

		DrawDefaultInspector();

		if (aerodynamics == null)
		{
			aerodynamics = (Aerodynamics)target;
			aircraft = aerodynamics.GetComponentInParent<Aircraft>();
		}
		if (aircraft != null)
        {

        }

		GUIContent content = new GUIContent();
		content.text = "Lift to Drag Ratio";
		content.tooltip = "Lift to Drag ratio for angles of attack between 0-90 degrees simulated at a speed of 30 m/s.  |  y-axis: L/D  |  x-axis: degrees";

		if (aircraft != null)
		{
			warningNumber = 0;
			AnimationCurve simCurve = CreateSimulatedCurve();
			float peak = FindMax(simCurve);

			EditorGUILayout.CurveField(content, simCurve, GUILayout.Height(200));
			EditorGUILayout.LabelField("peak L/D", peak.ToString());
		}
		else
        {
			if (warningNumber == 0)
			{
				Debug.LogWarning("All Aerodynamics components must have an Aircraft component somewhere in their parent heirarchy", aerodynamics.gameObject);
				warningNumber = 1;
			}
        }

	}

	AnimationCurve CreateSimulatedCurve()
    {
		AnimationCurve curve = new AnimationCurve();

		Vector3 v0 = -aerodynamics.transform.forward * 30f;

        for (int i = 0; i < 90; i++)
        {
			Vector3 v = Quaternion.AngleAxis(i, aerodynamics.transform.right) * v0;
			(Vector3 l, Vector3 d) = aerodynamics.Aerodynamic_Force(v, aircraft.fluid_density);

			float ldRatio = l.magnitude / d.magnitude;

			curve.AddKey(i, ldRatio);
        }
		 
		return curve;
	}

	float FindMax(AnimationCurve curve)
    {
		float value = 0f;
		foreach(Keyframe key in curve.keys)
        {
			if(key.value > value)
            {
				value = key.value;
            }
        }

		return value;
    }

}