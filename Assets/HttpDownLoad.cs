using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Profiling;

public class HttpDownLoad : MonoBehaviour
{
    public float progress;

    public bool isDone;

    private bool isStop;

    Coroutine coroutine;
    private string MyURL = "https://game-swyl.wyx.cn/ntxs/release2/res/ResRoot/ResData/gengxin3/model/gengxin3_model0.zip";
    private const int CHUNK_SIZE = 10 * 1024 * 1024;  //断点续传时,每次请求10M

    void Start()
    {
        Debug.Log(Application.persistentDataPath);

        coroutine = StartCoroutine(BreakpointResume(MyURL, Application.persistentDataPath + "/MP4/HttpDownLoad333.zip"));

    }

    /// <summary>
    /// 检查服务器端对断点续传的支持
    /// </summary>
    [MenuItem("检查服务器端对断点续传的支持")]
    private static bool IsAcceptRanges(UnityWebRequest request)
    {
        var isAcceptRanges = request.GetResponseHeader("Accept-Ranges");
        if (isAcceptRanges == null)
            return true;
        return isAcceptRanges != "none";
    }

    public IEnumerator BreakpointResume(string url, string filePath, Action callBack = null)
    {
        var headRequest = UnityWebRequest.Head(url);

        yield return headRequest.SendWebRequest();

        var totalLength = long.Parse(headRequest.GetResponseHeader("Content-Length"));

        Debug.Log(IsAcceptRanges(headRequest));

        var dirPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        Debug.Log("totalLength  " + totalLength);

        using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            var fileLength = fs.Length;

            while (fileLength < totalLength)
            {
                fs.Seek(fileLength, SeekOrigin.Begin);

                //var request = UnityWebRequest.Get(url);
                //request.SetRequestHeader("Range", "bytes=" + fileLength + "-" );
                //request.SendWebRequest();

                Dictionary<string, string> httpHead = new Dictionary<string, string>();
                httpHead.Add("Range", "bytes=" + fileLength + "-"  + (fileLength + CHUNK_SIZE));
                WWW request = new WWW(url, null, httpHead);

                yield return request;

                
                if (!request.isDone || (request.error != null && request.error.Length > 0))
                {
                   // Debug.Log("nStartPos:" + nStartPos + " CHUNK_SIZE:" + CHUNK_SIZE + " total:" + (nStartPos + CHUNK_SIZE));
                    Debug.LogError("error " + request.error);
                    //fs.Seek(0, SeekOrigin.Begin);
                    //fs.SetLength(0);
                    fs.Close();
                    fs.Dispose();
                    File.Delete(filePath);
                    yield break;
                }
                else
                {
                    if (isStop) break;
                    //yield return null;
                    var buff = request.bytes;
                    if (buff != null)
                    {
                        fs.Write(buff, 0, buff.Length);
                        fileLength += buff.Length;

                        if (fileLength == totalLength)
                        {
                            progress = 1f;
                        }
                        else
                        {
                            progress = fileLength / (float)totalLength;
                        }
                    }

                }

                var index = 0;
                if (request.isDone)
                {
                }
            }

            {
                progress = 1f;
            }

            fs.Close();
            fs.Dispose();
        }

        if (progress >= 1f)
        {
            isDone = true;
            if (callBack != null)
            {
                callBack();
            }
        }
    }

    public void Stop()
    {
        isStop = true;
    }
}