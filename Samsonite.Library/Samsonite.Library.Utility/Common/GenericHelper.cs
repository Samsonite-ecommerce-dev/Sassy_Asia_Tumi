using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Samsonite.Library.Utility
{
    public class GenericHelper
    {
        /// <summary>
        /// 对象赋值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objT">复制对象</param>
        /// <returns></returns>
        public static T TCopyValue<T>(T objT)
        {
            try
            {
                T result = System.Activator.CreateInstance<T>();
                PropertyInfo[] _propertyInfos = objT.GetType().GetProperties();
                foreach (var _p in _propertyInfos)
                {
                    //对象属性模型赋值
                    _p.SetValue(result, _p.GetValue(objT));
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 对象在泛型集合中是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objList">泛型集合</param>
        /// <param name="objT"></param>
        /// <returns></returns>
        public static bool TIsContains<T>(List<T> objList, T objT)
        {
            try
            {
                bool result = false;
                PropertyInfo[] _propertyInfos = objT.GetType().GetProperties();
                foreach (var _O in objList)
                {
                    bool _tmp = true;
                    foreach (var _p in _propertyInfos)
                    {
                        if (_p.GetValue(_O) != _p.GetValue(objT))
                        {
                            _tmp = false;
                        }
                    }
                    if (_tmp)
                    {
                        result = true;
                        break;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 泛型对象转换成Dynamic
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static dynamic ConvertToDynamic(object obj)
        {
            try
            {
                //利用字典集合转换
                dynamic dy = new System.Dynamic.ExpandoObject();
                var _iDict = (IDictionary<string, object>)dy;
                PropertyInfo[] _propertyInfos = obj.GetType().GetProperties();
                foreach (var _p in _propertyInfos)
                {
                    _iDict.Add(_p.Name, _p.GetValue(obj));
                }
                return (dynamic)_iDict;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
