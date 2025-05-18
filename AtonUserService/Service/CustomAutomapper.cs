using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using TechProcessSupportSys.Interfaces;

namespace AtonUserService.Service
{
    public class CustomAutomapper : IAutomapper
    {
        public T Map<T, K>(K obj)
            where T : class, new()
            where K : class
        {
            T new_obj = new T();
            foreach (PropertyInfo tProp in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {

                bool b = false;
                foreach (var attr in tProp.GetCustomAttributes(true))
                {
                    if (attr as NotMappedAttribute != null) b = true;
                }
                if (!tProp.CanWrite || !tProp.CanRead) b = true;
                if (b) continue;

                foreach (PropertyInfo kProp in typeof(K).GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (tProp!.Name.ToLower() == kProp.Name.ToLower())
                    {
                        tProp?.SetValue(new_obj, kProp.GetValue(obj));
                    }
                }
            }
            return new_obj;
        }
    }
}