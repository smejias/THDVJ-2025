using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bulletsCounter;
    [SerializeField] private GameObject clipIconPrefab;
    [SerializeField] private Transform clipsContainer;
    [SerializeField] private GunController gunController;
    [SerializeField] private TextMeshProUGUI reloadLabel;

    private List<GameObject> clipIcons = new();

    private void Start()
    {
        if (gunController != null)
        {
            gunController.OnAmmoChanged += UpdateUI;
            gunController.OnReloadStateChanged += ToggleReloadLabel;
            CreateClipIcons();
            UpdateUI();
        }

        if (reloadLabel != null)
            reloadLabel.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (gunController != null)
        {
            gunController.OnAmmoChanged -= UpdateUI;
            gunController.OnReloadStateChanged -= ToggleReloadLabel;
        }
    }

    private void UpdateUI()
    {
        if (gunController == null) return;

        bulletsCounter.text = $"{gunController.BulletsLeftInClip}/{gunController.ClipSize}";

        for (int i = 0; i < clipIcons.Count; i++)
            clipIcons[i].SetActive(i < gunController.ClipsLeft);
    }

    private void CreateClipIcons()
    {
        if (clipIconPrefab == null || clipsContainer == null) return;

        for (int i = 0; i < gunController.ClipsLeft + 1; i++)
        {
            GameObject icon = Instantiate(clipIconPrefab, clipsContainer);
            clipIcons.Add(icon);
        }
    }

    private void ToggleReloadLabel(bool isReloading)
    {
        if (reloadLabel != null)
            reloadLabel.gameObject.SetActive(isReloading);
    }
}
