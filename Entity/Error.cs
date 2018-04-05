using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace WebConnections.Core.Entity
{
    [DataContract]
    public class Error
    {
        [DataMember]
        public string message
        {
            get { return _message; }
            set { _message = value; }
        } private string _message;
    }

    [DataContract]
    public class Errors
    {
        [DataMember]
        public Error error
        {
            get { return _error; }
            set { _error = value; }
        } private Error _error;

        public Errors()
        {
            error = new Error();
        }
    }

    [DataContract]
    public class MessageError : BaseEntity
    {
        [DataMember]
        public Errors errors
        {
            get { return _errors; }
            set { _errors = value; }
        } private Errors _errors;

        public MessageError()
            : base()
        {

        }

        public override void Initialize()
        {
            errors = new Errors();
        }

        [OnDeserializing]
        public void OnDeserializing(StreamingContext ctx)
        {
            this.Initialize();
        }

        public static MessageError NewMessageError(string mensagem)
        {
            MessageError error = new MessageError();
            error.errors.error.message = mensagem;
            return error;
        }
    }
}
