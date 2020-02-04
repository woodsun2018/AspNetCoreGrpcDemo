using Grpc.Net.Client;
using GrpcShare;
using ProtoBuf.Grpc.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UploadClient
{
    class Program
    {
        static ConcurrentQueue<UploadReply> replyQueue = new ConcurrentQueue<UploadReply>();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.ReadLine();

            //如果服务端没有加密传输，客户端必须设置
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            using var http = GrpcChannel.ForAddress("http://localhost:9988");
            var client = http.CreateGrpcService<IUpload>();

            //简单测试
            string request1 = "Hi, 我是UploadClient";
            Console.WriteLine(request1);

            var result1 = await client.Hi(request1);
            Console.WriteLine($"收到服务端回应={result1}");

            //测试
            var request2 = new Message()
            {
                Context = "Hello, 我是UploadClient",
            };
            Console.WriteLine(request2.Context);

            var result2 = await client.Hello(request2);
            Console.WriteLine($"收到服务端回应={result2.Context}");

            //流式上传数据包
            await foreach (var reply in client.Upload(SendPackage()))
            {
                //收到服务端回应后，丢到FIFO
                replyQueue.Enqueue(reply);
            }

            Console.ReadLine();
        }

        private static async IAsyncEnumerable<UploadRequest> SendPackage()
        {
            //上传第一包数据
            var request = new UploadRequest
            {
                Index = 1,
                SampleTime = DateTime.Now,
                Content = Encoding.UTF8.GetBytes(DateTime.Now.ToString()),
            };

            Console.WriteLine(request);

            yield return request;

            while (request.Index < 10)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                //从FIFO取出服务端回应
                if (!replyQueue.TryDequeue(out UploadReply reply))
                    continue;

                Console.WriteLine($"收到服务端回应={reply}");

                if (reply.ArchiveSuccess)
                {
                    //如果服务端存档成功，上传下一包数据
                    request = new UploadRequest
                    {
                        Index = reply.Index + 1,
                        SampleTime = DateTime.Now,
                        Content = Encoding.UTF8.GetBytes(DateTime.Now.ToString()),
                    };
                }
                else
                {
                    //如果服务端存档失败，重传上一包数据
                }

                Console.WriteLine(request);

                yield return request;
            }
        }
    }
}
