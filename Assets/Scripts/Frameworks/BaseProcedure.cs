using System;

namespace WebsiteWorkshop.Frameworks
{
    public abstract class BaseProcedure
    {
        /// <summary>
        /// 标记当前流程是否完成
        /// </summary>
        protected bool IsDone { get; set; }
        /// <summary>
        /// 下一流程
        /// </summary>
        protected BaseProcedure Next { get; set; }
        /// <summary>
        /// 可用的下一流程
        /// </summary>
        protected BaseProcedure[] Siblings { get; set; }

        public virtual event Action OnStart;
        public virtual event Action OnComplete;
        public virtual event Action OnError;

        public BaseProcedure(params BaseProcedure[] siblings)
        {
            this.Siblings = siblings;
            if (siblings.Length == 1)
            {
                Next = siblings[0];
            }
            this.IsDone = false;
        }
        /// <summary>
        /// 检查处理当前流程的必要数据是否完备
        /// </summary>
        /// <returns>数据完备时，返回true</returns>
        protected abstract bool CheckDependency();
        /// <summary>
        /// 处理当前流程
        /// </summary>
        protected abstract void HandleProcedure();
        /// <summary>
        /// 流程开始时调用
        /// </summary>
        /// <param name="onStart">开始节点的回调</param>
        public void StartProcedure()
        {
            if (CheckDependency())
            {
                OnStart?.Invoke();
                HandleProcedure();
            }
        }
        /// <summary>
        /// 流程完成时的收尾处理，需要根据具体的流程结束情况自行确定调用时机
        /// </summary>
        /// <param name="onComplete">完成节点的回调</param>
        public virtual void PostProcedure()
        {
            IsDone = true;
            OnComplete?.Invoke();
            if (Next != null)
            {
                Next.StartProcedure();
            }
        }

    }

    public class SharedProcedureData<T> : SingletonTemplate<T> where T : new()
    {
    }
}

