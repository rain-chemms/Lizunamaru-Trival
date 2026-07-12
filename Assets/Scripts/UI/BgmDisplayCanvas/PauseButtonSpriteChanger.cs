using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BgmPauseButton))]
public class PauseButtonSpriteChanger : MonoBehaviour
{
    [SerializeField] private BgmPauseButton bgmPauseButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (bgmPauseButton == null) bgmPauseButton = GetComponent<BgmPauseButton>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckChangeButtonSprite();
    }

    [SerializeField] private Sprite pauseSprite;
    public void SetPauseSprite(Sprite sprite)
    {
        pauseSprite = sprite;
    }
    public Sprite GetPauseSprite()
    {
        return pauseSprite;
    }
    [SerializeField] private Sprite resumeSprite;
    public void SetResumeSprite(Sprite sprite)
    {
        resumeSprite = sprite;
    }
    public Sprite GetResumeSprite()
    {
        return resumeSprite;
    }
    public void CheckChangeButtonSprite()
    {
        if (bgmPauseButton == null) return;
        Button bt = bgmPauseButton.GetButton();
        if (bt != null)
        {
            if (!bgmPauseButton.IsPause())
            {
                bt.image.sprite = pauseSprite;
            }
            else
            {
                bt.image.sprite = resumeSprite;
            }
        }
    }
}
