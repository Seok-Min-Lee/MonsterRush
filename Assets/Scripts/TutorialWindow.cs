using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TutorialWindow : MonoBehaviour
{
    [SerializeField] private Transform background;
    [SerializeField] private Transform tutorial;

    [SerializeField] private Transform[] parts;
    [SerializeField] private GameObject[] guides;
    [SerializeField] private PopupGuide[] popups;
    [SerializeField] private GameObject previousButton;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject startButton;

    private int siblingIndex => tutorial.GetSiblingIndex() - 1;

    private bool isUsing;
    private int usedPopupCount = 0;
    private int stage;
    private List<int> indexOrigins = new List<int>();
    private Queue<PopupGuide> popupQueue = new Queue<PopupGuide>();

    private Coroutine coroutine;
    public void Init(bool isUsing)
    {
        //
        for (int i = 0; i < parts.Length; i++)
        {
            indexOrigins.Add(parts[i].GetSiblingIndex());
        }

        for (int i = 0; i < guides.Length; i++)
        {
            guides[i].SetActive(false);
        }

        for (int i = 0; i < popups.Length; i++)
        {
            popups[i].gameObject.SetActive(false);
        }

        background.gameObject.SetActive(false);
        tutorial.gameObject.SetActive(false);

        // 
        if (isUsing)
        {
            background.gameObject.SetActive(true);
            tutorial.gameObject.SetActive(true);

            parts[stage].SetSiblingIndex(siblingIndex);
            guides[stage].SetActive(true);

            previousButton.SetActive(false);
            nextButton.SetActive(true);
            startButton.SetActive(false);
        }

        this.isUsing = isUsing;
    }
    public void OnReward(RewardInfo rewardInfo)
    {
        if (rewardInfo.type == RewardInfo.Type.Weapon && rewardInfo.index == 0)
        {
            PopupGuide pg = null;
            switch (rewardInfo.groupId)
            {
                case 1000:
                    pg = popups[0];
                    break;
                case 2000:
                    pg = popups[1];
                    break;
                case 3000:
                    pg = popups[2];
                    break;
                case 4000:
                    pg = popups[3];
                    break;
            }

            if (isUsing && pg != null)
            {
                popupQueue.Enqueue(pg);
            }
            else
            {
                usedPopupCount++;
            }
        }
    }
    public void OnClickNext()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);

        parts[stage].SetSiblingIndex(indexOrigins[stage]);
        guides[stage++].SetActive(false);

        parts[stage].SetSiblingIndex(siblingIndex);
        guides[stage].SetActive(true);

        previousButton.SetActive(true);
        nextButton.SetActive(stage < parts.Length - 1);
        startButton.SetActive(stage == parts.Length - 1);
    }

    public void OnClickPrevious()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);

        parts[stage].SetSiblingIndex(indexOrigins[stage]);
        guides[stage--].SetActive(false);

        parts[stage].SetSiblingIndex(siblingIndex);
        guides[stage].SetActive(true);

        previousButton.SetActive(stage > 0);
        nextButton.SetActive(true);
        startButton.SetActive(false);
    }
    public void OnClickStart()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);
        GameCtrl.Instance.OnGameStart();

        parts[stage].SetSiblingIndex(indexOrigins[stage]);
        guides[stage].SetActive(false);

        background.gameObject.SetActive(false);
        tutorial.gameObject.SetActive(false);

        popupQueue.Enqueue(popups[StaticValues.playerCharacterNum]);
        coroutine = StartCoroutine(RewardGuideCor());
    }
    public void OnChangeUI(bool isLeftHand)
    {
        RectTransform joystickGuide = guides[0].transform.GetChild(0).GetComponent<RectTransform>();
        RectTransform toggleGuide = guides[3].transform.GetChild(0).GetComponent<RectTransform>();
        RectTransform popupGuide = popups[0].transform.parent.GetComponent<RectTransform>();

        if (isLeftHand)
        {
            joystickGuide.anchorMin = new Vector2(0, 0);
            joystickGuide.anchorMax = new Vector2(0, 0);
            joystickGuide.pivot = new Vector2(0, 0);
            joystickGuide.anchoredPosition = new Vector2(100, 400);

            toggleGuide.anchorMin = new Vector2(1, 0);
            toggleGuide.anchorMax = new Vector2(1, 0);
            toggleGuide.pivot = new Vector2(1, 0);
            toggleGuide.anchoredPosition = new Vector2(-32, 108);

            popupGuide.anchorMin = new Vector2(1, 0);
            popupGuide.anchorMax = new Vector2(1, 0);
            popupGuide.pivot = new Vector2(1, 0);
            popupGuide.anchoredPosition = new Vector2(-275, 500);
        }
        else
        {
            joystickGuide.anchorMin = new Vector2(1, 0);
            joystickGuide.anchorMax = new Vector2(1, 0);
            joystickGuide.pivot = new Vector2(1, 0);
            joystickGuide.anchoredPosition = new Vector2(-100, 400);

            toggleGuide.anchorMin = new Vector2(0, 0);
            toggleGuide.anchorMax = new Vector2(0, 0);
            toggleGuide.pivot = new Vector2(0, 0);
            toggleGuide.anchoredPosition = new Vector2(16, 108);

            popupGuide.anchorMin = new Vector2(0, 0);
            popupGuide.anchorMax = new Vector2(0, 0);
            popupGuide.pivot = new Vector2(0, 0);
            popupGuide.anchoredPosition = new Vector2(275, 500);
        }
    }
    public void OnChangeTutorial(bool value)
    {
        if (isUsing == value)
        {
            return;
        }
        isUsing = value;

        if (value)
        {
            coroutine = StartCoroutine(RewardGuideCor());
        }
        else
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }
    private IEnumerator RewardGuideCor()
    {
        while (true)
        {
            if (usedPopupCount == popups.Length)
            {
                break;
            }

            if (popupQueue.Count > 0)
            {
                PopupGuide popupGuide = popupQueue.Dequeue();
                int position = popups.Where(x => x.gameObject.activeSelf).Count();

                popupGuide.Show(position);
                usedPopupCount++;
            }

            yield return new WaitForSeconds(0.1f);
        }

        yield break;
    }
}