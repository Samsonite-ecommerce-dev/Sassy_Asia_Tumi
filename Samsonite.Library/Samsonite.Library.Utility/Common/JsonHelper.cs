using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Samsonite.Library.Utility
{
    public class JsonHelper
    {
        /// <summary>
        /// 序列化json对象
        /// </summary>
        /// <param name="objT"></param>
        /// <returns></returns>
        public static string JsonSerialize(object objT)
        {
            try
            {
                return JsonConvert.SerializeObject(objT);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured when serialize the Json data:" + ex.Message);
            }
        }

        /// <summary>
        /// 序列化json对象
        /// </summary>
        /// <param name="objT"></param>
        /// <param name="objSettings"></param>
        /// <returns></returns>
        public static string JsonSerialize(object objT, JsonSerializerSettings objSettings)
        {
            try
            {
                return JsonConvert.SerializeObject(objT, objSettings);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured when serialize the Json data:" + ex.Message);
            }
        }

        /// <summary>
        /// 序列化json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objT"></param>
        /// <returns></returns>
        public static string JsonSerialize<T>(T objT)
        {
            try
            {
                return JsonConvert.SerializeObject(objT);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured when serialize the Json data,Message:" + ex.Message);
            }
        }

        /// <summary>
        /// 序列化json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objT"></param>
        /// <param name="objSettings"></param>
        /// <returns></returns>
        public static string JsonSerialize<T>(T objT, JsonSerializerSettings objSettings)
        {
            try
            {
                return JsonConvert.SerializeObject(objT, objSettings);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured when serialize the Json data,Message:" + ex.Message);
            }
        }

        /// <summary>
        /// 反序列化json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objStr"></param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(string objStr)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(objStr);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured when deserialize the Json data,Message:" + ex.Message);
            }
        }

        /// <summary>
        /// 反序列化json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objStr"></param>
        /// <param name="objSettings"></param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(string objStr, JsonSerializerSettings objSettings)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(objStr, objSettings);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured when deserialize the Json data,Message:" + ex.Message);
            }
        }
    }
}
