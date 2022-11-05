using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private PlayerController playerController;

    [Header("Settings")]
    [SerializeField][Range(0, 25)] private float fireRate;
    [SerializeField][Range(0, 999)] private int magazineSize;
    [SerializeField][Range(0, 999)] private int reservedAmmoCapacity;

    [Header("Shooting")]
    public bool isAutomatic;
    private int currentAmmo;
    private int reservedAmmo;
    private bool canShoot;

    [Header("ADS")]
    [SerializeField] private Transform ADSPoint;
    [SerializeField][Range(0, 10)] private float ADSSmoothing;
    [HideInInspector] public bool isADS;
    private Vector3 initialPosition;

    [Header("Jump Effect")]
    [SerializeField][Range(0, 10)] private float jumpEffectSmoothing;

    [Header("Sway")]
    [SerializeField][Range(-10, 10)] private float swayAmount;

    [Header("Weapon Bobbing")]
    [SerializeField][Range(0, 50)] private float walkBobSpeed;
    [SerializeField][Range(0, 10)] private float walkBobAmount;
    [SerializeField][Range(0, 50)] private float sprintBobSpeed;
    [SerializeField][Range(0, 10)] private float sprintBobAmount;
    [SerializeField][Range(0, 50)] private float crouchBobSpeed;
    [SerializeField][Range(0, 10)] private float crouchBobAmount;
    [SerializeField][Range(0, 50)] private float ADSBobSpeed;
    [SerializeField][Range(0, 10)] private float ADSBobAmount;
    [SerializeField][Range(0, 50)] private float ADSCrouchBobSpeed;
    [SerializeField][Range(0, 10)] private float ADSCrouchBobAmount;
    private float defaultYPosition;
    private float timer;

    [Header("Muzzle Flash")]
    [SerializeField] private Image muzzleFlash;
    [SerializeField] private Sprite[] flashes;
    [SerializeField][Range(0, 1)] private float muzzleFlashLength;

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

    public void Shoot() {

        if (canShoot && currentAmmo > 0) {

            canShoot = false;
            currentAmmo--;
            StartCoroutine(ShootGun());

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

    IEnumerator ShootGun() {

        CalculateRecoil();
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
