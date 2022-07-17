using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WWWTest : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(RunWithContinue());
    }

    private WWW m_iWWW = null;
    private const int CHUNK_SIZE = 10 * 1024 * 1024;  //断点续传时,每次请求10M
    private long m_nChunk = 0;               //在分段下载中,记录当前下载段的长度
    private long m_nContinueDownloaded = 0;  //在分段下载中,记录当前下载文件已下载长度
    private IEnumerator RunWithContinue()
    {
        int nStartPos = 0;
        long nFileTotalLength = 102627178;

        System.IO.FileStream fsTmpFile = System.IO.File.Create(@"E:\UnitySpace\Test\Assets\abc.zip");

        while (nFileTotalLength > nStartPos)
        {
            string _url = "";

            Dictionary<string, string> httpHead = new Dictionary<string, string>();
            httpHead.Add("Range", "bytes=" + nStartPos.ToString() + "-" + (nStartPos + CHUNK_SIZE).ToString());

            m_iWWW = new WWW(_url, null, httpHead);
            yield return StartCoroutine(WaitForWWWWithTimeOut(m_iWWW, 10)); // 替换成带超时的版本 [3/11/2014 yao]

            if (!m_iWWW.isDone || (m_iWWW.error != null && m_iWWW.error.Length > 0))
            {
                Debug.Log("nStartPos:" + nStartPos + " CHUNK_SIZE:" + CHUNK_SIZE + " total:" + (nStartPos + CHUNK_SIZE));
                Debug.LogError("error======" + m_iWWW.error);
            }

            try
            {
                fsTmpFile.Write(m_iWWW.bytes, 0, m_iWWW.bytes.Length);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Exception======");
                break;
            }

            nStartPos += m_iWWW.bytes.Length;
            m_nContinueDownloaded = nStartPos;
            m_iWWW.Dispose();
            m_iWWW = null;
        }

        fsTmpFile.Close();
        fsTmpFile = null;
        Debug.LogError("结束");
    }

    private IEnumerator WaitForWWWWithTimeOut(WWW iWWW, float fTimeOut)
    {
        float fWait = 0;
        float fProgress = 0;
        do
        {
            //判断是否完成并等待
            if (iWWW.isDone)
            {
                yield break;
            }
            yield return new WaitForSeconds(0.1f);

            //累计超时时间
            fWait += 0.1f;
            if (fWait > fTimeOut)
            {
                yield break;
            }

            //如果进度走动了，则重新判断超时时间
            if (m_iWWW.progress > fProgress)
            {
                fProgress = m_iWWW.progress;
                fWait = 0;
            }
        } while (true);
    }
}
