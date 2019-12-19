using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoOraculo.Interface
{
    public interface IUtil
    {
        public ConnectionFactory ConnectionFactory();
    }
}
