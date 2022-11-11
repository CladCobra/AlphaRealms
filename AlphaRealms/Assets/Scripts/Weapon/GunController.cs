using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour {

    [Header("Developer Testing")]
    [SerializeField] private bool oldRecoilEnabled;
    [SerializeField] private bool newRecoilEnabled;

    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CameraRecoil cameraRecoil;
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject bulletTrail;
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
    [Range(0f, 25f)] public float positionalRecoilSpeed;
    [Range(0f, 25f)] public float positionalReturnSpeed;
    [Range(0f, 10f)] public float snappiness;
    [Range(0f, 10f)] public float returnSpeed;
    [HideInInspector] public Vector3 positionalRecoil;

    [Header("Hipfire Recoil")]
    public float recoilX;
    public float recoilY;
    public float recoilZ;
    public float recoilKickbackX;
    public float recoilKickbackY;
    public float recoilKickbackZ;

    [Header("ADS Recoil")]
    public float ADSRecoilX;
    public float ADSRecoilY;
    public float ADSRecoilZ;
    public float ADSRecoilKickbackX;
    public float ADSRecoilKickbackY;
    public float ADSRecoilKickbackZ;

    [Header("Muzzle Flash")]
    [SerializeField] private ParticleSystem muzzleFlash;

    [Header("Bullet Impact")]
    [SerializeField] private GameObject bulletImpact;
    [SerializeField][Range(0f, 25f)] private float bulletImpactDestroyTime;

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
        recoilPosition.localPosition = Vector3.Slerp(recoilPosition.localPosition, positionalRecoil, positionalRecoilSpeed * Time.fixedDeltaTime);

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

    private IEnumerator ShootGun() {

        FindObjectOfType<AudioManager>().PlaySound("Shoot");

        if (oldRecoilEnabled) {

            CalculateRecoil();

        }

        if (newRecoilEnabled) {

            cameraRecoil.HandleRecoil();

        }

        RaycastHit hit;

        if (Physics.Raycast(playerController.camera.ScreenToWorldPoint(crosshair.GetComponent<RectTransform>().position),
            transform.forward, out hit)) {

            Vector3 hitPoint = hit.point + (transform.localPosition - (isADS ? ADSPoint.localPosition : initialPosition));

            GameObject newBulletTrail = Instantiate(bulletTrail, muzzle.position, Quaternion.identity);
            StartCoroutine(SpawnBulletTrail(newBulletTrail.GetComponent<TrailRenderer>(), hitPoint));

            GameObject newBulletImpact = Instantiate(bulletImpact, hitPoint, Quaternion.LookRotation(hit.normal + new Vector3(0f, 0.1f, 0f)));
            Destroy(newBulletImpact, bulletImpactDestroyTime);

        }

        muzzleFlash.Play();

        yield return new WaitForSeconds(1f / fireRate);
        canShoot = true;

    }

    private IEnumerator SpawnBulletTrail(TrailRenderer trailRenderer, Vector3 targetPosition) {

        float time = 0;
        Vector3 startPosition = trailRenderer.transform.position;

        while (time < 1) {

            trailRenderer.transform.position = Vector3.Lerp(startPosition, targetPosition, time);
            time += Time.deltaTime / trailRenderer.time;

            yield return null;

        }
    }
}
