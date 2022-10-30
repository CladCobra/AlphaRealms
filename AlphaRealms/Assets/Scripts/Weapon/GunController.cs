using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private PlayerController playerController;

    [Header("Settings")]
    [SerializeField] private float fireRate;
    [SerializeField] private int magazineSize;
    [SerializeField] private int reservedAmmoCapacity;

    [Header("Shooting")]
    public bool isAutomatic;
    private int currentAmmo;
    private int reservedAmmo;
    private bool canShoot;

    [Header("Aiming")]
    [SerializeField] private Transform ADSPoint;
    [SerializeField] private float ADSSmoothing;
    private Vector3 initialPosition;
    private bool isADS;

    [Header("Sway")]
    [SerializeField] private float swayAmount;

    [Header("Muzzle Flash")]
    [SerializeField] private Image muzzleFlash;
    [SerializeField] private Sprite[] flashes;
    [SerializeField] private float muzzleFlashLength;

    private void Start() {

        initialPosition = transform.localPosition;

        currentAmmo = magazineSize;
        reservedAmmo = reservedAmmoCapacity;
        canShoot = true;
        isADS = false;

    }

    private void Update() {

        CalculateSway();
        ADS();

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

    public void ADS() {

        Vector3 target = (isADS ? ADSPoint.localPosition : initialPosition);

        Vector3 targetPosition = Vector3.Lerp(transform.localPosition, target, ADSSmoothing * Time.deltaTime);
        transform.localPosition = targetPosition;

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

    private void CalculateSway() {

        transform.localPosition += (Vector3) playerController.mouseInput * swayAmount / 1000;

    }

    private void CalculateRecoil() {

        transform.localPosition -= Vector3.forward * 0.1f;

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
