using System;
using UnityEngine;

public class JournalCamButton : MonoBehaviour
{
    public int animalIndexPhoto;

    //public static event Action<int> OnAnimalPhotoRequested;
    //public static event Action<bool> OnPhotoModeStarted;
    //public static event Action<bool> OnPhotoUiTriggered;

    public void StartTakePhoto() // Trigger to open photo mode at animal foto in journal ui and request the photo of the animal based on the index
    {
        //OnAnimalPhotoRequested?.Invoke(animalIndexPhoto);
        //OnPhotoModeStarted?.Invoke(true);
        //OnPhotoUiTriggered?.Invoke(true);
    }
}
