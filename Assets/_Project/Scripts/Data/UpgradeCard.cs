using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeCard", menuName = "Game/UpgradeCard")]
public class UpgradeCard : ScriptableObject
{
    [SerializeField] private string _title;
    [SerializeField] private StatType _stat;
    [SerializeField] private float _amount;

    public string Title => _title;
    public StatType Stat => _stat;
    public float Amount => _amount;
}
