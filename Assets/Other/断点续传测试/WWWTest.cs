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
    private const int CHUNK_SIZE = 10 * 1024 * 1024;  //�ϵ�����ʱ,ÿ������10M
    private long m_nChunk = 0;               //�ڷֶ�������,��¼��ǰ���ضεĳ���
    private long m_nContinueDownloaded = 0;  //�ڷֶ�������,��¼��ǰ�����ļ������س���
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
            yield return StartCoroutine(WaitForWWWWithTimeOut(m_iWWW, 10)); // �滻�ɴ���ʱ�İ汾 [3/11/2014 yao]

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
        Debug.LogError("����");
    }

    private IEnumerator WaitForWWWWithTimeOut(WWW iWWW, float fTimeOut)
    {
        float fWait = 0;
        float fProgress = 0;
        do
        {
            //�ж��Ƿ���ɲ��ȴ�
            if (iWWW.isDone)
            {
                yield break;
            }
            yield return new WaitForSeconds(0.1f);

            //�ۼƳ�ʱʱ��
            fWait += 0.1f;
            if (fWait > fTimeOut)
            {
                yield break;
            }

            //��������߶��ˣ��������жϳ�ʱʱ��
            if (m_iWWW.progress > fProgress)
            {
                fProgress = m_iWWW.progress;
                fWait = 0;
            }
        } while (true);
    }
}
