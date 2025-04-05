using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UpgradeManager : MonoBehaviour
{
    [Header("Referências")]
    public SubmarineStats stats;

    [Header("UI")]
    public TMP_Text goldText;
    public TMP_Text speedText;
    public TMP_Text miningSpeedText;
    public TMP_Text miningDistanceText;
    public TMP_Text lightDistanceText;
    public TMP_Text maxDepthText;

    [Header("Custos de Upgrade")]
    public int speedUpgradeCost = 5;
    public int miningSpeedUpgradeCost = 6;
    public int miningDistanceUpgradeCost = 4;
    public int lightDistanceUpgradeCost = 3;
    public int maxDepthUpgradeCost = 7;

    [Header("Valores de Upgrade")]
    public float speedUpgradeAmount = 0.1f;
    public float miningSpeedUpgradeAmount = -0.2f; // menor = mais rápido
    public float miningDistanceUpgradeAmount = 2f;
    public float lightDistanceUpgradeAmount = 5f;
    public float maxDepthUpgradeAmount = -10f; // mais negativo = mais profundo

    private void Start()
    {
        UpdateUI();
    }

    public void UpgradeSpeed()
    {
        if (TrySpendGold(speedUpgradeCost))
        {
            stats.speed += speedUpgradeAmount;
            UpdateUI();
        }
    }

    public void UpgradeMiningSpeed()
    {
        if (TrySpendGold(miningSpeedUpgradeCost))
        {
            stats.miningSpeed = Mathf.Max(0.2f, stats.miningSpeed + miningSpeedUpgradeAmount);
            UpdateUI();
        }
    }

    public void UpgradeMiningDistance()
    {
        if (TrySpendGold(miningDistanceUpgradeCost))
        {
            stats.miningDistance += miningDistanceUpgradeAmount;
            UpdateUI();
        }
    }

    public void UpgradeLightDistance()
    {
        if (TrySpendGold(lightDistanceUpgradeCost))
        {
            stats.lightDistance += lightDistanceUpgradeAmount;
            UpdateUI();
        }
    }

    public void UpgradeMaxDepth()
    {
        if (TrySpendGold(maxDepthUpgradeCost))
        {
            stats.maxDepth += maxDepthUpgradeAmount; // deve ser negativo para descer mais
            UpdateUI();
        }
    }

    private bool TrySpendGold(int cost)
    {
        if (stats.gold >= cost)
        {
            stats.gold -= cost;
            return true;
        }

        Debug.Log("❌ Ouro insuficiente para esse upgrade.");
        return false;
    }

    private void UpdateUI()
    {
        goldText.text = $"Gold: {stats.gold}";
        speedText.text = $"Speed: {stats.speed:F1}";
        miningSpeedText.text = $"Mining Speed: {stats.miningSpeed:F1}s";
        miningDistanceText.text = $"Mining Range: {stats.miningDistance}";
        lightDistanceText.text = $"Light Range: {stats.lightDistance}";
        maxDepthText.text = $"Max Depth: {stats.maxDepth}";
    }

    public void ReturnToGameScene()
    {
        SceneManager.LoadScene("IgorTest");
    }
}
