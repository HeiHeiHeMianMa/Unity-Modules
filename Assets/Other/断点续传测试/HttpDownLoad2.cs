using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class HttpDownLoad2
{
    public float progress { get; private set; }

    public bool isDone { get; private set; }

    private bool isStop;

    public IEnumerator Start(string url, string filePath, Action callBack = null)
    {
        var headRequest = UnityWebRequest.Head(url);

        yield return headRequest.SendWebRequest();

        var totalLength = long.Parse(GetContentLength(headRequest));
        var isAcceptRanges = IsAcceptRanges(headRequest);

        var dirPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
        {

            var fileLength = isAcceptRanges ? fs.Length : 0L;

            if (fileLength < totalLength)
            {
                fs.Seek(fileLength, SeekOrigin.Begin);

                var request = UnityWebRequest.Get(url);
                request.SetRequestHeader("Range", "bytes=" + fileLength + "-" + totalLength);
                request.SendWebRequest();

                var index = 0;
                while (!request.isDone)
                {
                    if (isStop) break;
                    yield return null;
                    var buff = request.downloadHandler.data;
                    if (buff != null)
                    {
                        var length = buff.Length - index;
                        fs.Write(buff, index, length);
                        index += length;
                        fileLength += length;
                        Debug.Log(length + " " + fileLength);
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
            }
            else
            {
                progress = 1f;
            }

            fs.Close();
            fs.Dispose();
        }

        if (progress >= 1f)
        {
            isDone = true;
            callBack?.Invoke();
        }
    }

    public void Stop()
    {
        isStop = true;
    }

    private static string GetContentLength(UnityWebRequest request)
    {
        if (null == request)
            return null;

        return request.GetResponseHeader("Content-Length");
    }

    /// <summary>
    /// ���������˶Զϵ�������֧��
    /// </summary>
    private static bool IsAcceptRanges(UnityWebRequest request)
    {
        var isAcceptRanges = request.GetResponseHeader("Accept-Ranges");
        if (isAcceptRanges == null)
            return true;
        return isAcceptRanges != "none";
    }

    /// �����������ļ��Ƿ�仯
    /// ���´�Ҫ��������ʱ�����ȷ���������ϵ��ļ����ǵ���������һ����Ǹ��ļ�������������ϵ��ļ��Ѿ������ˣ���������ζ���Ҫ���´�ͷ��ʼ���ء�
    /// ֻ���ڷ������ϵ��ļ�û�з����仯������£��ϵ������������塣
    /// ����������⣬HTTP ��ӦͷΪ�����ṩ�˲�ͬ��ѡ��ETag �� Last-Modified �����������
    /// ETag ����һ����ʶ��ǰ�������ݵ��ַ��������������Դ�����仯�󣬶�Ӧ�� ETag Ҳ��仯��
    /// Last-Modified �������������Դ�ڷ������ϵ����һ���޸�ʱ�䡣
    /// ��򵥵İ취�ǵ�һ������ʱ������Ӧͷ�е� ETag ���������´�����ʱ���Ƚϡ�
    /// ���оͱȽ϶����md5����ʼ����ʱ��������md5�ļ����ȶ�md5��ͨ���ſ�ʼ���ء�

}