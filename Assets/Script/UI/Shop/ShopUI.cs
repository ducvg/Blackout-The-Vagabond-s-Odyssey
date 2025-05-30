using UnityEngine;
using Zenject.Asteroids;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }

    public bool isOpenShop {  get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            gameObject.SetActive(false);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Show()
    {
        isOpenShop = true;
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Hide()
    {
        isOpenShop = false;
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    
}
