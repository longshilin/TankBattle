﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework.Download;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameFramework.Resource
{
    internal sealed partial class ResourceManager : GameFrameworkModule, IResourceManager
    {
        /// <summary>
        /// 资源更新器。
        /// </summary>
        private sealed partial class ResourceUpdater
        {
            private readonly ResourceManager m_ResourceManager;
            private readonly List<UpdateInfo> m_UpdateWaitingInfo;
            private IDownloadManager m_DownloadManager;
            private bool m_CheckResourcesComplete;
            private bool m_UpdateAllowed;
            private bool m_UpdateComplete;
            private int m_GenerateReadWriteListLength;
            private int m_CurrentGenerateReadWriteListLength;
            private int m_RetryCount;
            private int m_UpdatingCount;

            public GameFrameworkAction<ResourceName, string, string, int, int, int> ResourceUpdateStart;
            public GameFrameworkAction<ResourceName, string, string, int, int> ResourceUpdateChanged;
            public GameFrameworkAction<ResourceName, string, string, int, int> ResourceUpdateSuccess;
            public GameFrameworkAction<ResourceName, string, int, int, string> ResourceUpdateFailure;
            public GameFrameworkAction ResourceUpdateAllComplete;

            /// <summary>
            /// 初始化资源更新器的新实例。
            /// </summary>
            /// <param name="resourceManager">资源管理器。</param>
            public ResourceUpdater(ResourceManager resourceManager)
            {
                m_ResourceManager = resourceManager;
                m_UpdateWaitingInfo = new List<UpdateInfo>();
                m_DownloadManager = null;
                m_CheckResourcesComplete = false;
                m_UpdateAllowed = false;
                m_UpdateComplete = false;
                m_GenerateReadWriteListLength = 0;
                m_CurrentGenerateReadWriteListLength = 0;
                m_RetryCount = 3;
                m_UpdatingCount = 0;

                ResourceUpdateStart = null;
                ResourceUpdateChanged = null;
                ResourceUpdateSuccess = null;
                ResourceUpdateFailure = null;
                ResourceUpdateAllComplete = null;
            }

            /// <summary>
            /// 获取或设置每下载多少字节的资源，刷新一次资源列表。
            /// </summary>
            public int GenerateReadWriteListLength
            {
                get
                {
                    return m_GenerateReadWriteListLength;
                }
                set
                {
                    m_GenerateReadWriteListLength = value;
                }
            }

            /// <summary>
            /// 获取或设置资源更新重试次数。
            /// </summary>
            public int RetryCount
            {
                get
                {
                    return m_RetryCount;
                }
                set
                {
                    m_RetryCount = value;
                }
            }

            /// <summary>
            /// 获取等待更新队列大小。
            /// </summary>
            public int UpdateWaitingCount
            {
                get
                {
                    return m_UpdateWaitingInfo.Count;
                }
            }

            /// <summary>
            /// 获取正在更新队列大小。
            /// </summary>
            public int UpdatingCount
            {
                get
                {
                    return m_UpdatingCount;
                }
            }

            /// <summary>
            /// 资源更新器轮询。
            /// </summary>
            /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
            /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
            public void Update(float elapseSeconds, float realElapseSeconds)
            {
                if (m_UpdateAllowed && !m_UpdateComplete)
                {
                    if (m_UpdateWaitingInfo.Count > 0)
                    {
                        if (m_DownloadManager.FreeAgentCount > 0)
                        {
                            UpdateInfo updateInfo = m_UpdateWaitingInfo[0];
                            m_UpdateWaitingInfo.RemoveAt(0);
                            m_DownloadManager.AddDownload(updateInfo.DownloadPath, updateInfo.DownloadUri, updateInfo);
                            m_UpdatingCount++;
                        }
                    }
                    else if (m_UpdatingCount <= 0)
                    {
                        m_UpdateComplete = true;
                        Utility.Path.RemoveEmptyDirectory(m_ResourceManager.m_ReadWritePath);
                        if (ResourceUpdateAllComplete != null)
                        {
                            ResourceUpdateAllComplete();
                        }
                    }
                }
            }

            /// <summary>
            /// 关闭并清理资源更新器。
            /// </summary>
            public void Shutdown()
            {
                if (m_DownloadManager != null)
                {
                    m_DownloadManager.DownloadStart -= OnDownloadStart;
                    m_DownloadManager.DownloadUpdate -= OnDownloadUpdate;
                    m_DownloadManager.DownloadSuccess -= OnDownloadSuccess;
                    m_DownloadManager.DownloadFailure -= OnDownloadFailure;
                }

                m_UpdateWaitingInfo.Clear();
            }

            /// <summary>
            /// 设置下载管理器。
            /// </summary>
            /// <param name="downloadManager">下载管理器。</param>
            public void SetDownloadManager(IDownloadManager downloadManager)
            {
                if (downloadManager == null)
                {
                    throw new GameFrameworkException("Download manager is invalid.");
                }

                m_DownloadManager = downloadManager;
                m_DownloadManager.DownloadStart += OnDownloadStart;
                m_DownloadManager.DownloadUpdate += OnDownloadUpdate;
                m_DownloadManager.DownloadSuccess += OnDownloadSuccess;
                m_DownloadManager.DownloadFailure += OnDownloadFailure;
            }

            /// <summary>
            /// 增加资源更新。
            /// </summary>
            /// <param name="resourceName">资源名称。</param>
            /// <param name="loadType">资源加载方式。</param>
            /// <param name="length">资源大小。</param>
            /// <param name="hashCode">资源哈希值。</param>
            /// <param name="zipLength">压缩包大小。</param>
            /// <param name="zipHashCode">压缩包哈希值。</param>
            /// <param name="downloadPath">下载后存放路径。</param>
            /// <param name="downloadUri">下载地址。</param>
            /// <param name="retryCount">已重试次数。</param>
            public void AddResourceUpdate(ResourceName resourceName, LoadType loadType, int length, int hashCode, int zipLength, int zipHashCode, string downloadPath, string downloadUri, int retryCount)
            {
                m_UpdateWaitingInfo.Add(new UpdateInfo(resourceName, loadType, length, hashCode, zipLength, zipHashCode, downloadPath, downloadUri, retryCount));
            }

            /// <summary>
            /// 检查资源完成。
            /// </summary>
            /// <param name="needGenerateReadWriteList">是否需要生成读写区资源列表。</param>
            public void CheckResourceComplete(bool needGenerateReadWriteList)
            {
                m_CheckResourcesComplete = true;
                if (needGenerateReadWriteList)
                {
                    GenerateReadWriteList();
                }
            }

            /// <summary>
            /// 更新资源。
            /// </summary>
            public void UpdateResources()
            {
                if (m_DownloadManager == null)
                {
                    throw new GameFrameworkException("You must set download manager first.");
                }

                if (!m_CheckResourcesComplete)
                {
                    throw new GameFrameworkException("You must check resources complete first.");
                }

                m_UpdateAllowed = true;
            }

            private void GenerateReadWriteList()
            {
                string file = Utility.Path.GetCombinePath(m_ResourceManager.m_ReadWritePath, Utility.Path.GetResourceNameWithSuffix(ResourceListFileName));
                string backupFile = null;

                if (File.Exists(file))
                {
                    backupFile = file + BackupFileSuffixName;
                    if (File.Exists(backupFile))
                    {
                        File.Delete(backupFile);
                    }

                    File.Move(file, backupFile);
                }

                FileStream fileStream = null;
                try
                {
                    fileStream = new FileStream(file, FileMode.CreateNew, FileAccess.Write);
                    using (BinaryWriter binaryWriter = new BinaryWriter(fileStream, Encoding.UTF8))
                    {
                        fileStream = null;
                        byte[] encryptCode = new byte[4];
                        Utility.Random.GetRandomBytes(encryptCode);

                        binaryWriter.Write(ReadWriteListHeader);
                        binaryWriter.Write(ReadWriteListVersionHeader);
                        binaryWriter.Write(encryptCode);
                        binaryWriter.Write(m_ResourceManager.m_ReadWriteResourceInfos.Count);
                        foreach (KeyValuePair<ResourceName, ReadWriteResourceInfo> i in m_ResourceManager.m_ReadWriteResourceInfos)
                        {
                            byte[] nameBytes = Utility.Encryption.GetSelfXorBytes(Utility.Converter.GetBytes(i.Key.Name), encryptCode);
                            binaryWriter.Write((byte)nameBytes.Length);
                            binaryWriter.Write(nameBytes);

                            if (i.Key.Variant == null)
                            {
                                binaryWriter.Write((byte)0);
                            }
                            else
                            {
                                byte[] variantBytes = Utility.Encryption.GetSelfXorBytes(Utility.Converter.GetBytes(i.Key.Variant), encryptCode);
                                binaryWriter.Write((byte)variantBytes.Length);
                                binaryWriter.Write(variantBytes);
                            }

                            binaryWriter.Write((byte)i.Value.LoadType);
                            binaryWriter.Write(i.Value.Length);
                            binaryWriter.Write(i.Value.HashCode);
                        }
                    }

                    if (!string.IsNullOrEmpty(backupFile))
                    {
                        File.Delete(backupFile);
                    }
                }
                catch (Exception exception)
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }

                    if (!string.IsNullOrEmpty(backupFile))
                    {
                        File.Move(backupFile, file);
                    }

                    throw new GameFrameworkException(Utility.Text.Format("Pack save exception '{0}'.", exception.Message), exception);
                }
                finally
                {
                    if (fileStream != null)
                    {
                        fileStream.Dispose();
                        fileStream = null;
                    }
                }
            }

            private void OnDownloadStart(object sender, DownloadStartEventArgs e)
            {
                UpdateInfo updateInfo = e.UserData as UpdateInfo;
                if (updateInfo == null)
                {
                    return;
                }

                if (m_DownloadManager == null)
                {
                    throw new GameFrameworkException("You must set download manager first.");
                }

                if (e.CurrentLength > updateInfo.ZipLength)
                {
                    m_DownloadManager.RemoveDownload(e.SerialId);
                    string downloadFile = Utility.Text.Format("{0}.download", e.DownloadPath);
                    if (File.Exists(downloadFile))
                    {
                        File.Delete(downloadFile);
                    }

                    string errorMessage = Utility.Text.Format("When download start, downloaded length is larger than zip length, need '{0}', current '{1}'.", updateInfo.ZipLength.ToString(), e.CurrentLength.ToString());
                    OnDownloadFailure(this, new DownloadFailureEventArgs(e.SerialId, e.DownloadPath, e.DownloadUri, errorMessage, e.UserData));
                    return;
                }

                if (ResourceUpdateStart != null)
                {
                    ResourceUpdateStart(updateInfo.ResourceName, e.DownloadPath, e.DownloadUri, e.CurrentLength, updateInfo.ZipLength, updateInfo.RetryCount);
                }
            }

            private void OnDownloadUpdate(object sender, DownloadUpdateEventArgs e)
            {
                UpdateInfo updateInfo = e.UserData as UpdateInfo;
                if (updateInfo == null)
                {
                    return;
                }

                if (m_DownloadManager == null)
                {
                    throw new GameFrameworkException("You must set download manager first.");
                }

                if (e.CurrentLength > updateInfo.ZipLength)
                {
                    m_DownloadManager.RemoveDownload(e.SerialId);
                    string downloadFile = Utility.Text.Format("{0}.download", e.DownloadPath);
                    if (File.Exists(downloadFile))
                    {
                        File.Delete(downloadFile);
                    }

                    string errorMessage = Utility.Text.Format("When download update, downloaded length is larger than zip length, need '{0}', current '{1}'.", updateInfo.ZipLength.ToString(), e.CurrentLength.ToString());
                    OnDownloadFailure(this, new DownloadFailureEventArgs(e.SerialId, e.DownloadPath, e.DownloadUri, errorMessage, e.UserData));
                    return;
                }

                if (ResourceUpdateChanged != null)
                {
                    ResourceUpdateChanged(updateInfo.ResourceName, e.DownloadPath, e.DownloadUri, e.CurrentLength, updateInfo.ZipLength);
                }
            }

            private void OnDownloadSuccess(object sender, DownloadSuccessEventArgs e)
            {
                UpdateInfo updateInfo = e.UserData as UpdateInfo;
                if (updateInfo == null)
                {
                    return;
                }

                using (FileStream fileStream = new FileStream(e.DownloadPath, FileMode.Open, FileAccess.ReadWrite))
                {
                    bool zip = (updateInfo.Length != updateInfo.ZipLength || updateInfo.HashCode != updateInfo.ZipHashCode);

                    int length = (int)fileStream.Length;
                    if (length != updateInfo.ZipLength)
                    {
                        string errorMessage = Utility.Text.Format("Zip length error, need '{0}', downloaded '{1}'.", updateInfo.ZipLength.ToString(), length.ToString());
                        OnDownloadFailure(this, new DownloadFailureEventArgs(e.SerialId, e.DownloadPath, e.DownloadUri, errorMessage, e.UserData));
                        return;
                    }

                    if (m_ResourceManager.m_UpdateFileCache == null || m_ResourceManager.m_UpdateFileCache.Length < length)
                    {
                        m_ResourceManager.m_UpdateFileCache = new byte[(length / OneMegaBytes + 1) * OneMegaBytes];
                    }

                    int offset = 0;
                    int count = length;
                    while (count > 0)
                    {
                        int bytesRead = fileStream.Read(m_ResourceManager.m_UpdateFileCache, offset, count);
                        if (bytesRead <= 0)
                        {
                            throw new GameFrameworkException(Utility.Text.Format("Unknown error when load file '{0}'.", e.DownloadPath));
                        }

                        offset += bytesRead;
                        count -= bytesRead;
                    }

                    if (!zip)
                    {
                        byte[] hashBytes = Utility.Converter.GetBytes(updateInfo.HashCode);
                        if (updateInfo.LoadType == LoadType.LoadFromMemoryAndQuickDecrypt)
                        {
                            Utility.Encryption.GetQuickSelfXorBytes(m_ResourceManager.m_UpdateFileCache, hashBytes);
                        }
                        else if (updateInfo.LoadType == LoadType.LoadFromMemoryAndDecrypt)
                        {
                            Utility.Encryption.GetSelfXorBytes(m_ResourceManager.m_UpdateFileCache, hashBytes, length);
                        }
                    }

                    int hashCode = Utility.Converter.GetInt32(Utility.Verifier.GetCrc32(m_ResourceManager.m_UpdateFileCache, 0, length));
                    if (hashCode != updateInfo.ZipHashCode)
                    {
                        string errorMessage = Utility.Text.Format("Zip hash code error, need '{0}', downloaded '{1}'.", updateInfo.ZipHashCode.ToString("X8"), hashCode.ToString("X8"));
                        OnDownloadFailure(this, new DownloadFailureEventArgs(e.SerialId, e.DownloadPath, e.DownloadUri, errorMessage, e.UserData));
                        return;
                    }

                    if (zip)
                    {
                        try
                        {
                            if (m_ResourceManager.m_DecompressCache == null)
                            {
                                m_ResourceManager.m_DecompressCache = new MemoryStream();
                            }

                            m_ResourceManager.m_DecompressCache.Position = 0L;
                            m_ResourceManager.m_DecompressCache.SetLength(0L);
                            if (!Utility.Zip.Decompress(m_ResourceManager.m_UpdateFileCache, 0, length, m_ResourceManager.m_DecompressCache))
                            {
                                string errorMessage = Utility.Text.Format("Unable to decompress from file '{0}'.", e.DownloadPath);
                                OnDownloadFailure(this, new DownloadFailureEventArgs(e.SerialId, e.DownloadPath, e.DownloadUri, errorMessage, e.UserData));
                                return;
                            }

                            if (m_ResourceManager.m_DecompressCache.Length != updateInfo.Length)
                            {
                                string errorMessage = Utility.Text.Format("Resource length error, need '{0}', downloaded '{1}'.", updateInfo.Length.ToString(), m_ResourceManager.m_DecompressCache.Length.ToString());
                                OnDownloadFailure(this, new DownloadFailureEventArgs(e.SerialId, e.DownloadPath, e.DownloadUri, errorMessage, e.UserData));
                                return;
                            }

                            fileStream.Position = 0L;
                            fileStream.SetLength(0L);
                            m_ResourceManager.m_DecompressCache.Position = 0L;
                            int bytesRead = 0;
                            while ((bytesRead = m_ResourceManager.m_DecompressCache.Read(m_ResourceManager.m_UpdateFileCache, 0, m_ResourceManager.m_UpdateFileCache.Length)) > 0)
                            {
                                fileStream.Write(m_ResourceManager.m_UpdateFileCache, 0, bytesRead);
                            }
                        }
                        catch (Exception exception)
                        {
                            string errorMessage = Utility.Text.Format("Unable to decompress from file '{0}' with error message '{1}'.", e.DownloadPath, exception.Message);
                            OnDownloadFailure(this, new DownloadFailureEventArgs(e.SerialId, e.DownloadPath, e.DownloadUri, errorMessage, e.UserData));
                            return;
                        }
                    }
                }

                m_UpdatingCount--;

                if (m_ResourceManager.m_ResourceInfos.ContainsKey(updateInfo.ResourceName))
                {
                    throw new GameFrameworkException(Utility.Text.Format("Resource info '{0}' is already exist.", updateInfo.ResourceName.FullName));
                }

                m_ResourceManager.m_ResourceInfos.Add(updateInfo.ResourceName, new ResourceInfo(updateInfo.ResourceName, updateInfo.LoadType, updateInfo.Length, updateInfo.HashCode, false));

                if (m_ResourceManager.m_ReadWriteResourceInfos.ContainsKey(updateInfo.ResourceName))
                {
                    throw new GameFrameworkException(Utility.Text.Format("Read-write resource info '{0}' is already exist.", updateInfo.ResourceName.FullName));
                }

                m_ResourceManager.m_ReadWriteResourceInfos.Add(updateInfo.ResourceName, new ReadWriteResourceInfo(updateInfo.LoadType, updateInfo.Length, updateInfo.HashCode));

                m_CurrentGenerateReadWriteListLength += updateInfo.ZipLength;
                if (m_UpdatingCount <= 0 || m_CurrentGenerateReadWriteListLength >= m_GenerateReadWriteListLength)
                {
                    m_CurrentGenerateReadWriteListLength = 0;
                    GenerateReadWriteList();
                }

                if (ResourceUpdateSuccess != null)
                {
                    ResourceUpdateSuccess(updateInfo.ResourceName, e.DownloadPath, e.DownloadUri, updateInfo.Length, updateInfo.ZipLength);
                }
            }

            private void OnDownloadFailure(object sender, DownloadFailureEventArgs e)
            {
                UpdateInfo updateInfo = e.UserData as UpdateInfo;
                if (updateInfo == null)
                {
                    return;
                }

                if (File.Exists(e.DownloadPath))
                {
                    File.Delete(e.DownloadPath);
                }

                if (ResourceUpdateFailure != null)
                {
                    ResourceUpdateFailure(updateInfo.ResourceName, e.DownloadUri, updateInfo.RetryCount, m_RetryCount, e.ErrorMessage);
                }

                if (updateInfo.RetryCount < m_RetryCount)
                {
                    m_UpdatingCount--;
                    UpdateInfo newUpdateInfo = new UpdateInfo(updateInfo.ResourceName, updateInfo.LoadType, updateInfo.Length, updateInfo.HashCode, updateInfo.ZipLength, updateInfo.ZipHashCode, updateInfo.DownloadPath, updateInfo.DownloadUri, updateInfo.RetryCount + 1);
                    if (m_UpdateAllowed)
                    {
                        m_UpdateWaitingInfo.Add(newUpdateInfo);
                    }
                    else
                    {
                        throw new GameFrameworkException("Update state error.");
                    }
                }
            }
        }
    }
}
