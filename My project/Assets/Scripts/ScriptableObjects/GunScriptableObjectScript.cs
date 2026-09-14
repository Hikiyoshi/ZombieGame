using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Scriptable Objects/Gun")]
public class GunScriptableObjectScript : ScriptableObject
{
    public string name;
    public int damgePerTime;
    public int knockback;
    public float timePerAttk;
    public int magazine;
    public Transform prefabTransform;
    public Transform ammoPrefabTransform;
    public Transform vfxPrefabTransform;
    public GunType gunType;
}

public enum GunType
{
    Assault,
    Shotgun,
    Smg,
}