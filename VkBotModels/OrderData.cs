using GeoCoordinatePortable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using VkNet.Model;
using VkNet.Model.Keyboard;

namespace WebBot
{
    /// <summary>
    /// Данные о заказе
    /// </summary>
    class OrderData
    {
        public List<Market> Products { get; set; } = new List<Market>();
        public Geo DeliveryAddress { get; set; }
        public DateTime DeliveryDateTime { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public int DeliveryCost { get; set; }
        public string OrderStatus { get; set; }
        public string OrderId { get; set; }

        public long CustomerId { get; set; }

        public OrderStage OrderStage { get; set; } = OrderStage.NoOrder;

        public int TotalCost { get => FindTotalCost(); }

        public int OrderDiscont { get; set; }
        public PromoCode Promocode { get; set; }

        public GeoCoordinate DeliveryAreaCenter { get; private set; } = new GeoCoordinate(56.8528253, 53.2062464); //Координаты центра

        public OrderData(long userId)
        {
            CustomerId = userId;
            DeliveryCost = -1;
            OrderId = Guid.NewGuid().ToString().Split('-')[0].ToUpper();
        }

        /// <summary>
        /// Рассчитать финальную стоимость заказа
        /// </summary>
        /// <returns>Стоимость заказа</returns>
        private int FindTotalCost()
        {
            var productsCost = (int)Products.Sum(p => p.Price.Amount.Value / 100);
            var prodCostWithDiscount =  productsCost - productsCost * OrderDiscont / 100.0;
            var finalCost = (int)prodCostWithDiscount + DeliveryCost;
            return finalCost;
        }

        /// <summary>
        /// Рассчитать стоимость доставки
        /// </summary>
        public void FindDeliveryCost()
        {
            var distance = FindDistanteToDeliveryPoint(); //расстояние в метрах
            if (distance < 1000)
                DeliveryCost = 0;
            else if (distance > 1000 && distance < 1500)
                DeliveryCost = (distance - 1000) / 100 * 2;
            else if (distance > 1500 && distance < 2300)
                DeliveryCost = 15 + (distance - 1500) / 100 * 2;
            else if (distance > 2300 && distance < 5000)
                DeliveryCost = 30 + (distance - 2300) / 100;
            else if (distance > 5000 && distance < 10000)
                DeliveryCost = 50 + (distance - 5000) / 100;
        }

        /// <summary>
        /// Найти расстояние в метрах до точки доставки
        /// </summary>
        /// <returns>Расстояние в метрах</returns>
        public int FindDistanteToDeliveryPoint()
        {
            var point = new GeoCoordinate(DeliveryAddress.Coordinates.Latitude,
                DeliveryAddress.Coordinates.Longitude);
            return (int)DeliveryAreaCenter.GetDistanceTo(point);
        }
    }
    
    /// <summary>
    /// Способ оплаты
    /// </summary>
    public enum PaymentMethod
    {
        наличными,
        картой
    };
}
