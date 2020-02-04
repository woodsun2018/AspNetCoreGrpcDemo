using GrpcShare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcDemo
{
    /// <summary>
    /// 上传数据包服务
    /// </summary>
    public class UploadService : IUpload
    {
        /// <summary>
        /// 简单测试
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ValueTask<string> Hi(string message)
        {
            Console.WriteLine($"收到客户端问候={message}");

            return new ValueTask<string>("Hi，我是UploadService");
        }

        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ValueTask<Message> Hello(Message message)
        {
            Console.WriteLine($"收到客户端问候={message.Context}");

            var reply = new Message()
            {
                Context = "Hello，我是UploadService",
            };

            return new ValueTask<Message>(reply);
        }

        /// <summary>
        /// 双向流式上传数据包
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<UploadReply> Upload(IAsyncEnumerable<UploadRequest> stream)
        {
            await foreach (var request in stream)
            {
                Console.WriteLine(request);

                await Task.Delay(TimeSpan.FromSeconds(1));

                var reply = new UploadReply
                {
                    Index = request.Index,
                    //模拟保存失败
                    ArchiveSuccess = (DateTime.Now.Second % 3 < 2),
                };

                yield return reply;
            }

            Console.WriteLine($"客户端关闭连接");
        }
    }
}
