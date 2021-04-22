using System;
namespace Api
{
    public class ErroService
    {
        public void GerarErro1()
        {
            try
            {
                GerarErro2();
            }
            catch(Exception e)
            {
                throw new Exception($"Erro 1. {e.Message}", e);
            }
            
        }

        public void GerarErro2()
        {
            throw new Exception("Gerando erro 2");
        }

    }
}
