using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private const float Y_ANGLE_MIN = -20f;
    private const float Y_ANGLE_MAX = 20f;


    [SerializeField]
    public GameObject[] weapons;
    [SerializeField]
    public WeaponSettings[] weaponSettings;
    [SerializeField]
    public ParticleSystem[] shootParticles;
    [SerializeField]
    public Light[] shootLights;
    [SerializeField]
    public AudioClip[] gunSounds;
    [SerializeField]
    public LineRenderer[] gunLine;

    [SerializeField]
    public TextMeshProUGUI weaponName;
    [SerializeField]
    public TextMeshProUGUI ammoInfo;


    public float maxLife = 100f;
    public float damage = 10f;
    public float fireRange = 50f;
    public float fireLimit = 0.25f;
    int currentMag;
    int currentAmmo;


    public Transform shootDir;


    public float speed = 6f;

    GameObject myCam;
    AudioSource gunAudioSource;
    Animator playerAnim;
    int weaponType = 0;
    int oldWeapon;

    private float currentX = 0f;
    private float currentY = 0f;

    public float bodyVertical = 0;
    float horizontalMove;
    float verticalMove;
    float playerSpeed = 0f;
    float elapsedTime = 0f;
    float animDuration = 0f;
    float camField = 0f;
    float curVelocity;
    bool shooting = false;
    public bool isGrounded = false;
    float effectsDisplayTime = 0.2f;
    float reloadTimer = 0;
    bool reloading = false;

    Rigidbody playerRigidbody;
    Ray shootRay;
    RaycastHit shootHit;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        myCam = GetComponentInChildren<Camera>().gameObject;
        gunAudioSource = GetComponent<AudioSource>();
        playerAnim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnim.SetFloat("Speed_f", playerSpeed);
        gunAudioSource.clip = gunSounds[weaponType];
        playerAnim.SetInteger("WeaponType_int", weaponType);
        foreach (WeaponSettings item in weaponSettings)
        {
            item.currentMag = item.magazine;
            item.currentAmmo = item.maxAmmo - item.magazine;
        }
        weaponName.text = weaponSettings[weaponType].weaponName;
        UpdateAmmoInfoText();
    }

    void Update()
    {
        currentX += Input.GetAxis("Mouse X");
        currentY += Input.GetAxis("Mouse Y");
        currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
        playerAnim.SetBool("Shoot_b", false);
        elapsedTime += Time.deltaTime;

        if (reloading)
        {
            reloadTimer += Time.deltaTime;

            if(reloadTimer >= 0.5f)
                playerAnim.SetBool("Reload_b", false);

            if (reloadTimer >= weaponSettings[weaponType].reloadingTime)
            {
                reloadTimer = 0;
                reloading = false;
            }
        }

        horizontalMove = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;
        verticalMove = Input.GetAxisRaw("Vertical") * speed * Time.deltaTime;

        //myCam.transform.localRotation = Quaternion.Euler(currentY, 0, 0);
        transform.rotation = Quaternion.Euler(0, currentX, 0);
        playerAnim.SetFloat("Body_Vertical_f", -currentY / 50);

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && !Input.GetButton("Fire2") && !shooting
                //(weaponType==0 && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) || 
                )
            {
                Move(horizontalMove, verticalMove);
                playerSpeed = 0.7f;
                playerAnim.SetFloat("Speed_f", playerSpeed);
                speed = 10f;
            }
            else
            {
                Move(horizontalMove, verticalMove);
                playerSpeed = 0.3f;
                playerAnim.SetFloat("Speed_f", playerSpeed);
                speed = 5f;
            }
        }
        else
        {
            playerSpeed = 0;
            playerAnim.SetFloat("Speed_f", playerSpeed);
        }

        if (shooting)
        {
            animDuration += Time.deltaTime;
        }

        if (weaponType != 0)
        {

            if (Input.GetButton("Fire2"))
            {
                shooting = true;
                animDuration = 0f;
                camField = Mathf.SmoothDamp(myCam.GetComponent<Camera>().fieldOfView, 30f, ref curVelocity, 0.5f);

                myCam.GetComponent<Camera>().fieldOfView = camField;
                Debug.Log(camField);
                // myCam.transform.localPosition = Vector3.Lerp(myCam.transform.localPosition, new Vector3(-1f, 0, -2f), 5f * Time.deltaTime);



            }
            else
            {

                camField = Mathf.SmoothDamp(myCam.GetComponent<Camera>().fieldOfView, 60f, ref curVelocity, 0.5f);
                // myCam.transform.localPosition = Vector3.Lerp(myCam.transform.localPosition, new Vector3(0f, 0f, -4f), 5f * Time.deltaTime);
                myCam.GetComponent<Camera>().fieldOfView = camField;

                if (animDuration >= 1f)
                {

                    shooting = false;
                    animDuration = 0f;
                }

            }

            if (Input.GetButton("Fire1") && elapsedTime >= fireLimit && Time.timeScale != 0 && !reloading && currentMag > 0)
            {
                shooting = true;

                gunLine[weaponType - 1].enabled = true;
                gunLine[weaponType - 1].SetPosition(0, shootParticles[weaponType - 1].transform.position);
                ShootEnemy();


                shootLights[weaponType - 1].enabled = true;
                shootParticles[weaponType - 1].Play();
                gunAudioSource.clip = gunSounds[weaponType];
                gunAudioSource.Play();
                playerAnim.SetBool("Shoot_b", true);
                animDuration = 0f;
                elapsedTime = 0f;

                currentMag--;
                weaponSettings[weaponType].currentMag = currentMag;

                UpdateAmmoInfoText();

                if (currentMag <= 0)
                {
                    Reload();
                }

            }

            if (elapsedTime >= fireLimit * effectsDisplayTime)
            {
                DisableEffects();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Reload();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            oldWeapon = weaponType;
            weaponType = 1;
            ChangeWeapon(weaponType, oldWeapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            oldWeapon = weaponType;
            weaponType = 2;
            ChangeWeapon(weaponType, oldWeapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            oldWeapon = weaponType;
            weaponType = 3;
            ChangeWeapon(weaponType, oldWeapon);
        }
        //else if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    oldWeapon = weaponType;
        //    weaponType = 4;
        //    ChangeWeapon(weaponType, oldWeapon);
        //}
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            oldWeapon = weaponType;
            weaponType = 5;
            ChangeWeapon(weaponType, oldWeapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            oldWeapon = weaponType;
            weaponType = 6;
            ChangeWeapon(weaponType, oldWeapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            oldWeapon = weaponType;
            weaponType = 7;
            ChangeWeapon(weaponType, oldWeapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            oldWeapon = weaponType;
            weaponType = 0;
            ChangeWeapon(weaponType, oldWeapon);
        }
        shootRay.direction = myCam.transform.forward;
        Debug.DrawRay(myCam.transform.position, shootRay.direction * fireRange, Color.red);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            playerRigidbody.AddForce(new Vector3(0, 250f, 0));
        }
    }

    void Move(float horizontalMove, float verticalMove)
    {
        transform.position += transform.right * horizontalMove / 2;
        transform.position += transform.forward * verticalMove;
    }

    public void ChangeWeapon(int weapon, int previousWeapon)
    {
        playerAnim.SetFloat("Head_Horizontal_f", weaponSettings[weaponType].headHorizontal);
        fireRange = weaponSettings[weaponType].fireRange;
        damage = weaponSettings[weaponType].damage;
        fireLimit = weaponSettings[weaponType].fireLimit;
        gunAudioSource.clip = gunSounds[weaponType];
        playerAnim.SetBool("FullAuto_b", weaponSettings[weaponType].isFullAuto);

        PlayerPrefs.SetInt("OldWeapon", previousWeapon);
        weapons[previousWeapon].SetActive(false);
        playerAnim.SetFloat("Body_Horizontal_f", weaponSettings[weaponType].bodyHorizontal);
        weapons[weapon].SetActive(true);
        playerAnim.SetInteger("WeaponType_int", weapon);
        currentAmmo = weaponSettings[weaponType].currentAmmo;
        currentMag = weaponSettings[weaponType].currentMag;
        weaponName.text = weaponSettings[weaponType].weaponName;
        UpdateAmmoInfoText();
    }

    void ShootEnemy()
    {
        shootRay.direction = myCam.transform.forward;
        if (Physics.Raycast(myCam.transform.position, shootRay.direction, out shootHit, fireRange))
        {
            if (shootHit.transform.CompareTag("Enemy"))
            {
                shootHit.transform.GetComponent<EnemyHealth>().TakeDamage((int)damage, shootHit.transform.position);
            }
            gunLine[weaponType - 1].SetPosition(1, shootHit.point);
        }
        else
        {
            gunLine[weaponType - 1].SetPosition(1, shootDir.position);
        }

    }

    public void DisableEffects()
    {
        if (weaponType != 0)
        {
            gunLine[weaponType - 1].enabled = false;
            shootParticles[weaponType - 1].Stop();
            shootLights[weaponType - 1].enabled = false;
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }


    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void Reload()
    {
        if (currentAmmo <= 0 || currentMag == weaponSettings[weaponType].magazine)
            return;
        reloading = true;
        playerAnim.SetBool("Reload_b", true);
        int magDiff = weaponSettings[weaponType].magazine - currentMag;

        int amountToReload = Mathf.Min(magDiff, currentAmmo);

        currentAmmo -= amountToReload;
        currentMag += amountToReload;

        weaponSettings[weaponType].currentMag = currentMag;
        weaponSettings[weaponType].currentAmmo = currentAmmo;
        UpdateAmmoInfoText();
    }

    void UpdateAmmoInfoText()
    {
        if (weaponType == 0) return;
        ammoInfo.text = currentMag + " / " + currentAmmo;
    }
}
