using DG.Tweening;
using System;
using UnityEngine;

public class ScreenSwitcher : MonoBehaviour
{
    [SerializeField] private SpriteMask mask;
    [SerializeField] private Sprite[] maskTextures;

    private Transform maskTransform;

    private Vector3 maskScaleMax = Vector3.one * 60;
    private Vector3 maskScaleMin = Vector3.zero;

    private Sequence seq;
    private void Awake()
    {
        maskTransform = mask.transform;
        maskTransform.localScale = maskScaleMin;
    }
    public void Show(Action preprocess, Action postprocess)
    {
        mask.sprite = maskTextures[StaticValues.playerCharacterNum];
        maskTransform.localScale = maskScaleMin;

        if (seq != null)
        {
            seq?.Kill();
        }
        seq = DOTween.Sequence();
        
        seq.AppendCallback(() => preprocess?.Invoke());
        seq.Append(maskTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
        seq.AppendInterval(0.25f);
        seq.Append(maskTransform.DOScale(maskScaleMax, 1f));
        seq.AppendCallback(() => postprocess?.Invoke());
    }
    public void Hide(Action preprocess, Action postprocess)
    {
        mask.sprite = maskTextures[StaticValues.playerCharacterNum];
        maskTransform.localScale = maskScaleMax;

        if (seq != null)
        {
            seq?.Kill();
        }
        seq = DOTween.Sequence();

        seq.AppendCallback(() => preprocess?.Invoke());
        seq.Append(maskTransform.DOScale(Vector3.one, 1f).SetEase(Ease.InOutCubic));
        seq.AppendInterval(0.25f);
        seq.Append(maskTransform.DOScale(maskScaleMin, 0.25f));
        seq.AppendCallback(() => postprocess?.Invoke());
    }
}
