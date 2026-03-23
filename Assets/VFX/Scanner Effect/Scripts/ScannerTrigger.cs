using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ScannerTrigger : MonoBehaviour
{
    [SerializeField] private float _expansionSpeed = 5f;
    [SerializeField] private float _maxRadius = 30f;
    [SerializeField] private ParticleSystem _scannerVFX;

    public Action<GameObject> OnEnemyDetected;

    private SphereCollider scannerCollider;
    private Material highlightMaterial;
    private bool scanning = false;

    private readonly HashSet<GameObject> enemiesDetected = new();

    private void Awake()
    {
        scannerCollider = GetComponent<SphereCollider>();
        scannerCollider.isTrigger = true;
        scannerCollider.radius = 0f;
    }

    private void Update()
    {
        if (!scanning) return;

        scannerCollider.radius += _expansionSpeed * Time.deltaTime;

        if (scannerCollider.radius >= _maxRadius)
            StopScan();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!scanning) return;
        if (enemiesDetected.Contains(other.gameObject)) return;

        IDetectable enemy = other.gameObject.GetComponent<IDetectable>();

        if (enemy != null)
            enemy.TriggerHightlight();

        enemiesDetected.Add(other.gameObject);

        OnEnemyDetected?.Invoke(other.gameObject);
    }

    public void SetupScanner(float scannerRanger, float duration)
    {
        var mainModule = _scannerVFX.main;

        mainModule.startSize = scannerRanger;
        _maxRadius = scannerRanger / 2f;
        _expansionSpeed = _maxRadius / duration;
    }

    public void StartScan()
    {
        scannerCollider.radius = 0f;
        enemiesDetected.Clear();
        _scannerVFX.Play();
        scanning = true;
    }

    public void StopScan()
    {
        scanning = false;
        _scannerVFX.Stop();
    }

    public void SetHighlightMaterial(Material target)
    {
        highlightMaterial = target;
    }
}
