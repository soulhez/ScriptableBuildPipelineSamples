using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Custom Object/Color Data Object", order = 1)]
public class DataObject : ScriptableObject
{
	public string objectName = "New MyScriptableObject";
	public Color thisColor = Color.white;
}