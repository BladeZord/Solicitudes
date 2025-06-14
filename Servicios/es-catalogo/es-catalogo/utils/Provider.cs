using es_catalogo.Constans;

namespace es_catalogo.utils
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
