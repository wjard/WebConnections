using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace WebConnections.Core.Entity
{
    [DataContract]
    public class BaseEntity : INotifyPropertyChanged
    {
        public BaseEntity()
        {
            Initialize();
        }

        public virtual void Initialize()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
