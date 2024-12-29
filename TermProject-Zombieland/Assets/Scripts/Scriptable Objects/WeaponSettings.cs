using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponSettings", menuName = "Weapons/WeaponSettings")]
public class WeaponSettings : ScriptableObject
{
    public string weaponName;
    public int maxAmmo;
    public int magazine;
    public int currentAmmo;
    public int currentMag;
    public float damage;
    public float fireRange;
    public float fireLimit;
    public float reloadingTime;
    public AudioClip gunSound;
    public bool isFullAuto;
    public float bodyHorizontal;
    public float headHorizontal;
}
