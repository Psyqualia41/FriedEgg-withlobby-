using System.Collections;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Weapon : MonoBehaviour
{
    [Header("Stats")]
    public int damage;
    public float fireRate;

    [Header("References")]
    public Camera weaponCamera;                // was "camera"
    public Animation weaponAnimation;          // was "animation"
    public AnimationClip reload;

    [Header("VFX")]
    public GameObject hitVFX;

    [Header("Ammo")]
    public int mag = 5;
    public int ammo = 30;
    public int magAmmo = 30;

    [Header("Projectile")]
    public Transform firePoint;

    private float nextFire;

    [Header("UI")]
    public TextMeshProUGUI magText;
    public TextMeshProUGUI ammoText;

    [Header("Tracer")]
    public GameObject tracerPrefab;
    public float tracerDuration = 0.05f;

    [Header("Recoil Settings")]
    [Range(0, 2)] public float recoverPercent = 0.7f;
    public float recoilUp = 1f;
    public float recoilBack = 0f;

    private Vector3 originalPosition;
    private Vector3 recoilVelocity = Vector3.zero;
    private float recoilLength;
    private float recoverLength;
    private bool recoiling;
    private bool recovering;

    void Awake()
    {
        if (firePoint == null)
        {
            firePoint = transform.Find("FirePoint");
            if (firePoint == null)
            {
                Debug.LogError("FirePoint not assigned");
            }
        }
    }

    void Start()
    {
        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;

        originalPosition = transform.localPosition;

        recoilLength = 0;
        recoverLength = 1 / fireRate * recoverPercent;
    }

    void Update()
    {
        if (nextFire > 0)
            nextFire -= Time.deltaTime;

        if (Input.GetButton("Fire1") && nextFire <= 0 && ammo > 0 && !weaponAnimation.isPlaying)
        {
            nextFire = 1 / fireRate;
            ammo--;

            magText.text = mag.ToString();
            ammoText.text = ammo + "/" + magAmmo;

            Fire();
        }

        if (Input.GetKeyDown(KeyCode.R) && mag > 0)
        {
            Reload();
        }

        if (recoiling) Recoil();
        if (recovering) Recovering();
    }

    void Reload()
    {
        weaponAnimation.Play(reload.name);
        if (mag > 0)
        {
            mag--;
            ammo = magAmmo;
        }

        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;
    }

    void Fire()
    {
        recoiling = true;
        recovering = false;

        Ray ray = new Ray(weaponCamera.transform.position, weaponCamera.transform.forward);
        RaycastHit hit;
        Vector3 hitPoint = ray.origin + ray.direction * 100f;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);

            var health = hit.transform.gameObject.GetComponent<Health>();
            if (health)
            {
                hit.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
            }

            hitPoint = hit.point;
        }

        if (firePoint == null)
        {
            Debug.LogError("FirePoint missing.");
            return;
        }

        StartCoroutine(SpawnTracer(firePoint.position, hitPoint));
    }

    IEnumerator SpawnTracer(Vector3 start, Vector3 end)
    {
        GameObject tracer = Instantiate(tracerPrefab, start, Quaternion.identity);

        float distance = Vector3.Distance(start, end);
        float speed = 200f;
        float travelTime = distance / speed;
        float elapsed = 0f;

        while (elapsed < travelTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / travelTime;
            tracer.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        tracer.transform.position = end;
        Destroy(tracer, tracer.GetComponent<TrailRenderer>().time);
    }

    void Recoil()
    {
        Vector3 finalPosition = new Vector3(originalPosition.x, originalPosition.y + recoilUp, originalPosition.z - recoilBack);
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoilLength);

        if (transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = true;
        }
    }

    void Recovering()
    {
        Vector3 finalPosition = originalPosition;
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoverLength);

        if (transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = false;
        }
    }
}
