using UnityEngine;

[CreateAssetMenu(fileName = "New Animal Data", menuName = "WildGuard/Animal Data")]
public class SODataHewan : ScriptableObject
{
    public int animalID;
    public string animalName;
    [TextArea(5, 10)]
    public string animalDescription;
    public Sprite animalSprite;
}


