using System.Runtime.Serialization;

namespace es_catalogo.exception
{
    [Serializable]
    public class ServiceException : Exception
    {
        public TipoError Codigo { get; set; }

        public ServiceException(string mensaje)
            : base(mensaje)
        {
        }

        public ServiceException(string mensaje, Exception excepcionInterna)
            : base(mensaje, excepcionInterna)
        {
        }

        protected ServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Codigo = (TipoError)info.GetValue(nameof(Codigo), typeof(TipoError));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            base.GetObjectData(info, context);
            info.AddValue(nameof(Codigo), Codigo);
        }
    }
}
