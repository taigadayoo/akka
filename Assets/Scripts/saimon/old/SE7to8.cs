using System.Collections;
using UnityEngine;

public class SE7to8 : MonoBehaviour
{
    public GameObject TargetObject; 
    public GameObject TargetObject1;
    public GameObject TargetObject2;
    public GameObject TargetObject3;  
    public GameObject TargetObject4; 
    public GameObject TargetObject5; 
    public GameObject TargetObject6; 
    public GameObject TargetObject7; 
    public GameObject TargetObject8; 
    public GameObject TargetObject9; 
    public AudioClip SoundEffect;
    public AudioClip SoundEffect1;
    private AudioSource _audioSource;
    private AudioSource _audioSource1;
    public Canvas Canvas;

    private bool _hasPlayed = false;   // SEが再生されたかどうかをチェックするフラグ

    void Start()
    {
        // AudioSourceの取得
        _audioSource = GetComponent<AudioSource>();
        _audioSource1 = GetComponent<AudioSource>();

        // AudioSourceがなければ追加
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        if (_audioSource1 == null)
        {
            _audioSource1 = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            // ゲームオブジェクトがキャンバス上に表示されている場合
            if (IsObjectVisible(TargetObject) && !_hasPlayed)
            {
                // SEを再生
                _audioSource.PlayOneShot(SoundEffect);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録
            }
            if (IsObjectVisible(TargetObject1) && !_hasPlayed)
            {
                // SEを再生
                _audioSource.PlayOneShot(SoundEffect);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録
            }
            if (IsObjectVisible(TargetObject2) && !_hasPlayed)
            {
                // SEを再生
                _audioSource.PlayOneShot(SoundEffect);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録
            }
            if (IsObjectVisible(TargetObject3) && !_hasPlayed)
            {
                // SEを再生
                _audioSource.PlayOneShot(SoundEffect);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録
            }
            if (IsObjectVisible(TargetObject4) && !_hasPlayed)
            {
                // SEを再生
                _audioSource.PlayOneShot(SoundEffect);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録

            }
            if (IsObjectVisible(TargetObject5) && !_hasPlayed)
            {
                // SEを再生
                _audioSource.PlayOneShot(SoundEffect);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録

            }
            if (IsObjectVisible(TargetObject6) && !_hasPlayed)
            {
                // SEを再生
                _audioSource.PlayOneShot(SoundEffect);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録

            }
            if (IsObjectVisible(TargetObject7) && !_hasPlayed)
            {
                // SEを再生
                _audioSource.PlayOneShot(SoundEffect);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録

            }
            if (IsObjectVisible(TargetObject8) && !_hasPlayed)
            {
                // SEを再生
                _audioSource.PlayOneShot(SoundEffect);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録

            }
            if (IsObjectVisible(TargetObject9) && !_hasPlayed)
            {
                // SEを再生
                _audioSource.PlayOneShot(SoundEffect);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録

            }
            else
            {
                // SEを再生
                _audioSource1.PlayOneShot(SoundEffect1);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録
            }
        }

        if (Input.GetKeyDown(KeyCode.Joystick2Button1))
        {
            // ゲームオブジェクトがキャンバス上に表示されている場合
            if (IsObjectVisible(TargetObject) && !_hasPlayed)
            {
                // SEを再生
                _audioSource.PlayOneShot(SoundEffect);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録
            }
            if (IsObjectVisible(TargetObject1) && !_hasPlayed)
            {
                // SEを再生
                _audioSource.PlayOneShot(SoundEffect);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録
            }
            if (IsObjectVisible(TargetObject2) && !_hasPlayed)
            {
                // SEを再生
                _audioSource.PlayOneShot(SoundEffect);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録
            }
            if (IsObjectVisible(TargetObject3) && !_hasPlayed)
            {
                // SEを再生
                _audioSource.PlayOneShot(SoundEffect);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録
            }
            if (IsObjectVisible(TargetObject4) && !_hasPlayed)
            {
                // SEを再生
                _audioSource.PlayOneShot(SoundEffect);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録

            }
            if (IsObjectVisible(TargetObject5) && !_hasPlayed)
            {
                // SEを再生
                _audioSource.PlayOneShot(SoundEffect);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録

            }
            if (IsObjectVisible(TargetObject6) && !_hasPlayed)
            {
                // SEを再生
                _audioSource.PlayOneShot(SoundEffect);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録

            }
            if (IsObjectVisible(TargetObject7) && !_hasPlayed)
            {
                // SEを再生
                _audioSource.PlayOneShot(SoundEffect);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録

            }
            if (IsObjectVisible(TargetObject8) && !_hasPlayed)
            {
                // SEを再生
                _audioSource.PlayOneShot(SoundEffect);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録

            }
            if (IsObjectVisible(TargetObject9) && !_hasPlayed)
            {
                // SEを再生
                _audioSource.PlayOneShot(SoundEffect);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録

            }
            else
            {
                // SEを再生
                _audioSource1.PlayOneShot(SoundEffect1);
                //Debug.Log("再生された");

                _hasPlayed = true;  // SEが再生されたことを記録
            }
        }
    }


    // RectTransformを使ってオブジェクトがキャンバス上に表示されているかチェックするメソッド
    bool IsObjectVisible(GameObject obj)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            // キャンバスのスクリーン座標系に変換して、オブジェクトがキャンバス内にあるかチェック
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Canvas.GetComponent<RectTransform>(), Input.mousePosition, Canvas.worldCamera, out localPoint);

            // オブジェクトのRectTransform内にカーソル（または画面位置）があるか確認
            return rectTransform.rect.Contains(localPoint);
        }

        // RectTransformがない場合は表示されていないとみなす
        return false;
    }
}
