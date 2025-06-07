using es_solicitudes.Constans;

namespace es_solicitudes.utils
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
