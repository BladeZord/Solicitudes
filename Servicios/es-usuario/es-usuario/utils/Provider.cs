using es_usuario.Constans;

namespace es_usuario.utils
{
    public class Provider
    {
        public UrlService Url { get; private set; }
        public Provider(string connectionString)
        {
            Url = new UrlService { SQLConnection = connectionString };
        }
    }
}
