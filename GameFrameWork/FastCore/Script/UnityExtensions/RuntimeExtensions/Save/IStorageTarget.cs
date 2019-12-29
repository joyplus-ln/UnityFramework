
namespace UnityExtensions
{
    /// <summary>
    /// 持久储存目标（比如一个文件）
    /// </summary>
    public interface IStorageTarget
    {
        /// <summary>
        /// 将数据写入储存目标（可抛出异常）
        /// </summary>
        void Write(byte[] data);


        /// <summary>
        /// 从储存目标读取数据（可抛出异常）
        /// </summary>
        byte[] Read();


        /// <summary>
        /// 删除储存目标（可抛出异常）
        /// </summary>
        void Delete();

    } // interface IPersistentTarget

} // namespace UnityExtensions