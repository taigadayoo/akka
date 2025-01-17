using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class SliderController : MonoBehaviour
{
    public Slider Slider; // アタッチするSlider
    public float Duration = 3f; // 3秒間で減少
    private Coroutine _valueDecreaseCoroutine;

    // Sliderのvalueを最大値にする関数
    public void SetValueToMax()
    {
        if (Slider != null)
        {
            Slider.value = Slider.maxValue;
        }
    }
    private void OnEnable()
    {
        SetValueToMax();
        StartValueDecrease();
    }
    // Sliderのvalueをduration秒かけて最大値から0に減少させる関数
    public void StartValueDecrease()
    {
        if (Slider != null)
        {
            // コルーチンが動作中の場合、停止する
            if (_valueDecreaseCoroutine != null)
            {
                StopCoroutine(_valueDecreaseCoroutine);
            }
            _valueDecreaseCoroutine = StartCoroutine(DecreaseValueOverTime());
        }
    }

    private IEnumerator DecreaseValueOverTime()
    {
        float elapsedTime = 0f;
        float startValue = Slider.value;

        while (elapsedTime < Duration)
        {
            elapsedTime += Time.deltaTime;
            Slider.value = Mathf.Lerp(startValue, 0f, elapsedTime / Duration);
            yield return null;
        }

        Slider.value = 0f; // 最後に完全に0に設定
    }
}