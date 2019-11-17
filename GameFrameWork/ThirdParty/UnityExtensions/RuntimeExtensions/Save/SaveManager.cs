using System;
using System.Collections.Generic;

namespace UnityExtensions
{
    /// <summary>
    /// 存档系统任务
    /// </summary>
    public class SaveTask
    {
        /// <summary>
        /// 存档系统任务类型
        /// </summary>
        public enum Type
        {
            Load,
            Save,
            Delete,
        }


        public Type type;                   // 任务类型
        public Save save;                   // 存档对象
        public IStorageTarget target;       // 外存目标
        public byte[] data;                 // 数据内容
        public Exception exception;         // 执行结果 (null 代表成功)


        public bool success => exception == null;
    }


    /// <summary>
    /// 存档管理器
    /// 所有存档相关操作本质都是异步的，但所有公开的 API 可以在主线程放心使用
    /// 针对每一个存档对象，你需要根据需求选择恰当的 SaveManager 和 StorageTarget
    /// </summary>
    public abstract class SaveManager : IDisposable
    {
        Queue<SaveTask> _tasks = new Queue<SaveTask>(4);    // 请求的任务队列
        protected volatile bool _finished = false;
        bool _disposed = false;

        
        /// <summary>
        /// 任务结束时触发
        /// </summary>
        public event Action<SaveTask> onTaskFinished;


        public SaveManager()
        {
            ApplicationKit.update += Update;
        }


        /// <summary>
        /// 终止并释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                if (hasTask)
                {
                    UnityEngine.Debug.LogError("SaveManager has unfinished task!");
                    return;
                }

                ApplicationKit.update -= Update;
                OnDispose();

                _disposed = true;
            }
        }


        /// <summary>
        /// 每帧更新
        /// </summary>
        protected virtual void Update()
        {
            if (_finished)
            {
                var task = _tasks.Dequeue();

                // 后续处理
                switch (task.type)
                {
                    case SaveTask.Type.Load:
                        if (task.success)
                            task.exception = task.save.FromBytes(ref task.data);
                        else
                            task.save.Reset();
                        break;
                }

                FinishTask(task);
                _finished = false;

                onTaskFinished?.Invoke(task);

                if (_tasks.Count > 0)
                {
                    BeginTask(_tasks.Peek());
                }
            }
        }


        /// <summary>
        /// 终止并释放资源
        /// </summary>
        protected abstract void OnDispose();


        /// <summary>
        /// 开始任务时触发。当任务完成时设置 _finished 标记通知主线程
        /// </summary>
        protected abstract void BeginTask(SaveTask task);


        /// <summary>
        /// 完成任务时触发
        /// </summary>
        protected abstract void FinishTask(SaveTask task);


        /// <summary>
        /// 是否有未完成的任务
        /// 在游戏退出前需检查所有任务是否已经完成
        /// </summary>
        public bool hasTask
        {
            get { return _tasks.Count > 0; }
        }


        // 新建任务
        void NewTask(SaveTask.Type type, Save save, IStorageTarget target)
        {
            if (_disposed) throw new Exception("SaveManager had already disposed.");

            var task = new SaveTask()
            {
                type = type,
                save = save,
                target = target,
            };

            // 前期处理
            switch (type)
            {
                case SaveTask.Type.Save:
                    task.exception = save.ToBytes(out task.data);
                    break;

                default:
                    task.data = null;
                    task.exception = null;
                    break;
            }

            _tasks.Enqueue(task);

            // 第一个任务需要主动唤起处理线程
            if (_tasks.Count == 1)
            {
                BeginTask(_tasks.Peek());
            }
        }


        /// <summary>
        /// 新建保存任务
        /// </summary>
        public void NewSaveTask(Save save, IStorageTarget target)
        {
            NewTask(SaveTask.Type.Save, save, target);
        }


        /// <summary>
        /// 新建加载任务
        /// </summary>
        public void NewLoadTask(Save save, IStorageTarget target)
        {
            NewTask(SaveTask.Type.Load, save, target);
        }


        /// <summary>
        /// 新建删除任务
        /// </summary>
        public void NewDeleteTask(IStorageTarget target)
        {
            NewTask(SaveTask.Type.Delete, null, target);
        }

    } // class SaveManager

} // namespace UnityExtensions
