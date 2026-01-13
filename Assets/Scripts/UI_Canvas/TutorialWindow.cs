using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialWindow : MonoBehaviour
{
    [SerializeField] private Transform background;
    [SerializeField] private Transform tutorial;

    [SerializeField] private Transform[] parts;
    [SerializeField] private GameObject[] guides;
    [SerializeField] private Pagination[] paginations;
    [SerializeField] private PopupGuide[] popups;
    [SerializeField] private GameObject previousButton;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject startButton;

    private int siblingIndex => tutorial.GetSiblingIndex() - 1;

    private bool isUsing;
    private int usedPopupCount = 0;
    private int stage = 0;
    private List<int> indexOrigins = new List<int>();
    private Queue<PopupGuide> popupQueue = new Queue<PopupGuide>();

    private Coroutine coroutine;

    private WaitForSeconds WaitForMoment = new WaitForSeconds(0.1f);
    public void Init(bool isUsing)
    {
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

        //
        if (isUsing)
        {
            // 스와이프 이벤트 연결
            if (background.TryGetComponent<SwipeDetector>(out SwipeDetector swipeDetector))
            {
                swipeDetector.ToLeft += ShowPrevious;
                swipeDetector.ToRight += ShowNext;
            }

            // 첫 스테이지 활성화
            parts[stage].SetSiblingIndex(siblingIndex);
            guides[stage].SetActive(true);

            // 버튼 상태 업데이트
            previousButton.SetActive(false);
            nextButton.SetActive(true);
            startButton.SetActive(false);

            // 페이지네이션 업데이트
            for (int i = 0; i < paginations.Length; i++)
            {
                paginations[i].SetValue(i == stage);
            }
        }

        background.gameObject.SetActive(isUsing);
        tutorial.gameObject.SetActive(isUsing);

        this.isUsing = isUsing;
    }
    public void OnReward(RewardInfo rewardInfo)
    {
        // 보상 최초 획득시 팝업 노출
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

            // 튜토리얼 보기 설정 바뀌었을 수 있으므로 큐에 넣거나 바로 카운트 증가
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
        ShowNext();
    }
    public void OnClickPrevious()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);
        ShowPrevious();
    }
    public void OnClickStart()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);
        GameCtrl.Instance.OnGameStart();
    }
    public void OnGameStart()
    {
        if (isUsing)
        {
            // 현재 스테이지 비활성화
            parts[stage].SetSiblingIndex(indexOrigins[stage]);
            guides[stage].SetActive(false);

            // 전체 튜토리얼 비활성화
            background.gameObject.SetActive(false);
            tutorial.gameObject.SetActive(false);

            // 캐릭터별 팝업 큐에 추가
            popupQueue.Enqueue(popups[StaticValues.playerCharacterNum]);
            coroutine = StartCoroutine(RewardGuideCor());
        }
        else
        {
            usedPopupCount++;
        }
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
    private void ShowNext()
    {
        if (stage < guides.Length - 1)
        {
            // 현재 스테이지 비활성화
            parts[stage].SetSiblingIndex(indexOrigins[stage]);
            guides[stage++].SetActive(false);

            // 다음 스테이지 활성화
            parts[stage].SetSiblingIndex(siblingIndex);
            guides[stage].SetActive(true);

            // 페이지네이션 업데이트
            for (int i = 0; i < paginations.Length; i++)
            {
                paginations[i].SetValue(i == stage);
            }

            // 버튼 상태 업데이트
            previousButton.SetActive(true);
            nextButton.SetActive(stage < parts.Length - 1);
            startButton.SetActive(stage == parts.Length - 1);
        }
    }
    private void ShowPrevious()
    {
        if (stage > 0)
        {
            // 현재 스테이지 비활성화
            parts[stage].SetSiblingIndex(indexOrigins[stage]);
            guides[stage--].SetActive(false);

            // 이전 스테이지 활성화
            parts[stage].SetSiblingIndex(siblingIndex);
            guides[stage].SetActive(true);

            // 페이지네이션 업데이트
            for (int i = 0; i < paginations.Length; i++)
            {
                paginations[i].SetValue(i == stage);
            }

            // 버튼 상태 업데이트
            previousButton.SetActive(stage > 0);
            nextButton.SetActive(true);
            startButton.SetActive(false);
        }
    }
    private IEnumerator RewardGuideCor()
    {
        // 팝업 위치 리스트 생성
        List<int> all = new List<int>();
        for (int i = 0; i < popups.Length; i++)
        {
            all.Add(i);
        }

        while (true)
        {
            // 모든 팝업이 사용된 경우 종료
            if (usedPopupCount == popups.Length)
            {
                break;
            }

            // 큐에 대기중인 팝업이 있으면 출력
            if (popupQueue.Count > 0)
            {
                PopupGuide popupGuide = popupQueue.Dequeue();

                List<int> samples = new List<int>();
                samples.AddRange(all);

                // 이미 활성화된 팝업의 위치는 제외
                foreach (PopupGuide guide in popups.Where(x => x.gameObject.activeSelf))
                {
                    samples.Remove(guide.positionId);
                }

                // 남은 위치 중에서 가장 앞 쪽에 팝업 출력
                popupGuide.Show(samples.Min());

                // 사용된 팝업 카운트 증가
                usedPopupCount++;
                if (usedPopupCount == popups.Length)
                {
                    PlayerPrefs.SetInt(PlayerPrefKeys.VISIBLE_TUTORIAL, 0);
                }
            }

            yield return WaitForMoment;
        }

        yield break;
    }
}