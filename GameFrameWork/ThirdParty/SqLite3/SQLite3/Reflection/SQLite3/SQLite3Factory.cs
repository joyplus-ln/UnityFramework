using System.IO;
using UnityEngine;

namespace Framework.Reflection.SQLite3Helper
{
    public static class SQLite3Factory
    {
        /// <summary>
        /// Open a SQLite3 database that exists in the persistentDataPath directory as read-only.
        /// </summary>
        /// <param name="InDbName">The name of the SQLite3 database.</param>
        /// <returns>Operation SQLite3 database handle.</returns>
        public static SQLite3Operate OpenToRead(string InDbName, bool InNeedCheck)
        {
#if UNITY_EDITOR
            string dbPath = Path.Combine(Application.streamingAssetsPath, InDbName);
            if (File.Exists(dbPath)) return new SQLite3Operate(dbPath, SQLite3OpenFlags.ReadOnly);
            else return null;
#else
            string dbPath = Path.Combine(Application.persistentDataPath, InDbName);
            
            //return null;
            bool needUpdate = true;
            if (File.Exists(dbPath))
            {
#if DEBUG_MODE
                needUpdate = false;
#else
                if (InNeedCheck)
                {
                    SQLite3Version version = Resources.Load<SQLite3Version>("SQLite3Version");
                    if (null == version) needUpdate = true;
                    else if (MD5Utility.GetFileMD5(dbPath).Equals(version.DbMd5)) needUpdate = false;
                }
                else needUpdate = false;
#endif
            }

            if (needUpdate)
            {
#if UNITY_ANDROID
                using (WWW www = new WWW(Path.Combine("jar:file://" + Application.dataPath + "!/assets/", InDbName)))
                {
                    while (!www.isDone)
                    {

                    }
                    
                    if (string.IsNullOrEmpty(www.error)) File.WriteAllBytes(dbPath, www.bytes);
                    else Debug.LogError("www error " + www.error);
                }
#elif UNITY_IOS
                
                File.Copy(Path.Combine(Application.streamingAssetsPath, InDbName), dbPath, true);
#endif
        }
             return new SQLite3Operate(dbPath, SQLite3OpenFlags.ReadOnly);
#endif
        }

        //        /// <summary>
        //        /// If there is no database in the persistentDataPath directory,
        //        /// Then copy the database from the streamingAssetsPath directory to the persistentDataPath directory and open the database in read-only mode
        //        /// Else if need to match detection
        //        ///        then If the incoming Md5 is not empty, it is determined whether the database Md5 of the persistentDataPath directory matches.
        //        ///                else  the incoming Md5 is empty, determine whether the database in the persistentDataPath directory is the same as the streamingAssetsPath directory.
        //        ///        Else Open the existing database.
        //        /// </summary>
        //        /// <param name="InDbName">The name of the SQLite3 database.</param>
        //        /// <param name="InNeedMatchDetection">Whether need to match detection.</param>
        //        /// <param name="InMd5"></param>
        //        /// <returns>Operation SQLite3 database handle.</returns>
        //        public static SQLite3Operate OpenToRead(string InDbName, bool InNeedMatchDetection, string InMd5 = null)
        //        {
        //            string persistentDbPath = Path.Combine(Application.persistentDataPath, InDbName);

        //#if !UNITY_EDITOR && UNITY_ANDROID
        //            string streamDbPath = Path.Combine("jar:file://" + Application.dataPath + "!/assets/", InDbName);
        //#elif UNITY_IOS
        //            string streamDbPath = Path.Combine(Application.dataPath + "/Raw/", InDbName);
        //#else
        //            string streamDbPath = Path.Combine(Application.streamingAssetsPath, InDbName);
        //#endif

        //            bool isNeedOverride = false;
        //            byte[] dbBytes = null;
        //            if (File.Exists(persistentDbPath))
        //            {
        //                if (InNeedMatchDetection)
        //                {
        //                    if (string.IsNullOrEmpty(InMd5))
        //                    {
        //#if !UNITY_EDITOR && UNITY_ANDROID
        //                        using (WWW www = new WWW(streamDbPath))
        //                        {
        //                            while (!www.isDone)
        //                            {
        //                            }

        //                            if (string.IsNullOrEmpty(www.error))
        //                            {
        //                                dbBytes = www.bytes;
        //                                isNeedOverride = !SQLite3Utility.GetBytesMD5(dbBytes).Equals(SQLite3Utility.GetFileMD5(persistentDbPath));
        //                            }
        //                            else isNeedOverride = true;
        //                        }
        //#else
        //                        dbBytes = File.ReadAllBytes(streamDbPath);
        //                        isNeedOverride = !MD5Utility.GetBytesMD5(dbBytes).Equals(MD5Utility.GetFileMD5(persistentDbPath));
        //#endif
        //                    }
        //                    else isNeedOverride = !InMd5.Equals(persistentDbPath);
        //                }
        //            }
        //            else isNeedOverride = true;

        //            if (isNeedOverride)
        //            {
        //                if (null == dbBytes)
        //                {
        //#if !UNITY_EDITOR && UNITY_ANDROID
        //                    using (WWW www = new WWW(streamDbPath))
        //                    {
        //                        while (!www.isDone)
        //                        {
        //                        }

        //                        if (string.IsNullOrEmpty(www.error)) dbBytes = www.bytes;
        //                        else Debug.LogError("Copy database from streamingAssetsPath to persistentDataPath error. " + www.error);
        //                    }
        //#else
        //                    dbBytes = File.ReadAllBytes(streamDbPath);
        //#endif
        //                }

        //                File.WriteAllBytes(persistentDbPath, dbBytes);
        //            }

        //            return new SQLite3Operate(persistentDbPath, SQLite3OpenFlags.ReadOnly);
        //        }

        /// <summary>
        /// Open a SQLite3 database that exists in the persistentDataPath directory as read-write,
        /// If the database does not exist, create an empty database.
        /// </summary>
        /// <param name="InDbName">The name of the SQLite3 database.</param>
        /// <returns>Operation SQLite3 database handle.</returns>
        public static SQLite3Operate OpenToWrite(string InDbName)
        {
            string persistentDbPath = Path.Combine(Application.persistentDataPath, InDbName);

            return new SQLite3Operate(persistentDbPath, SQLite3OpenFlags.Create | SQLite3OpenFlags.ReadWrite);
        }

        public static SQLite3Operate OpenToWriteSpecialFile(string InDbPath)
        {
            return new SQLite3Operate(InDbPath, SQLite3OpenFlags.Create | SQLite3OpenFlags.ReadWrite);
        }
    }
}