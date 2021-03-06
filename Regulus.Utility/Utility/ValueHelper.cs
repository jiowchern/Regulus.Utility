using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Regulus.Utility
{
    public class ValueHelper
    {
        public static object StringConvert(Type type, string value)
        {
            object outValue;
            Regulus.Utility.Command.TryConversion(value, out outValue, type);
            return outValue;
        }

        public IEnumerator<T> GetEnumItems<T>() where T : class
        {
            foreach (object item in Enum.GetValues(typeof(T)))
            {
                yield return item as T;
            }
        }

        public static T DeepCopy<T>(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("Object cannot be null");
            }

            return (T)ValueHelper._CopyProcess(obj);
        }

        public static object DeepCopy(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("Object cannot be null");
            }

            return ValueHelper._CopyProcess(obj);
        }

        private static object _CopyProcess(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            Type type = obj.GetType();
            if (type.IsValueType || type == typeof(string))
            {
                return obj;
            }

            if (type.IsArray)
            {
                Type elementType = type.GetElementType();

                Array array = obj as Array;
                Array copied = Array.CreateInstance(elementType, array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    copied.SetValue(ValueHelper._CopyProcess(array.GetValue(i)), i);
                }

                return Convert.ChangeType(copied, obj.GetType());
            }

            if (type.IsClass)
            {
                object toret = Activator.CreateInstance(obj.GetType());
                FieldInfo[] fields = type.GetFields(
                    BindingFlags.Public |
                    BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue == null)
                    {
                        continue;
                    }

                    field.SetValue(toret, ValueHelper._CopyProcess(fieldValue));
                }

                return toret;
            }

            throw new ArgumentException("Unknown type");
        }

        public static bool DeepEqual(object obj1, object obj2)
        {
            return ValueHelper._EqualProcess(obj1, obj2);
        }

        private static bool _EqualProcess(object obj, object obj1)
        {
            if (obj == null && obj1 == null)
            {
                return true;
            }

            if (obj != null && obj1 == null)
            {
                return false;
            }

            if (obj == null && obj1 != null)
            {
                return false;
            }

            if (obj1.GetType() != obj.GetType())
            {
                return false;
            }

            Type type = obj.GetType();
            if (type.IsValueType || type == typeof(string))
            {
                return obj.Equals(obj1);
            }

            if (type.IsArray)
            {
                Type elementType = Type.GetType(
                    type.FullName.Replace("[]", string.Empty));
                Array array = obj as Array;

                // Array copied = Array.CreateInstance(elementType, array.Length);
                Array copied = obj1 as Array;
                if (copied.Length != array.Length)
                    return false;

                for (int i = 0; i < array.Length; i++)
                {
                    if (ValueHelper._EqualProcess(array.GetValue(i), copied.GetValue(i)) == false)
                    {
                        return false;
                    }
                }

                return true;
            }

            if (type.IsClass)
            {
                FieldInfo[] fields = type.GetFields(
                    BindingFlags.Public |
                    BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (ValueHelper._EqualProcess(field.GetValue(obj), field.GetValue(obj1)) == false)
                    {
                        return false;
                    }
                }

                return true;
            }

            throw new ArgumentException("無法比較的類型.");
        }







    }

    public class Comparison<T>
    {
        public readonly bool Same;
        public Comparison(IEnumerable<T> array1, IEnumerable<T> array2, Func<T, T, bool> comparison)
        {
            Queue<T> queue1 = new Queue<T>(array1);
            Queue<T> queue2 = new Queue<T>(array2);


            while (queue1.Any() && queue2.Any())
            {
                if (comparison(queue1.Dequeue(), queue2.Dequeue()) == false)
                {
                    Same = false;
                    return;
                }
            }


            Same = queue1.Count() == queue2.Count();
        }


    }


}
