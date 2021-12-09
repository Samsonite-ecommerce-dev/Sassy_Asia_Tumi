using System;
using System.Reflection;

namespace Samsonite.Library.Utility
{
    public class ValidateHelper
    {
        /// <summary>
        /// 过滤请求参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objT">参数对象</param>
        /// <returns></returns>
        public static T Validate<T>(T objT)
        {
            try
            {
                PropertyInfo[] _propertyInfos = objT.GetType().GetProperties();
                foreach (var _p in _propertyInfos)
                {
                    var _type = _p.PropertyType;
                    if (_type == typeof(Int16))
                    {
                        _p.SetValue(objT, VariableHelper.SaferequestInt16(_p.GetValue(objT)));
                    }
                    else if (_type == typeof(Int32))
                    {
                        _p.SetValue(objT, VariableHelper.SaferequestInt(_p.GetValue(objT)));
                    }
                    else if (_type == typeof(Int64))
                    {
                        _p.SetValue(objT, VariableHelper.SaferequestInt64(_p.GetValue(objT)));
                    }
                    else if (_type == typeof(Double))
                    {
                        _p.SetValue(objT, VariableHelper.SaferequestDouble(_p.GetValue(objT)));
                    }
                    else if (_type == typeof(Decimal))
                    {
                        _p.SetValue(objT, VariableHelper.SaferequestDecimal(_p.GetValue(objT)));
                    }
                    else if (_type == typeof(float))
                    {
                        _p.SetValue(objT, VariableHelper.SaferequestFloat(_p.GetValue(objT)));
                    }
                    else if (_type == typeof(Byte))
                    {
                        _p.SetValue(objT, VariableHelper.SaferequestByte(_p.GetValue(objT)));
                    }
                    else if (_type == typeof(SByte))
                    {
                        _p.SetValue(objT, VariableHelper.SaferequestSByte(_p.GetValue(objT)));
                    }
                    else if (_type == typeof(string))
                    {
                        _p.SetValue(objT, VariableHelper.SaferequestStr(_p.GetValue(objT)));
                    }
                    else if (_type == typeof(DateTime))
                    {
                        _p.SetValue(objT, VariableHelper.SaferequestTime(_p.GetValue(objT)));
                    }
                    else if (_type == typeof(DateTime?))
                    {
                        _p.SetValue(objT, VariableHelper.SaferequestNullTime(_p.GetValue(objT)));
                    }
                    else if (_type == typeof(bool))
                    {
                        _p.SetValue(objT, VariableHelper.SaferequestBool(_p.GetValue(objT)));
                    }
                    else if (_type == typeof(int[]))
                    {
                        _p.SetValue(objT, VariableHelper.SaferequestIntArray(_p.GetValue(objT)));
                    }
                    else if (_type == typeof(Int64[]))
                    {
                        _p.SetValue(objT, VariableHelper.SaferequestInt64Array(_p.GetValue(objT)));
                    }
                    else if (_type == typeof(string[]))
                    {
                        _p.SetValue(objT, VariableHelper.SaferequestStrArray(_p.GetValue(objT)));
                    }
                    else { }
                }
                return objT;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}