using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class Test1 : MonoBehaviour
{
    void Start()
    {
        //WWWForm form = new WWWForm();
        //form.AddField("desc", "test upload log file");
        //form.AddBinaryData("errlog", File.ReadAllBytes(), "test_log.txt", "application/x-gzip");

        //UnityWebRequest request = UnityWebRequest.Post("http://192.168.23.28/CDN", form);
        //var result = request.SendWebRequest();
        //while (!result.isDone)
        //{
        //    if (!string.IsNullOrEmpty(request.error))
        //    {
        //        Debug.LogError(request.error);
        //        break;
        //    }
        //    Debug.Log("result.progress: " + request.uploadProgress);
        //}
        //Debug.LogError("123");

        //StartCoroutine(UpWebDav("D:/UnityTestProj/EditorTest/Assets/ttt.zip", "http://192.168.23.28"));
        StartCoroutine(UploadFile("D:/UnityTestProj/EditorTest/Assets/ttt.zip", "https://tmpfile.nnhhmm.com:9999/"));
        //StartCoroutine(UploadFile("D:/UnityTestProj/EditorTest/Assets/ttt.zip", "http://127.0.0.1"));
    }



    IEnumerator UploadFile(string path, string url)
    {
        string filePath = path;
        string serverURL = url;

        if (System.IO.File.Exists(filePath))
        {
            using (UnityWebRequest www = new UnityWebRequest(serverURL + "/abc.aaa", "PUT"))
            {
                www.uploadHandler = new UploadHandlerFile(filePath);
                www.downloadHandler = new DownloadHandlerBuffer();

                www.SetRequestHeader("Content-Type", "application/octet-stream");
                www.SetRequestHeader("Expect", "");

                www.SendWebRequest();

                while (!www.isDone)
                {
                    Debug.LogError($"{www.uploadProgress * 100: 0.00}%");
                    yield return null;
                }

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("File upload failed: " + www.error);
                }
                else
                {
                    Debug.Log("File upload complete!");
                }
            }
        }
        else
        {
            Debug.LogError("File not found at path: " + filePath);
        }
    }

    //IEnumerator UploadFile(string path, string url)
    //{

    //    var form = new WWWForm();
    //    form.AddBinaryData("file", File.ReadAllBytes(path), "ttt.zip", "text/plain");
    //    form.headers["Content-Type"] = "multipart/form-data";
    //    var request = UnityWebRequest.Post(url, form);
    //    request.SetRequestHeader("Content-Type", "multipart/form-data");

    //    // 发送请求并等待响应
    //    request.SendWebRequest();

    //    Debug.Log("12");
    //    while (!request.isDone)
    //    {
    //        Debug.LogError(request.uploadProgress);
    //        yield return null;
    //    }

    //    // 检查是否有错误发生
    //    if (request.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.LogError("Error uploading file: " + request.error);
    //    }
    //    else
    //    {
    //        Debug.Log("File upload complete!" + request.isDone);
    //    }
    //}


    IEnumerator UpWebDav(string path, string url)
    {
        string filePath = path; // 本地文件路径
        string destinationUrl = "http://192.168.99.97:9002/dav/abc.abc"; // WebDAV服务器地址
        string username = "webDav"; // 用户名
        string password = "0051"; // 密码
                                  // 创建UnityWebRequest对象

        UnityWebRequest request = UnityWebRequest.Put(destinationUrl, System.IO.File.ReadAllBytes(filePath));
        // 添加Basic Authentication头部信息
        string auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
        request.SetRequestHeader("Authorization", "Basic " + auth);

        var foo = request.SendWebRequest();

        while (!request.isDone)
        {
            Debug.LogError(request.uploadProgress);
            yield return null;
        }
        Debug.Log("上传完成");
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("上传完成，服务器响应：" + request.responseCode);
        }
        else
        {
            Debug.LogError("上传失败，错误消息：" + request.error);
        }
    }

}
