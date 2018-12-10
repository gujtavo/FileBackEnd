using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;

namespace BackEndSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "68.183.130.209", UserName = "test", Password = "Password123" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "StorageRequest", durable: false,
                  exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: "StorageRequest",
                  autoAck: false, consumer: consumer);
                Console.WriteLine(" [x] Awaiting RPC requests");

                consumer.Received += (model, ea) =>
                {
                    string response = null;

                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        var message = Encoding.UTF8.GetString(body);
                        
                        //Console.WriteLine(" [.] fib({0})", message);
                        response = handleMessage(message);
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine(" [.] " + e.Message);
                        response = "";
                    }
                    finally
                    {
                        var responseBytes = Encoding.UTF8.GetBytes(response);
                        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                          basicProperties: replyProps, body: responseBytes);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag,
                          multiple: false);
                    }
                };

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private static string handleMessage(string message)
        {
            var request = JsonConvert.DeserializeObject<FileRequest>(message);
            if(request.accion == 2)
            {
                if (!Directory.Exists(@".\Files")){
                    Directory.CreateDirectory(@".\Files");
                }
                int i = 0;
                List<FileModel> list = new List<FileModel>();
                foreach (string file in Directory.EnumerateFiles(@".\Files"))
                {
                    FileModel f = new FileModel();
                    f.filename = Path.GetFileName(file);
                    f.id = getFileMD5(file);
                    i++;
                    list.Add(f);
                }
                return JsonConvert.SerializeObject(list);
            }
            else if (request.accion == 1)
            {
                var file = request.file;
                File.WriteAllBytes(@".\Files\"+file.filename, Convert.FromBase64String(file.file));
                return "true";
            }else if (request.accion == 3)
            {
                foreach (string file in Directory.EnumerateFiles(@".\Files"))
                {
                    FileModel f = new FileModel();
                    f.id = getFileMD5(file);
                    if (f.id == request.file.id)
                    {
                        File.Delete(file);
                    }
                }
                return "true";
            }
            return "";
        }

        private static string getFileMD5(string file)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(file))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
