using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace GrpcShare
{
    /// <summary>
    /// 文本消息
    /// </summary>
    [DataContract]
    public class Message
    {
        /// <summary>
        /// 内容
        /// </summary>
        [DataMember(Order = 1)]
        public string Context { get; set; }
    }

    /// <summary>
    /// 上传数据包请求
    /// </summary>
    [DataContract]
    public class UploadRequest
    {
        /// <summary>
        /// 数据包索引
        /// </summary>
        [DataMember(Order = 1)]
        public int Index { get; set; }

        /// <summary>
        /// 采样时间
        /// </summary>
        [DataMember(Order = 2)]
        public DateTime SampleTime { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [DataMember(Order = 3)]
        public byte[] Content { get; set; }

        public override string ToString()
        {
            return $"发送第{Index}包数据, {SampleTime}, 长度={Content.Length}";
        }
    }

    /// <summary>
    /// 上传数据包应答
    /// </summary>
    [DataContract]
    public class UploadReply
    {
        /// <summary>
        /// 数据包索引
        /// </summary>
        [DataMember(Order = 1)]
        public int Index { get; set; }

        /// <summary>
        /// 保存到数据库成功标志
        /// </summary>
        [DataMember(Order = 2)]
        public bool ArchiveSuccess { get; set; }

        public override string ToString()
        {
            return $"收到第{Index}包数据, 保存成功标志={ArchiveSuccess}";
        }
    }

    /// <summary>
    /// 上传数据包接口
    /// </summary>
    [ServiceContract]
    public interface IUpload
    {
        /// <summary>
        /// 简单测试
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [OperationContract]
        ValueTask<string> Hi(string message);

        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [OperationContract]
        ValueTask<Message> Hello(Message message);

        /// <summary>
        /// 双向流式上传数据包
        /// 注意IAsyncEnumerable需要NuGet安装Microsoft.Bcl.AsyncInterfaces，不是System.Interactive.Async
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [OperationContract]
        IAsyncEnumerable<UploadReply> Upload(IAsyncEnumerable<UploadRequest> stream);
    }
}
