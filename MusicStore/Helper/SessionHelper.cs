using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using MusicStore.Model.Entities;
using System.Collections.Generic;

namespace MusicStore.Helper
{
    public static class SessionHelper
    {
        public static List<CartItem> GetCart(this ISession session)
        {
            var cartJson = session.GetString("Cart");
            if (cartJson == null)
            {
                return new List<CartItem>();
            }

            return JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
        }

        public static void SaveCart(this ISession session, List<CartItem> cart)
        {
            session.SetString("Cart", JsonConvert.SerializeObject(cart));
        }

        public static void ClearCart(this ISession session)
        {
            session.Remove("Cart");
        }
    }
}
