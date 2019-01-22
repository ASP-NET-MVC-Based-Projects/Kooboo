﻿using Kooboo.IndexedDB.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
   public class ByteFieldConverter<T> : IFieldConverter<T>
    {
        Func<T, byte> getValue;
        Action<T, byte> setValue;

        public ByteFieldConverter(string FieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, byte>(FieldName);
            this.setValue = ObjectHelper.GetSetValue<T, byte>(FieldName);
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
        }

        public int ByteLength
        {
            get
            {
                return 1;
            }
        }

        public int FieldNameHash
        {
            get; set;
        }

        public void SetByteValues(T value, byte[] bytes)
        {
            byte bytevalue = ValueConverter.ToByte(bytes);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(T Value)
        {
            byte fieldvalue = this.getValue(Value);
            return ValueConverter.ToBytes(fieldvalue);
        }

    }


    public class ByteFieldConverter : IFieldConverter
    {
        Func<object, byte> getValue;
        Action<object, byte> setValue;

        public ByteFieldConverter(string FieldName, Type ObjectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<byte>(FieldName, ObjectType);
            this.setValue = ObjectHelper.GetSetFieldValue<byte>(FieldName, ObjectType);
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
        }

        public int ByteLength
        {
            get
            {
                return 1;
            }
        }

        public int FieldNameHash
        {
            get; set;
        }

        public void SetByteValues(object value, byte[] bytes)
        {
            byte bytevalue = ValueConverter.ToByte(bytes);
            this.setValue(value, bytevalue);
        }
         

        public byte[] ToBytes(object Value)
        {
            byte fieldvalue = this.getValue(Value);
            return ValueConverter.ToBytes(fieldvalue);
        }
         
    }
}
