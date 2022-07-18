using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FileDownloader : MonoBehaviour
{
    public Text text;
    private string MyURL = "https://game-swyl.wyx.cn/ntxs/release2/res/ResRoot/ResData/gengxin3/model/gengxin3_model0.zip";

    UnityWebRequest request;
    Coroutine coroutine;

    //ʮ���ֽ�Ϊһ��
    double loadedBytes = 100000000;
    void Start()
    {
        Debug.Log(Application.persistentDataPath);

        coroutine = StartCoroutine(BreakpointResume(MyURL, Application.persistentDataPath + "/MP4/gengxin3_model0.zip"));

    }
    private void OnDestroy()
    {
        request?.Dispose();
        if(coroutine != null) StopCoroutine(coroutine);
    }

    /// <summary>
    /// �ֶΣ��ϵ������ļ�
    /// </summary>
    /// <param name="loadPath">���ص�ַ</param>
    /// <param name="savePath">����·��</param>
    /// <returns></returns>
    IEnumerator BreakpointResume(string loadPath, string savePath)
    {
        //UnityWebRequest �����ÿɴ��� HTTP HEAD ����� UnityWebRequest��
        UnityWebRequest headRequest = UnityWebRequest.Head(loadPath);
        //��ʼ��Զ�̷�����ͨ�š�
        yield return headRequest.SendWebRequest();

        if (!string.IsNullOrEmpty(headRequest.error))
        {
            Debug.LogError("��ȡ������Դ�ļ�");
            yield break;
        }
        //��ȡ�ļ��ܴ�С
        ulong totalLength = ulong.Parse(headRequest.GetResponseHeader("Content-Length"));
        Debug.Log("��ȡ��С" + totalLength);
        headRequest.Dispose();
        request = UnityWebRequest.Get(loadPath);
        //append����Ϊtrue�ļ�д�뷽ʽΪ����д�룬������ԭ�ļ���
        request.downloadHandler = new DownloadHandlerFile(savePath, true);
        //�����ļ�
        FileInfo file = new FileInfo(savePath);
        //��ǰ���ص��ļ�����
        ulong fileLength = (ulong)file.Length;

        //�����������ݴӵ�fileLength�������ֽڣ�
        request.SetRequestHeader("Range", "bytes=" + fileLength + "-");

        if (!string.IsNullOrEmpty(request.error))
        {
            Debug.LogError("����ʧ��" + request.error);
        }
        if (fileLength < totalLength)
        {
            request.SendWebRequest();
            while (!request.isDone)
            {
                double progress = (request.downloadedBytes + fileLength) / (double)totalLength;
                text.text = (progress * 100 + 0.01f).ToString("f2") + "%";
                // Debug.Log("������" + Request.downloadedBytes);
                //����һ�����ֽڹر����ڵ�Э�̣������µ�Э�̣�����Դ�ֶ�����

                yield return null;

                //if (request.downloadedBytes >= loadedBytes)
                //{
                    //StopCoroutine(coroutine);

                    //��� UnityWebRequest �ڽ����У���ֹͣ��
                    //request.Abort();
                    if (!string.IsNullOrEmpty(request.error))
                    {
                        Debug.LogError("����ʧ��" + request.error);
                    }

                    //request.Dispose();
                    //yield return null;
                    

                    //yield return StartCoroutine(BreakpointResume(loadPath, savePath));
                //}


                //yield return null;
            }
        }
        if (string.IsNullOrEmpty(request.error))
        {
            Debug.Log("���سɹ�" + savePath);
            text.text = "100%";
        }



        //��ʾ����ʹ�ô� UnityWebRequest������Ӧ������ʹ�õ�������Դ��
        request.Dispose();
    }
}