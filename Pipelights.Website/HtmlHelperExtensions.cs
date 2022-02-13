//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using Newtonsoft.Json;
//using Pipelights.Website.Controllers;

//namespace Pipelights.Website
//{
//    public static class HtmlHelperExtensions
//    {
//        public static int GetBazaarVoiceLoader(this HtmlHelper helper)
//        {
//            var cart = HttpContext.Session.GetString("cart");
//            if (cart != null)
//            {
//                return 0;
//            }

//            var cartItems = JsonConvert.DeserializeObject<Cart>(cart);

//            return cartItems.products?.Sum(x => x.Value);
//        }
//    }

//    public static class SessionHelper
//    {
//        public static void SetObjectAsJson(this ISession session, string key, object value)
//        {
//            session.SetString(key, JsonConvert.SerializeObject(value));
//        }

//        public static T GetObjectFromJson<T>(this ISession session, string key)
//        {
//            var value = session.GetString(key);
//            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
//        }
//    }
//}
