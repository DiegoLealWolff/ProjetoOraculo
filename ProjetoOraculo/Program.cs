using Newtonsoft.Json;
using ProjetoOraculo.Entity;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjetoOraculo
{
    class Program
    {
        static void Main(string[] args)
        {
            //var factory = new ConnectionFactory
            //{
            //    UserName = "teste",
            //    Password = "teste",
            //    HostName = "10.20.34.31"
            //};

            //var connectionFactory = new ConnectionFactory
            //{
            //    UserName = "teste",
            //    Password = "teste",
            //    HostName = "10.20.34.31",
            //    //Port = 15672
            //};

            var payload = "";
            var utilObject = new Util();

            //PASSO 1 - RECEBE A MENSAGEM
            var factory = utilObject.ConnectionFactory();
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "leitura_mensagem",
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        payload = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", payload);
                    };

                    channel.BasicConsume(queue: "leitura_mensagem",
                                         autoAck: true,
                                         consumer: consumer);
                }
            }
            
            //PASSO 2: TRATA A MESAGEM
            var result = JsonConvert.DeserializeObject<Payload>(payload);            
            var connectionFactory = factory.CreateConnection();

            //PASSO 3: ENCAMINHAR PARA O EXCHANGE CERTO
            if (string.IsNullOrWhiteSpace(result?.resposta))
            {
                result.resposta = utilObject.PegaResposta();

                //MANDA PRA EXCHANGE ENVIO_BANCO        

                using (var channel = connectionFactory.CreateModel())
                {
                    var result2 = JsonConvert.SerializeObject(result);
                    byte[] body = Encoding.UTF8.GetBytes(result2);

                    channel.BasicPublish(exchange: "",
                                        routingKey: "envio_banco",
                                        basicProperties: null,
                                        body: body);
                };
            }
            else
            {
                //MANDA PRA EXCHANGE ENVIO_MENSAGEM        
                using (var channel = connectionFactory.CreateModel())
                {
                    byte[] body = Encoding.UTF8.GetBytes(payload);

                    channel.BasicPublish(exchange: "",
                                        routingKey: "envio_mensagem",
                                        basicProperties: null,
                                        body: body);
                };
            }
        }       
    }
}
