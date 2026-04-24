using UnityEngine;

[CreateAssetMenu(fileName = "New Journal Database", menuName = "WildGuard/Journal Database")]
public class SODataJournal : ScriptableObject
{
    public SODataHewan[] animalDatabase;
}