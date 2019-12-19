using ProjetoOraculo.Interface;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjetoOraculo
{
    public class Util : IUtil
    {
        public ConnectionFactory ConnectionFactory()
        {
            var factory = new ConnectionFactory
            {
                UserName = "teste",
                Password = "teste",
                HostName = "10.20.34.31"
                //Port = 15672
            };

            return factory;
        }

        public string PegaResposta()
        {
            List<string> lstMsg = new List<string> {
                "Mensagem vazia",
                "Sei lá",
                "Não tenho Resposta",
                "Cruzeiro vai cair",
                "Chape vai cair"
            };

            Random random = new Random(5);

            return lstMsg.ElementAt(random.Next(1, 5));
        }
    }
}
