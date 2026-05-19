using UnityEngine;

[CreateAssetMenu(fileName = "ObjectTemplate", menuName = "Scriptable Objects/ObjectTemplate")]
public class ObjectTemplate : ScriptableObject
{
    public string objectType;
    public Sprite objectIcon;
    public GameObject objectPrefab;
}
