using UnityEngine;

public class SoundManagerTester : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SampleSoundManager.Instance.PlayBgm(BgmType.BGM1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SampleSoundManager.Instance.PlayBgm(BgmType.BGM2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SampleSoundManager.Instance.StopBgm();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SampleSoundManager.Instance.PlaySe(SeType.SE1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SampleSoundManager.Instance.PlaySe(SeType.SE2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SampleSoundManager.Instance.PlaySe(SeType.SE3);
        }
    }
}
