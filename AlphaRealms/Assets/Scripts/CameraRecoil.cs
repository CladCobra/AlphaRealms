using UnityEngine;

public class CameraRecoil : MonoBehaviour {

    [Header("Developer Testing")]
    [SerializeField] private bool cameraRotationEnabled;

    [Header("References")]
    [SerializeField] private GunController gunController;
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    private void Update() {

        if (cameraRotationEnabled) {

            targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, gunController.returnSpeed * Time.deltaTime);
            currentRotation = Vector3.Slerp(currentRotation, targetRotation, gunController.snappiness * Time.fixedDeltaTime);
            transform.localRotation = Quaternion.Euler(currentRotation);

        }
    }

    public void HandleRecoil() {

        if (gunController.isADS) {

            gunController.positionalRecoil += new Vector3(Random.Range(-gunController.ADSRecoilKickbackX, gunController.ADSRecoilKickbackX),
                Random.Range(-gunController.ADSRecoilKickbackY, gunController.ADSRecoilKickbackY),
                gunController.ADSRecoilKickbackZ);

            targetRotation += new Vector3(gunController.ADSRecoilX,
                Random.Range(-gunController.ADSRecoilY, gunController.ADSRecoilY),
                Random.Range(0, gunController.ADSRecoilZ));

        } else {

            gunController.positionalRecoil += new Vector3(Random.Range(-gunController.recoilKickbackX, gunController.recoilKickbackX),
                Random.Range(-gunController.recoilKickbackY, gunController.recoilKickbackY),
                gunController.recoilKickbackZ);

            targetRotation += new Vector3(gunController.recoilX,
                Random.Range(-gunController.recoilY, gunController.recoilY),
                Random.Range(0, gunController.recoilZ));

        }
    }
}
