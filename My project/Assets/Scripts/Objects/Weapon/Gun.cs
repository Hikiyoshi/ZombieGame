using UnityEngine;

public class Gun
{
     public string name {get; set;}
    public int damagePerTime {get; set;}
    public int knockback {get; set;}
    public Transform prefabTransform {get; set;}
    public GunType gunType {get; set;}
    public float timePerAttk {get; set;}
    public int magazine {get; set;}
    public Transform ammoPrefabTransform {get; set;}
    public Transform vfxPrefabTransform {get; set;}

    public Gun(GunScriptableObjectScript gun)
    {
        this.name = gun.name;
        this.damagePerTime = gun.damgePerTime;
        this.knockback = gun.knockback;
        this.timePerAttk = gun.timePerAttk;
        this.magazine = magazine;
        this.prefabTransform = gun.prefabTransform;
        this.ammoPrefabTransform = gun.ammoPrefabTransform;
        this.vfxPrefabTransform = gun.vfxPrefabTransform;
        this.gunType = gun.gunType;
    }

    public Gun(string name, int dPT, int knockback, int magazine, Transform prefabTransform, Transform ammoPrefabTransform, Transform vfxPrefabTransform, GunType gunType)
    {
        this.name = name;
        this.damagePerTime = dPT;
        this.knockback = knockback;
        this.timePerAttk = timePerAttk;
        this.magazine = magazine;
        this.prefabTransform = prefabTransform;
        this.ammoPrefabTransform = ammoPrefabTransform;
        this.vfxPrefabTransform = vfxPrefabTransform;
        this.gunType = gunType;
    }
}
