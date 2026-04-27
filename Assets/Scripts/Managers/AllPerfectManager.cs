using UnityEngine;
#nullable enable

public class AllPerfectManager : MonoBehaviour
{
    [SerializeField]
    GameObject AllPerfect;
    
    private void Awake()
    {
        Majdata<AllPerfectManager>.Instance = this;
    }
    
    private void Start()
    {
        AllPerfect.SetActive(false);
    }
    
    private void Update()
    {
        if (PlayManager.Summary.State is not ViewStatus.Playing)
            return;
        
        if (Majdata<ObjectCounter>.Instance!.AllFinished && AllPerfect) 
            AllPerfect.SetActive(true);
    }

    public void ResetState()
    { 
        AllPerfect.SetActive(false);
    }
}