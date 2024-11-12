using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DownloadHelper : MonoBehaviour
{
    public Text text;
    public delegate void DelegateDownloadFinish(bool bSuccess);

    public long AlreadyDownloadSize
    {
        get
        {
            if (_curTempWWW != null && _curTempWWW.isDone == false)
            {
                return _alreadyDownloadSize + (long)(_curTempWWW.progress * _curTempFileSize);
            }
            else
            {
                return _alreadyDownloadSize;
            }
        }
    }
    public long DownloadSpeed
    {
        get
        {
            if (_isStop)
            {
                return 0;
            }
            else
            {
                return (long)(AlreadyDownloadSize / (Time.time - _startDownloadTime));
            }
        }
    }

    private WWW _curTempWWW;
    private long _curTempFileSize;
    private long _alreadyDownloadSize = 0;
    private float _startDownloadTime;
    private readonly DownloadFileInfo[] _curDownloadUrlArray;
    private readonly string[] _curDownloadSizeArray;
    private readonly string[] _curFilePathArray;

    private readonly DelegateDownloadFinish _curDelFun;
    private bool _isStop = false;

    private FileStream rangeFS;//断点续传的FileStream
    private Dictionary<string, string> rangeHttpHead; //断点续传的Head缓存
    private const int ChunkSize = 10 * 1024 * 1024;  //断点续传时,每次请求字节数

    void Start()
    {
        var url = "https://game-swyl.wyx.cn/ntxs/release2/res/ResRoot/ResData/gengxin3/model/gengxin3_model0.zip";

        var _dataFileDownloader = DownloadHelper.StartDownloadRange(this, url, true, Application.persistentDataPath + "/MP4/gengxin3_model0.info", (bSucess) =>
        {
            Debug.LogError(bSucess);
        });
    }

    private DownloadHelper(string url, bool bRemote, string fileSavePath, DelegateDownloadFinish delFun)
    {
        _curDownloadUrlArray = new DownloadFileInfo[1];
        _curFilePathArray = new string[1];
        _curFilePathArray[0] = fileSavePath;
        if (bRemote)
        {
            _curDownloadUrlArray[0].Url = DownloadHelper.AddTimestampToUrl(url);
        }
        else
        {
            _curDownloadUrlArray[0].Url = url;
        }

        _curDelFun = delFun;
        _isStop = false;
    }
    private DownloadHelper(List<DownloadFileInfo> urlList, bool bRemote, List<string> fileSavePathList, DelegateDownloadFinish delFun)
    {
        _curDownloadUrlArray = new DownloadFileInfo[urlList.Count];
        for (int i = 0; i < urlList.Count; i++)
        {
            _curDownloadUrlArray[i] = urlList[i];
            if (bRemote)
            {
                _curDownloadUrlArray[i].Url = DownloadHelper.AddTimestampToUrl(urlList[i].Url);
            }
        }

        _curFilePathArray = new string[fileSavePathList.Count];
        for (int i = 0; i < fileSavePathList.Count; i++)
        {
            _curFilePathArray[i] = fileSavePathList[i];
        }

        _curDelFun = delFun;
        _isStop = false;
    }

    public static DownloadHelper StartDownload(MonoBehaviour monoBehavior, string url, bool bRemote, string fileSavePath, DelegateDownloadFinish delFun = null)
    {
        DownloadHelper helper = new DownloadHelper(url, bRemote, fileSavePath, delFun);
        monoBehavior.StartCoroutine(helper.DownloadFile());
        return helper;
    }
    public static DownloadHelper StartDownload(MonoBehaviour monoBehavior, List<DownloadFileInfo> urlList, bool bRemote, List<string> fileSavePathList, DelegateDownloadFinish delFun = null)
    {
        DownloadHelper helper = new DownloadHelper(urlList, bRemote, fileSavePathList, delFun);
        monoBehavior.StartCoroutine(helper.DownloadFile());
        return helper;
    }
    public static DownloadHelper StartDownloadRange(MonoBehaviour monoBehavior, string url, bool bRemote, string fileSavePath, DelegateDownloadFinish delFun = null)
    {
        DownloadHelper helper = new DownloadHelper(url, bRemote, fileSavePath, delFun);
        monoBehavior.StartCoroutine(helper.DownloadFile_Range());
        return helper;
    }

    public static string AddTimestampToUrl(string url)
    {
        return url + "?" + System.DateTime.Now.Millisecond.ToString();
    }


    public static IEnumerator GetWWW(string url, System.Action<WWW> retWWW, bool bAddStamp = true, Dictionary<string, string> httpHead = null)
    {
        string firstUrl = url;
        if (bAddStamp)
        {
            firstUrl = AddTimestampToUrl(url);
        }
        WWW wwwOrg;
        if (httpHead == null)
            wwwOrg = new WWW(firstUrl);
        else
            wwwOrg = new WWW(firstUrl, null, httpHead);
        retWWW(wwwOrg);
        yield return wwwOrg;
    }
    public void Stop()
    {
        _isStop = true;
        StopFS();
    }

    public void StopFS()
    {
        if (rangeFS != null)
        {
            rangeFS.Dispose();
            rangeFS.Close();
            rangeFS = null;
            rangeHttpHead = null;
        }
    }

    private IEnumerator DownloadFile()
    {
        _startDownloadTime = Time.time;

        if (_curDownloadUrlArray == null || _curFilePathArray == null)
        {
            Debug.LogError("url array or file array is null ");
            if (_curDelFun != null)
            {
                _curDelFun(false);
            }
            yield break;
        }

        if (_curDownloadUrlArray.Length != _curFilePathArray.Length)
        {
            Debug.LogError("url array size is not equal file path size");
            if (_curDelFun != null)
            {
                _curDelFun(false);
            }
            yield break;
        }
        for (int i = 0; i < _curDownloadUrlArray.Length; i++)
        {
            if (_isStop)
            {
                yield break;
            }
            yield return new WaitWhile(GetNeedWait);

            WWW wwwCurFile = null;
            _curTempFileSize = _curDownloadUrlArray[i].Size;
            yield return GetWWW(_curDownloadUrlArray[i].Url, value => _curTempWWW = wwwCurFile = value, false);
            if (!string.IsNullOrEmpty(wwwCurFile.error))
            {
                Debug.LogError("download file fail:    " + _curDownloadUrlArray[i].Url + "\nerror:    " + wwwCurFile.error);
                if (_curDelFun != null)
                {
                    _curDelFun(false);
                }
                wwwCurFile.Dispose();
                _curTempWWW = null;
                yield break;
            }
            else
            {
                try
                {
                    CheckTargetPath(_curFilePathArray[i]);
                    FileStream fs = new FileStream(_curFilePathArray[i], FileMode.OpenOrCreate);
                    fs.Write(wwwCurFile.bytes, 0, wwwCurFile.bytesDownloaded);
                    fs.Close();
                }
                catch (Exception ex)
                {
                    Debug.LogError("download file error:" + ex.ToString());
                    if (_curDelFun != null)
                    {
                        _curDelFun(false);
                    }
                    _isStop = true;
                }
                wwwCurFile.Dispose();
                _curTempWWW = null;
                _alreadyDownloadSize += _curTempFileSize;

            }
        }

        if (_curDelFun != null)
        {
            _curDelFun(!_isStop);
        }
    }


    private IEnumerator DownloadFile_Range()
    {
        _startDownloadTime = Time.time;

        //Check
        {
            if (_curDownloadUrlArray == null || _curFilePathArray == null)
            {
                Debug.LogError("url array or file array is null ");
                if (_curDelFun != null)
                {
                    _curDelFun(false);
                }
                yield break;
            }

            if (_curDownloadUrlArray.Length != _curFilePathArray.Length)
            {
                Debug.LogError("url array size is not equal file path size");
                if (_curDelFun != null)
                {
                    _curDelFun(false);
                }
                yield break;
            }
        }


        for (int i = 0; i < _curDownloadUrlArray.Length; i++)
        {
            if (_isStop) yield break;
            yield return new WaitWhile(GetNeedWait);

            var info = _curDownloadUrlArray[i];
            var writePath = _curFilePathArray[i];

            //检查路径
            CheckTargetPath(writePath);

            UnityWebRequest headRequest = UnityWebRequest.Head(info.Url);
            yield return headRequest.SendWebRequest();
            info.Size = long.Parse(headRequest.GetResponseHeader("Content-Length"));
            Debug.Log("获取大小" + info.Size);

            var request = UnityWebRequest.Get(info.Url);
            request.downloadHandler = new DownloadHandlerFile(writePath, true);
            var fileLength = new FileInfo(writePath).Length;
            request.SetRequestHeader("Range", $"bytes={fileLength}-");

            if (fileLength < info.Size)
            {
                request.SendWebRequest();
                while (!request.isDone)
                {
                    double progress = (request.downloadedBytes + (ulong)fileLength) / (double)info.Size;
                    Debug.Log((progress * 100 + 0.01f).ToString("f2") + "%");

                    //检查状态
                    if (_isStop)
                    {
                        StopFS();
                        yield break;
                    }

                    Debug.Log(request.downloadedBytes);

                    yield return null;

                    Debug.Log(request.downloadedBytes);

                    if (!string.IsNullOrEmpty(request.error))
                    {
                        //Error处理
                        Debug.LogError($"download file fail:{info.Url}  \nerror:{_curTempWWW.error}");
                        _curTempWWW.Dispose();
                        _curTempWWW = null;
                        StopFS();
                        File.Delete(writePath);
                        if (_curDelFun != null) _curDelFun(false);
                        yield break;
                    }
                }
                request.Dispose();
            }

            Debug.Log("下载成功");

            _curTempFileSize = info.Size;
            _alreadyDownloadSize += _curTempFileSize;

            if (_curDelFun != null)
            {
                _curDelFun(!_isStop);
            }
        }
    }

    bool GetNeedWait()
    {
        return false;
    }

    public static void CheckTargetPath(string targetPath)
    {
        targetPath = targetPath.Replace('\\', '/');

        int dotPos = targetPath.LastIndexOf('.');
        int lastPathPos = targetPath.LastIndexOf('/');

        if (dotPos > 0 && lastPathPos < dotPos)
        {
            targetPath = targetPath.Substring(0, lastPathPos);
        }
        if (Directory.Exists(targetPath))
        {
            return;
        }


        string[] subPath = targetPath.Split('/');
        string curCheckPath = "";
        int subContentSize = subPath.Length;
        for (int i = 0; i < subContentSize; i++)
        {
            curCheckPath += subPath[i] + '/';
            if (!Directory.Exists(curCheckPath))
            {
                Directory.CreateDirectory(curCheckPath);
            }
        }
    }
}

public struct DownloadFileInfo
{
    public string Url;
    public long Size;

    public DownloadFileInfo(string url, long size)
    {
        Url = url;
        Size = size;
    }
}