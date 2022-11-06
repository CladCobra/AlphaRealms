using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Image crosshair;

    [Header("Settings")]
    [SerializeField][Range(0, 50)] private float fireRate;
    [SerializeField][Range(0, 999)] private int magazineSize;
    [SerializeField][Range(0, 999)] private int reservedAmmoCapacity;

    [Header("Shooting")]
    public bool isAutomatic;
    private int currentAmmo;
    private int reservedAmmo;
    private bool canShoot;

    [Header("ADS")]
    [SerializeField] private Transform recoilPosition;
    [SerializeField] private Transform rotationPoint;
    [SerializeField] private Transform ADSPoint;
    [SerializeField][Range(0f, 10f)] private float ADSSmoothing;
    [HideInInspector] public bool isADS;
    private Vector3 initialPosition;

    [Header("Jump Effect")]
    [SerializeField][Range(0f, 10f)] private float jumpEffectSmoothing;

    [Header("Sway")]
    [SerializeField][Range(-10f, 10f)] private float swayAmount;

    [Header("Bobbing")]
    [SerializeField][Range(0f, 25f)] private float walkBobSpeed;
    [SerializeField][Range(0f, 1f)] private float walkBobAmount;
    [SerializeField][Range(0f, 25f)] private float sprintBobSpeed;
    [SerializeField][Range(0f, 1f)] private float sprintBobAmount;
    [SerializeField][Range(0f, 25f)] private float crouchBobSpeed;
    [SerializeField][Range(0f, 1f)] private float crouchBobAmount;
    [SerializeField][Range(0f, 25f)] private float ADSBobSpeed;
    [SerializeField][Range(0f, 1f)] private float ADSBobAmount;
    [SerializeField][Range(0f, 25f)] private float ADSCrouchBobSpeed;
    [SerializeField][Range(0f, 1f)] private float ADSCrouchBobAmount;
    private float defaultYPosition;
    private float timer;

    [Header("Recoil")]
    [SerializeField] private float positionalRecoilSpeed;
    [SerializeField] private float rotationalRecoilSpeed;
    [SerializeField] private float positionalReturnSpeed;
    [SerializeField] private float rotationalReturnSpeed;
    [SerializeField] private Vector3 recoilRotation;
    [SerializeField] private Vector3 recoilKickback;
    [SerializeField] private Vector3 ADSRecoilRotation;
    [SerializeField] private Vector3 ADSRecoilKickback;
    private Vector3 positionalRecoil;
    private Vector3 rotationalRecoil;
    private Vector3 rotation;

    [Header("Muzzle Flash")]
    [SerializeField] private Image muzzleFlash;
    [SerializeField] private Sprite[] flashes;
    [SerializeField][Range(0f, 1f)] private float muzzleFlashLength;

    [Header("Bullet Impact")]
    [SerializeField] private GameObject bulletImpact;

    private void Start() {

        initialPosition = transform.localPosition;

        currentAmmo = magazineSize;
        reservedAmmo = reservedAmmoCapacity;
        canShoot = true;
        isADS = false;

    }

    private void Update() {

        CalculateSway();
        HandleGunPosition();
        HandleWeaponBob();

    }

    private void FixedUpdate() {

        positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, positionalReturnSpeed * Time.deltaTime);
        rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, rotationalReturnSpeed * Time.deltaTime);

        recoilPosition.localPosition = Vector3.Slerp(recoilPosition.localPosition, positionalRecoil, positionalRecoilSpeed * Time.fixedDeltaTime);
        rotation = Vector3.Slerp(rotation, rotationalRecoil, rotationalRecoilSpeed * Time.fixedDeltaTime);
        rotationPoint.localRotation = Quaternion.Euler(rotation);

    }

    public void Shoot() {

        if (canShoot && currentAmmo > 0) {

            canShoot = false;
            currentAmmo--;
            StartCoroutine(ShootGun());
            HandleRecoil();

        }
    }

    public void ToggleADS() {

        isADS = !isADS;

    }

    public void Reload() {

        if (currentAmmo < magazineSize && reservedAmmo > 0) {

            int amountToReload = magazineSize - currentAmmo;

            if (amountToReload > reservedAmmo) {

                currentAmmo += reservedAmmo;
                reservedAmmo -= amountToReload;

            } else {

                currentAmmo = magazineSize;
                reservedAmmo -= amountToReload;

            }
        }
    }

    private void HandleGunPosition() {

        if (isADS) {

            transform.localPosition = Vector3.Lerp(transform.localPosition, ADSPoint.localPosition, ADSSmoothing * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, ADSSmoothing * Time.deltaTime);

        } else {

            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, ADSSmoothing * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, ADSSmoothing * Time.deltaTime);

        }
    }

    private void CalculateSway() {

        transform.localPosition += (Vector3) playerController.mouseInput * swayAmount / 1000;

    }

    private void CalculateRecoil() {

        transform.localPosition -= Vector3.forward * 0.1f;

    }

    private void HandleWeaponBob() {

        if (Mathf.Abs(playerController.rb.velocity.magnitude) > 0.1f && !isADS) {

            if (playerController.isGrounded && playerController.isSprinting && !playerController.isCrouching && !isADS) {

                timer += sprintBobSpeed * Time.deltaTime;
                transform.parent.localPosition = new Vector3(
                    transform.parent.transform.localPosition.x,
                    defaultYPosition + Mathf.Sin(timer) * sprintBobAmount,
                    transform.parent.transform.localPosition.z);

            } else if (playerController.isGrounded && !playerController.isSprinting && !playerController.isCrouching && !isADS) {

                timer += walkBobSpeed * Time.deltaTime;
                transform.parent.localPosition = new Vector3(
                    transform.parent.transform.localPosition.x,
                    defaultYPosition + Mathf.Sin(timer) * walkBobAmount,
                    transform.parent.transform.localPosition.z);

            } else if (playerController.isGrounded && playerController.isCrouching && !isADS) {

                timer += crouchBobSpeed * Time.deltaTime;
                transform.parent.localPosition = new Vector3(
                    transform.parent.transform.localPosition.x,
                    defaultYPosition + Mathf.Sin(timer) * crouchBobAmount,
                    transform.parent.transform.localPosition.z);

            } else if (playerController.isGrounded && !playerController.isCrouching && isADS) {

                timer += ADSBobSpeed * Time.deltaTime;
                transform.parent.localPosition = new Vector3(
                    transform.parent.transform.localPosition.x,
                    defaultYPosition + Mathf.Sin(timer) * ADSBobAmount,
                    transform.parent.transform.localPosition.z);

            } else if (playerController.isGrounded && playerController.isCrouching && isADS) {

                timer += ADSCrouchBobSpeed * Time.deltaTime;
                transform.parent.localPosition = new Vector3(
                    transform.parent.transform.localPosition.x,
                    defaultYPosition + Mathf.Sin(timer) * ADSCrouchBobAmount,
                    transform.parent.transform.localPosition.z);

            }
        }
    }

    private void HandleRecoil() {

        if (isADS) {

            rotationalRecoil += new Vector3(-ADSRecoilRotation.x,
                Random.Range(-ADSRecoilRotation.y, ADSRecoilRotation.y),
                Random.Range(-ADSRecoilRotation.z, ADSRecoilRotation.z));
            positionalRecoil += new Vector3(Random.Range(-ADSRecoilKickback.x, ADSRecoilKickback.x),
                Random.Range(-ADSRecoilKickback.y, ADSRecoilKickback.y), ADSRecoilKickback.z);

        } else {

            rotationalRecoil += new Vector3(-recoilRotation.x,
                Random.Range(-recoilRotation.y, recoilRotation.y),
                Random.Range(-recoilRotation.z, recoilRotation.z));
            positionalRecoil += new Vector3(Random.Range(-recoilKickback.x, recoilKickback.x),
                Random.Range(-recoilKickback.y, recoilKickback.y), recoilKickback.z);

        }
    }

    IEnumerator ShootGun() {

        FindObjectOfType<AudioManager>().PlaySound("Shoot");

        RaycastHit hit;
        Debug.DrawRay(playerController.camera.ScreenToWorldPoint(crosshair.GetComponent<RectTransform>().position), transform.forward);

        if (Physics.Raycast(playerController.camera.ScreenToWorldPoint(crosshair.GetComponent<RectTransform>().position),
            transform.forward, out hit)) {

            Destroy(Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal + new Vector3(0f, 0.1f, 0f))), 3);

        }

        // CalculateRecoil();
        StartCoroutine(MuzzleFlash());

        yield return new WaitForSeconds(1f / fireRate);
        canShoot = true;

    }

    IEnumerator MuzzleFlash() {

        muzzleFlash.sprite = flashes[Random.Range(0, flashes.Length)];
        muzzleFlash.color = Color.white;
        yield return new WaitForSeconds(muzzleFlashLength);
        muzzleFlash.sprite = null;
        muzzleFlash.color = new Color(0, 0, 0, 0);

    }
}
