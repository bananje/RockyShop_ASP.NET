using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Rocky_Utility
{
    public static class SessionExtension
    {
        // key - название ссесии (храниться в WC)
        public static void Set<T>(this ISession session, string key, T value) //реализация сериализации
        {
            session.SetString(key, JsonSerializer.Serialize(value)); // сохранение объекта в виде строки в JSON
        }

        public static T Get<T>(this ISession session, string key) // реализация десирилиазации 
        {
            var value = session.GetString(key); // проверка ключа
            return value == null ? default : JsonSerializer.Deserialize<T>(value); // получаем объект
        }    
    }
}
