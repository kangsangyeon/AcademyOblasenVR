using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class NetworkSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(NextScene());
    }

    private IEnumerator NextScene()
    {

        yield return new WaitForSeconds(3);

        if (PhotonNetwork.IsConnected)
        {
            // ���� ��Ʈ��ũ�� �����մϴ�.
            PhotonNetwork.Disconnect();
            yield return null;
        }

        yield return new WaitForSeconds(10);

        SceneManager.LoadScene("Credit");
    }
}
