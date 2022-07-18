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

    //十亿字节为一段
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
    /// 分段，断点下载文件
    /// </summary>
    /// <param name="loadPath">下载地址</param>
    /// <param name="savePath">保存路径</param>
    /// <returns></returns>
    IEnumerator BreakpointResume(string loadPath, string savePath)
    {
        //UnityWebRequest 经配置可传输 HTTP HEAD 请求的 UnityWebRequest。
        UnityWebRequest headRequest = UnityWebRequest.Head(loadPath);
        //开始与远程服务器通信。
        yield return headRequest.SendWebRequest();

        if (!string.IsNullOrEmpty(headRequest.error))
        {
            Debug.LogError("获取不到资源文件");
            yield break;
        }
        //获取文件总大小
        ulong totalLength = ulong.Parse(headRequest.GetResponseHeader("Content-Length"));
        Debug.Log("获取大小" + totalLength);
        headRequest.Dispose();
        request = UnityWebRequest.Get(loadPath);
        //append设置为true文件写入方式为接续写入，不覆盖原文件。
        request.downloadHandler = new DownloadHandlerFile(savePath, true);
        //创建文件
        FileInfo file = new FileInfo(savePath);
        //当前下载的文件长度
        ulong fileLength = (ulong)file.Length;

        //请求网络数据从第fileLength到最后的字节；
        request.SetRequestHeader("Range", "bytes=" + fileLength + "-");

        if (!string.IsNullOrEmpty(request.error))
        {
            Debug.LogError("下载失败" + request.error);
        }
        if (fileLength < totalLength)
        {
            request.SendWebRequest();
            while (!request.isDone)
            {
                double progress = (request.downloadedBytes + fileLength) / (double)totalLength;
                text.text = (progress * 100 + 0.01f).ToString("f2") + "%";
                // Debug.Log("下载量" + Request.downloadedBytes);
                //超过一定的字节关闭现在的协程，开启新的协程，将资源分段下载

                yield return null;

                //if (request.downloadedBytes >= loadedBytes)
                //{
                    //StopCoroutine(coroutine);

                    //如果 UnityWebRequest 在进行中，就停止。
                    //request.Abort();
                    if (!string.IsNullOrEmpty(request.error))
                    {
                        Debug.LogError("下载失败" + request.error);
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
            Debug.Log("下载成功" + savePath);
            text.text = "100%";
        }



        //表示不再使用此 UnityWebRequest，并且应清理它使用的所有资源。
        request.Dispose();
    }
}